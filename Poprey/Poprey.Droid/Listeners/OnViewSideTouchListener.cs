using System;
using Android.Content.Res;
using Android.Views;
using Poprey.Droid.Util;

namespace Poprey.Droid.Listeners
{
    public class OnViewSideTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private const int RightClickOffset = 150;

        private readonly Action _onLeftTouched;
        private readonly Action _onRightTouched;
        private readonly Resources _resources;

        private float _startX;
        private float _startY;

        public OnViewSideTouchListener(Resources resources, Action onLeftTouched, Action onRightTouched)
        {
            _resources = resources;
            _onLeftTouched = onLeftTouched;
            _onRightTouched = onRightTouched;
        }

        public bool OnTouch(View view, MotionEvent e)
        {
            var action = e.Action;

            switch (action)
            {
                case MotionEventActions.Down:
                    _startX = e.RawX;
                    _startY = e.RawY;
                    break;
                case MotionEventActions.Up:
                    var endX = e.RawX;
                    var endY = e.RawY;
                    if (IsAClick(_startX, endX, _startY, endY))
                    {
                        if (IsRightPartOfViewPager(view, _startX))
                        {
                            _onRightTouched?.Invoke();
                        }
                        else if (IsLeftPartOfViewPager(_startX))
                        {
                            _onLeftTouched?.Invoke();
                        }
                        return true;
                    }
                    break;
            }

            return false;
        }

        private static bool IsAClick(float startX, float endX, float startY, float endY)
        {
            var clickActionThreshold = 5;

            var differenceX = Math.Abs(startX - endX);
            var differenceY = Math.Abs(startY - endY);
            return !(differenceX > clickActionThreshold || differenceY > clickActionThreshold);
        }

        private bool IsLeftPartOfViewPager(float startX)
        {
            return startX < DpConverter.ConvertDpToPx(RightClickOffset, _resources);
        }

        private bool IsRightPartOfViewPager(View view, float startX)
        {
            return view.Width - startX < DpConverter.ConvertDpToPx(RightClickOffset, _resources);
        }
    }
}