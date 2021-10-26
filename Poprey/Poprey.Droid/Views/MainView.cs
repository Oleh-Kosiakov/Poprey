using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core;
using Poprey.Core.Messages;
using Poprey.Core.Util;
using Poprey.Core.ViewModels;
using Poprey.Droid.Components;
using Poprey.Droid.Listeners;

namespace Poprey.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Label = "Poprey",
        Theme = "@style/AppTheme",
        LaunchMode = LaunchMode.SingleTop,
        Name = "poprey.droid.views.MainView",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainView : BaseAppCompatActivity<MainViewModel>, DrawerLayout.IDrawerListener
    {
        private readonly MessageTokenHelper _messageTokenHelper = Mvx.IoCProvider.Resolve<MessageTokenHelper>();

        protected override int ContentViewId => Resource.Layout.MainView;

        private BagView _bagView;
        public DrawerLayout DrawerLayout { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            AppCenter.Start(Constants.AppCenterDevKey, typeof(Analytics), typeof(Crashes));
            UserDialogs.Init(this);

            _messageTokenHelper.Subscribe<SwitchPageMessage>(message => HideSoftKeyboard());
            _bagView.DataContext = ViewModel.BagViewModel;

            var parentView = ((ViewGroup)FindViewById(Android.Resource.Id.Content)).GetChildAt(0);
            var keyboardListener = new KeyboardVisibilityListener(parentView, OnKeyboardToggled);

            parentView.ViewTreeObserver.AddOnGlobalLayoutListener(keyboardListener);

            if (bundle == null)
            {
                ViewModel.ShowFirstViewModelCommand.Execute(null);
                ViewModel.ShowMenuViewModelCommand.Execute(null);
            }
        }


        protected override void InitComponents()
        {
            base.InitComponents();

            _bagView = FindViewById<BagView>(Resource.Id.bagView);
            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            DrawerLayout.AddDrawerListener(this);
        }

        public override void OnBackPressed()
        {
            if (DrawerLayout != null && DrawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                DrawerLayout.CloseDrawers();
            }

            if (_bagView.BagState != BagViewModelSingleton.BagState.Collapsed)
            {
                _bagView.BagState = BagViewModelSingleton.BagState.Collapsed;
            }
            else
            {
                SupportFragmentManager.PopBackStack();
            }
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<MainView, MainViewModel>();

            bindingSet.Bind(HeaderView).For(v => v.DataContext).To(vm => vm.HeaderViewModel);
            bindingSet.Bind(this).For(v => v.IsDrawerExpanded).To(vm => vm.HeaderViewModel.IsDrawerMenuExpanded);

            bindingSet.Apply();
        }

        private bool _isDrawerExpanded;

        public bool IsDrawerExpanded
        {
            get => _isDrawerExpanded;
            set
            {
                _isDrawerExpanded = value;

                if (DrawerLayout.IsDrawerOpen(GravityCompat.Start) && _isDrawerExpanded) return;
                if (!DrawerLayout.IsDrawerOpen(GravityCompat.Start) && !_isDrawerExpanded) return;

                if (_isDrawerExpanded)
                {
                    DrawerLayout.OpenDrawer(GravityCompat.Start);
                }
                else if (!_isDrawerExpanded)
                {
                    DrawerLayout.CloseDrawers();
                }
            }
        }
        private void OnKeyboardToggled(bool isAppeared)
        {
            ViewModel.KeyboardToggled(isAppeared);
        }

        public void HideSoftKeyboard()
        {
            if (CurrentFocus == null)
                return;

            var inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);

            CurrentFocus.ClearFocus();
        }

        #region DrawerLayoutListener
        public void OnDrawerClosed(View drawerView)
        {
            ViewModel.HeaderViewModel.IsDrawerMenuExpanded = false;
        }

        public void OnDrawerOpened(View drawerView)
        {
            ViewModel.HeaderViewModel.IsDrawerMenuExpanded = true;
        }

        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
        }

        public void OnDrawerStateChanged(int newState)
        {
        }

        #endregion
    }
}

