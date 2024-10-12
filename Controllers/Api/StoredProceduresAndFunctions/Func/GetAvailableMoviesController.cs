using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class GetAvailableMoviesController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetAvailableMovies
        public IHttpActionResult Get()
        {
            var data = db.GetAvailableMovies();
            return Ok(new { status = "success", data });
        }
    }
}
