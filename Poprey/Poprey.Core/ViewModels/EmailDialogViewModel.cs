using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Poprey.Core.Resources;

namespace Poprey.Core.ViewModels
{
    public class EmailDialogViewModel : MvxViewModel
    {
        public string HeaderText => Strings.Email_Header;
        public string EmailHintText => Strings.Email_Hint;
        public string LicenseText => Strings.Email_License;

        public string EnteredEmail { get; set; }

        private bool _shouldDismiss;
        public bool ShouldDismiss
        {
            get => _shouldDismiss;
            set
            {
                _shouldDismiss = value;

                RaisePropertyChanged(() => ShouldDismiss);
            }
        }

        public void Reset()
        {
            EnteredEmail = string.Empty;

            RaiseAllPropertiesChanged();
        }

        public Func<Task> OnProceedWithEmail { get; set; }

        #region Commands

        private IMvxCommand _closeCommand;
        public IMvxCommand CloseCommand => _closeCommand = _closeCommand ?? new MvxCommand(() => { ShouldDismiss = true; });
      
        public IMvxCommand ShowLicenseCommand { get; set; }

        private IMvxCommand _proceedWithEmailCommand;
        public IMvxCommand ProceedWithEmailCommand => _proceedWithEmailCommand = _proceedWithEmailCommand ?? new MvxAsyncCommand(
        async () =>
        {
            ShouldDismiss = true;

            await OnProceedWithEmail.Invoke();
        });
        #endregion
    }
}