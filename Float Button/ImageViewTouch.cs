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
    public class ImageViewTouch : ImageView
    {
        static bool isFirstTouch = true;
        Context cont;

        static long max_click_time = 300;
        static bool IsDoubleTap = false;
        static long first_click = 0;
        static long current_down_time = 0;
        static long count_click_run = 0;
        static long total_click { get; set; }

        private ButtonActivity ba = new ButtonActivity();
        private MainActivity ma = new MainActivity();

        private int initialX, initialY, initialTouchX, initialTouchY;
        public ImageViewTouch(Context context) : base(context)
        {
            cont = context;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                initialX = ba.WindowManagerLayoutParams().X;
                initialY = ba.WindowManagerLayoutParams().Y;

                initialTouchX = (int)e.RawX;
                initialTouchY = (int)e.RawY;

                if ((e.DownTime - first_click) > max_click_time)
                {
                    isFirstTouch = true;
                }

                if (isFirstTouch)
                {
                    first_click = e.DownTime;
                    isFirstTouch = false;
                }
                else first_click = current_down_time;

                current_down_time = e.DownTime;
                total_click = current_down_time - first_click;

                if (total_click > 0 && total_click < max_click_time)
                {
                    IsDoubleTap = true;
                }
                else
                {
                    IsDoubleTap = false;
                }
            }

            if (IsDoubleTap)
            {

                if (ba.IsPasteText())
                {
                    ba.SetBoolPaste(false);

                    Console.WriteLine("Count : " + count_click_run + " | False");

                    Toast.MakeText(cont, "Paste Text is Inactive.", ToastLength.Short).Show();

                    ma.ChangeTextPaste();
                }
                else
                {
                    ba.SetBoolPaste(true);

                    Console.WriteLine("Count : " + count_click_run + " | True");

                    Toast.MakeText(cont, "Paste Text is Active.", ToastLength.Short).Show();

                    ma.ChangeTextPaste();
                }

                first_click = 0;
                count_click_run = 0;
                ba.ChangeButton();
                IsDoubleTap = false;
                ma.ChangeTextPaste();
                return true;
            }
            else
            {

                if (e.Action == MotionEventActions.Up && count_click_run < 4)
                {
                    count_click_run = 0;
                }
                else if (e.Action == MotionEventActions.Move && count_click_run > 4)
                {
                    MoveImage(ba.WindowManagerLayoutParams(), e);
                    count_click_run = 0;
                }
            }

            count_click_run++;
            return false;
        }
        public void MoveImage(WindowManagerLayoutParams window, MotionEvent e)
        {
            // window.X = (int)System.Math.Round(e.RawX - initialTouchX);
            // window.Y = (int)System.Math.Round(e.RawY - initialTouchY);
            window.X = initialX + (int)(e.RawX - initialTouchX);
            window.Y = initialY + (int)(e.RawY - initialTouchY);

            ba.WindowManager().UpdateViewLayout(this, window);
        }
    }
}