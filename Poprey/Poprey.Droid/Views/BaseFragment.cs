using System;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.ViewModels;
using Poprey.Core.ViewModels;

namespace Poprey.Droid.Views
{
    public abstract class BaseFragment : MvxFragment
    {
        protected View FragmentView;

        protected MvxAppCompatActivity ParentActivity => (MvxAppCompatActivity)Activity;

        protected abstract int FragmentId { get; }

        protected abstract void InitComponents(View fragmentView);

        protected abstract void ApplyBindings();

        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            FragmentView = this.BindingInflate(FragmentId, null);

            InitComponents(FragmentView);
            ApplyBindings();

            return FragmentView;
        }

        protected MvxViewPagerFragmentInfo PrepareHeaderMenuAdapterItem(Type typeOfFragmentItem, IHeaderMenuItemViewModel dataContext)
        {
            return new MvxViewPagerFragmentInfo(
                dataContext.HeaderItem.Title,
                dataContext.HeaderItem.Title,
                typeOfFragmentItem,
                dataContext.HeaderItem);
        }


        protected MvxViewPagerFragmentInfo PrepareMenuAdapterItem(Type typeOfFragmentItem, IHeaderMenuItemViewModel dataContext)
        {
            return new MvxViewPagerFragmentInfo(
                dataContext.HeaderItem.Title,
                dataContext.HeaderItem.Title,
                typeOfFragmentItem,
                dataContext);
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : class, IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get => (TViewModel)base.ViewModel;
            set => base.ViewModel = value;
        }
    }
}
