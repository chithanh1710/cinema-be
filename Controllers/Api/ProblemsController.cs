using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class ProblemsController : ApiController
    {
        // GET: api/Problems
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Problems/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Problems
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Problems/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Problems/5
        public void Delete(int id)
        {
        }
    }
}
