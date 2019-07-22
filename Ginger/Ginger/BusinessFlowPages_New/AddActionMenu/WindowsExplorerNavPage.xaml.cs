﻿using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.UIElement;
using Ginger.BusinessFlowPages;
using Ginger.WindowExplorer;
using GingerCore.Platforms;
using GingerCoreNET;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Ginger.BusinessFlowsLibNew.AddActionMenu
{
    /// <summary>
    /// Interaction logic for WindowsExplorerNavPage.xaml
    /// </summary>
    public partial class WindowsExplorerNavPage : Page
    {
        Context mContext;
        IWindowExplorer mDriver;
        List<AgentPageMappingHelper> mWinExplorerPageList = null;
        WindowExplorerPage mCurrentLoadedPage = null;

        public WindowsExplorerNavPage(Context context)
        {
            InitializeComponent();

            mContext = context;            
            context.PropertyChanged += Context_PropertyChanged;

            if (mContext.Agent != null)
            {
                mDriver = mContext.Agent.Driver as IWindowExplorer;
            }
            else
            {
                mDriver = null;
            }

            LoadWindowExplorerPage();            
        }
        
        /// <summary>
        /// Context Property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Context.AgentStatus) || e.PropertyName == nameof(Context.Agent))
            {
                if (mContext.Agent != null)
                {
                    mDriver = mContext.Agent.Driver as IWindowExplorer;
                }
                else
                {
                    mDriver = null;
                }

                if (e.PropertyName == nameof(Context.Agent) && mContext.Agent != null)
                {
                    LoadWindowExplorerPage();
                }
                else
                {
                    mCurrentLoadedPage.SetDriver(mDriver);
                }
            }
        }

        /// <summary>
        /// This method is used to do the search
        /// </summary>
        /// <returns></returns>
        public bool DoSearchControls()
        {            
            return mCurrentLoadedPage.DoSearchControls();
        }

        /// <summary>
        /// This method is used to get the new WindowExplorerPage based on Context and Agent
        /// </summary>
        /// <returns></returns>
        private void LoadWindowExplorerPage()
        {
            this.Dispatcher.Invoke(() =>
            {
                bool isLoaded = false;
                if (mWinExplorerPageList != null && mWinExplorerPageList.Count > 0)
                {
                    AgentPageMappingHelper objHelper = mWinExplorerPageList.Where(x => x.ObjectAgent.DriverType == mContext.Agent.DriverType &&
                                                                                    x.ObjectAgent.ItemName == mContext.Agent.ItemName).FirstOrDefault();
                    if (objHelper != null && objHelper.ObjectWindowPage != null)
                    {
                        mCurrentLoadedPage = (WindowExplorerPage)objHelper.ObjectWindowPage;
                        isLoaded = true;
                    }
                }

                if (!isLoaded)
                {
                    ApplicationAgent appAgent = AgentHelper.GetAppAgent(mContext.BusinessFlow.CurrentActivity, mContext.Runner, mContext);
                    if (appAgent != null)
                    {
                        mCurrentLoadedPage = new WindowExplorerPage(appAgent, mContext);
                        mCurrentLoadedPage.SetDriver(mDriver);
                        if (mWinExplorerPageList == null)
                        {
                            mWinExplorerPageList = new List<AgentPageMappingHelper>();
                        }
                        mWinExplorerPageList.Add(new AgentPageMappingHelper(mContext.Agent, mCurrentLoadedPage));
                    }
                }

                xSelectedItemFrame.Content = mCurrentLoadedPage;
            });
        }
    }
}
