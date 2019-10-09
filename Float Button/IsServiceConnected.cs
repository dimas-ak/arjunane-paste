using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Float_Button
{
    class IsServiceConnected
    {
        public bool IsConnected(Context context)
        {
            ComponentName expectedComponentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(TouchService)).Name);

            string enabledServicesSetting = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.EnabledAccessibilityServices);

            if (enabledServicesSetting == null)
                return false;

            TextUtils.SimpleStringSplitter colonSplitter = new TextUtils.SimpleStringSplitter(':');
            colonSplitter.SetString(enabledServicesSetting);

            while (colonSplitter.HasNext)
            {
                string componentNameString = colonSplitter.Next();
                ComponentName enabledService = ComponentName.UnflattenFromString(componentNameString);
                if (enabledService != null && enabledService.Equals(expectedComponentName))
                    return true;
            }

            return false;
        }
    }
}