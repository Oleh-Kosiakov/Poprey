using System;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Binding.Extensions;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.ViewHolders;

namespace Poprey.Droid.Adapters
{
    public class BagItemsAdapter : MvxRecyclerAdapter
    {
        public BagItemsAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

            View view;
            MvxRecyclerViewHolder viewHolder;

            if (viewType == CompactBagItemViewHolder.Identifier)
            {
                view = itemBindingContext.BindingInflate(Resource.Layout.compact_bag_item_template, parent, false);
                viewHolder = new CompactBagItemViewHolder(view, itemBindingContext);
            }
            else if(viewType == ExpandedBagItemViewHolder.Identifier)
            {
                view = itemBindingContext.BindingInflate(Resource.Layout.expanded_bag_item_template, parent, false);
                viewHolder = new ExpandedBagItemViewHolder(view, itemBindingContext);
            }
            else
            {
                throw new ArgumentException("Not found ViewHolder for passed viewType identifier.");
            }

            viewHolder.Click = ItemClick;
            viewHolder.LongClick = ItemLongClick;

            return viewHolder;
        }

        public override int GetItemViewType(int position)
        {
            var bagItem = ItemsSource.ElementAt(position) as BagItem;

            if (bagItem == null)
            {
                return base.GetItemViewType(position);
            }

            return bagItem.IsCompact ? CompactBagItemViewHolder.Identifier : ExpandedBagItemViewHolder.Identifier;
        }
    }
}