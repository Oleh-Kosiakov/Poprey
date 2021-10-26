using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Poprey.Core.DisplayModels
{
    public class AddInstagramAccount : MvxViewModel
    {
        public string ImageUrl { get; set; }

        public string AccountName { get; set; }

        private bool _showSeparator;
        public bool ShowSeparator
        {
            get => _showSeparator;
            set
            {
                if (value == _showSeparator)
                    return;

                _showSeparator = value;
                RaisePropertyChanged(() => ShowSeparator);
            }
        }

        public Action<AddInstagramAccount> OnRemoveCommand { get; set; }
        public Func<AddInstagramAccount, Task> OnSwitchCommand { get; set; }

        private IMvxCommand _removeCommand;
        public IMvxCommand RemoveCommand =>
            _removeCommand = _removeCommand ?? new MvxCommand(() => OnRemoveCommand?.Invoke(this));

        private IMvxCommand _switchCommand;
        public IMvxCommand SwitchCommand =>
            _switchCommand = _switchCommand ?? new MvxAsyncCommand(async () =>
            {
                if (OnSwitchCommand != null)
                {
                    await OnSwitchCommand.Invoke(this);
                }
            });
    }
}