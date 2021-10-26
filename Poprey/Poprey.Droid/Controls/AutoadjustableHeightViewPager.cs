using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Poprey.Droid.Util;

namespace Poprey.Droid.Controls
{
    public class AutoadjustableHeightViewPager: ViewPager
    {
        private View _currentView;

        protected AutoadjustableHeightViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AutoadjustableHeightViewPager(Context context) : base(context)
        {
        }

        public AutoadjustableHeightViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (_currentView == null && ChildCount == 0)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                return;
            }

            var mode = MeasureSpec.GetMode(heightMeasureSpec);
            // Unspecified means that the ViewPager is in a ScrollView WRAP_CONTENT.
            // At Most means that the ViewPager is not in a ScrollView WRAP_CONTENT.
            if (mode == MeasureSpecMode.Unspecified || mode == MeasureSpecMode.AtMost)
            {
                // super has to be called in the beginning so the child views can be initialized.
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

                var height = 0;
                for (var i = 0; i < ChildCount; i++)
                {
                    _currentView.Measure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                    if (_currentView.MeasuredHeight > height)
                    {
                        height = _currentView.MeasuredHeight;
                    }
                }

                var fullScreenHeight = GetFullScreenHeight();

                if (fullScreenHeight > height)
                {
                    height = fullScreenHeight;
                }

                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            }
            // super has to be called again so the new specs are treated as exact measurements
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        private int GetFullScreenHeight()
        {
            var currentViewLocations = new int[2];
            GetLocationInWindow(currentViewLocations);

            var screenHeight = WindowUtil.GetFullScreenHeight(Context);
            return screenHeight - currentViewLocations[1];
        }

        public void MeasureCurrentView(View currentView)
        {
            _currentView = currentView;

            RequestLayout();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }
    }
}