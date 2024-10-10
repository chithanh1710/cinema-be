using CINEMA_BE.Utils;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;


namespace CINEMA_BE.Controllers.Api
{
    public class ScreenRoomsController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities(); // Sử dụng DbContext có sẵn

        // GET: api/ScreenRooms
        // Lấy danh sách các phòng chiếu có thể phân trang và tìm kiếm
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<screen_rooms> screenRoomContext = new ApiContext<screen_rooms>(db.screen_rooms);

                var data = screenRoomContext
                 .Filter(sr => sr.name.Contains(q))   // Lọc theo tên phòng chiếu
                 .SortBy(sr => sr.id, false)          // Sắp xếp theo id, giảm dần
                 .Pagination(page, pageSize)          // Áp dụng phân trang
                 .SelectProperties(sr => new
                 {
                     sr.id,
                     sr.name,
                     sr.amount_seats,
                     cinemaId = sr.cinema.id,   // Đổi tên id của cinema thành cinemaId
                     cinemaName = sr.cinema.name // Đổi tên name của cinema thành cinemaName
                 })
                 .ToList();


                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = screenRoomContext.TotalItem();

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

        // GET: api/ScreenRooms/5
        // Lấy thông tin chi tiết của phòng chiếu theo id
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<screen_rooms> screenRoomContext = new ApiContext<screen_rooms>(db.screen_rooms);

                var data = screenRoomContext
                            .Filter(sr => sr.id == id)
                            .SelectProperties(sr => new
                            {
                                sr.id,
                                sr.name,
                                sr.amount_seats,
                                cinema = new
                                {
                                    sr.cinema.id,
                                    sr.cinema.name
                                },
                                show_times = sr.show_times.Select(st => new
                                {
                                    st.id,
                                    st.time_start,
                                    st.time_end
                                })
                            })
                            .ToList();
                if (data == null || !data.Any())
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

        // POST: api/ScreenRooms
        // Thêm mới một phòng chiếu
        public IHttpActionResult Post([FromBody] screen_rooms screenRoom)
        {
            try
            {
                if (screenRoom == null)
                {
                    return BadRequest("Screen room data cannot be null");
                }

                db.screen_rooms.Add(screenRoom);  // Thêm mới phòng chiếu vào DB
                db.SaveChanges();  // Lưu thay đổi vào CSDL

                return Ok(new
                {
                    status = "success",
                    message = "Screen room added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/ScreenRooms/5
        // Cập nhật thông tin của một phòng chiếu dựa trên id
        public IHttpActionResult Put(int id, [FromBody] screen_rooms screenRoom)
        {
            try
            {
                if (screenRoom == null)
                {
                    return BadRequest("Screen room data cannot be null");
                }

                var existingRoom = db.screen_rooms.FirstOrDefault(sr => sr.id == id);
                if (existingRoom == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin phòng chiếu
                existingRoom.name = screenRoom.name;
                existingRoom.amount_seats = screenRoom.amount_seats;
                existingRoom.id_cinema = screenRoom.id_cinema;

                db.SaveChanges();  // Lưu thay đổi

                return Ok(new
                {
                    status = "success",
                    message = "Screen room updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ScreenRooms/5
        // Xóa một phòng chiếu dựa trên id
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var screenRoom = db.screen_rooms.FirstOrDefault(sr => sr.id == id);
                if (screenRoom == null)
                {
                    return NotFound();
                }
                db.screen_rooms.Remove(screenRoom);  // Xóa phòng chiếu
                db.SaveChanges();  // Lưu thay đổi

                return Ok(new
                {
                    status = "success",
                    message = "Screen room deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}