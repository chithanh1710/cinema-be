using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class TicketsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // POST: api/Tickets
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

        // PUT: api/Tickets/5
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

        // DELETE: api/Tickets/5
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
