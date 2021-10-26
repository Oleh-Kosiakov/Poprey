using System;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;

namespace Poprey.Droid.Listeners
{
    public class KeyboardVisibilityListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly View _parentView;
        private readonly Action<bool> _onKeyboardToggled;

        private bool _alreadyOpen;
        private const int DefaultKeyboardHeightDp = 100;
        private readonly int _estimatedKeyboardDp = DefaultKeyboardHeightDp + (Build.VERSION.SdkInt >= BuildVersionCodes.M ? 48 : 0);
        private readonly Rect _rect = new Rect();


        public KeyboardVisibilityListener(View parentView, Action<bool> onKeyboardToggled)
        {
            _parentView = parentView;
            _onKeyboardToggled = onKeyboardToggled;
        }

       
        public void OnGlobalLayout()
        {
            var estimatedKeyboardHeight = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, _estimatedKeyboardDp, _parentView.Resources.DisplayMetrics);
            _parentView.GetWindowVisibleDisplayFrame(_rect);
            var heightDiff = _parentView.RootView.Height - (_rect.Bottom - _rect.Top);
            var isShown = heightDiff >= estimatedKeyboardHeight;

            if (isShown == _alreadyOpen)
            {
                return;
            }
            _alreadyOpen = isShown;
            _onKeyboardToggled?.Invoke(isShown);
        }
    }
}