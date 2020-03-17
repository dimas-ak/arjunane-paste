using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View.Accessibility;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using Action = Android.Views.Accessibility.Action;

namespace Float_Button
{
    [   
        Service(Label = "-=[ Touch Service ]=-", Permission = Manifest.Permission.BindAccessibilityService), 
        MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service_config")
    ]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    public class TouchService : AccessibilityService
    {
        private static AccessibilityNodeInfo _nodeInfo;
        ButtonActivity ba = new ButtonActivity();
        SharedData sd = new SharedData();

        static long delay_paste = 300;
        static long current_time = 0;
        static long first_time = 0;
        static long compare_time = 0;
        static int click_time = 0;

        public override void OnCreate()
        {
            base.OnCreate();
            Console.WriteLine("On Create Service");
        }
        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            Console.WriteLine("On aksesibility");
            try
            {

                AccessibilityNodeInfo nodeInfo = e.Source;

                string nodeText = nodeInfo.Text;
                string nodeHint = nodeInfo.HintText;
                int startCursor = nodeInfo.TextSelectionStart;
                int endCursor   = nodeInfo.TextSelectionEnd;

                Console.WriteLine("Action       : " + e.GetAction());
                Console.WriteLine("ClassName    : " + e.ClassName);

                if ( ba.IsPasteText() && e.ClassName.IndexOf("EditText") != -1 && 
                     (  
                        nodeText.Length == 0 || 
                        (nodeText.Length != 0 && startCursor == -1 && endCursor == -1) || 
                        nodeText == nodeHint) 
                     )
                {

                    if (first_time == 0)
                    {
                        first_time = e.EventTime;
                    }
                    else
                    {
                        current_time = e.EventTime;
                    }

                    compare_time = current_time - first_time;

                    if (compare_time < delay_paste && compare_time > 100)
                    {
                        click_time += 1;
                    }

                    if(compare_time > delay_paste)
                    {
                        click_time = 0;
                        first_time = e.EventTime;
                    }

                    if(click_time == 1)
                    {
                        ClipboardManager clipboard = (ClipboardManager)GetSystemService(ClipboardService);
                        ClipData clip = ClipData.NewPlainText("tamvan", sd.GetData("characters"));
                        clipboard.PrimaryClip = clip;

                        nodeInfo.PerformAction(Action.Paste);

                        Bundle arguments = new Bundle();

                        arguments.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionStartInt, int.Parse(sd.GetData("cursor_index")));
                        arguments.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionEndInt, int.Parse(sd.GetData("cursor_index")));
                        nodeInfo.PerformAction(Action.SetSelection, arguments);
                    }

                    /*
                    */
                    //nodeInfo.PerformAction(Action.Click); //set focus into EditText

                }
                Console.WriteLine("First Time       : " + first_time);
                Console.WriteLine("Current Time     : " + current_time);
                Console.WriteLine("Compare Time     : " + compare_time);
                Console.WriteLine("Click Time       : " + click_time);


                nodeInfo.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error access : " + ex);
                Console.WriteLine("error access : " + ex.Message);
            }

        }

        public AccessibilityNodeInfo NodeInfo()
        {
            return _nodeInfo ?? null;
        }

        public override void OnInterrupt()
        {
            throw new NotImplementedException();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        /*
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            Console.WriteLine("-onServiceConnected-");
            var accessibilityServiceInfo = new AccessibilityServiceInfo
            {
                Flags = Android.AccessibilityServices.AccessibilityServiceFlags.Default |
                        Android.AccessibilityServices.AccessibilityServiceFlags.RetrieveInteractiveWindows |
                        Android.AccessibilityServices.AccessibilityServiceFlags.IncludeNotImportantViews |
                        Android.AccessibilityServices.AccessibilityServiceFlags.ReportViewIds,//enum 1 - AccessibilityServiceFlags.Default | 
                EventTypes = EventTypes.AllMask,
                NotificationTimeout = 100,
                FeedbackType = Android.AccessibilityServices.FeedbackFlags.Generic // | Android.AccessibilityServices.FeedbackFlags.Generic
            };
            AccessibilityServiceInfo getInfo = accessibilityServiceInfo;//pm.GetServiceInfo(cm, PackageInfoFlags.Services);
            Console.WriteLine("Info Flags : " + getInfo.Flags);
            Console.WriteLine("Event Types: " + getInfo.EventTypes);
            Console.WriteLine("Description: " + getInfo.Description);

            SetServiceInfo(Android.AccessibilityServices.AccessibilityServiceInfo.);
        }
        */
    }
}