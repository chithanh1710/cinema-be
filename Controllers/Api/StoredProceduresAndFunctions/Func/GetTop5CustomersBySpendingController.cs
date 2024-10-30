using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.Func
{
    public class GetTop5CustomersBySpendingController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/GetTop5CustomersBySpending
        public IHttpActionResult Get()
        {
            var data = db.GetTop5Customers();
            return Ok(new { status = "success", data });
        }
    }
}
