using System;
using Android.Views;

namespace Poprey.Droid.Listeners
{
    class SwipeTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private readonly GestureDetector _gestureListener;

        public SwipeTouchListener(Action onSwipeRight = null, Action onSwipeLeft = null,
                                     Action onSwipeBottom = null, Action onSwipeTop = null)
        {
            _gestureListener = new GestureDetector(new SwipeGestureListener(onSwipeRight, onSwipeLeft, onSwipeBottom, onSwipeTop));
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return _gestureListener.OnTouchEvent(e);
        }
    }
}