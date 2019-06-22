using Apworks.Integration.AspNetCore.DataServices;
using Apworks.Repositories;
using EasyBooking.MeetingRooms.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBooking.MeetingRooms.API.Controllers
{
    [Route("api/[controller]")]
    public class MeetingRoomsController : DataServiceController<Guid, MeetingRoom>
    {
        public MeetingRoomsController(IRepositoryContext context)
            : base(context) { }

        public override Task<IActionResult> Post([FromBody] MeetingRoom aggregateRoot)
        {
            aggregateRoot.DateCreated = DateTime.UtcNow;
            return base.Post(aggregateRoot);
        }
    }
}
