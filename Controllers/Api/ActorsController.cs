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
                    movies = a.movies.Select(m => new { m.id, m.name }),
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = actorContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<actor>.settings))
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
                    movies = a.movies.Select(m => new { m.id, m.name }),
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<actor>.settings))
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
                    data = actor
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
            if (actor == null || actor.id != id)
            {
                return BadRequest("Invalid actor data.");
            }

            try
            {
                db.actors.Attach(actor);
                db.Entry(actor).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { status = "success", message = "Actor updated successfully", data = actor });
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
