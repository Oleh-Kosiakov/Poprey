using Poprey.Core.ViewModels;

namespace Poprey.Core.Services.Interfaces
{
    public interface IPopupService
    {
        void ShowAuthenticationDialog(AuthenticationDialogViewModel dataContext);
        void ShowEmailDialog(EmailDialogViewModel dataContext);
        void ShowLoading();
        void HideLoading();
        void Alert(string message);
    }
}