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
    public class ButtonActivity
    {
        public static bool IsPaste = true;

        private static Context context;
        private static WindowManagerLayoutParams window;
        private static IWindowManager windowManager;
        private static ImageView imageView;
        public ButtonActivity(Context cont = null)
        {
            if(cont != null) context = cont;
        }
        public void GetImageView(Android.Widget.ImageView iv)
        {
            imageView = iv;
        }
        public void GetWindowManagerLayoutParams(WindowManagerLayoutParams _window)
        {
            window = _window;
        }
        public void GetWindowManager(IWindowManager _windowManager)
        {
            windowManager = _windowManager;
        }
        public WindowManagerLayoutParams WindowManagerLayoutParams()
        {
            return window;
        }

        public void SetBoolPaste(bool isPaste)
        {
            IsPaste = isPaste;
        }

        public bool IsPasteText()
        {
            return IsPaste;
        }
        public IWindowManager WindowManager()
        {
            return windowManager;
        }
        public void ChangeButton()
        {
            if (IsPaste)
            {
                imageView.Background = context.GetDrawable(Resource.Drawable.shadow);
            }
            else
            {
                imageView.Background = null;
            }
        }
    }
}