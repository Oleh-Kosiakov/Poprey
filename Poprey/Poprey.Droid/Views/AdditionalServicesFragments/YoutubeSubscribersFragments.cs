using System.Threading.Tasks;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.ViewModels.AdditionalServicesMenuItems;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;
using Poprey.Droid.Views.InstagramFragments;

namespace Poprey.Droid.Views.AdditionalServicesFragments
{
    public class YoutubeSubscribersFragments : MenuItemBaseFragment<YoutubeSubscribersItemViewModel>
    {
        protected override int FragmentId => Resource.Layout.youtube_subscribers_fragment;

        private View _sliderBackgroundView;
        private AnyFontTextView _subscribersCounter;

        private AnyFontTextView _youtubeSubscribersIncrementCount;
        private AnyFontTextView _youtubeSubscribersIncrement;
        private AnyFontTextView _youtubeSubscribersDecrement;

        private ExtendedSeekBar _extendedSeekBar;

        private View _underline;
        private AnyFontEditText _urlEditText;
        private AnyFontTextView _urlAdditionalHintTextView;

        protected override void InitComponents(View fragmentView)
        {
            _sliderBackgroundView = fragmentView.FindViewById<View>(Resource.Id.youtube_subscribers_slider_background);
            _subscribersCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_subscribers_counter);
            _youtubeSubscribersIncrementCount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_subscribers_increment_count);
            _youtubeSubscribersIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_subscribers_increment);
            _youtubeSubscribersDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_subscribers_decrement);
            _extendedSeekBar = fragmentView.FindViewById<ExtendedSeekBar>(Resource.Id.youtube_subscribers_extended_seek_bar);
            _urlEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.youtube_subscribers_video_url_edittext);
            _urlAdditionalHintTextView = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_subscribers_url_additional_hint);
            _underline = fragmentView.FindViewById<View>(Resource.Id.youtube_subscribers_underline);

            _extendedSeekBar.ProgressChanged += OnSubscribersCountChanged;
            _extendedSeekBar.TouchUpOnSeekBar += OnSeekBarTouchUp;
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<YoutubeSubscribersFragments, YoutubeSubscribersItemViewModel>();

            bindingSet.Bind(_subscribersCounter).For(v => v.Text).To(vm => vm.SubscribersCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_extendedSeekBar).For(v => v.Max).To(vm => vm.SubscribersLimit);
            bindingSet.Bind(_extendedSeekBar).For(v => v.Progress).To(vm => vm.SubscribersCounter);

            bindingSet.Bind(_youtubeSubscribersIncrementCount).For(v => v.Text).To(vm => vm.IncrementCount).OneWay().WithConversion("ServiceIncrement");
            bindingSet.Bind(_youtubeSubscribersIncrementCount).For(v => v.Visibility).To(vm => vm.IncrementCount).OneWay().WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();

            bindingSet.Bind(_urlEditText).For(v => v.Text).To(vm => vm.SelectedAccountUrl);
            bindingSet.Bind(_urlEditText).For(v => v.Hint).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Text).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Visibility).To(vm => vm.IsAdditionalHintVisible).WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IsInErrorState).To(vm => vm.IsInErrorState);

            bindingSet.Bind(_youtubeSubscribersIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_youtubeSubscribersDecrement).For("Click").To(vm => vm.DecrementCommand);
            bindingSet.Bind(_youtubeSubscribersIncrementCount).For("Click").To(vm => vm.IncrementCommand);

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

                ToggleButtonState(_youtubeSubscribersIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_youtubeSubscribersDecrement, _decrementButtonActive);
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

        private void OnSubscribersCountChanged(object sender, ExtendedSeekBarProgressChangedEventArgs e)
        {
            ViewModel.SubscribersCounter = e.Progress;

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
            ViewModel.AdjustNumberOfDesiredSubscribers();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _extendedSeekBar.ProgressChanged -= OnSubscribersCountChanged;
                _extendedSeekBar.TouchUpOnSeekBar -= OnSeekBarTouchUp;
            }

            base.Dispose(disposing);
        }
    }
}