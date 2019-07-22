﻿using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.Enums;
using Amdocs.Ginger.Common.UIElement;
using Amdocs.Ginger.CoreNET;
using Amdocs.Ginger.Plugin.Core;
using Amdocs.Ginger.Repository;
using Ginger.BusinessFlowPages;
using Ginger.SolutionWindows.TreeViewItems.ApplicationModelsTreeItems;
using Ginger.UserControls;
using GingerCore.Platforms.PlatformsInfo;
using GingerWPF.UserControlsLib.UCTreeView;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ginger.BusinessFlowsLibNew.AddActionMenu
{
    /// <summary>
    /// Interaction logic for RecordNavAction.xaml
    /// </summary>
    public partial class RecordNavPage : Page
    {
        public bool IsRecording = false;
        IWindowExplorer mDriver;
        Context mContext;
        RecordingManager mRecordingMngr;
        SingleItemTreeViewSelectionPage mApplicationPOMSelectionPage = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public RecordNavPage(Context context)
        {
            InitializeComponent();

            mContext = context;
            context.PropertyChanged += Context_PropertyChanged;

            SetDriver();          
            SetRecordingControls();
            SetSelectedPOMsGridView();
        }

        /// <summary>
        /// Context Property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e != null &&  e.PropertyName == nameof(Context.Agent) ||  e.PropertyName == nameof(Context.AgentStatus))
            {
                if (IsRecording)
                {
                    IsRecording = false;
                    StopRecording();
                }
                SetDriver();
                SetRecordingControls();
            }
            else if(e != null && e.PropertyName == nameof(Context.Activity))
            {
                SetSelectedPOMsGridView();
                mApplicationPOMSelectionPage = null;
            }
        }

        private void SetSelectedPOMsGridView()
        {
            xSelectedPOMsGrid.SetTitleLightStyle = true;
            GridViewDef view = new GridViewDef(GridViewDef.DefaultViewName);
            view.GridColsView = new ObservableList<GridColView>();
            view.GridColsView.Add(new GridColView() { Field = nameof(POMBindingObjectHelper.ItemName), Header = "Name", WidthWeight = 250, AllowSorting = true, BindingMode = BindingMode.OneWay, ReadOnly = true });
            xSelectedPOMsGrid.btnAdd.Click -= BtnAdd_Click;
            xSelectedPOMsGrid.btnAdd.Click += BtnAdd_Click;
            xSelectedPOMsGrid.SetAllColumnsDefaultView(view);
            xSelectedPOMsGrid.InitViewItems();
            PomModels = new ObservableList<POMBindingObjectHelper>();

            xSelectedPOMsGrid.DataSourceList = PomModels;
        }
        
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {            
            //if (mApplicationPOMSelectionPage == null)
            //{
                ApplicationPOMsTreeItem appModelFolder;
                RepositoryFolder<ApplicationPOMModel> repositoryFolder = WorkSpace.Instance.SolutionRepository.GetRepositoryItemRootFolder<ApplicationPOMModel>();
                appModelFolder = new ApplicationPOMsTreeItem(repositoryFolder);
                mApplicationPOMSelectionPage = new SingleItemTreeViewSelectionPage("Page Objects Model Element", eImageType.ApplicationPOMModel, appModelFolder,
                                                                                        SingleItemTreeViewSelectionPage.eItemSelectionType.MultiStayOpenOnDoubleClick, true,
                                                                                                new Tuple<string, string>(nameof(ApplicationPOMModel.TargetApplicationKey) + "." +
                                                                                                nameof(ApplicationPOMModel.TargetApplicationKey.ItemName),
                                                                                                mContext.Activity.TargetApplication));
                mApplicationPOMSelectionPage.SelectionDone += MAppModelSelectionPage_SelectionDone; 
            //}

            List<object> selectedPOMs = mApplicationPOMSelectionPage.ShowAsWindow();
            AddSelectedPOM(selectedPOMs);
        }

        private void SetDriver()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (mContext.Agent != null && (mContext.Agent.IsSupportRecording() || mContext.Agent.Driver is IRecord))
                {
                    mDriver = mContext.Agent.Driver as IWindowExplorer;

                    xWindowSelectionUC.mWindowExplorerDriver = mDriver;
                    xWindowSelectionUC.mPlatform = PlatformInfoBase.GetPlatformImpl(mContext.Platform);
                    if (mDriver == null)
                    {
                        xWindowSelectionUC.WindowsComboBox.ItemsSource = null;
                    }

                    if (mDriver != null && xWindowSelectionUC.WindowsComboBox.ItemsSource == null)
                    {
                        xWindowSelectionUC.UpdateWindowsList();
                    }

                    if (PlatformInfoBase.GetPlatformImpl(mContext.Platform) != null
                        && PlatformInfoBase.GetPlatformImpl(mContext.Platform).IsPlatformSupportPOM())
                    {
                        xPOMPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        xPOMPanel.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }

        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (xWindowSelectionUC.SelectedWindow != null)
            {
                IsRecording = true;
                StartRecording();
                SetRecordingControls();
                return;
            }
            else
            {
                Reporter.ToUser(eUserMsgKey.TargetWindowNotSelected);
                IsRecording = false;
                SetRecordingControls();              
            }
        }

        private void xStopRecordingBtn_Click(object sender, RoutedEventArgs e)
        {
            IsRecording = false;
            StopRecording();
            SetRecordingControls();
        }

        private void StartRecording()
        {
            IRecord record = (IRecord)mDriver;            
            IPlatformInfo platformInfo = PlatformInfoBase.GetPlatformImpl(mContext.Platform);

            List<ApplicationPOMModel> applicationPOMs = null;
            if (Convert.ToBoolean(xIntegratePOM.IsChecked))
            {
                applicationPOMs = new List<ApplicationPOMModel>();
                foreach (var pom in PomModels)
                {
                    if (pom.IsChecked)
                    {
                        applicationPOMs.Add(pom.ItemObject);
                    }
                } 
            }

            mRecordingMngr = new RecordingManager(applicationPOMs, mContext.BusinessFlow, mContext, record, platformInfo);
            mRecordingMngr.StartRecording();
        }

        public void StopRecording()
        {
            if (mRecordingMngr != null)
            {
                mRecordingMngr.StopRecording();
            }
        }

        private void SetRecordingControls()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (IsRecording)
                {
                    xRecordingButton.ButtonText = "Recording...";
                    xRecordingButton.ToolTip = "Recording Window Operations";
                    xRecordingButton.ButtonImageType = eImageType.Processing;
                    xRecordingButton.IsEnabled = false;
                    xPOMPanel.IsEnabled = false;

                    xStopRecordingBtn.Visibility = Visibility.Visible;

                    xPOMPanel.IsEnabled = false;
                }
                else
                {
                    xRecordingButton.ButtonText = "Record";
                    xRecordingButton.ToolTip = "Start Recording";
                    xRecordingButton.ButtonImageType = eImageType.Camera;
                    xRecordingButton.IsEnabled = true;

                    xStopRecordingBtn.Visibility = Visibility.Collapsed;

                    xPOMPanel.IsEnabled = true;
                }
                xRecordingButton.ButtonStyle = (Style)FindResource("$RoundTextAndImageButtonStyle_Highlighted");
            });
        }

        ObservableList<POMBindingObjectHelper> PomModels = new ObservableList<POMBindingObjectHelper>();
        
        private void MAppModelSelectionPage_SelectionDone(object sender, SelectionTreeEventArgs e)
        {
            AddSelectedPOM(e.SelectedItems);
        }

        private void AddSelectedPOM(List<object> selectedPOMs)
        {
            if (selectedPOMs != null && selectedPOMs.Count > 0)
            {
                foreach (ApplicationPOMModel pom in selectedPOMs)
                {
                    if (!IsPOMAlreadyAdded(pom.ItemName))
                    {
                        ApplicationPOMModel pomToAdd = (ApplicationPOMModel)pom.CreateCopy(false);
                        PomModels.Add(new POMBindingObjectHelper() { IsChecked = true, ItemName = pomToAdd.ItemName, ContainingFolder = pom.ContainingFolder, ItemObject = pom });
                    }
                    else
                    {
                        Reporter.ToUser(eUserMsgKey.StaticWarnMessage, @"""" + pom.ItemName + @""" POM is already added!");
                    }
                }
                xSelectedPOMsGrid.DataSourceList = PomModels;
            }
        }

        private bool IsPOMAlreadyAdded(string itemName)
        {
            bool isPresent = false;
            if(PomModels != null && PomModels.Count > 0)
            {
                foreach (var item in PomModels)
                {
                    if(item.ItemName == itemName)
                    {
                        isPresent = true;
                        break;
                    }
                }
            }
            return isPresent;
        }

        private void XIntegratePOM_Checked(object sender, RoutedEventArgs e)
        {
            xSelectedPOMsGrid.Visibility = (bool)xIntegratePOM.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void XIntegratePOM_Unchecked(object sender, RoutedEventArgs e)
        {
            xSelectedPOMsGrid.Visibility = (bool)xIntegratePOM.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }
    }    
}