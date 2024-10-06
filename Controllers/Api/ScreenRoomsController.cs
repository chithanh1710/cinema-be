using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class ScreenRoomsController : ApiController
    {
        // GET: api/ScreenRooms
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ScreenRooms/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ScreenRooms
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ScreenRooms/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ScreenRooms/5
        public void Delete(int id)
        {
        }
    }
}
