using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace CINEMA_BE.Controllers
{
    public class ValuesController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // GET api/values
        public IHttpActionResult Get(int page = 1,int pageSize = 10)
        {
            try
            {
                ApiContext<movy> movieContext = new ApiContext<movy>(db.movies);

                var data = movieContext.Filter(m => m.id > 1).SortBy(m => m.id, false).Pagination(1, 3).SelectProperties(m => new { m.id }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = movieContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<movy>.settings))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
