using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.Seats
{
    public class HoldSeatsRequest
    {
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }
        public int CustomerId { get; set; } // Thêm CustomerId để xác định ai giữ ghế
    }
    public class UnHoldSeatController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        public IHttpActionResult Post([FromBody] HoldSeatsRequest request)
        {
            try
            {
                var seat = db.screen_rooms_seats
                    .FirstOrDefault(s => s.id_showtime == request.ShowtimeId && s.id_seat == request.SeatId);

                if (seat == null || seat.status != "ĐANG GIỮ" || seat.reservedBy != request.CustomerId)
                {
                    return BadRequest($"Ghế {request.SeatId} không khả dụng hoặc bạn không có quyền hủy giữ ghế này.");
                }

                db.UnholdSeatAndDeleteTicket(request.ShowtimeId, request.SeatId, request.CustomerId);
                return Ok(new { message = "Hủy giữ ghế thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
