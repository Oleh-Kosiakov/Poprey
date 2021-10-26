using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using Poprey.Droid.Controls;
using Object = Java.Lang.Object;

namespace Poprey.Droid.Adapters
{
    public class SwipeMenuAdapter : MvxCachingFragmentStatePagerAdapter
    {
        private int _mCurrentPosition=-1;

        protected SwipeMenuAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SwipeMenuAdapter(Context context, FragmentManager fragmentManager, List<MvxViewPagerFragmentInfo> fragmentsInfo) : base(context, fragmentManager, fragmentsInfo)
        {
        }

        public override void SetPrimaryItem(ViewGroup container, int position, Object objectValue)
        {
            base.SetPrimaryItem(container, position, objectValue);

            if (position == _mCurrentPosition)
                return;

            var fragment = (Fragment)objectValue;
            var pager = (AutoadjustableHeightViewPager)container;

            if (fragment?.View == null)
                return;

            _mCurrentPosition = position;
            pager.MeasureCurrentView(fragment.View);
        }
    }
}