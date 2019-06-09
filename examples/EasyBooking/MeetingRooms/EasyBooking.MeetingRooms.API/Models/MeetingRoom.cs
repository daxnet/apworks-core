using Apworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBooking.MeetingRooms.API.Models
{
    public class MeetingRoom : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public bool? IsAvailable { get; set; }

        public Location Location { get; set; }

        public override string ToString() => Name;
    }
}
