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
using GingerCore.Actions.Common;
using GingerCore.Platforms.PlatformsInfo;
using GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace Ginger.Actions._Common.ActUIElementLib
{
    /// <summary>
    /// Interaction logic for UIElementMouseClickAndValidate.xaml
    /// </summary>
    public partial class UIElementClickAndValidateEditPage : Page
    {
        public ActUIElement mAct;        
        public enum eClickType
        {
            [EnumValueDescription("Click")]
            Click,
            [EnumValueDescription("Simple Click")]
            SimpleClick,
            [EnumValueDescription("Click At")]
            ClickAt,
            [EnumValueDescription("Mouse Click")]
            MouseClick,
            [EnumValueDescription("Async Click")]
            AsyncClick
        }

        public enum eValidationType
        {
            [EnumValueDescription("Exist")]
            Exist,
            [EnumValueDescription("Does not exist")]
            NotExist,
            [EnumValueDescription("Enabled")]
            Enabled,
            [EnumValueDescription("Visible")]
            Visible
        }

        public UIElementClickAndValidateEditPage(ActUIElement Act, PlatformInfoBase mPlatform)
        {
            mAct = Act;
            InitializeComponent();

            //TODO: Binding of all UI elements
            ClickType.Init(mAct.GetOrCreateInputParam(ActUIElement.Fields.ClickType), mPlatform.GetPlatformUIClickTypeList(), false, null);
            ValidationType.Init(mAct.GetOrCreateInputParam(ActUIElement.Fields.ValidationType), mPlatform.GetPlatformUIValidationTypesList(), false, null);
            ValidationElement.Init(mAct.GetOrCreateInputParam(ActUIElement.Fields.ValidationElement), mPlatform.GetPlatformUIElementsType(), false, null);
            LocateByComboBox.Init(mAct.GetOrCreateInputParam(ActUIElement.Fields.ValidationElementLocateBy), mPlatform.GetPlatformUIElementLocatorsList(), false, null);
            LocatorValue.Init(mAct.GetOrCreateInputParam(ActUIElement.Fields.ValidationElementLocatorValue), true, false, UCValueExpression.eBrowserType.Folder);
            GingerCore.General.ActInputValueBinding(LoopThroughClicks, CheckBox.IsCheckedProperty, mAct.GetOrCreateInputParam(ActUIElement.Fields.LoopThroughClicks, "False"));            
        }    

        public Page GetPlatformEditPage()
        {
            PlatformInfoBase mPlatform = null;
            if (mPlatform != null)
            {
                string pageName = mPlatform.GetPlatformGenericElementEditControls();
                if (!String.IsNullOrEmpty(pageName))
                {
                    string classname = "Ginger.Actions." + pageName;
                    Type t = Assembly.GetExecutingAssembly().GetType(classname);
                    if (t == null)
                    {
                        throw new Exception("Action edit page not found - " + classname);
                    }
                    Page platformPage = (Page)Activator.CreateInstance(t, mAct);

                    if (platformPage != null)
                    {
                        return platformPage;
                    }
                } 
            }
            return null;
        }

        private ePlatformType GetActionPlatform()
        {
            string targetapp = App.BusinessFlow.CurrentActivity.TargetApplication;
            ePlatformType platform = (from x in App.UserProfile.Solution.ApplicationPlatforms where x.AppName == targetapp select x.Platform).FirstOrDefault();
            return platform;
        }
    }
}
