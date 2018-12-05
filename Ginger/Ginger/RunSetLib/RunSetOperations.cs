﻿using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Repository;
using Ginger.Run;
using GingerCore;
using GingerCore.GeneralLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amdocs.Ginger
{
    // TODO: move to GingerCoreNET once RIs moved to GingerCoreCommon
    public static class RunSetOperations
    {
        public static RunSetConfig CreateNewRunset(string runSetName="", RepositoryFolder<RunSetConfig> runSetsFolder = null)
        {
            if (string.IsNullOrEmpty(runSetName))
            {
                if (!InputBoxWindow.GetInputWithValidation(string.Format("Add New {0}", GingerDicser.GetTermResValue(eTermResKey.RunSet)), string.Format("{0} Name:", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ref runSetName, System.IO.Path.GetInvalidPathChars()))
                {
                    return null;
                }

                while (CheckAmbiguity(runSetName))
                {
                    Reporter.ToUser(eUserMsgKeys.ValueIssue, runSetName + " already exists ! Run Set name should be unique !");

                    if (!InputBoxWindow.GetInputWithValidation(string.Format("Add New {0}", GingerDicser.GetTermResValue(eTermResKey.RunSet)), string.Format("{0} Name:", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ref runSetName, System.IO.Path.GetInvalidPathChars()))
                    {
                        return null;
                    }
                }
            }

            RunSetConfig runSetConfig = new RunSetConfig();
            runSetConfig.Name = runSetName;
            runSetConfig.GingerRunners.Add(new GingerRunner() { Name = "Runner 1" });

            if (runSetsFolder == null)
            {
                WorkSpace.Instance.SolutionRepository.AddRepositoryItem(runSetConfig);
            }
            else
            {
                runSetsFolder.AddRepositoryItem(runSetConfig);
            }

            return runSetConfig;
        }

        private static bool CheckAmbiguity(string runSetName)
        {
            Common.ObservableList<RunSetConfig> allRunsets = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<RunSetConfig>();

            foreach (RunSetConfig existingRunset in allRunsets)
            {
                if (existingRunset.Name.Equals(runSetName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
