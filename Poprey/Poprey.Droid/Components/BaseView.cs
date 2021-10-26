using Android.Content;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Base;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.BindingContext;

namespace Poprey.Droid.Components
{
    public abstract class BaseView : FrameLayout, IMvxBindingContextOwner, IMvxDataConsumer
    {
        private Context _context;
        private bool _bindingInitialized;

        public BaseView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs);
        }

        public IMvxBindingContext BindingContext
        {
            get;
            set;
        }

        private void Initialize(Context context, IAttributeSet attrs)
        {
            _context = context;

            this.CreateBindingContext();
            Initialize(this, attrs);

            this.DelayBind(BindingAction);
        }

        private void BindingAction()
        {
            if (!_bindingInitialized)
            {
                _bindingInitialized = true;

                ApplyBindings();
            }
        }

        protected virtual void ApplyBindings()
        {
        }

        protected abstract void Initialize(ViewGroup viewGroup, IAttributeSet attrs);

        [MvxSetToNullAfterBinding]
        public virtual object DataContext
        {
            get => BindingContext.DataContext;
            set => BindingContext.DataContext = value;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BindingContext.ClearAllBindings();
            }

            base.Dispose(disposing);
        }
    }
}