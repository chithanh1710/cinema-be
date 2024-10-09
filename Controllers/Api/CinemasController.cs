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
        QL_RCP_Entities db=new QL_RCP_Entities();
        // GET: api/Cinemas
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<cinema> cinemaContext = new ApiContext<cinema>(db.cinemas);
                string search = Util.RemoveDiacritics(q);

                var data = cinemaContext.Filter(c => c.name.Contains(search)).SortBy(c => c.id, false).Pagination(page, pageSize).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.address,
                    c.amount_rooms,
                    sreenRooms= c.screen_rooms.Select(r => new { r.id, r.name ,r.amount_seats}),
                    
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = cinemaContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<cinema>.settings))
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
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<cinema>.settings))
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
                    message = "Cinema added successfully",
                    data = cinema
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
            if (cinema == null || cinema.id != id)
            {
                return BadRequest("Invalid cinema data.");
            }

            try
            {
                db.cinemas.Attach(cinema);
                db.Entry(cinema).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { status = "success", message = "Cinema updated successfully", data = cinema });

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

        }

        // DELETE: api/Cinemas/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var cinema = db.cinemas.FirstOrDefault(c => c.id == id);
                if (cinema == null)
                {
                    return NotFound();
                }

                db.cinemas.Remove(cinema);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Cinema deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
