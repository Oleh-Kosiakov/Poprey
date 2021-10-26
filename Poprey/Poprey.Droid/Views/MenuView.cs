using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core.ViewModels;

namespace Poprey.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.navigation_frame)]
    [Register("poprey.droid.views.MenuView")]
    public class MenuFragment : BaseFragment<MenuViewModel>
    {
        private TextView _popreyLabel;
        private TextView _visitSiteLabel;
        private MvxRecyclerView _menuItemsList;

        protected override int FragmentId => Resource.Layout.MenuView;

        protected override void InitComponents(View fragmentView)
        {
            _visitSiteLabel = fragmentView.FindViewById<TextView>(Resource.Id.visit_site_label);
            _popreyLabel = fragmentView.FindViewById<TextView>(Resource.Id.mainLabel);
            _menuItemsList = fragmentView.FindViewById<MvxRecyclerView>(Resource.Id.MenuListView);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<MenuFragment, MenuViewModel>();

            bindingSet.Bind(_popreyLabel).For(v => v.Text).To(vm => vm.PopreyText);

            bindingSet.Bind(_visitSiteLabel).For(v => v.Text).To(vm => vm.VisitSiteItemText);
            bindingSet.Bind(_visitSiteLabel).For("Click").To(vm => vm.VisitSiteCommand);

            bindingSet.Bind(_menuItemsList).For(v => v.ItemsSource).To(vm => vm.MenuItemsCollection);
            bindingSet.Bind(_menuItemsList).For(v => v.ItemClick).To(vm => vm.MenuItemClickedCommand);

            bindingSet.Apply();
        }

    }
}
