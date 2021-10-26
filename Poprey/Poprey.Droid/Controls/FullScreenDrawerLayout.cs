using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace Poprey.Droid.Controls
{
    public class FullScreenDrawerLayout : DrawerLayout
    {
        public FullScreenDrawerLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public FullScreenDrawerLayout(Context context) : base(context)
        {
        }

        public FullScreenDrawerLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public FullScreenDrawerLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }


        private const int MIN_DRAWER_MARGIN = 0; // dp

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            var widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            var heightSize = MeasureSpec.GetSize(heightMeasureSpec);

            if (widthMode != MeasureSpecMode.Exactly || heightMode != MeasureSpecMode.Exactly)
            {
                throw new IllegalArgumentException(
                    "DrawerLayout must be measured with MeasureSpec.EXACTLY.");
            }

            SetMeasuredDimension(widthSize, heightSize);

            // Gravity value for each drawer we've seen. Only one of each permitted.
            const GravityFlags foundDrawers = 0;
            var childCount = ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = GetChildAt(i);

                if (child.Visibility == ViewStates.Gone)
                {
                    continue;
                }

                var lp = (LayoutParams) child.LayoutParameters;

                if (IsContentView(child))
                {
                    // Content views get measured at exactly the layout's size.
                    var contentWidthSpec = MeasureSpec.MakeMeasureSpec(
                        widthSize - lp.LeftMargin - lp.RightMargin, MeasureSpecMode.Exactly);
                    var contentHeightSpec = MeasureSpec.MakeMeasureSpec(
                        heightSize - lp.TopMargin - lp.BottomMargin, MeasureSpecMode.Exactly);

                    child.Measure(contentWidthSpec, contentHeightSpec);
                }
                else if (IsDrawerView(child))
                {
                    var childGravity = getDrawerViewGravity(child) & GravityFlags.HorizontalGravityMask;

                    if ((foundDrawers & childGravity) != 0)
                    {
                        throw new IllegalStateException("Child drawer has absolute gravity " +
                                                        GravityToString(childGravity) + " but this already has a " +
                                                        "drawer view along that edge");
                    }

                    int drawerWidthSpec = GetChildMeasureSpec(widthMeasureSpec,
                        MIN_DRAWER_MARGIN + lp.LeftMargin + lp.RightMargin,
                        lp.Width);
                    int drawerHeightSpec = GetChildMeasureSpec(heightMeasureSpec,
                        lp.TopMargin + lp.BottomMargin,
                        lp.Height);
                    child.Measure(drawerWidthSpec, drawerHeightSpec);
                }
                else
                {
                    throw new IllegalStateException("Child " + child + " at index " + i +
                                                    " does not have a valid layout_gravity - must be Gravity.LEFT, " +
                                                    "Gravity.RIGHT or Gravity.NO_GRAVITY");
                }
            }
        }

        static bool IsContentView(View child)
        {
            return ((LayoutParams) child.LayoutParameters).Gravity == (int)GravityFlags.NoGravity;
        }

        bool IsDrawerView(View child)
        {
            var gravity = (GravityFlags)((LayoutParams)child.LayoutParameters).Gravity;
            var absGravity = Gravity.GetAbsoluteGravity(gravity, (GravityFlags)child.LayoutDirection);

            return (absGravity & (GravityFlags.Left | GravityFlags.Right)) != 0;
        }

        GravityFlags getDrawerViewGravity(View drawerView)
        {
            var gravity = (GravityFlags)((LayoutParams)drawerView.LayoutParameters).Gravity;
            return Gravity.GetAbsoluteGravity(gravity, (GravityFlags)drawerView.LayoutDirection);
        }

        static string GravityToString(GravityFlags gravity)
        {
            if ((gravity & GravityFlags.Left) == GravityFlags.Left)
            {
                return "LEFT";
            }
            if ((gravity & GravityFlags.Right) == GravityFlags.Right)
            {
                return "RIGHT";
            }
            return gravity.ToString();
        }
    }
}