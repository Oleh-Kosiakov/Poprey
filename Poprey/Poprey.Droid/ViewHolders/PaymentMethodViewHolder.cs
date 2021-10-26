using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Poprey.Core.DisplayModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.ViewHolders
{
    public class PaymentMethodViewHolder : MvxRecyclerViewHolder
    {
        private AnyFontTextView _nameLabel;

        public PaymentMethodViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            InitComponents(itemView);
            BindComponents();
        }

        private void InitComponents(View itemView)
        {
            _nameLabel = itemView.FindViewById<AnyFontTextView>(Resource.Id.payment_method_text);
        }

        private void BindComponents()
        {
            var bindingSet = this.CreateBindingSet<PaymentMethodViewHolder, PaymentMethodItem>();

            bindingSet.Bind(_nameLabel).For(v => v.Text).To(vm => vm.Name);

            bindingSet.Apply();
        }
    }
}