using System.Text;
using MvvmCross;
using MvvmCross.ViewModels;
using Poprey.Core.Messages;
using Poprey.Core.Util;

namespace Poprey.Core.DisplayModels
{
    public class MenuHeaderItem : MvxViewModel
    {
        private readonly MessageTokenHelper _messenger = Mvx.IoCProvider.Resolve<MessageTokenHelper>();
        private const int MaxCharactersInTitle = 15;

        private bool _toLeftOfSelection;

        public bool ToLeftOfSelection
        {
            get => _toLeftOfSelection;
            set
            {
                _toLeftOfSelection = value;

                RaisePropertyChanged(() => Title);
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                if (ToLeftOfSelection)
                {
                    return PutSpacesInTheBeginningOfTheTitle(_title);
                }

                return _title;
            }
            set
            {
                _title = value;

                RaisePropertyChanged(() => Title);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;

                _messenger.Publish(new SetCollapsedBagDataMessage(this)
                {
                    TempServiceName = Title
                });

                RaisePropertyChanged(() => IsActive);
            }
        }

        private string PutSpacesInTheBeginningOfTheTitle(string title)
        {
            var strBuilder = new StringBuilder(title);

            for (var i = 0; i < MaxCharactersInTitle - title.Length; i++)
            {
                strBuilder.Insert(0, ' ');
            }

            return strBuilder.ToString();
        }
    }
}