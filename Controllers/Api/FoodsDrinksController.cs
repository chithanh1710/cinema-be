using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class FoodsDrinksController : ApiController
    {
        // GET: api/FoodsDrinks
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FoodsDrinks/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FoodsDrinks
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/FoodsDrinks/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/FoodsDrinks/5
        public void Delete(int id)
        {
        }
    }
}
