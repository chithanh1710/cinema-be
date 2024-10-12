using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.Func
{
    public class GetAvailableShowtimesController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetAvailableShowtimes
        public IHttpActionResult Get(int movieId, int cinemaId, DateTime showDate)
        {
            var data = db.GetAvailableShowtimes(movieId, cinemaId, showDate);
            return Ok(new { status = "success", data });
        }
    }
}
