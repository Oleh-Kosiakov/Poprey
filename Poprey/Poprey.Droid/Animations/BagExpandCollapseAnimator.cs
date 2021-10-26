using System.Threading.Tasks;
using Android.Views;
using Android.Views.Animations;

namespace Poprey.Droid.Animations
{
    public static class BagExpandCollapseAnimator
    {
        public static Task Expand(View viewToAnimate)
        {
            var matchParentMeasureSpec =
                View.MeasureSpec.MakeMeasureSpec(((View)viewToAnimate.Parent).Width, MeasureSpecMode.Exactly);

            var wrapContentMeasureSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            viewToAnimate.Measure(matchParentMeasureSpec, wrapContentMeasureSpec);
            var targetHeight = viewToAnimate.MeasuredHeight;
            var animationListener = new AnimationListener();

            var animation = new ExpandAnimation(viewToAnimate, targetHeight)
            {
                // Expansion speed of 1dp/ms
                Duration = (int)(targetHeight / viewToAnimate.Context.Resources.DisplayMetrics.Density * 1.5)
            };
            animation.SetAnimationListener(animationListener);

            viewToAnimate.StartAnimation(animation);

            return animationListener.AnimationEnded;
        }

        public static Task Collapse(View viewToAnimate)
        {
            var animationListener = new AnimationListener();

            var animation = new CollapseAnimation(viewToAnimate)
            {
                // Expansion speed of 1dp/ms
                Duration = (int)(viewToAnimate.MeasuredHeight / viewToAnimate.Context.Resources.DisplayMetrics.Density / 1.5)
            };
            animation.SetAnimationListener(animationListener);

            viewToAnimate.StartAnimation(animation);

            return animationListener.AnimationEnded;
        }
    }

    class ExpandAnimation : Animation
    {
        private readonly View _viewToAnimate;
        private readonly int _targetHeight;

        public ExpandAnimation(View view, int targetHeight)
        {
            _viewToAnimate = view;
            _targetHeight = targetHeight;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            _viewToAnimate.LayoutParameters.Height = interpolatedTime == 1
                ? ViewGroup.LayoutParams.WrapContent
                : (int)(_targetHeight * interpolatedTime);

            _viewToAnimate.RequestLayout();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }

    class CollapseAnimation : Animation
    {
        private readonly View _viewToAnimate;
        private readonly int _initialHeight;

        public CollapseAnimation(View view)
        {
            _viewToAnimate = view;
            _initialHeight = view.MeasuredHeight;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            _viewToAnimate.LayoutParameters.Height = _initialHeight - (int)(_initialHeight * interpolatedTime);
            _viewToAnimate.RequestLayout();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }
}