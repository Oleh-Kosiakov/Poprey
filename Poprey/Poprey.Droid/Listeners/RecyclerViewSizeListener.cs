using System;
using Android.Views;

namespace Poprey.Droid.Listeners
{
    public class RecyclerViewSizeListener : Java.Lang.Object, View.IOnLayoutChangeListener
    {
        private readonly Action<int> _onHeightChangedAction;

        public RecyclerViewSizeListener(Action<int> onHeightChangedAction)
        {
            _onHeightChangedAction = onHeightChangedAction;
        }

        public void OnLayoutChange(View view, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight,int oldBottom)
        {
            var heightWas = oldBottom - oldTop;

            if (view.Height != 0 && view.Height != heightWas)
            {
                _onHeightChangedAction?.Invoke(view.Height);
            }
        }
    }
}