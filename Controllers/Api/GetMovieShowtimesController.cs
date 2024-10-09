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
        public IHttpActionResult Get(int idMovie, string cinemaName = null, DateTime? date = null)
        {
            if (idMovie <= 0)
            {
                return BadRequest("Invalid movie ID.");
            }

            try
            {
                var showtimes = db.GetMovieShowtimes(idMovie, cinemaName, date).ToList();

                var groupedData = showtimes
                .GroupBy(st => st.CinemaName)
                .Select(cinemaGroup => new
                {
                    CinemaName = cinemaGroup.Key,
                    ScreenRooms = cinemaGroup
                        .GroupBy(st => st.ScreenRoomName)
                        .Select(screenRoomGroup => new
                        {
                            ScreenRoomName = screenRoomGroup.Key,
                            ShowDates = screenRoomGroup
                                .GroupBy(st => st.ShowTimeDate)
                                .Select(dateGroup => new
                                {
                                    ShowTimeDate = dateGroup.Key,
                                    ShowTimes = dateGroup.Select(st => new
                                    {
                                        ShowTimeStart = st.ShowTimeStart,
                                        ShowTimeEnd = st.ShowTimeEnd
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
