using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Poprey.Core.DisplayModels;
using Object = Java.Lang.Object;

namespace Poprey.Droid.Adapters
{
    public class MenuHeaderAdapter : PagerAdapter
    {
        private readonly Context _context;
        private List<MenuHeaderItem> _dataSource;

        public List<MenuHeaderItem> DataSource
        {
            get => _dataSource;
            set => _dataSource = value;
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view == @object;
        }

        public override int Count => _dataSource?.Count ?? 0;

        public MenuHeaderAdapter(Context context)
        {
            _context = context;
        }

        //public override Object InstantiateItem(ViewGroup container, int position)
        //{
        //    var inflater = (LayoutInflater) _context.GetSystemService(Context.LayoutInflaterService);
        //    var view = inflater.Inflate(Resource.Layout.menu_header_template, null);

        //}

        [Obsolete("deprecated")]
        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(view as View);
        }
    }
}