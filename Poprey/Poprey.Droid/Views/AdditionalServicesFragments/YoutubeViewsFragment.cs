using System.Threading.Tasks;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.ViewModels.AdditionalServicesMenuItems;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;

namespace Poprey.Droid.Views.AdditionalServicesFragments
{
    public class YoutubeViewsFragment : MenuItemBaseFragment<YoutubeViewsItemViewModel>
    {
        protected override int FragmentId => Resource.Layout.youtube_views_fragment;

        private View _sliderBackgroundView;
        private AnyFontTextView _viewsCounter;

        private AnyFontTextView _youtubeViewsIncrementCount;
        private AnyFontTextView _youtubeViewsIncrement;
        private AnyFontTextView _youtubeViewsDecrement;

        private ExtendedSeekBar _extendedSeekBar;

        private View _underline;
        private AnyFontEditText _videoUrlEditText;
        private AnyFontTextView _urlAdditionalHintTextView;

        protected override void InitComponents(View fragmentView)
        {
            _sliderBackgroundView = fragmentView.FindViewById<View>(Resource.Id.youtube_views_slider_background);
            _viewsCounter = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_views_counter);
            _youtubeViewsIncrementCount = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_views_increment_count);
            _youtubeViewsIncrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_views_increment);
            _youtubeViewsDecrement = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_views_decrement);
            _extendedSeekBar = fragmentView.FindViewById<ExtendedSeekBar>(Resource.Id.youtube_views_extended_seek_bar);
            _videoUrlEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.youtube_views_video_url_edittext);
            _urlAdditionalHintTextView = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.youtube_views_url_additional_hint);
            _underline = fragmentView.FindViewById<View>(Resource.Id.youtube_views_underline);

            _extendedSeekBar.ProgressChanged += OnViewsCountChanged;
            _extendedSeekBar.TouchUpOnSeekBar += OnSeekBarTouchUp;
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<YoutubeViewsFragment, YoutubeViewsItemViewModel>();

            bindingSet.Bind(_viewsCounter).For(v => v.Text).To(vm => vm.ViewsCounter).OneWay().WithConversion("ServiceCount");
            bindingSet.Bind(_extendedSeekBar).For(v => v.Max).To(vm => vm.ViewsLimit);
            bindingSet.Bind(_extendedSeekBar).For(v => v.Progress).To(vm => vm.ViewsCounter);

            bindingSet.Bind(_youtubeViewsIncrementCount).For(v => v.Text).To(vm => vm.IncrementCount).OneWay().WithConversion("ServiceIncrement");
            bindingSet.Bind(_youtubeViewsIncrementCount).For(v => v.Visibility).To(vm => vm.IncrementCount).OneWay().WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.IncrementButtonActive).To(vm => vm.IncrementButtonActive).OneWay();
            bindingSet.Bind(this).For(v => v.DecrementButtonActive).To(vm => vm.DecrementButtonActive).OneWay();

            bindingSet.Bind(_videoUrlEditText).For(v => v.Text).To(vm => vm.SelectedVideoUrl);
            bindingSet.Bind(_videoUrlEditText).For(v => v.Hint).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Text).To(vm => vm.UrlHintText);
            bindingSet.Bind(_urlAdditionalHintTextView).For(v => v.Visibility).To(vm => vm.IsAdditionalHintVisible).WithConversion("Visibility");

            bindingSet.Bind(this).For(v => v.IsInErrorState).To(vm => vm.IsInErrorState);

            bindingSet.Bind(_youtubeViewsIncrement).For("Click").To(vm => vm.IncrementCommand);
            bindingSet.Bind(_youtubeViewsDecrement).For("Click").To(vm => vm.DecrementCommand);
            bindingSet.Bind(_youtubeViewsIncrementCount).For("Click").To(vm => vm.IncrementCommand);

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

                ToggleButtonState(_youtubeViewsIncrement, _incrementButtonActive);
            }
        }

        private bool _decrementButtonActive;
        public bool DecrementButtonActive
        {
            get => _decrementButtonActive;
            set
            {
                _decrementButtonActive = value;

                ToggleButtonState(_youtubeViewsDecrement, _decrementButtonActive);
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

        private void OnViewsCountChanged(object sender, ExtendedSeekBarProgressChangedEventArgs e)
        {
            ViewModel.ViewsCounter = e.Progress;

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
                _extendedSeekBar.ProgressChanged -= OnViewsCountChanged;
                _extendedSeekBar.TouchUpOnSeekBar -= OnSeekBarTouchUp;
            }

            base.Dispose(disposing);
        }
    }
}