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
        public IHttpActionResult Get(int id, string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<ticket> ticketContext = new ApiContext<ticket>(db.tickets);
                string search = Util.RemoveDiacritics(q);

                var data = ticketContext.Filter(t => t.id == id).SelectProperties(t => new
                {
                    t.id,
                    show_time = db.show_times.FirstOrDefault(st => st.id == t.id_show_time), // Adjust for foreign key relation
                    t.seat
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = ticketContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data)) // Without custom settings
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/tickets/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<ticket> ticketContext = new ApiContext<ticket>(db.tickets);

                var data = ticketContext.Filter(t => t.id == id).SelectProperties(t => new
                {
                    t.id,
                    show_time = db.show_times.FirstOrDefault(st => st.id == t.id_show_time), // Adjust for foreign key relation
                    t.seat
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data)) // Without custom settings
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
