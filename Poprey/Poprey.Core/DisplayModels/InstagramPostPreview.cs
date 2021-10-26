using MvvmCross.ViewModels;

namespace Poprey.Core.DisplayModels
{
    public class InstagramPostPreview : MvxViewModel
    {
        public string ImageUrl { get; set; }
        public string PostLink { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                RaisePropertyChanged(() => IsSelected);
            }
        }

        private int _number;

        public int Number
        {
            get => _number;
            set
            {
                _number = value;

                RaisePropertyChanged(() => Number);
            }
        }

    }
}