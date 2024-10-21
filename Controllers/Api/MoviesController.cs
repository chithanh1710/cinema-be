using CINEMA_BE.Utils;
using Microsoft.Ajax.Utilities;
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
        public IHttpActionResult Get(string q = "",int page = 1,int pageSize = 10,string type = "",string cinemaName = "",int? directorId = null,int? actorId = null)
        {
            string typeQuery = type == "showing" ? "ĐANG CHIẾU" : type == "upcoming" ? "SẮP CHIẾU" : "";
            try
            {
                ApiContext<movy> movieContext = new ApiContext<movy>(db.movies);

                var data = movieContext
                    .Filter(m => m.name.Contains(q)
                        && (string.IsNullOrEmpty(typeQuery) || m.type.Equals(typeQuery))
                        && (directorId == null || m.id_director == directorId)
                        && (actorId == null || m.actors.Select(a=>a.id).Any(a=>a == actorId))
                        && (string.IsNullOrEmpty(cinemaName) || m.show_times.Any(s => s.screen_rooms.cinema.name.Equals(cinemaName))))
                    .SortBy(m => m.id, false)
                    .Pagination(page, pageSize)
                    .SelectProperties(m => new
                    {
                        m.id,
                        m.name,
                        m.duration,
                        m.description,
                        m.star,
                        m.old,
                        m.type,
                        m.trailer,
                        m.thumbnail,
                        show_times = m.show_times.Select(sh => new { sh.time_start,cinemaName = sh.screen_rooms.cinema.name,sh.id }),
                        genres = m.genres.Select(g => g.name),
                        actors = m.actors.Select(a => a.name),
                        director = new { m.director.id, m.director.name },
                        m.image,
                        m.release_date
                    }).ToList();

                int totalItem = movieContext.TotalItem();

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
                    m.duration,
                    m.star,
                    m.old,
                    m.type,
                    m.trailer,
                    m.thumbnail,
                    genres = m.genres.Select(g => new { g.id, g.name }),
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
                    data
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

                var validGenres = new List<genre>();
                var validActors = new List<actor>();
                var validShowTimes = new List<show_times>();

                // Tạo bản sao của danh sách genres, actors và show_times
                var genresCopy = movie.genres.ToList();
                var actorsCopy = movie.actors.ToList();
                var showTimesCopy = movie.show_times.ToList();

                // Lặp qua các thể loại được thêm vào
                foreach (var genre in genresCopy)
                {
                    var existingGenre = db.genres.Find(genre.id);
                    if (existingGenre != null)
                    {
                        validGenres.Add(existingGenre);
                    }
                }

                // Lặp qua các diễn viên được thêm vào
                foreach (var actor in actorsCopy)
                {
                    var existingActor = db.actors.Find(actor.id);
                    if (existingActor != null)
                    {
                        validActors.Add(existingActor);
                    }
                }

                // Lặp qua các show_times được thêm vào
                foreach (var showTime in showTimesCopy)
                {
                    var existingShowTime = db.show_times.Find(showTime.id);
                    if (existingShowTime != null)
                    {
                        validShowTimes.Add(existingShowTime);
                    }
                }

                // Gán lại danh sách đã lọc
                movie.genres = validGenres;
                movie.actors = validActors;
                movie.show_times = validShowTimes;

                db.movies.Add(movie);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Movie added successfully"
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
            try
            {
                if (movie == null)
                {
                    return BadRequest("Movie data cannot be null");
                }

                var existingMovie = db.movies.FirstOrDefault(m => m.id == id);

                if (existingMovie == null)
                {
                    return NotFound();
                }

                // Cập nhật các thông tin cơ bản của phim
                existingMovie.name = movie.name;
                existingMovie.duration = movie.duration;
                existingMovie.image = movie.image;
                existingMovie.description = movie.description;
                existingMovie.release_date = movie.release_date;
                existingMovie.id_director = movie.id_director;

                // Xử lý genres
                var validGenres = new List<genre>();
                var genresCopy = movie.genres.ToList();
                foreach (var genre in genresCopy)
                {
                    var existingGenre = db.genres.Find(genre.id);
                    if (existingGenre != null)
                    {
                        validGenres.Add(existingGenre);
                    }
                }
                existingMovie.genres.Clear(); // Xóa các liên kết hiện tại
                existingMovie.genres = validGenres; // Gán danh sách mới

                // Xử lý actors
                var validActors = new List<actor>();
                var actorsCopy = movie.actors.ToList();
                foreach (var actor in actorsCopy)
                {
                    var existingActor = db.actors.Find(actor.id);
                    if (existingActor != null)
                    {
                        validActors.Add(existingActor);
                    }
                }
                existingMovie.actors.Clear(); // Xóa các liên kết hiện tại
                existingMovie.actors = validActors; // Gán danh sách mới

                // Xử lý show_times
                var validShowTimes = new List<show_times>();
                var showTimesCopy = movie.show_times.ToList();

                foreach (var showTime in showTimesCopy)
                {
                    var newShowTime = new show_times
                    {
                        id_screen_room = showTime.id_screen_room,
                        time_start = showTime.time_start,
                        time_end = showTime.time_end
                    };

                    validShowTimes.Add(newShowTime);
                }
                existingMovie.show_times.Clear(); // Xóa các liên kết hiện tại
                existingMovie.show_times = validShowTimes; // Gán danh sách mới

                // Lưu thay đổi
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Movie updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
