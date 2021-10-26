using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    [Register("poprey.droid.views.InstagramView")]
    public class InstagramView : BaseFragment<InstagramViewModel>
    {
        private AnyFontTextView _pageHeader;
        private ImageView _atImageView;
        private AnyFontEditText _accountEditText;
        private View _instagramUnderline;
        private AnyFontTextView _errorTextView;
        private ImageView _goImageView;

        protected override int FragmentId => Resource.Layout.InstagramView;

        protected override void InitComponents(View fragmentView)
        {
            _pageHeader = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.InstagramPageHeader);
            _atImageView = fragmentView.FindViewById<ImageView>(Resource.Id.at_image);
            _accountEditText = fragmentView.FindViewById<AnyFontEditText>(Resource.Id.InstagramEditText);
            _instagramUnderline = fragmentView.FindViewById<View>(Resource.Id.InstagramUnderline);
            _errorTextView = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.InstagramErrorText);
            _goImageView = fragmentView.FindViewById<ImageView>(Resource.Id.InstagramArrowRightGo);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<InstagramView, InstagramViewModel>();

            bindingSet.Bind(_pageHeader).For(v => v.Text).To(vm => vm.HeaderText);
            bindingSet.Bind(_accountEditText).For(v => v.Text).To(vm => vm.SelectedAccount);
            bindingSet.Bind(_accountEditText).For(v => v.Hint).To(vm => vm.PlaceholderText);
            bindingSet.Bind(_errorTextView).For(v => v.Text).To(vm => vm.ErrorText);
            bindingSet.Bind(this).For(v => v.IsInErrorState).To(vm => vm.IsInErrorState);

            bindingSet.Bind(_goImageView).For("Click").To(vm => vm.SearchForAccountCommand);
            bindingSet.Bind(_goImageView).For(v => v.Visibility).To(vm => vm.IsGoButtonVisible).WithConversion("Visibility");

            bindingSet.Apply();
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
                    _errorTextView.Visibility = ViewStates.Visible;
                    _atImageView.SetImageResource(Resource.Drawable.at_red);
                    _instagramUnderline.SetBackgroundResource(Resource.Color.app_error);
                }
                else
                {
                    _errorTextView.Visibility = ViewStates.Gone;
                    _atImageView.SetImageResource(Resource.Drawable.at_gray);
                    _instagramUnderline.SetBackgroundResource(Resource.Color.colorAccent);
                }
            }
        }
    }
}