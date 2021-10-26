﻿using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Droid.ViewHolders;

namespace Poprey.Droid.Adapters
{
    public class AddInstagramAccountAdapter : MvxRecyclerAdapter
    {
        public AddInstagramAccountAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, this.BindingContext.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(Resource.Layout.authentication_account_item_template, parent, false);

            return new AddInstagramAccountViewHolder(view, itemBindingContext);
        }
    }
}