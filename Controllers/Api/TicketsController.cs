using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace CINEMA_BE.Controllers
{
    public class TicketsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        // GET api/tickets
        [HttpGet]
        [Route("api/tickets")]

        // POST api/tickets
        // GET api/tickets
        public IHttpActionResult GetAllTickets(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var data = db.tickets
                    .Include(t => t.show_times)
                    .Include(t => t.show_times.movy)
                    .Include(t => t.seat)
                    .ToList(); // Tải tất cả dữ liệu vào bộ nhớ

                // Chuyển đổi các ticket thành một danh sách mới với dữ liệu đã được xử lý
                string search = Util.RemoveDiacritics(q);

                var filteredData = data
                    .Where(t => string.IsNullOrEmpty(q) ||
                        Util.RemoveDiacritics(t.show_times.movy.name).Contains(search))
                    .Select(t => new
                    {
                        t.id,
                        ShowTime = new
                        {
                            t.show_times.id,
                            t.show_times.time_start,
                            t.show_times.time_end,
                            MovieName = t.show_times.movy.name
                        },
                        Seat = new
                        {
                            t.seat.id,
                            t.seat.number_of_row,
                            t.seat.number_of_column,
                            t.seat.genre_seats
                        }
                    })
                    .OrderBy(t => t.id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (filteredData == null || !filteredData.Any())
                {
                    return NotFound();
                }

                int totalItem = db.tickets.Count();

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(filteredData)),
                    totalItems = totalItem,
                    page = page,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult Post([FromBody] ticket newTicket)
        {
            try
            {
                if (newTicket == null)
                {
                    return BadRequest("Ticket data cannot be null");
                }

                db.tickets.Add(newTicket);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Ticket added successfully",
                    data = newTicket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/tickets/5
        [HttpPut]
        [Route("api/tickets/{id}")]
        public IHttpActionResult Put(int id, [FromBody] ticket updatedTicket)
        {
            if (updatedTicket == null || updatedTicket.id != id)
            {
                return BadRequest("Invalid ticket data.");
            }

            try
            {
                var existingTicket = db.tickets.FirstOrDefault(t => t.id == id);
                if (existingTicket == null)
                {
                    return NotFound();
                }

                existingTicket.id_seat = updatedTicket.id_seat;
                existingTicket.id_show_time = updatedTicket.id_show_time;

                db.Entry(existingTicket).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Ticket updated successfully",
                    data = updatedTicket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/tickets/5
        [HttpDelete]
        [Route("api/tickets/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var ticket = db.tickets.FirstOrDefault(t => t.id == id);
                if (ticket == null)
                {
                    return NotFound();
                }

                db.tickets.Remove(ticket);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Ticket deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
