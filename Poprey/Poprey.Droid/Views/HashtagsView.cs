using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, addToBackStack: false)]
    [Register("poprey.droid.views.HastagsView")]
    public class HashtagsView : BaseFragment<HashtagsViewModel>
    {
        private AnyFontTextView _pageHeader;
        private AnyFontEditText _hashtagEditText;
        private ImageView _goArrow;

        protected override int FragmentId => Resource.Layout.HashtagsView;

        protected override void InitComponents(View fragmentView)
        {
            _pageHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.HashtagPageHeader);
            _hashtagEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.HashtagEditText);
            _goArrow = fragmentView.FindViewById<ImageView>(Resource.Id.ArrowRightGo);
        }
        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<HashtagsView, HashtagsViewModel>();

            bindingSet.Bind(_pageHeader).For(v => v.Text).To(vm => vm.HeaderText);
            bindingSet.Bind(_hashtagEditText).For(v => v.Text).To(vm => vm.SelectedHashtag);
            bindingSet.Bind(_hashtagEditText).For(v => v.Hint).To(vm => vm.PlaceholderText);
            bindingSet.Bind(_goArrow).For("Click").To(vm => vm.SearchForHashtagCommand);
            bindingSet.Bind(_goArrow).For(v => v.Visibility).To(vm => vm.ArrowVisible).WithConversion("Visibility");

            bindingSet.Apply();
        }
    }
}
