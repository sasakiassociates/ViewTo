﻿using System;
using System.Drawing;

namespace Sasaki.ViewObjects;

public class ExplorerSettings
{

  public ExplorerSettings()
  { }
    
  /// <summary>
  ///   Min value from <see cref="IExplorer" /> active values
  /// </summary>
  public double min {get;set;} = 0.0;

  /// <summary>
  ///   Max value from <see cref="IExplorer" /> active values
  /// </summary>
  public double max {get;set;} = 1.0;

  /// <summary>
  ///   When set to true will normalize the active values
  /// </summary>
  public bool normalize {get;set;} = false;

  /// <summary>
  ///   Gradient ramp for visualizing the value of point
  /// </summary>
  public Color[] colorRamp {get;set;} = Array.Empty<Color>();

  /// <summary>
  ///   Color for any point with no value in cloud
  /// </summary>
  public Color invalidColor {get;set;} = Color.Black;
  
  /// <summary>
  ///   Get a color along the gradient ramp
  /// </summary>
  /// <param name="t"></param>
  /// <returns></returns>
  public Color GetColor(double t)
  {
    return t>=0 ? colorRamp[(int)Math.Round((colorRamp.Length-1.0)*Clamp(t, 0.0, 1.0), 0)] : invalidColor;
  }

  static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
  {
    if(val.CompareTo(min)<0)
    {
      return min;
    }

    return val.CompareTo(max)>0 ? max : val;
  }
}