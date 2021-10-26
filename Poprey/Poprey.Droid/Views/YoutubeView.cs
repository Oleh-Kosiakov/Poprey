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
using Poprey.Droid.Views.AdditionalServicesFragments;
using Poprey.Droid.Views.InstagramFragments;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, addToBackStack: true)]
    [Register("poprey.droid.views.YoutubeView")]
    public class YoutubeView : BaseFragment<YoutubeViewModel>
    {
        protected override int FragmentId => Resource.Layout.YoutubeView;

        private AnyFontTextView _menuHeaderSubtitle;

        private ViewPager _headerMenuViewPager;
        private ViewPager _contentViewPager;
        private List<MvxViewPagerFragmentInfo> _headerFragments;

        protected override void InitComponents(View fragmentView)
        {
            _menuHeaderSubtitle = fragmentView.FindViewById<AnyFontTextView>(Resource.Id.menuHeaderSubtitle);
            _headerMenuViewPager = fragmentView.FindViewById<ViewPager>(Resource.Id.youtube_header_view_pager);
            _contentViewPager = fragmentView.FindViewById<ViewPager>(Resource.Id.youtube_content_view_pager);

            _contentViewPager.OffscreenPageLimit = 2;

            _headerMenuViewPager.PageSelected += HeaderMenuViewPagerOnPageSelected;
            _headerMenuViewPager.SetOnTouchListener(new OnViewSideTouchListener(Resources, OnLeftTouched, OnRightTouched));
        }

        protected override void ApplyBindings()
        {
            _menuHeaderSubtitle.Text = ViewModel.YoutubeServiceName;

            PrepareMenuAdapterItems();
        }

        private void PrepareMenuAdapterItems()
        {
            var firstMenuHeaderItem = PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.YoutubeViewsItemViewModel);
            (firstMenuHeaderItem.ViewModel as MenuHeaderItem).IsActive = true;

            //Preparing header menu items
            _headerFragments = new List<MvxViewPagerFragmentInfo>
            {
                firstMenuHeaderItem,
                PrepareHeaderMenuAdapterItem(typeof(MenuHeaderFragment), ViewModel.YoutubeSubscribersItemViewModel),
            };
            _headerMenuViewPager.Adapter = new MvxFragmentStatePagerAdapter(Activity, ChildFragmentManager, _headerFragments);

            var contentFragments = new List<MvxViewPagerFragmentInfo>
            {
                PrepareMenuAdapterItem(typeof(YoutubeViewsFragment), ViewModel.YoutubeViewsItemViewModel),
                PrepareMenuAdapterItem(typeof(YoutubeSubscribersFragments), ViewModel.YoutubeSubscribersItemViewModel),
              
            };
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
                }

                menuHeaderItem.IsActive = true;
            }

            _contentViewPager.SetCurrentItem(pageSelectedEventArgs.Position, true);
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
    }
}