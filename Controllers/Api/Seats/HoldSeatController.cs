using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.Seats
{

    public class HoldSeatController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        public IHttpActionResult Post([FromBody] HoldSeatsRequest request)
        {
            try
            {
                db.HoldSeatAndCreateTicket(request.ShowtimeId, request.SeatId, request.CustomerId);
                return Ok(new { message = "Ghế đã được giữ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
