using MvvmCross.ViewModels;
using Poprey.Core.DisplayModels;

namespace Poprey.Core.ViewModels
{
    public interface IHeaderMenuItemViewModel : IMvxViewModel
    {
        MenuHeaderItem HeaderItem { get; }
    }
}