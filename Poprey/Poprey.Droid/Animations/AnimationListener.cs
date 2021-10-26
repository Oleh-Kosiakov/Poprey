using System.Threading.Tasks;
using Android.Views.Animations;

namespace Poprey.Droid.Animations
{
    class AnimationListener : Java.Lang.Object, Animation.IAnimationListener
    {
        private readonly TaskCompletionSource<bool> _animationEndedCompletionSource = new TaskCompletionSource<bool>();

        public Task AnimationEnded => _animationEndedCompletionSource.Task;

        public void OnAnimationEnd(Animation animation)
        {
           _animationEndedCompletionSource.SetResult(true);
        }

        public void OnAnimationRepeat(Animation animation)
        {
        }

        public void OnAnimationStart(Animation animation)
        {
        }
    }
}