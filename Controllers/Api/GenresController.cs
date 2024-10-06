using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class GenresController : ApiController
    {
        // GET: api/Genres
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Genres/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Genres
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Genres/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Genres/5
        public void Delete(int id)
        {
        }
    }
}
