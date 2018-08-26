using FitnessIntl.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FitnessIntl.ViewModels
{
    public class AnnouncementsViewModel //: ObservableCollection<AnnouncementViewModel>
    {
        //public string Country { get; set; }
        //public string Date { get; set; }

        public ObservableCollection<AnnouncementViewModel> List { get; set; }

        public AnnouncementsViewModel()
        {
            var announcementsList = new List<AnnouncementViewModel>();
            List = new ObservableCollection<AnnouncementViewModel>(announcementsList);
        }
    }

    public class AnnouncementViewModel : ObservableObject
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}