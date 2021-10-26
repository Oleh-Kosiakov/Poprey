using System;
using Android.Flexbox;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;
using Poprey.Droid.Adapters;
using Poprey.Droid.Components;
using Poprey.Droid.Controls;
using Poprey.Droid.EventArgs;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, addToBackStack: true)]
    [Register("poprey.droid.views.HashtagsListView")]
    public class HashtagsListView : BaseFragment<HashtagsListViewModel>
    {
        private const int RowFlexDirection = 0;

        private MvxRecyclerView _hashtagsRecyclerView;
        private HashtagAdapter _hashtagAdapter;

        private MvxRecyclerView _hashtagsBottomRecyclerView;
        private HashtagAdapter _hashtagBottomAdapter;

        private View _widthRangeBackgroundView;
        private AnyFontTextView _headerHashtagTextView;
        private ExtendedSeekBar _extendedSeekBar;

        private RelativeLayout _copyButton;
        private RelativeLayout _copiedButton;

        protected override int FragmentId => Resource.Layout.HashtagsListView;

        protected override void InitComponents(View fragmentView)
        {
            _extendedSeekBar = fragmentView.FindViewById<ExtendedSeekBar>(Resource.Id.extended_seek_bar);
            _widthRangeBackgroundView = fragmentView.FindViewById<View>(Resource.Id.width_range_background);
            _headerHashtagTextView = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.HashtagTextView);
            _copyButton = fragmentView.FindViewById<RelativeLayout>(Resource.Id.CopyButton);
            _copiedButton = fragmentView.FindViewById<RelativeLayout>(Resource.Id.CopiedButton);

            _extendedSeekBar.ProgressChanged += ExtendedSeekBarOnProgressChanged;

            //Main recycler view

            _hashtagsRecyclerView = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.hashtags_list);
            _hashtagsRecyclerView.LayoutFrozen = true;

            var layoutManager = new FlexboxLayoutManager(Context, RowFlexDirection);

            _hashtagsRecyclerView.SetLayoutManager(layoutManager);
            _hashtagsRecyclerView.HasFixedSize = true;

            _hashtagAdapter = new HashtagAdapter((IMvxAndroidBindingContext)BindingContext);
            _hashtagsRecyclerView.Adapter = _hashtagAdapter;

            //Bottom recycler view

            _hashtagsBottomRecyclerView = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.bottom_hashtags_list);

            var bottomLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);

            _hashtagsBottomRecyclerView.SetLayoutManager(bottomLayoutManager);
            _hashtagsBottomRecyclerView.HasFixedSize = false;

            _hashtagBottomAdapter = new HashtagAdapter((IMvxAndroidBindingContext)BindingContext);
            _hashtagsBottomRecyclerView.Adapter = _hashtagBottomAdapter;
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<HashtagsListView, HashtagsListViewModel>();

            bindingSet.Bind(_hashtagAdapter).For(v => v.ItemsSource).To(vm => vm.VisibleHashtags);
            bindingSet.Bind(_hashtagAdapter).For(v => v.ItemClick).To(vm => vm.RemoveHashtagCommand);
            bindingSet.Bind(_hashtagAdapter).For(v => v.ItemLongClick).To(vm => vm.CopyHashtagCommand);
            bindingSet.Bind(_hashtagBottomAdapter).For(v => v.ItemsSource).To(vm => vm.RemovedHashtags);
            bindingSet.Bind(_hashtagBottomAdapter).For(v => v.ItemClick).To(vm => vm.RestoreHashtagCommand);

            bindingSet.Bind(_headerHashtagTextView).For(v => v.Text).To(vm => vm.SelectedHashtagText);
            bindingSet.Bind(_extendedSeekBar).For(v => v.CounterText).To(vm => vm.VisibleHashtagsCount);
            bindingSet.Bind(_extendedSeekBar).For(v => v.Progress).To(vm => vm.VisibleHashtagsCount);
            bindingSet.Bind(_extendedSeekBar).For(v => v.Max).To(vm => vm.AllHashtagsCount);

            bindingSet.Bind(_copyButton).For("Click").To(vm => vm.CopyAllCommand);
            bindingSet.Bind(_copiedButton).For(v => v.Visibility).To(vm => vm.IsCopied).WithConversion("Visibility");


            bindingSet.Apply();
        }

        private void ExtendedSeekBarOnProgressChanged(object sender, ExtendedSeekBarProgressChangedEventArgs extendedSeekBarProgressChangedEventArgs)
        {
            ViewModel.VisibleHashtagsCount = extendedSeekBarProgressChangedEventArgs.Progress;

            var backgroundViewAbsolutePosition = new int[2];
            _widthRangeBackgroundView.GetLocationInWindow(backgroundViewAbsolutePosition);

            var desiredWidth = extendedSeekBarProgressChangedEventArgs.ProgressLeftThumbnailOffsetOnWindow - backgroundViewAbsolutePosition[0];

            _widthRangeBackgroundView.LayoutParameters.Width = desiredWidth;
            ViewModel.BackgroundOverlayWidthInNativeUnits = desiredWidth;

            _widthRangeBackgroundView.RequestLayout();
        }
    }
}