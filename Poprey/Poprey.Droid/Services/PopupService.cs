using Acr.UserDialogs;
using Android.App;
using MvvmCross;
using MvvmCross.Platforms.Android;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.ViewModels;
using Poprey.Droid.Components;

namespace Poprey.Droid.Services
{
    public class PopupService : IPopupService
    {
        private LoadingDialog _loadingDialog;
        private Activity CurrentTopActivity => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

        public void ShowAuthenticationDialog(AuthenticationDialogViewModel dataContext)
        {
            var authDialog = new AuthenticationDialog(CurrentTopActivity, Resource.Style.AuthenticationDialogTheme);//Don't use object initialization !
            authDialog.DataContext = dataContext;   

            authDialog.Show();
        }

        public void ShowEmailDialog(EmailDialogViewModel dataContext)
        {
            var emailDialog = new EmailDialog(CurrentTopActivity, Resource.Style.EmailDialogTheme);
            emailDialog.DataContext = dataContext;

            emailDialog.Show();
        }

        public void ShowLoading()
        {
            _loadingDialog?.Dismiss();

            _loadingDialog = new LoadingDialog(CurrentTopActivity, Resource.Style.LoadingDialogTheme);

            _loadingDialog.Show();
        }

        public void HideLoading()
        {
            _loadingDialog?.Dismiss();
            _loadingDialog = null;
        }

        public void Alert(string message)
        {
            UserDialogs.Instance.Alert(message);
        }
    }
}