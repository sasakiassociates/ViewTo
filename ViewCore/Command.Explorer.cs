﻿using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Explorer;

namespace ViewTo
{
  public static partial class Commander
  {

    #region filtering commands

    /// <summary>
    /// Grab a collection of values from the explorer.
    /// </summary>
    /// <param name="explorer"></param>
    /// <param name="type">The type of values to fetch for</param>
    /// <param name="normalize">When set to true will normalize collection against max and min</param>
    /// <returns></returns>
    public static IEnumerable<double> Fetch(this IResultExplorer explorer, ResultType type, bool normalize = false)
    {
      var data = new List<double>();

      explorer.TryGetValues(type, ref data);

      if (!normalize)
        return data;

      data.GetMaxMin(out var max, out var min);

      return data.NormalizeValues(max, min);
    }

    public static void SetActiveValues(this ResultExplorer explorer, ResultType type, string target = null, bool normalize = true)
    {
      if (target.Valid() && explorer.targets.Contains(target) && !explorer.activeTarget.Valid() || !explorer.activeTarget.Equals(target))
        explorer.activeTarget = target;

      if (explorer.activeType != type)
        explorer.activeType = type;

      var values = new List<double>();
      if (!explorer.TryGetValues(explorer.activeType, ref values))
        return;

      if (!normalize)
      {
        explorer.activeValues = values.ToArray();
        return;
      }

      values.GetMaxMin(out var max, out var min);

      explorer.activeValues = values.NormalizeValues(max, min).ToArray();
    }

    public static IEnumerable<double> GetExistingOverPotential(this IResultExplorer explorer)
    {
      return explorer.GetComparedValues(ResultType.Existing, ResultType.Potential);
    }

    public static IEnumerable<double> GetProposedOverExisting(this IResultExplorer explorer)
    {
      return explorer.GetComparedValues(ResultType.Proposed, ResultType.Existing);
    }

    public static IEnumerable<double> GetProposedOverPotential(this IResultExplorer explorer)
    {
      return explorer.GetComparedValues(ResultType.Proposed, ResultType.Potential);
    }

    public static bool InRange(this IExploreRange obj, double value) => value >= obj.min && value <= obj.max;

    public static IEnumerable<double> GetComparedValues(this IResultExplorer explorer, ResultType typeA, ResultType typeB)
    {
      var dataA = new List<double>();
      var dataB = new List<double>();

      if (explorer.TryGetValues(typeA, ref dataA))
        if (explorer.TryGetValues(typeB, ref dataB))
          return dataA.NormalizeValues(dataB);

      return null;
    }

    #endregion

    public static bool TryGetValues(this IResultExplorer explorer, ResultType type, ref List<double> data)
    {
      try
      {
        // select pixel data with target 
        foreach (var d in explorer.storedData)
        {
          if (!d.content.Equals(explorer.activeTarget))
            continue;

          if (type.CheckAgainstString(d.stage))
          {
            data = d.values;
            break;
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      return data != null && data.Any();
    }

    public static bool CheckActiveTarget(this IResultExplorer explorer, string target)
    {
      return target.Valid() && target.Equals(explorer.activeTarget);
    }

    public static bool DataIsReady(this IResultExplorer exp)
    {
      return exp.storedData.Valid() && exp.activeTarget.Valid();
    }

    /// <summary>
    /// uses active values
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="valueToFind"></param>
    /// <returns></returns>
    public static int FindPointWithValue(this IResultExplorer obj, double valueToFind) => obj.activeValues.FindPointWithValue(valueToFind);

    public static int FindPointWithValue(this double[] values, double valueToFind, double unimportantDifference = 0.0001)
    {
      var res = -1;

      if (double.IsNaN(valueToFind) || !values.Valid())
        return res;

      var sampleOfValues = new List<int>();

      // compare data 
      for (var i = 0; i < values.Length; i++)
        if (values[i].NearlyEqual(valueToFind, unimportantDifference))
          sampleOfValues.Add(i);

      // if no values were found from sample set we keep searching
      if (!sampleOfValues.Valid())
      {
        // if no values were found we look for the nearest values
        var nearest = 1.0;

        for (var i = 0; i < values.Length; i++)
        {
          var diff = Math.Abs(values[i] - valueToFind);
          if (diff < nearest)
          {
            nearest = diff;
            res = i;
          }
        }

        sampleOfValues.Add(res);
      }

      var r = new Random(DateTime.Now.Millisecond);

      return sampleOfValues[r.Next(0, sampleOfValues.Count - 1)];
    }
  }
}
