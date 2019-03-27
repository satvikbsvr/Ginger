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
using Amdocs.Ginger.Common;
using Ginger;
using Ginger.BusinessFlowLib;
using Ginger.BusinessFlowsLibNew.AddActionMenu;
using Ginger.BusinessFlowWindows;
using Ginger.Run;
using GingerCore;
using GingerCore.Environments;
using GingerCore.GeneralLib;
using GingerCoreNET.RunLib;
using GingerWPF.GeneralLib;
using GingerWPF.RunLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace GingerWPF.BusinessFlowsLib
{
    //public delegate void ToggelPanelEventHandler(object sender, object e);

    /// <summary>
    /// Interaction logic for BusinessFlowPage.xaml
    /// </summary>
    public partial class NewAutomatePage : Page
    {
        //Activity currentActivity = null;
        //public event EventHandler ToggelPanelEvent;

        //protected virtual void OnToggelPanelEvent(EventArgs e)
        //{
        //    EventHandler handler = ToggelPanelEvent;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}


        BusinessFlow mBusinessFlow;
        GingerRunner mGingerRunner;
        Context mContext = new Context();
        GridLength mlastActionMenuFrameColWidth = new GridLength(300);
        public NewAutomatePage(BusinessFlow businessFlow)
        {
            InitializeComponent();

            mGingerRunner = App.AutomateTabGingerRunner;
            mContext = new Context() { BusinessFlow = businessFlow, Activity = businessFlow.Activities[0], Runner = mGingerRunner };

            mBusinessFlow = businessFlow;
            //Binding
            BusinessFlowNameLabel.BindControl(mBusinessFlow, nameof(BusinessFlow.Name));
            // TODO: break it down to each folder and show parts with hyperlink
            FlowPathLabel.Content = mBusinessFlow.ContainingFolder;
            EnvironmentComboBox.ItemsSource = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<ProjEnvironment>();
            EnvironmentComboBox.DisplayMemberPath = nameof(ProjEnvironment.Name);

            if (mBusinessFlow.CurrentActivity == null && mBusinessFlow.Activities.Count > 0)
            {
                mBusinessFlow.CurrentActivity = mBusinessFlow.Activities[0];
            }

            FlowDiagramFrmae.Content = new BusinessFlowDiagramPage(mBusinessFlow);
            ActivitiesList.ItemsSource = mBusinessFlow.Activities;

            //TODO: Move these lines to GR to be one function call
            //WorkSpace.Instance.GingerRunner.BusinessFlows.Clear();
            //WorkSpace.Instance.GingerRunner.BusinessFlows.Add(BusinessFlow);
            //WorkSpace.Instance.GingerRunner.CurrentBusinessFlow = BusinessFlow;

            //WorkSpace.Instance.GingerRunner.CurrentBusinessFlow.PropertyChanged += CurrentBusinessFlow_PropertyChanged;
            //WorkSpace.Instance.GingerRunner.GingerRunnerEvent += GingerRunner_GingerRunnerEvent;


            App.PropertyChanged += App_PropertyChanged;

            //CurrentActivityFrame.Content = new ActivityPage((Activity)mBusinessFlow.Activities[0]);  // TODO: use binding? or keep each activity page
            CurrentActivityFrame.Content = new ActivityPage(mContext);  // TODO: use binding? or keep each activity page

            InitGingerRunnerControls();
        }

        internal void GoToBusFlowsListHandler(object v)
        {
            throw new NotImplementedException();
        }

        private void App_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        static GingerRunnerControlsPage mGingerRunnerControlsPage;
        private void InitGingerRunnerControls()
        {
            // TODO: if this page is going to be used as standalone pass the controls page as input
            if (mGingerRunnerControlsPage == null)
            {
                mGingerRunnerControlsPage = new GingerRunnerControlsPage(mGingerRunner);
            }
            //GingerRunnerControlsFrame.Content = mGingerRunnerControlsPage;
        }

        private void GingerRunner_GingerRunnerEvent(GingerRunnerEventArgs EventArgs)
        {
            switch (EventArgs.EventType)
            {
                case GingerRunnerEventArgs.eEventType.ActivityStart:
                    Activity a = (Activity)EventArgs.Object;
                    // Just to show we can display progress
                    this.Dispatcher.Invoke(() =>
                    {
                        //StatusLabel.Content = "Running " + a.ActivityName;
                    });

                    break;
                case GingerRunnerEventArgs.eEventType.ActionEnd:
                    this.Dispatcher.Invoke(() =>
                    {
                        // just quick code to show activity progress..
                        int c = (from x in mBusinessFlow.Activities where x.Status != Amdocs.Ginger.CoreNET.Execution.eRunStatus.Pending select x).Count();
                        //ProgressBar.Maximum = mBusinessFlow.Activities.Count;
                        //ProgressBar.Value = c;
                    });
                    break;
            }
        }

        private void CurrentBusinessFlow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BusinessFlow.CurrentActivity))
            {
                ActivitiesList.Dispatcher.Invoke(() =>
                {
                    ActivitiesList.SelectedItem = mBusinessFlow.CurrentActivity;
                });

            }
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            List<ActionSelectorItem> actions = new List<ActionSelectorItem>();
            actions.Add(new ActionSelectorItem() { Name = "Add Activity using recording", Action = AddActivity });
            actions.Add(new ActionSelectorItem() { Name = "Add Empty Activity", Action = AddActivity });
            actions.Add(new ActionSelectorItem() { Name = "Add Activity from shared repository", Action = AddActivity });

            ActionSelectorWindow w = new ActionSelectorWindow("What would you like to add?", actions);
            w.Show();
        }

        private void AddActivity()
        {
            Activity activity = new Activity();
            bool b = InputBoxWindow.OpenDialog("Add new Activity", "Activity Name", activity, nameof(Activity.ActivityName));
            if (b)
            {
                mBusinessFlow.Activities.Add(activity);
            }
        }

        private void ActivitiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Activity SelectedActivity = (Activity)ActivitiesList.SelectedItem;

            mGingerRunner.CurrentBusinessFlow.CurrentActivity = SelectedActivity;
            App.AutomateTabGingerRunner.CurrentBusinessFlow.CurrentActivity = SelectedActivity;
            if (SelectedActivity.Acts.CurrentItem == null && SelectedActivity.Acts.Count > 0)
            {
                SelectedActivity.Acts.CurrentItem = SelectedActivity.Acts[0];
            }
            mContext.Activity = SelectedActivity;
            //CurrentActivityFrame.Content = new ActivityPage(SelectedActivity);
            CurrentActivityFrame.Content = new ActivityPage(mContext);
        }

        private void BusinessFlowsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            WorkSpace.Instance.EventHandler.ShowBusinessFlows();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WorkSpace.Instance.SolutionRepository.SaveRepositoryItem(mBusinessFlow);
            //TODO: show message item save or Ginger Helper
        }

        private void SHAddActionPanel_Click(object sender, RoutedEventArgs e)
        {
            if (AddActionMenuFrame.Content == null)
                AddActionMenuFrame.Content = new MainAddActionsNavigationPage(mContext);

            if (AddActionMenuFrame.Visibility == Visibility.Collapsed)
            {
                AddActionMenuFrame.Visibility = Visibility.Visible;
                //xAddActionMenuGrid.ColumnDefinitions[2].Width = new GridLength(300, GridUnitType.Star);
                //xActionActivitiesGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
                SHAddActionPanel.Content = "-";
            }
            else
            {
                AddActionMenuFrame.Visibility = Visibility.Collapsed;
                //xAddActionMenuGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
                //xActionActivitiesGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);
                SHAddActionPanel.Content = "+";
            }
            xPanelResizer.IsEnabled = !xPanelResizer.IsEnabled;
        }

        public void GoToBusFlowsListHandler(RoutedEventHandler clickHandler)
        {
            xStepBack.Click += clickHandler;
        }

        private void XAddActionPanel_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void XAddActionPanel_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void XAddActionPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void AddActionMenuFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AddActionMenuFrame.Width = xAddActionMenuGrid.ColumnDefinitions[2].Width.Value;
        }
    }
}
