using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI;

namespace CINEMA_BE.Controllers
{
    public class MoviesController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // GET api/movies
        public IHttpActionResult Get(string q = "",int page = 1,int pageSize = 10)
        {
            try
            {
                ApiContext<movy> movieContext = new ApiContext<movy>(db.movies);
                string search = Util.RemoveDiacritics(q);

                var data = movieContext.Filter(m => m.name.Contains(search)).SortBy(m => m.id, false).Pagination(page, pageSize).SelectProperties(m => new
                {
                    m.id,
                    m.name,
                    m.description,
                    director = new { m.director.id, m.director.name },
                    actors = m.actors.Select(a => new { a.id, a.name }),
                    m.image,
                    show_times = m.show_times.Select(time => new { time.id, time.id_screen_room, time.time_start, time.time_end }),
                    m.release_date,
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = movieContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<movy>.settings))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/movies/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<movy> movieContext = new ApiContext<movy>(db.movies);

                var data = movieContext.Filter(m => m.id == id).SelectProperties(m => new
                {
                    m.id,
                    m.name,
                    m.description,
                    director = new { m.director.id, m.director.name },
                    actors = m.actors.Select(a => new { a.id, a.name }),
                    m.image,
                    show_times = m.show_times.Select(time => new { time.id, time.id_screen_room, time.time_start, time.time_end }),
                    m.release_date,
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = movieContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<movy>.settings))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/movies
        public IHttpActionResult Post([FromBody] movy movie)
        {
            try
            {
                if (movie == null)
                {
                    return BadRequest("Movie data cannot be null");
                }

                db.movies.Add(movie);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Movie added successfully",
                    data = movie
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/movies/5
        public IHttpActionResult Put(int id, [FromBody] movy movie)
        {
            if (movie == null || movie.id != id)
            {
                return BadRequest("Invalid movie data.");
            }

            try
            {
                // Gán ID cho đối tượng movie để cập nhật
                db.movies.Attach(movie);

                // Đánh dấu đối tượng là đã thay đổi
                db.Entry(movie).State = EntityState.Modified;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return Ok(new { status = "success", message = "Movie updated successfully", data = movie });

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

        }


        // DELETE api/movies/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                // Tìm movie dựa trên id
                var movie = db.movies.FirstOrDefault(m => m.id == id);
                if (movie == null)
                {
                    return NotFound();
                }

                db.movies.Remove(movie);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Movie deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
