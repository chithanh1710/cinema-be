using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.Func
{
    public class GetMonthlyRevenueController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetAvailableDates
        public IHttpActionResult Get()
        {
            var data = db.GetMonthlyRevenue();
            return Ok(new { status = "success", data });
        }
    }
}
