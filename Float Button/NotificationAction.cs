using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Float_Button
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    public class NotificationAction : BroadcastReceiver
    {
        Context cont;
        private NotificationManager notifManager;
        private NotificationService ns = new NotificationService();
        private ButtonActivity ba = new ButtonActivity();
        private MainActivity ma = new MainActivity();
        private static bool is_paste;
        public override void OnReceive(Context context, Intent intent)
        {
            cont = context;
            string action = intent.Action;
            Console.WriteLine("On Receive");
            switch(action)
            {
                case "BTN_PASTE":
                    ChangePaste();
                    break;
                case "BTN_NOTIF":
                    CloseNotif();
                    break;
            }

            // close notification
            Intent it = new Intent(Intent.ActionCloseSystemDialogs);
            context.SendBroadcast(it);
        }

        private void ChangePaste()
        {

            if (ba.IsPasteText())
            {
                is_paste = false;

                Toast.MakeText(cont, "Paste Text is Inactive.", ToastLength.Short).Show();
            }
            else
            {
                is_paste = true;

                Toast.MakeText(cont, "Paste Text is Active.", ToastLength.Short).Show();
            }

            Console.WriteLine("Notification ter-klik");

            ba.SetBoolPaste(is_paste);

            ma.ChangeTextPaste();
        }

        private void CloseNotif()
        {
            ns.SetBoolShow(false);

            notifManager = (NotificationManager)ns.GetContext().GetSystemService(Context.NotificationService);

            notifManager.DeleteNotificationChannel("arjunane_tamvan");
            ma.ChangeNotificationText();
        }
    }
}