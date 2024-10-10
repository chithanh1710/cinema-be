using CINEMA_BE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class TransactionsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();

        // GET: api/transactions
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<transaction> transactions = new ApiContext<transaction>(db.transactions);

                var data = transactions
                    .Filter(t => t.type_transaction.Contains(q))
                    .SortBy(t => t.id)
                    .Pagination(page,pageSize)
                    .SelectProperties(t => new
                    {
                        t.id,
                        t.id_customer,
                        t.id_ticket,
                        t.id_staff,
                        t.total_amount,
                        t.time_transaction,
                        t.type_transaction,
                        customer = new { t.customer.id, t.customer.name }, // Giả định có thuộc tính customer
                        ticket = new { t.ticket.id } // Giả định có thuộc tính ticket
                    }).ToList();

                int totalItem = transactions.TotalItem();

                return Ok(new
                {
                    status = "success",
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

        // GET: api/transactions/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<transaction> transactions = new ApiContext<transaction>(db.transactions);

                var data = transactions
                    .Filter(t => t.id == id)
                    .SelectProperties(t => new
                    {
                        t.id,
                        t.id_customer,
                        t.id_ticket,
                        t.id_staff,
                        t.total_amount,
                        t.time_transaction,
                        t.type_transaction,
                        customer = new { t.customer.id, t.customer.name },
                        ticket = new { t.ticket.id }
                    }).ToList();

                if (data == null)
                {
                    return NotFound();
                }

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

        // POST: api/transactions
        public IHttpActionResult Post([FromBody] transaction newTransaction)
        {
            try
            {
                if (newTransaction == null)
                {
                    return BadRequest("Transaction data cannot be null");
                }

                db.transactions.Add(newTransaction);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Transaction added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/transactions/5
        public IHttpActionResult Put(int id, [FromBody] transaction updatedTransaction)
        {
            try
            {
                if (updatedTransaction == null)
                {
                    return BadRequest("Transaction data cannot be null");
                }

                var existingTransaction = db.transactions.FirstOrDefault(t => t.id == id);
                if (existingTransaction == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin giao dịch
                existingTransaction.id_customer = updatedTransaction.id_customer;
                existingTransaction.id_ticket = updatedTransaction.id_ticket;
                existingTransaction.id_staff = updatedTransaction.id_staff;
                existingTransaction.total_amount = updatedTransaction.total_amount;
                existingTransaction.time_transaction = updatedTransaction.time_transaction;
                existingTransaction.type_transaction = updatedTransaction.type_transaction;

                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Transaction updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/transactions/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var transaction = db.transactions.FirstOrDefault(t => t.id == id);
                if (transaction == null)
                {
                    return NotFound();
                }

                db.transactions.Remove(transaction);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Transaction deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}