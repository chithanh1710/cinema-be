using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class GetMovieShowtimesController : ApiController
    {
        private readonly QL_RCP_Entities db = new QL_RCP_Entities();

        // GET: api/GetMovieShowtimes?idMovie={id}&cinemaName={cinemaName}&date={date}
        public IHttpActionResult Get(int movieId, string cinemaName = null, string cityName = null)
        {
            if (movieId <= 0)
            {
                return BadRequest("Invalid movie ID.");
            }

            try
            {
                var showtimes = db.GetMovieShowtimes1(movieId, cityName, cinemaName).ToList();

                var groupedData = showtimes
                .GroupBy(st => new { st.cinema_name, st.city_name })
                .Select(cinemaGroup => new
                {
                    cinema_name = cinemaGroup.Key.cinema_name,
                    city_name = cinemaGroup.Key.city_name,
                    screen_rooms = cinemaGroup
                        .GroupBy(st => st.screen_room_name)
                        .Select(screenRoomGroup => new
                        {
                            screen_room_name = screenRoomGroup.Key,
                            show_dates = screenRoomGroup
                                .GroupBy(st => st.show_time_date)
                                .Select(dateGroup => new
                                {
                                    show_time_date = dateGroup.Key,
                                    show_times = dateGroup.Select(st => new
                                    {
                                        show_time_start = st.show_time_start,
                                        show_time_end = st.show_time_end
                                    }).ToList()
                                }).ToList()
                        }).ToList()
                }).ToList();


                return Ok(new
                {
                    status = "success",
                    data = groupedData
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
