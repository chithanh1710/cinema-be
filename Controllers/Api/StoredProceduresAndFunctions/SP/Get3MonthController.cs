using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.SP
{
    public class Get3MonthController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/Get3Month
        public IHttpActionResult Get()
        {
            var data = db.Get3Month();
            return Ok(new { status = "success", data });
        }
    }
}
