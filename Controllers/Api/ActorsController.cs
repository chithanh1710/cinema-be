using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class ActorsController : ApiController
    {
        // GET: api/Actors
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Actors/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Actors
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Actors/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Actors/5
        public void Delete(int id)
        {
        }
    }
}
