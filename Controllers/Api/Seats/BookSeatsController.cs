using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.Seats
{
    public class BookSeatsRequest
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; }
        public int CustomerId { get; set; } // Thêm CustomerId để xác định ai đặt ghế
    }

    public class BookSeatsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        public IHttpActionResult Post([FromBody] BookSeatsRequest request)
        {
            foreach (var seatId in request.SeatIds)
            {
                var seat = db.screen_rooms_seats
                    .FirstOrDefault(s => s.id_showtime == request.ShowtimeId && s.id_seat == seatId);

                if (seat == null || seat.status != "ĐANG GIỮ" || seat.reservedBy != request.CustomerId)
                {
                    return BadRequest($"Ghế {seatId} không khả dụng để đặt hoặc bạn không có quyền đặt ghế này.");
                }

                // Cập nhật trạng thái ghế thành "ĐÃ ĐẶT"
                seat.status = "ĐÃ ĐẶT";
                seat.reservedBy = request.CustomerId; // Ghi lại thông tin người đặt
            }

            db.SaveChanges();
            return Ok(new { message = "Ghế đã được đặt thành công." });
        }
    }
}
