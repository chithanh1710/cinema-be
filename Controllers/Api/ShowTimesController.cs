using CINEMA_BE.Utils; // Model reference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class ShowTimesController : ApiController
    {
        private QL_RCP_Entities db = new QL_RCP_Entities();

        // GET: api/ShowTimes
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var showTimes = db.show_times
                                .Where(s => s.screen_rooms.name.Contains(q) || s.movy.name.Contains(q))
                                .OrderBy(s => s.time_start)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Select(s => new
                                {
                                    s.id,
                                    s.time_start,
                                    s.time_end,
                                    movie = new { s.movy.id, s.movy.name },
                                    screen_room = new { s.screen_rooms.id, s.screen_rooms.name }
                                })
                                .ToList();

                if (!showTimes.Any())
                {
                    return NotFound();
                }

                var totalItems = db.show_times.Count();
                return Ok(new
                {
                    status = "success",
                    currentPage = page,
                    pageSize,
                    totalItems,
                    data = showTimes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/ShowTimes/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var showTime = db.show_times
                                .Where(s => s.id == id)
                                .Select(s => new
                                {
                                    s.id,
                                    s.time_start,
                                    s.time_end,
                                    movie = new { s.movy.id, s.movy.name },
                                    screen_room = new { s.screen_rooms.id, s.screen_rooms.name }
                                })
                                .FirstOrDefault();

                if (showTime == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    status = "success",
                    data = showTime
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/ShowTimes
        public IHttpActionResult Post([FromBody] show_times showTime)
        {
            try
            {
                if (showTime == null)
                {
                    return BadRequest("Show time data cannot be null");
                }

                // Kiểm tra movie và screen_room tồn tại
                var movie = db.movies.Find(showTime.id_movie);
                var screenRoom = db.screen_rooms.Find(showTime.id_screen_room);

                if (movie == null || screenRoom == null)
                {
                    return BadRequest("Movie or Screen Room not found");
                }

                // Thêm show_time vào cơ sở dữ liệu
                db.show_times.Add(showTime);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Show time added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/ShowTimes/5
        public IHttpActionResult Put(int id, [FromBody] show_times showTime)
        {
            try
            {
                if (showTime == null)
                {
                    return BadRequest("Show time data cannot be null");
                }

                var existingShowTime = db.show_times.FirstOrDefault(s => s.id == id);
                if (existingShowTime == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin show_time
                existingShowTime.id_movie = showTime.id_movie;
                existingShowTime.id_screen_room = showTime.id_screen_room;
                existingShowTime.time_start = showTime.time_start;
                existingShowTime.time_end = showTime.time_end;

                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Show time updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ShowTimes/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var showTime = db.show_times.FirstOrDefault(s => s.id == id);
                if (showTime == null)
                {
                    return NotFound();
                }

                db.show_times.Remove(showTime);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Show time deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
