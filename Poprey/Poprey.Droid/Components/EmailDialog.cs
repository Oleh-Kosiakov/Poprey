using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views.InputMethods;
using Android.Widget;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.BindingContext;
using Poprey.Core.Messages;
using Poprey.Core.Util;
using Poprey.Core.ViewModels;
using Poprey.Droid.Controls;

namespace Poprey.Droid.Components
{
    public class EmailDialog : Dialog, IMvxBindingContextOwner, IMvxDataConsumer
    {
        private ImageView _closeImage;
        private AnyFontTextView _headerLabel;
        private AnyFontEditText _emailEditText;
        private ImageView _proceedImage;
        private AnyFontTextView _licenseLabel;

        private MessageTokenHelper _messenger;
        public MessageTokenHelper MessageTokenHelper => _messenger ?? (_messenger = Mvx.IoCProvider.Resolve<MessageTokenHelper>());

        protected EmailDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public EmailDialog(Activity context, int themeResId) : base(context, themeResId)
        {
            this.CreateBindingContext();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.email_dialog);
            
            _closeImage = FindViewById<ImageView>(Resource.Id.email_close_image);
            _headerLabel = FindViewById<AnyFontTextView>(Resource.Id.email_header_label);
            _emailEditText = FindViewById<AnyFontEditText>(Resource.Id.email_edit_text);
            _proceedImage = FindViewById<ImageView>(Resource.Id.email_proceed_image);
            _licenseLabel = FindViewById<AnyFontTextView>(Resource.Id.email_license_label);

            _licenseLabel.PaintFlags = _licenseLabel.PaintFlags | PaintFlags.UnderlineText;
            MessageTokenHelper.Publish(new IgnoreKeyboardOnBagMessage(this, true));

            ApplyBindings();
        }

        private void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<EmailDialog, EmailDialogViewModel>();

            bindingSet.Bind(_headerLabel).For(v => v.Text).To(vm => vm.HeaderText);
            bindingSet.Bind(_emailEditText).For(v => v.Text).To(vm => vm.EnteredEmail);
            bindingSet.Bind(_emailEditText).For(v => v.Hint).To(vm => vm.EmailHintText);
            bindingSet.Bind(_licenseLabel).For(v => v.Text).To(vm => vm.LicenseText);

            bindingSet.Bind(this).For(v => v.ShouldDismiss).To(vm => vm.ShouldDismiss);

            bindingSet.Bind(_closeImage).For("Click").To(vm => vm.CloseCommand);
            bindingSet.Bind(_proceedImage).For("Click").To(vm => vm.ProceedWithEmailCommand);
            bindingSet.Bind(_licenseLabel).For("Click").To(vm => vm.ShowLicenseCommand);

            bindingSet.Apply();
        }

        public bool ShouldDismiss
        {
            get => true;
            set
            {
                if (value)
                {
                    this.Dismiss();
                }
            }
        }

        public IMvxBindingContext BindingContext { get; set; }

        [MvxSetToNullAfterBinding]
        public object DataContext
        {
            get => BindingContext.DataContext;
            set => BindingContext.DataContext = value;
        }

        public override void Dismiss()
        {
            MessageTokenHelper.Publish(new IgnoreKeyboardOnBagMessage(this, false));

            base.Dismiss();
        }
    }
}