using Android.Views;
using System;

namespace Poprey.Droid.Listeners
{
    public class SwipeGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private const int SwipeThreshold = 100;
        private const int SwipeVelocityThreshold = 100;

        private readonly Action _onSwipeRight;
        private readonly Action _onSwipeLeft;
        private readonly Action _onSwipeBottom;
        private readonly Action _onSwipeTop;

        public SwipeGestureListener(Action onSwipeRight = null, Action onSwipeLeft = null, 
                                    Action onSwipeBottom = null, Action onSwipeTop = null)
        {
            _onSwipeRight = onSwipeRight;
            _onSwipeLeft = onSwipeLeft;
            _onSwipeBottom = onSwipeBottom;
            _onSwipeTop = onSwipeTop;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            var result = false;

            try
            {
                var diffY = e2.GetY() - e1.GetY();
                var diffX = e2.GetX() - e1.GetX();

                if (Math.Abs(diffX) > Math.Abs(diffY))
                {
                    if (Math.Abs(diffX) > SwipeThreshold && Math.Abs(velocityX) > SwipeVelocityThreshold)
                    {
                        if (diffX > 0)
                        {
                            result = OnSwipeRight();
                        }
                        else
                        {
                            result = OnSwipeLeft();
                        }
                    }
                }
                else
                {
                    if (Math.Abs(diffY) > SwipeThreshold && Math.Abs(velocityY) > SwipeVelocityThreshold)
                    {
                        if (diffY > 0)
                        {
                            result = OnSwipeBottom();
                        }
                        else
                        {
                            result = OnSwipeTop();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        private bool OnSwipeRight()
        {
            _onSwipeRight?.Invoke();
            return false;
        }

        private bool OnSwipeLeft()
        {
            _onSwipeLeft?.Invoke();
            return false;
        }

        private bool OnSwipeTop()
        {
            _onSwipeTop?.Invoke();
            return false;
        }

        private bool OnSwipeBottom()
        {
            _onSwipeBottom?.Invoke();
            return false;
        }
    }
}
