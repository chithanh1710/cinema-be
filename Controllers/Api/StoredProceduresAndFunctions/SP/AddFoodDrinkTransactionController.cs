using CINEMA_BE.Controllers.Api.Seats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.SP
{
    public class AddFoodDrinkTransactionController : ApiController
    {
        // Khởi tạo đối tượng db để kết nối với cơ sở dữ liệu
        QL_RCP_Entities db = new QL_RCP_Entities();

        // Phương thức xử lý yêu cầu POST để thêm thực phẩm và đồ uống vào giao dịch
        public IHttpActionResult Post([FromBody] AddFoodDrinkRequest request)
        {
            try
            {
                // Lấy ID giao dịch dựa trên ghế đã chọn và ID khách hàng
                var transactionId = db.transactions
                    .Where(t => t.id_customer == request.customerId) // Lọc theo ID khách hàng
                    .Where(x => x.ticket.id_seat == request.selectedSeatId) // Lọc theo ghế đã chọn
                    .Select(x => x.id) // Chọn ID giao dịch
                    .FirstOrDefault(); // Lấy giao dịch đầu tiên hoặc mặc định nếu không có

                // Nếu không tìm thấy giao dịch, trả về thông báo lỗi
                if (transactionId == 0)
                {
                    return BadRequest("Không tìm thấy giao dịch cho ghế đã chọn.");
                }

                // Duyệt qua từng mục thực phẩm và đồ uống trong danh sách
                foreach (var item in request.listCart)
                {
                    // Kiểm tra nếu mục thực phẩm và đồ uống đã tồn tại trong giao dịch
                    var existingItem = db.transactions_foods_drinks
                        .FirstOrDefault(tf => tf.id_transaction == transactionId && tf.id_food_drink == item.id);

                    if (existingItem != null)
                    {
                        // Nếu số lượng được cập nhật thành 0, xóa mục
                        if (item.quantity <= 0)
                        {
                            db.transactions_foods_drinks.Remove(existingItem);
                        }
                        else
                        {
                            // Nếu mục đã tồn tại và số lượng khác 0, cập nhật số lượng
                            existingItem.quantity = item.quantity;
                        }
                    }
                    else if (item.quantity > 0)
                    {
                        // Nếu không tồn tại và số lượng lớn hơn 0, thêm mới mục
                        db.AddFoodDrinkTransaction(transactionId, item.id, item.quantity);
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Trả về thông báo thành công cùng với ID ghế đã chọn
                return Ok(new { message = "Thực phẩm và đồ uống đã được thêm/cập nhật/xóa thành công.", SeatId = request.selectedSeatId });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về thông báo lỗi
                return BadRequest(ex.Message);
            }
        }

    }

    // Lớp yêu cầu để thêm thực phẩm và đồ uống
    public class AddFoodDrinkRequest
    {
        public int customerId { get; set; } // ID của khách hàng đang thực hiện đơn hàng
        public int selectedSeatId { get; set; } // ID của ghế đã được khách hàng chọn
        public List<FoodDrinkSelection> listCart { get; set; } // Danh sách các mục thực phẩm và đồ uống
    }

    // Lớp đại diện cho sự lựa chọn thực phẩm và đồ uống
    public class FoodDrinkSelection
    {
        public int id { get; set; } // ID của mục thực phẩm hoặc đồ uống
        public int quantity { get; set; } // Số lượng của mục được thêm
    }
}
