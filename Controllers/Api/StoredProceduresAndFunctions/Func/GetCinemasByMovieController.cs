using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.Func
{
    public class GetCinemasByMovieController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetCinemasByMovie
        public IHttpActionResult Get(int movieId)
        {
            var data = db.GetCinemasByMovie1(movieId);
            return Ok(new { status = "success", data });
        }
    }
}
