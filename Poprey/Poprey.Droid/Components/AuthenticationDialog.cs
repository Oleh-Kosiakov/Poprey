using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Base;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using Poprey.Core.ViewModels;
using Poprey.Droid.Adapters;

namespace Poprey.Droid.Components
{
    public class AuthenticationDialog : Dialog, IMvxBindingContextOwner, IMvxDataConsumer
    {
        private readonly Activity _activity;
        private ViewGroup _rootLayout;
        private AdaptiveBackgroundImageView _addImageView;
        private ImageView _addArrowImageView;
        private EditText _addEditText;
        private EditText _helperEditText;
        private MvxRecyclerView _recyclerView;
        private AddInstagramAccountAdapter _addInstagramAdapter;

        protected AuthenticationDialog(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AuthenticationDialog(Activity context, int themeResId) : base(context, themeResId)
        {
            _activity = context;
            this.CreateBindingContext();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.authentication_dialog);

            var bindingContext = new MvxAndroidBindingContext(Context, (IMvxLayoutInflaterHolder)_activity);

            _rootLayout = FindViewById<RelativeLayout>(Resource.Id.rootLayout);
            _addImageView = FindViewById<AdaptiveBackgroundImageView>(Resource.Id.add_image);
            _addEditText = FindViewById<EditText>(Resource.Id.add_editText);
            _helperEditText = FindViewById<EditText>(Resource.Id.helper_editText);
            _recyclerView = FindViewById<MvxRecyclerView>(Resource.Id.accounts_list);
            _addArrowImageView = FindViewById<ImageView>(Resource.Id.arrow_right_image);

            _addInstagramAdapter = new AddInstagramAccountAdapter(bindingContext);
            _recyclerView.Adapter = _addInstagramAdapter;

            _addImageView.ShouldLoadImageFromWeb = false;
            _addImageView.ShouldShowGrayBackground = true;
            _addImageView.ShouldShowAddImage = true;

            ApplyBindings();
        }

        private void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<AuthenticationDialog, AuthenticationDialogViewModel>();

            bindingSet.Bind(_addEditText).For(v => v.Text).To(vm => vm.NewAccountName);
            bindingSet.Bind(_addEditText).For(v => v.Hint).To(vm => vm.NewAccountHint);

            bindingSet.Bind(_addInstagramAdapter).For(v => v.ItemsSource).To(vm => vm.Accounts);
            bindingSet.Bind(_addArrowImageView).For("Click").To(vm => vm.AddNewAccountCommand);
            bindingSet.Bind(_addArrowImageView).For(v => v.Visibility).To(vm => vm.IsArrowVisible).WithConversion("Visibility");
            bindingSet.Bind(this).For(v => v.DismissRequested).To(vm => vm.DismissRequested);
            bindingSet.Bind(_rootLayout).For("Click").To(vm => vm.DismissCommand);

            bindingSet.Apply();
        }

        public IMvxBindingContext BindingContext { get; set; }

        [MvxSetToNullAfterBinding]
        public object DataContext
        {
            get => BindingContext.DataContext;
            set => BindingContext.DataContext = value;
        }

        public bool DismissRequested
        {
            get => false;
            set => Dismiss();
        }
    }
}