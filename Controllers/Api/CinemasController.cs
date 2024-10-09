using CINEMA_BE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class CinemasController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/Cinemas
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<cinema> cinemaContext = new ApiContext<cinema>(db.cinemas);

                var data = cinemaContext.Filter(c => c.name.Contains(q)).SortBy(c => c.id, false).Pagination(page, pageSize).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.address,
                    c.city,
                    c.amount_rooms,
                    sreenRooms = c.screen_rooms.Select(r => new { r.id, r.name, r.amount_seats }),
                }).ToList();

                int totalItem = cinemaContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    currentPage = page,
                    pageSize,
                    totalItem,
                    totalPage = (int)Math.Ceiling((double)totalItem / pageSize),
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Cinemas/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<cinema> cinemaContext = new ApiContext<cinema>(db.cinemas);

                var data = cinemaContext.Filter(c => c.id == id).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.address,
                    c.city,
                    c.amount_rooms,
                    sreenRooms = c.screen_rooms.Select(r => new { r.id, r.name, r.amount_seats }),
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = cinemaContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/Cinemas
        public IHttpActionResult Post([FromBody] cinema cinema)
        {
            try
            {
                if (cinema == null)
                {
                    return BadRequest("Cinema data cannot be null");
                }

                db.cinemas.Add(cinema);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Cinema added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Cinemas/5
        public IHttpActionResult Put(int id, [FromBody] cinema cinema)
        {
            try
            {

                if (cinema == null)
                {
                    return BadRequest("Invalid cinema data.");
                }

                var existingCinema = db.cinemas.Find(id);
                if (existingCinema == null)
                {
                    return NotFound();
                }

                existingCinema.name = cinema.name;
                existingCinema.address = cinema.address;
                existingCinema.city = cinema.city;
                existingCinema.amount_rooms = cinema.amount_rooms;

                db.SaveChanges();

                return Ok(new { status = "success", message = "Cinema updated successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

        }
    }
}
