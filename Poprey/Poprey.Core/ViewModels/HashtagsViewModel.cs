using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Resources;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class HashtagsViewModel : BaseViewModel<object>
    {
        private readonly IHashtagsService _hashtagsService;

        public HashtagsViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService,
            MessageTokenHelper messenger, IPopupService popupService, IHashtagsService hashtagsService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _hashtagsService = hashtagsService;
            MainViewModel.HashtagViewModel = this;
        }

        public string HeaderText => Strings.HashtagHeader;

        public string PlaceholderText => Strings.HashtagPlaceholder;

        public bool ArrowVisible => !string.IsNullOrEmpty(SelectedHashtag);

        public string SelectedHashtag
        {
            get => _selectedHashtag;
            set
            {
                _selectedHashtag = value;

                RaisePropertyChanged(() => ArrowVisible);
                RaisePropertyChanged(() => SelectedHashtag);
            }
        }

        private IMvxCommand _searchForHashtagCommand;
        private string _selectedHashtag;

        public IMvxCommand SearchForHashtagCommand =>
            _searchForHashtagCommand ?? (_searchForHashtagCommand = new MvxAsyncCommand(
                 async () =>
                 {
                     var e = await WebRequest(() => _hashtagsService.LoadSimilarHashtags(SelectedHashtag, CtsHelper.CreateCts().Token));

                     switch (e)
                     {
                         case ServiceResolution.Success:
                             {
                                 await NavigationService.Navigate<HashtagsListViewModel, string>(SelectedHashtag);
                                 break;
                             }
                         case ServiceResolution.SimilarHashtagsNotFound:
                             PopupService.Alert(Strings.Hashtag_SimilarHashtagsNotFound);
                             break;
                     }
                 }));

    }
}
