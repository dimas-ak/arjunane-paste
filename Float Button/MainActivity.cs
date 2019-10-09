using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Xamarin.Forms;
using System;
using Xamarin.Essentials;
using Android.Views.Accessibility;
using Android.AccessibilityServices;
using Android;
using Android.InputMethodServices;
using Java.Lang;
using System.Threading.Tasks;
using Android.Views.Animations;
using Android.Text;

namespace Float_Button
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IWindowManager windowManager;

        private ButtonActivity ba;
        private static int max_cursor_index;
        private static string color_is_paste;
        private static Android.Content.ClipboardManager clipboard;
        private NotificationService ns = new NotificationService();
        private IsServiceConnected serviceConnected = new IsServiceConnected();

        private EditText input_characters, input_cursor;

        private Android.Widget.Button submit, btn_float, btn_open_as;

        private TextView info_characters, info_cursor, max_cursor;

        private static Android.Widget.Button btn_notification;
        private static TextView text_is_paste;

        private TouchService ts = new TouchService();
        SharedData sd = new SharedData();
        public virtual Android.Views.InputMethods.IInputConnection CurrentInputConnection { get; }
        /*;
        private static ActivityManager am;
        private static IList<RunningAppProcessInfo> taskInfo { get; set; }
        */
        WindowManagerLayoutParams paramsF;
        private static Android.Widget.ImageView btnImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            input_characters    = (EditText)                FindViewById(Resource.Id.input_characters);
            input_cursor        = (EditText)                FindViewById(Resource.Id.input_cursor);
            submit              = (Android.Widget.Button)   FindViewById(Resource.Id.submit_button);
            btn_float            = (Android.Widget.Button)   FindViewById(Resource.Id.btn_float);
            btn_open_as         = (Android.Widget.Button)   FindViewById(Resource.Id.btn_open_as);
            btn_notification     = (Android.Widget.Button)   FindViewById(Resource.Id.btn_notification);
            info_characters     = (TextView)                FindViewById(Resource.Id.text_characters);
            info_cursor         = (TextView)                FindViewById(Resource.Id.text_cursor);
            max_cursor          = (TextView)                FindViewById(Resource.Id.max_cursor);
            text_is_paste       = (TextView)                FindViewById(Resource.Id.text_is_paste_active);

            //clipboard = (Android.Content.ClipboardManager)GetSystemService(ClipboardService);
            
            ba = new ButtonActivity(this);

            Console.WriteLine("Is Paste : " + ba.IsPasteText() );
            ChangeTextPaste();
            ChangeButtonFloatText();
            ChangeNotificationText();
            ChangeButtonAccessibilityServiceText();

            btn_open_as.Click += Btn_open_as_Click;

            btn_float.Click += Btn_float_Click;

            btn_notification.Click += Btn_notification_Click;

            SubmitEntry();
            Console.WriteLine("On Create Main Activity");
        }

        private void Btn_notification_Click(object sender, EventArgs e)
        {
            bool notif_bool = ns.IsShowNotification() ? false : true;

            ns.SetBoolShow(notif_bool);

            ns.ShowNotification(this);

            ChangeNotificationText();
        }

        public void ChangeNotificationText()
        {
            string text_notification = "TURN NOTIFICATION : " + (ns.IsShowNotification() ? "ON" : "OFF");
            btn_notification.Text = text_notification;
        }

        protected override void OnStart()
        {
            base.OnStart();
            ChangeButtonAccessibilityServiceText();
            ChangeButtonFloatText();
            ChangeNotificationText();
            ChangeTextPaste();
        }

        private void Btn_open_as_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
            StartActivityForResult(intent, 0);

            ChangeButtonAccessibilityServiceText();
        }

        public void ChangeTextPaste()
        {
            ba = new ButtonActivity(this);

            try
            {
                color_is_paste = ba.IsPasteText() ? "#2ecc71" : "#e74c3c";
                text_is_paste.SetBackgroundColor(Android.Graphics.Color.ParseColor(color_is_paste));

                text_is_paste.Text = ba.IsPasteText() ? "ON" : "OFF";
                Console.WriteLine("Ganti Text Paste : " + text_is_paste.Text);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error Warna : " + ex);
                Console.WriteLine("Error Warna : " + ex.Message);
            }

        }

        private void ChangeButtonAccessibilityServiceText()
        {
            string text_btn_open_as = "Accessibility Service : " + (serviceConnected.IsConnected(this) ? "Active" : "Inactive");


            btn_open_as.Text = text_btn_open_as;
        }

        private void ChangeButtonFloatText()
        {
            string text_btn_float = "Button Float : " + (btnImage == null ? "Inactive" : "Active");
            btn_float.Text = text_btn_float;
        }

        private void Btn_float_Click(object sender, EventArgs e)
        {
            if(btnImage == null)
            {
                if (Android.Provider.Settings.CanDrawOverlays(this))
                {
                    ShowFloatView();
                }
                else
                {
                    var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission);
                    StartActivityForResult(intent, 0);
                }
            }
            else
            {
                ShowFloatView(false);
            }

            ChangeButtonFloatText();
        }

        

        private void SubmitEntry()
        {
            input_characters.Text   = sd.GetData("characters");
            input_cursor.Text       = sd.GetData("cursor_index");

            max_cursor_index    =  input_characters.Text.Length;
            max_cursor.Text     = max_cursor_index + " ]";

            info_characters.Visibility  = ViewStates.Invisible;
            info_cursor.Visibility      = ViewStates.Invisible;

            input_characters.TextChanged += (s, e) => 
            {
                int length = input_characters.Text.Length;
                max_cursor.Text = length + " ]";
            };

            submit.Click += (s, e) => 
            {
                info_characters.Visibility  = (input_characters.Text.Length == 0)   ? ViewStates.Visible : ViewStates.Invisible;
                info_cursor.Visibility      = (input_cursor.Text.Length == 0)       ? ViewStates.Visible : ViewStates.Invisible;

                if(input_characters.Text.Length == 0 || input_characters.Text.Length == 0)
                {
                    Toast.MakeText(this, "Please, fill all inputs.", ToastLength.Long).Show();
                    Console.WriteLine("Please");
                }
                else if(int.Parse(input_cursor.Text) > max_cursor_index)
                {
                    Toast.MakeText(this, "Max. Index cursor is " + max_cursor_index, ToastLength.Long).Show();
                    Console.WriteLine("Max");
                }
                else
                {
                    sd.SetData("characters",  input_characters.Text);
                    sd.SetData("cursor_index",      input_cursor.Text);
                    Toast.MakeText(this, "Done Kang.", ToastLength.Long).Show();
                    Console.WriteLine("Done Kang");
                }
                Console.WriteLine("Input Cursor Text : " + input_cursor.Text);
            };
        }

        private void ShowFloatView(bool isShowButton = true)
        {
            if(isShowButton)
            {
                windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
                if (btnImage != null)
                {
                    windowManager.RemoveView(btnImage);
                }

                btnImage = new ImageViewTouch(Android.App.Application.Context);
                btnImage.SetPadding(10, 10, 10, 10);

                ba.GetImageView(btnImage);
                btnImage.Background = GetDrawable(Resource.Drawable.shadow);
                btnImage.SetImageDrawable(GetDrawable(Resource.Drawable.power));

                paramsF = new WindowManagerLayoutParams(
                    95, //width
                    95, //height
                    WindowManagerTypes.ApplicationOverlay,
                    WindowManagerFlags.NotFocusable | WindowManagerFlags.NotTouchModal,
                    Format.Transparent)
                {
                    Gravity = GravityFlags.Top | GravityFlags.Left,
                    X = 100,
                    Y = 150
                };
                paramsF.HorizontalMargin = 0;
                paramsF.VerticalMargin = 0;
                paramsF.WindowAnimations = 10;
                ba.GetWindowManagerLayoutParams(paramsF);
                ba.GetWindowManager(windowManager);
                windowManager.AddView(btnImage, paramsF);
            }
            else
           {
                try
                {
                    windowManager.RemoveView(btnImage);
                    btnImage = null;
                }
                catch(System.Exception ex)
                {
                    Console.WriteLine("Error btnImage : " + ex);
                }
           }
        }
        /*

        public void SetText()
        {
            AccessibilityNodeInfo nodeInfo = ts.NodeInfo();
            if(nodeInfo != null)
            {
                ClipData clip = ClipData.NewPlainText("tamvan", "-=[  ]=-");
                clipboard.PrimaryClip = clip;
                nodeInfo.PerformAction(Android.Views.Accessibility.Action.Paste);

                Bundle arguments = new Bundle();
                arguments.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionStartInt, 4);
                arguments.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionEndInt, 4);
                nodeInfo.PerformAction(Android.Views.Accessibility.Action.SetSelection, arguments);

                nodeInfo.Refresh();

                Console.WriteLine("Text EditText : " + nodeInfo.Text);
            }
            
        }
        */

        protected override void OnPause()
        {
            base.OnPause();
        }

    }

}