#region License
/*
Copyright © 2014-2018 European Support Limited

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

using Amdocs.Ginger.Common;
using System.Windows.Controls;
using Ginger.WindowExplorer;
using GingerCore.Actions;
using GingerCore.Actions.Windows;
using GingerWPF.UserControlsLib.UCTreeView;
using GingerCore.Actions.Common;
using Amdocs.Ginger.Common.UIElement;

namespace Ginger.Drivers.Windows
{
    class WindowsItemTreeItem : WindowsElementTreeItemBase, ITreeViewItem, IWindowExplorerTreeItem
    {
        StackPanel ITreeViewItem.Header()
        {
            string ImageFileName = "@Agent_16x16.png";
            string Title = UIAElementInfo.ElementTitle;
            return TreeViewUtils.CreateItemHeader(Title, ImageFileName);
        }

        ObservableList<Act> IWindowExplorerTreeItem.GetElementActions()
        {
            ObservableList<Act> list = new ObservableList<Act>();
          
                list.Add(new ActWindowsControl()
                {
                    Description = "Select " + UIAElementInfo.ElementTitle,
                    LocateBy = eLocateBy.ByName,
                    LocateValue = UIAElementInfo.ElementTitle,
                    ControlAction = ActWindowsControl.eControlAction.Select
                });
            return list;
        }
    }
}
