using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace CINEMA_BE.Controllers
{
    public class DirectorsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        // GET api/directors
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<director> directorContext = new ApiContext<director>(db.directors);
                string search = Util.RemoveDiacritics(q);

                var data = directorContext.Filter(d => d.name.Contains(search)).SortBy(d => d.id, false).Pagination(page, pageSize).SelectProperties(d => new
                {
                    d.id,
                    d.name,
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = directorContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/directors/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<director> directorContext = new ApiContext<director>(db.directors);

                var data = directorContext.Filter(d => d.id == id).SelectProperties(d => new
                {
                    d.id,
                    d.name,
                    movies = d.movies.Select(m => new { m.id, m.name, m.description, m.image, m.release_date }),
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

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

        // POST api/directors
        public IHttpActionResult Post([FromBody] director director)
        {
            try
            {
                if (director == null)
                {
                    return BadRequest("Director data cannot be null");
                }

                db.directors.Add(director);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Director added successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/directors/5
        public IHttpActionResult Put(int id, [FromBody] director director)
        {
            try
            {
                if (director == null)
                {
                    return BadRequest("Invalid director data.");
                }

                var existingDirector = db.directors.Find(id);
                if (existingDirector == null)
                {
                    return NotFound();
                }

                existingDirector.name = director.name;

                db.SaveChanges();

                return Ok(new { status = "success", message = "Director updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // DELETE api/directors/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var director = db.directors.FirstOrDefault(d => d.id == id);
                if (director == null)
                {
                    return NotFound();
                }

                db.directors.Remove(director);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Director deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
