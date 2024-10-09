using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class CustomersController : ApiController
    {
        // GET: api/Customers
        QL_RCP_Entities db=new QL_RCP_Entities();
        
        // GET: api/Customers
        public IHttpActionResult Get(string q="",int page=1,int pageSize=10)
        {
            try
            {
                ApiContext<customer> customerContext = new ApiContext<customer>(db.customers);
                string search = Util.RemoveDiacritics(q);

                var data = customerContext.Filter(c => c.name.Contains(search)).SortBy(c => c.id, false).Pagination(page, pageSize).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.email,
                    c.phone,
                    c.rank,
                    transaction = c.transactions.Select(t => new { t.id, t.id_ticket, t.id_staff, t.total_amount, t.time_transaction, t.type_transaction }),
                    voucher_uses = c.voucher_uses.Select(t => new { t.id, t.id_customer, t.id_voucher, t.date_used, t.customer }),
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = customerContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
                    currentPage = page,
                    pageSize,
                    totalItem,
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<customer>.settings))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Customers/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<customer> customerContext = new ApiContext<customer>(db.customers);

                var data = customerContext.Filter(c => c.id == id).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.email,
                    c.phone,
                    c.rank,
                    transaction = c.transactions.Select(t => new { t.id, t.id_ticket, t.id_staff, t.total_amount, t.time_transaction, t.type_transaction }),
                    voucher_uses = c.voucher_uses.Select(t => new { t.id, t.id_customer, t.id_voucher, t.date_used, t.customer }),

                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = customerContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data, ApiContext<customer>.settings))
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Customers
        public IHttpActionResult Post([FromBody] customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Customer data cannot be null");
                }

                db.customers.Add(customer);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Customer added successfully",
                    data = customer
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Customers/5
        public IHttpActionResult Put(int id, [FromBody] customer customer)
        {
            if (customer == null || customer.id != id)
            {
                return BadRequest("Invalid customer data.");
            }

            try
            {
                db.customers.Attach(customer);
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { status = "success", message = "Customer updated successfully", data = customer });

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

        }


        // DELETE: api/Customers/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var customer = db.customers.FirstOrDefault(c => c.id == id);
                if (customer == null)
                {
                    return NotFound();
                }

                db.customers.Remove(customer);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Customer deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
