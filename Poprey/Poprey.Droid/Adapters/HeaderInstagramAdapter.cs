using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Droid.ViewHolders;

namespace Poprey.Droid.Adapters
{
    public class HeaderInstagramAdapter : MvxRecyclerAdapter
    {
        public HeaderInstagramAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, this.BindingContext.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(Resource.Layout.instagram_account_in_header_template, parent, false);

            return new HeaderInstagramViewHolder(view, itemBindingContext);
        }
    }
}