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
                    .Filter(t => t.id_customer == id)
                    .SelectProperties(t => new
                    {
                        t.id,
                        t.total_amount,
                        t.time_transaction,
                        customer = new { t.customer.id, t.customer.name },
                        ticket = new { t.ticket.id, t.ticket.seat.genre_seats, t.ticket.seat.number_of_column, t.ticket.seat.number_of_row },
                        foods_drinks = t.transactions_foods_drinks.Select(tfr => new { tfr.quantity, tfr.foods_drinks.name }),
                        movie = new { t.ticket.show_times.movy.name, t.ticket.show_times.movy.thumbnail, t.ticket.show_times.movy.old, t.ticket.show_times.movy.star },
                        cinema = new {t.ticket.show_times.screen_rooms.cinema.name},
                        sreenroom = new { t.ticket.show_times.screen_rooms.name },
                        showtime = new {t.ticket.show_times.time_start}
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
        public IHttpActionResult Put(int id)
        {
            try
            {
                // Tìm tất cả các giao dịch với customerId cụ thể
                var transactions = db.transactions.Where(t => t.id_customer == id && t.time_transaction == null).ToList();

                if (transactions.Count == 0)
                {
                    return NotFound(); // Không tìm thấy giao dịch nào với customerId này
                }

                // Cập nhật time_transaction cho tất cả các giao dịch
                foreach (var transaction in transactions)
                {
                    transaction.time_transaction = DateTime.Now; // Đặt thời gian hiện tại
                }

                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = $"Updated {transactions.Count} transactions' time_transaction to the current time"
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