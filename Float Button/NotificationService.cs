using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Float_Button
{
    public class NotificationService
    {
        private static bool IsShow = false;
        private string CLICK_PASTE = "BTN_PASTE";
        private string CLICK_NOTIF = "BTN_NOTIF";
        private static Context cont;
        private string chanel_id = "arjunane_tamvan";
        private static NotificationManager mNotificationManager;
        public void SetBoolShow(bool isShow)
        {
            IsShow = isShow;
        }
        public bool IsShowNotification()
        {
            Console.WriteLine("Notification Show : " + IsShow);
            return IsShow;
        }
        public Context GetContext()
        {
            return cont;
        }
        public void ShowNotification(Context context)
        {
            cont = context;
            Intent btn_paste = new Intent(context, typeof(NotificationAction));

            Intent btn_main = new Intent(context, typeof(MainActivity));

            PendingIntent open_app = PendingIntent.GetActivity(context, 0, btn_main, PendingIntentFlags.UpdateCurrent);

            btn_paste.SetAction(CLICK_PASTE);
            PendingIntent pending_paste = PendingIntent.GetBroadcast(context, 1, btn_paste, PendingIntentFlags.UpdateCurrent);
            NotificationCompat.Action action_paste = new NotificationCompat.Action.Builder(Resource.Drawable.coding, "ON/OFF PASTE", pending_paste).Build();

            btn_paste.SetAction(CLICK_NOTIF);
            PendingIntent pending_notif = PendingIntent.GetBroadcast(context, 2, btn_paste, PendingIntentFlags.UpdateCurrent);
            NotificationCompat.Action action_notif = new NotificationCompat.Action.Builder(Resource.Drawable.coding, "TURN OFF NOTIF", pending_notif).Build();


            if (IsShow)
            {

                mNotificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    NotificationChannel channel = new NotificationChannel(
                                                        chanel_id,
                                                        "Arjunane tamVan",
                                                        Android.App.NotificationImportance.Low
                                                    );
                    channel.SetShowBadge(false);

                    mNotificationManager.CreateNotificationChannel(channel);

                }

                NotificationCompat.Builder builder = new NotificationCompat.Builder(context, chanel_id);

                builder.SetSmallIcon(Resource.Drawable.power)
                        .SetContentTitle("Arjunane Paste")
                        .SetContentText("Turn ON/OFF the Paste Text.")
                        .SetPriority(NotificationCompat.PriorityMax)
                        .SetSound(null)
                        .SetContentIntent(open_app)
                        .AddAction(action_paste)
                        .AddAction(action_notif)
                        .SetOngoing(true);

                builder.SetChannelId(chanel_id);
                mNotificationManager.Notify(0, builder.Build());
            }
            else 
            {
                mNotificationManager.DeleteNotificationChannel("arjunane_tamvan");
            }
        }
    }
}