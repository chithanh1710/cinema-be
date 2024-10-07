using CINEMA_BE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class GenresController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // GET api/genres
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<genre> movieContext = new ApiContext<genre>(db.genres);

                var data = movieContext.Filter(g => g.name.Contains(q)).SortBy(g => g.id, false).Pagination(page, pageSize).SelectProperties(g => new
                {
                    g.id,
                    g.name,
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = movieContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q == "" ? null : q,
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

        // GET api/genres/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<genre> movieContext = new ApiContext<genre>(db.genres);

                var data = movieContext.Filter(g => g.id == id).SelectProperties(g => new
                {
                    g.id,
                    g.name,
                    totalMovies = g.movies.Count,
                    movies = g.movies.Select(m => new 
                    {
                        m.name, m.description, m.image, m.release_date, m.duration 
                    })
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

        // POST: api/Genres
        public IHttpActionResult Post([FromBody] genre newGenre)
        {
            try
            {
                if (newGenre == null)
                {
                    return BadRequest("Genre data cannot be null");
                }

                // Kiểm tra xem thể loại đã tồn tại chưa
                var existingGenre = db.genres.FirstOrDefault(g => g.name == newGenre.name);
                if (existingGenre != null)
                {
                    return BadRequest("Genre already exists");
                }

                db.genres.Add(newGenre);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Genre added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Genres/5
        public IHttpActionResult Put(int id, [FromBody] genre updatedGenre)
        {
            try
            {
                if (updatedGenre == null)
                {
                    return BadRequest("Genre data cannot be null");
                }

                // Tìm thể loại cần cập nhật
                var existingGenre = db.genres.Find(id);
                if (existingGenre == null)
                {
                    return NotFound(); // Trả về 404 nếu không tìm thấy thể loại
                }

                // Cập nhật thông tin thể loại
                existingGenre.name = updatedGenre.name; // Cập nhật tên thể loại
                                                        // Bạn có thể thêm các thuộc tính khác nếu cần

                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Genre updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/Genres/5
        public void Delete(int id)
        {
        }
    }
}
