using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.Func
{
    public class GetCurrentAndPreviousDayRevenueController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetCurrentAndPreviousDayRevenue
        public IHttpActionResult Get()
        {
            var data = db.GetCurrentAndPreviousDayRevenue();
            return Ok(new { status = "success", data });
        }
    }
}
