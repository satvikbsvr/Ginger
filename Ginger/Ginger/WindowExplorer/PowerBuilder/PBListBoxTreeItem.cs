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
using GingerWPF.UserControlsLib.UCTreeView;

namespace Ginger.Drivers.PowerBuilder
{
    public class PBListBoxTreeItem : PBControlTreeItemBase, ITreeViewItem, IWindowExplorerTreeItem
    {
        StackPanel ITreeViewItem.Header()
        {
            string ImageFileName = "@List_16x16.png";
            string Title = UIAElementInfo.ElementTitle;
            return TreeViewUtils.CreateItemHeader(Title, ImageFileName);            
        }

        ObservableList<Act> IWindowExplorerTreeItem.GetElementActions()
        {
            ObservableList<Act> list = new ObservableList<Act>();
            string Description = "Get Selected Item " + UIAElementInfo.ElementTitle;

            list.Add(new ActPBControl()
            {
                Description = Description,
                ControlAction = ActPBControl.eControlAction.GetSelected
            });

            list.Add(new ActPBControl()
            {
                Description = "Get All Items " + UIAElementInfo.ElementTitle,
                ControlAction = ActPBControl.eControlAction.GetValue
            });
            return list;
        }
    }
}
