using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.Seats
{
    public class GetSeatsByShowtimeController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        public IHttpActionResult Get(int showtimeId)
        {
            var seats = db.screen_rooms_seats
                .Where(s => s.id_showtime == showtimeId)
                .Select(s => new
                {
                    s.id_seat,
                    s.reservedBy,
                    s.seat.number_of_column,
                    s.seat.number_of_row,
                    s.seat.genre_seats,
                    s.status
                })
                .ToList();

            return Ok(seats);
        }
    }
}
