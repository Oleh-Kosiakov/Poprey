using System.Threading.Tasks;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.ViewModels.AdditionalServicesMenuItems;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;

namespace Poprey.Droid.Views.AdditionalServicesFragments
{
    public class TikTokLikesFragment : MenuItemBaseFragment<TikTokLikesItemViewModel>
    {
        protected override int FragmentId => Resource.Layout.tiktok_likes_fragment;

        private View _sliderBackgroundView;
        private AnyFontTextView _likesCounter;
        private ExtendedSeekBar _extendedSeekBar;

        private AnyFontTextView _tiktokLikesIncrementCount;
        private AnyFontTextView _tiktokLikesIncrement;
        private AnyFontTextView _tiktokLikesDecrement;

        private View _underline;
        private AnyFontEditText _videoUrlEditText;
        private AnyFontTextView _urlAdditionalHintTextView;

        protected override void InitComponents(View fragmentView)
        {
            _sliderBackgroundView = fragmentView.FindViewById<View>(Resource.Id.tiktok_likes_slider_background);
            _likesCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.tiktok_likes_counter);
            _tiktokLikesIncrementCount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.tiktok_likes_increment_count);
            _tiktokLikesIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.tiktok_likes_increment);
            _tiktokLikesDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.tiktok_likes_decrement);
            _extendedSeekBar = fragmentView.FindViewById<ExtendedSeekBar>(Resource.Id.tiktok_likes_extended_seek_bar);
            _videoUrlEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.tiktok_likes_video_url_edittext);
            _urlAdditionalHintTextView = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.tiktok_likes_url_additional_hint);
            _underline = fragmentView.FindViewById<View>(Resource.Id.tiktok_likes_underline);

            _extendedSeekBar.ProgressChanged += OnLikesCountChanged;
            _extendedSeekBar.TouchUpOnSeekBar += OnSeekBarTouchUp;
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<TikTokLikesFragment, TikTokLikesItemViewModel>();

            bindingSet.Bind(_likesCounter).For(v => v.Text).To(vm => vm.LikesCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_extendedSeekBar).For(v => v.Max).To(vm => vm.LikesLimit);
            bindingSet.Bind(_extendedSeekBar).For(v => v.Progress).To(vm => vm.LikesCounter);

            bindingSet.Bind(_tiktokLikesIncrementCount).For(v => v.Text).To(vm => vm.IncrementCount).OneWay().WithConversion("ServiceIncrement");
            bindingSet.Bind(_tiktokLikesIncrementCount).For(v => v.Visibility).To(vm => vm.IncrementCount).OneWay().WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();

            bindingSet.Bind(_videoUrlEditText).For(v => v.Text).To(vm => vm.SelectedVideoUrl);
            bindingSet.Bind(_videoUrlEditText).For(v => v.Hint).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Text).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Visibility).To(vm => vm.IsAdditionalHintVisible).WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IsInErrorState).To(vm => vm.IsInErrorState);

            bindingSet.Bind(_tiktokLikesIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_tiktokLikesDecrement).For("Click").To(vm => vm.DecrementCommand);
            bindingSet.Bind(_tiktokLikesIncrementCount).For("Click").To(vm => vm.IncrementCommand);

            bindingSet.Apply();

            ViewModel.Initialized = true;
        }

        private bool _incrementButtonActive;
        public bool IncrementButtonActive
        {
            get => _incrementButtonActive;
            set
            {
                _incrementButtonActive = value;

                ToggleButtonState(_tiktokLikesIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_tiktokLikesDecrement, _decrementButtonActive);
            }
        }


        private bool _isInErrorState;
        public bool IsInErrorState
        {
            get => _isInErrorState;
            set
            {
                if (_isInErrorState == value)
                    return;

                _isInErrorState = value;

                if (_isInErrorState)
                {
                    _underline.SetBackgroundResource(Resource.Color.app_error);
                }
                else
                {
                    _underline.SetBackgroundResource(Resource.Color.colorAccent);
                }
            }
        }

        private void OnLikesCountChanged(object sender, ExtendedSeekBarProgressChangedEventArgs e)
        {
            ViewModel.LikesCounter = e.Progress;

            var backgroundViewAbsolutePosition = new int[2];
            _sliderBackgroundView.GetLocationInWindow(backgroundViewAbsolutePosition);

            var desiredWidth = e.ProgressLeftThumbnailOffsetOnWindow - backgroundViewAbsolutePosition[0];
            _sliderBackgroundView.LayoutParameters.Width = desiredWidth;

            _sliderBackgroundView.RequestLayout();
        }

        private async void OnSeekBarTouchUp(object sender, System.EventArgs e)
        {
            //Skip forward OnCountChanged
            await Task.Delay(100);
            ViewModel.AdjustNumberOfDesiredLikes();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _extendedSeekBar.ProgressChanged -= OnLikesCountChanged;
                _extendedSeekBar.TouchUpOnSeekBar -= OnSeekBarTouchUp;
            }

            base.Dispose(disposing);
        }
    }
}