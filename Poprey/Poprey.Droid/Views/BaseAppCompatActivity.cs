using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.ViewModels;
using Poprey.Droid.Components;

namespace Poprey.Droid.Views
{
    public abstract class BaseAppCompatActivity<TViewModel> : MvxAppCompatActivity<TViewModel> where TViewModel: MvxViewModel
    {
        public HeaderView HeaderView { get; set; }

        protected abstract int ContentViewId { get; }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            SetContentView(ContentViewId);

            InitComponents();
            ApplyBindings();
        }

        protected virtual void InitComponents()
        {
            HeaderView = FindViewById<HeaderView>(Resource.Id.header_view);
        }

        protected abstract void ApplyBindings();
    }
}