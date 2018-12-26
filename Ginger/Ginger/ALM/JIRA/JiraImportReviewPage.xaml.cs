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
using GingerCore.ALM.JIRA;
using Ginger.UserControls;
using GingerCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ginger.ALM.JIRA
{
    /// <summary>
    /// Interaction logic for JiraImportReviewPage.xaml
    /// </summary>
    public partial class JiraImportReviewPage : Page
    {
        GenericWindow _pageGenericWin = null;
        private JiraTestSet mTestSet = null;
        private string mImportDestinationPath = string.Empty;

        public JiraImportReviewPage(JiraTestSet testSet = null, string importDestinationPath = "")
        {
            InitializeComponent();
            mTestSet = testSet;
            mImportDestinationPath = importDestinationPath;
            SetGridView();
            SetGridData();
        }

        private void SetGridView()
        {
            GridViewDef view = new GridViewDef(GridViewDef.DefaultViewName);
            view.GridColsView = new ObservableList<GridColView>();
            view.GridColsView.Add(new GridColView() { Field = mTestSet.TestSetID , Header = "Test Set ID", WidthWeight = 15, ReadOnly = true });
            view.GridColsView.Add(new GridColView() { Field = mTestSet.TestSetName, Header = "Test Set Name",  ReadOnly = true });
            view.GridColsView.Add(new GridColView() { Field = mTestSet.CreatedBy, Header = "Created By", WidthWeight = 25, ReadOnly = true });
            view.GridColsView.Add(new GridColView() { Field = mTestSet.DateCreated, Header = "Creation Date", WidthWeight = 25, ReadOnly = true });
            view.GridColsView.Add(new GridColView() { Field = "Import Test Set", WidthWeight = 20, StyleType = GridColView.eGridColStyleType.Template, CellTemplate = (DataTemplate)this.pageGrid.Resources["ImportButton"] });
            grdJiraTestSets.SetAllColumnsDefaultView(view);
            grdJiraTestSets.InitViewItems();

            grdJiraTestSets.btnRefresh.AddHandler(Button.ClickEvent, new RoutedEventHandler(RefreshGrid));
            //grdJiraTestPlanes.AddToolbarTool("@ImportScript_16x16.png", "Import The Selected TestSet", new RoutedEventHandler(ImportTestSet));
        }

        private void SetGridData()
        {

            Mouse.OverrideCursor = Cursors.Wait;
            ObservableList<JiraTestSet> mJiraTestSetsListSortedByDate = new ObservableList<JiraTestSet>();
            foreach (JiraTestSet testSet in JiraConnect.Instance.GetJiraTestSets())
            {
                mJiraTestSetsListSortedByDate.Add(testSet);
            }
            grdJiraTestSets.DataSourceList = mJiraTestSetsListSortedByDate;
            Mouse.OverrideCursor = null;
        }

        public void ShowAsWindow(eWindowShowStyle windowStyle = eWindowShowStyle.Dialog)
        {
            GingerCore.General.LoadGenericWindow(ref _pageGenericWin, App.MainWindow, windowStyle, this.Title, this);
        }

        private void RefreshGrid(object sender, RoutedEventArgs e)
        {
            SetGridData();
        }

        private void ImportTestPlan(object sender, RoutedEventArgs e)
        {
            ImportTestPlan();
        }

        private void ImportTestPlan(object sender, EventArgs e)
        {
            ImportTestPlan();
        }

        private void ImportTestPlan()
        {
            if (grdJiraTestSets.CurrentItem == null)
            {
                Reporter.ToUser(eUserMsgKeys.NoItemWasSelected);
                return;
            }

            if (ALMIntegration.Instance.ShowImportReviewPage(mImportDestinationPath, grdJiraTestSets.CurrentItem) == true)
            {

            }
        }

        private void ImportBtnClicked(object sender, RoutedEventArgs e)
        {
            ImportTestPlan();
        }
    }
}
