using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace CINEMA_BE.Controllers
{
    public class ActorsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        // GET api/actors
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<actor> actorContext = new ApiContext<actor>(db.actors);
                string search = Util.RemoveDiacritics(q);

                var data = actorContext.Filter(a => a.name.Contains(search)).SortBy(a => a.id, false).Pagination(page, pageSize).SelectProperties(a => new
                {
                    a.id,
                    a.name,
                }).ToList();

                int totalItem = actorContext.TotalItem();

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

        // GET api/actors/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<actor> actorContext = new ApiContext<actor>(db.actors);

                var data = actorContext.Filter(a => a.id == id).SelectProperties(a => new
                {
                    a.id,
                    a.name,
                    movies = a.movies.Select(m => new { m.id, m.name, m.description, m.image, m.release_date }),
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

        // POST api/actors
        public IHttpActionResult Post([FromBody] actor actor)
        {
            try
            {
                if (actor == null)
                {
                    return BadRequest("Actor data cannot be null");
                }

                db.actors.Add(actor);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Actor added successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/actors/5
        public IHttpActionResult Put(int id, [FromBody] actor actor)
        {
            try
            {
                if (actor == null)
                {
                    return BadRequest("Invalid actor data.");
                }

                var existingActor = db.actors.Find(id);
                if (existingActor == null)
                {
                    return NotFound();
                }

                existingActor.name = actor.name;

                db.SaveChanges();

                return Ok(new { status = "success", message = "Actor updated successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // DELETE api/actors/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var actor = db.actors.FirstOrDefault(a => a.id == id);
                if (actor == null)
                {
                    return NotFound();
                }

                db.actors.Remove(actor);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Actor deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
