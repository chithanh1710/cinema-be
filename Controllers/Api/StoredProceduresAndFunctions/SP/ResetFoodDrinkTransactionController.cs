using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api.StoredProceduresAndFunctions.SP
{
    public class ResetFoodDrinkTransactionController : ApiController
    {
        // Khởi tạo đối tượng db để kết nối với cơ sở dữ liệu
        QL_RCP_Entities db = new QL_RCP_Entities();

        // Phương thức xử lý yêu cầu POST để thêm thực phẩm và đồ uống vào giao dịch
        public IHttpActionResult Post([FromBody] ResetFoodDrinkRequest request)
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

                // Lấy tất cả các mục thực phẩm và đồ uống từ giao dịch
                var foodDrinkItems = db.transactions_foods_drinks
                    .Where(tf => tf.id_transaction == transactionId)
                    .ToList();

                // Cập nhật lại số lượng trong bảng foods_drinks
                foreach (var item in foodDrinkItems)
                {
                    var foodDrink = db.foods_drinks.FirstOrDefault(fd => fd.id == item.id_food_drink);
                    if (foodDrink != null)
                    {
                        // Tăng lại số lượng trong foods_drinks
                        foodDrink.stock_quantity += item.quantity;
                    }
                }

                // Xoá tất cả các mục thực phẩm và đồ uống trong giao dịch
                db.transactions_foods_drinks.RemoveRange(foodDrinkItems);

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
    public class ResetFoodDrinkRequest
    {
        public int customerId { get; set; } // ID của khách hàng đang thực hiện đơn hàng
        public int selectedSeatId { get; set; } // ID của ghế đã được khách hàng chọn
    }
}
