using FitnessIntl.Models;
using FitnessIntl.ViewModels;

namespace NavigationMenu.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public MenuItemViewModel Item { get; set; }
        public ItemDetailViewModel(MenuItemViewModel item = null)
        {
            Item = item;
            Title = item?.Name;
        }
    }
}