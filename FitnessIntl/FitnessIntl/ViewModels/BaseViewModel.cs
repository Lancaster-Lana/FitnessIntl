using FitnessIntl.Helpers;
using FitnessIntl.Models;
using FitnessIntl.Services;
using Xamarin.Forms;

namespace FitnessIntl.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get the azure service instance
        /// </summary>
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
     
    }
}

