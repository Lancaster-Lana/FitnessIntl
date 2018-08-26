using System;

namespace FitnessIntl.Models
{
    public class Announcement //: Entity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
