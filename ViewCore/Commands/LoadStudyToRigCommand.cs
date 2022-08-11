﻿using System.Collections.Generic;
using ViewObjects;
using ViewTo.Events.Args;
using ViewTo.Events.Process;

namespace ViewTo.Commands
{
  internal class LoadStudyToRigCommand : ILinkedCommands<IStudyCommand>
  {
    private readonly string studyName;

    public LoadStudyToRigCommand(IViewStudy study, ref IRig rig)
    {
      studyName = study.viewName;
      processArgs = new List<StudyProcessArgs>();

      sequence = new List<IStudyCommand>
      {
        new CheckStudyDataCommand(study), new CompareCloudsInStudyCommand(study)
      };


      var setupStudy = new SetupStudyObjectsCommand(study);
      var rigBuild = new PopulateRigCommand(rig);

      setupStudy.onPrimedEvent += data => rigBuild.ReceivePrimedData(data);

      sequence.Add(setupStudy);
      sequence.Add(rigBuild);
    }
    public CancelStudyArgs cancelStudyArgs { get; private set; }
    public List<StudyProcessArgs> processArgs { get; }
    public IList<IStudyCommand> sequence { get; }

    public void Run()
    {
      foreach (var cmd in sequence)
      {
        cmd.Run();

        if (cmd.processArgs != null)
          processArgs.AddRange(cmd.processArgs);

        if (!cmd.greatSuccess)
        {
          cancelStudyArgs = new CancelStudyArgs(studyName, cmd.errorFlag);
          break;
        }
      }
    }
  }
}
