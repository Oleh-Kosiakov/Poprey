using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.DisplayModels;
using Poprey.Core.ViewModels;
using Poprey.Droid.Adapters;
using Poprey.Droid.Controls;
using Poprey.Droid.Listeners;
using Poprey.Droid.ViewHolders;
using Poprey.Droid.Views.InstagramFragments;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, addToBackStack: false)]
    [Register("poprey.droid.views.InstagramMenuView")]
    public class InstagramMenuView : BaseFragment<InstagramMenuViewModel>
    {
        protected override int FragmentId => Resource.Layout.InstagramMenuView;

        private int _selectedMenuItemIndex = 0;

        private AnyFontTextView _menuHeaderSubtitle;

        private ViewPager _headerMenuViewPager;
        private ViewPager _contentViewPager;
        private List<MvxViewPagerFragmentInfo> _headerFragments;

        protected override void InitComponents(View fragmentView)
        {
            _menuHeaderSubtitle = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.menuHeaderSubtitle);
            _headerMenuViewPager = fragmentView.FindViewById<ViewPager>(Resource.Id.instagram_header_view_pager);
            _contentViewPager = fragmentView.FindViewById<ViewPager>(Resource.Id.instagram_content_view_pager);

            _contentViewPager.OffscreenPageLimit = 5;

            _headerMenuViewPager.PageSelected += HeaderMenuViewPagerOnPageSelected;
            _headerMenuViewPager.SetOnTouchListener(new OnViewSideTouchListener(Resources, OnLeftTouched, OnRightTouched));
        }


        protected override void ApplyBindings()
        {
            _menuHeaderSubtitle.Text = ViewModel.InstagramServiceName;

            PrepareMenuAdapterItems();
        }

        private void PrepareMenuAdapterItems()
        {
            _headerFragments = new List<MvxViewPagerFragmentInfo>();
           
            //Preparing menu items

            var contentFragments = new List<MvxViewPagerFragmentInfo>();

            if (ViewModel.IsLikesAvailable)
            {
                _headerFragments.Add(PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.InstagramLikeMenuItemViewModel));
                contentFragments.Add(PrepareMenuAdapterItem(typeof(InstagramLikesFragment), ViewModel.InstagramLikeMenuItemViewModel));
            }

            if (ViewModel.IsFollowersAvailable)
            {
                _headerFragments.Add(PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.InstagramFollowersMenuItemViewModel));
                contentFragments.Add(PrepareMenuAdapterItem(typeof(InstagramFollowersFragment), ViewModel.InstagramFollowersMenuItemViewModel));
            }

            if (ViewModel.IsViewsAvailable)
            {
                _headerFragments.Add(PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.InstagramViewsMenuItemViewModel));
                contentFragments.Add(PrepareMenuAdapterItem(typeof(InstagramViewsFragment), ViewModel.InstagramViewsMenuItemViewModel));
            }

            if (ViewModel.IsAutoLikesAvailable)
            {
                _headerFragments.Add(PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.InstagramAutoMenuItemViewModel));
                contentFragments.Add(PrepareMenuAdapterItem(typeof(InstagramAutoFragment), ViewModel.InstagramAutoMenuItemViewModel));
            }

            if (ViewModel.IsCommentsAvailable)
            {
                _headerFragments.Add(PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.InstagramCommentsItemViewModel));
                contentFragments.Add(PrepareMenuAdapterItem(typeof(InstagramCommentsFragment), ViewModel.InstagramCommentsItemViewModel));
            }

            (_headerFragments.First().ViewModel as MenuHeaderItem).IsActive = true;

            _headerMenuViewPager.Adapter = new MvxFragmentStatePagerAdapter(Activity, ChildFragmentManager, _headerFragments);
            _contentViewPager.Adapter = new SwipeMenuAdapter(Activity, ChildFragmentManager, contentFragments);
        }

        private void HeaderMenuViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs pageSelectedEventArgs)
        {
            var menuHeaderItem = _headerFragments[pageSelectedEventArgs.Position].ViewModel as MenuHeaderItem;

            if (menuHeaderItem != null)
            {
                foreach (var vm in _headerFragments.Select(f => f.ViewModel).Cast<MenuHeaderItem>())
                {
                    vm.IsActive = false;
                    vm.ToLeftOfSelection = false;
                }

                menuHeaderItem.IsActive = true;
            }

            var leftItemIndex = pageSelectedEventArgs.Position - 1;

            if (leftItemIndex >= 0)
            {
                var leftMenuItem = _headerFragments[leftItemIndex].ViewModel as MenuHeaderItem;
                leftMenuItem.ToLeftOfSelection = true;
            }

            _contentViewPager.SetCurrentItem(pageSelectedEventArgs.Position, true);
            _selectedMenuItemIndex = pageSelectedEventArgs.Position;
        }

        private void OnLeftTouched()
        {
            if (_headerMenuViewPager.CurrentItem - 1 >= 0)
            {
                _headerMenuViewPager.SetCurrentItem(_headerMenuViewPager.CurrentItem - 1, true);
            }
        }

        private void OnRightTouched()
        {
            if (_headerMenuViewPager.CurrentItem + 1 <= _headerMenuViewPager.Adapter.Count)
            {
                _headerMenuViewPager.SetCurrentItem(_headerMenuViewPager.CurrentItem + 1, true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _headerMenuViewPager.PageSelected -= HeaderMenuViewPagerOnPageSelected;
            }
        }
    }
}