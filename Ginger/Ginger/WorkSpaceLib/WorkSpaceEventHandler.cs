#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using amdocs.ginger.GingerCoreNET;
using Ginger;
using GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib;
using GingerWPF.ApplicationPlatformsLib;
using System.Diagnostics;
using System.IO;

namespace GingerWPF.WorkSpaceLib
{
    public class WorkSpaceEventHandler : IWorkSpaceEventHandler
    {        
        public void AddApplication()
        {
        }

        public void OpenAddAPIModelWizard()
        {
        }

        public void OpenContainingFolder(string folderPath)
        {
            string FullPath = WorkSpace.Instance.SolutionRepository.GetFolderFullPath(folderPath);
            if (string.IsNullOrEmpty(FullPath))
                return;

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
            Process.Start(FullPath);
        }

        public void ShowBusinessFlows()
        {
        }

        public void ShowDebugConsole(bool visible = true)
        {
            // TODO: decide if we want both or one and done !!!!!!!!!!!!


            // Ginger WPF window with buttons like clear and we can customize
            DebugConsoleWindow debugConsole = new DebugConsoleWindow();
            debugConsole.ShowAsWindow();

            // windows black console window
            App.ShowConsoleWindow();
        }


        public void SolutionClosed()
        {
        }
    }
}
