using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Poprey.Core.Messages;
using Poprey.Core.Models.Bag;
using Poprey.Core.Util;

namespace Poprey.Core.DisplayModels
{
    public class BagItem : MvxViewModel
    {
        private readonly MessageTokenHelper _messenger = Mvx.IoCProvider.Resolve<MessageTokenHelper>();
        public Order Order { get; set; }
        public string ServicePseudonym { get; set; }

        public bool IsCompact { get; set; }
        public bool OptionsVisibleInTheCompactMode { get; set; } = true;
        public string ServiceName { get; set; }
        public int TariffPlan { get; set; }
        public double ServicePrice { get; set; }
        public int ImpressionsCount { get; set; }
        public double ImpressionPrice { get; set; }
        public int DiscountPercent { get; set; }

        public MvxObservableCollection<InstagramPostPreview> PostPreviews { get; set; }

        private bool _isInAlternativeMode;
        public bool IsInAlternativeMode
        {
            get => _isInAlternativeMode;
            set
            {
                _isInAlternativeMode = value;

                RaisePropertyChanged(() => IsInAlternativeMode);
            }
        }

        private bool _isSeparatorVisible = true;
        public bool IsSeparatorVisible
        {
            get => _isSeparatorVisible;
            set
            {
                _isSeparatorVisible = value;

                RaisePropertyChanged(() => IsSeparatorVisible);
            }
        }

        private bool _impressionsOnceWereAdded;
        private bool _isImpressionsAdded;
        public bool IsImpressionsAdded
        {
            get => _isImpressionsAdded;
            set
            {
                _isImpressionsAdded = value;
                _impressionsOnceWereAdded = value || _impressionsOnceWereAdded;

                if (_impressionsOnceWereAdded)
                {
                    if (value)
                    {
                        _messenger.Publish(new SetCollapsedBagDataMessage(this)
                        {
                            AddToSum = ImpressionPrice
                        });
                    }
                    else
                    {
                        _messenger.Publish(new SetCollapsedBagDataMessage(this)
                        {
                            SubstractFromSum = ImpressionPrice
                        });
                    }
                }

                RaisePropertyChanged(() => IsImpressionsAdded);
            }
        }

        public string NormalLabelText { get; set; }
        public string AlternativeLabelText { get; set; }

        #region Commands

        public IMvxCommand RemoveCommand { get; set; }

        private IMvxCommand _normalModeSelectedCommand;
        public IMvxCommand NormalModeSelectedCommand => _normalModeSelectedCommand =
            _normalModeSelectedCommand ?? new MvxCommand(() =>
            {
            });

        private IMvxCommand _alternativeModeSelectedCommand;

        public IMvxCommand AlternativeModeSelectedCommand => _alternativeModeSelectedCommand =
            _alternativeModeSelectedCommand ?? new MvxCommand(() =>
            {
            });

        private IMvxCommand _toggleAddedImpressionsLayout;
        public IMvxCommand ToggleAddedImpressionsLayout => _toggleAddedImpressionsLayout = _toggleAddedImpressionsLayout ?? new MvxCommand(
           () =>
           {
               IsImpressionsAdded = !IsImpressionsAdded;
           });


        #endregion
    }
}