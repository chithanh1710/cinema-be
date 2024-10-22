using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CINEMA_BE.Utils;

namespace CINEMA_BE.Controllers.Api
{
    public class CustomersController : ApiController
    {
        QL_RCP_Entities db=new QL_RCP_Entities();
        
        // GET: api/Customers
        public IHttpActionResult Get(string q="",int page=1,int pageSize=10)
        {
            try
            {
                ApiContext<customer> customerContext = new ApiContext<customer>(db.customers);

                var data = customerContext.Filter(c => c.name.Contains(q)).SortBy(c => c.id, false).Pagination(page, pageSize).SelectProperties(c => new
                {
                    c.id,
                    c.name,
                    c.email,
                    c.phone,
                    c.rank,
                }).ToList();

                int totalItem = customerContext.TotalItem();

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
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Customers?email={email}
        public IHttpActionResult Get(string email)
        {
            try
            {
                ApiContext<customer> customerContext = new ApiContext<customer>(db.customers);

                var data = customerContext.Filter(c => c.email.Equals(email)).SelectProperties(c => new
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
                    data
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
                if (customer == null || string.IsNullOrWhiteSpace(customer.name) || string.IsNullOrWhiteSpace(customer.email))
                {
                    return BadRequest("Customer data cannot be null or empty");
                }

                // SQL raw command to insert into the database
                string insertCommand = string.Format("INSERT INTO customers (name, email) VALUES (N'{0}', N'{1}')", customer.name, customer.email);

                // Execute the raw SQL command
                db.Database.ExecuteSqlCommand(insertCommand);

                return Ok(new
                {
                    status = "success",
                    message = "Customer added successfully",
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
            try
            {
                if (customer == null)
                {
                    return BadRequest("Invalid customer data.");
                }

                customer existingCus = db.customers.Find(id);

                if (existingCus == null)
                {
                    NotFound();
                }

                existingCus.phone = customer.phone;
                existingCus.email = customer.email;
                existingCus.name = customer.name;
                existingCus.rank = customer.rank;


                db.SaveChanges();

                return Ok(new { status = "success", message = "Customer updated successfully" });

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
