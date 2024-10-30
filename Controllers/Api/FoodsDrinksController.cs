using CINEMA_BE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class FoodsDrinksController : ApiController
    {
        QL_RCP_Entities db = new QL_RCP_Entities();
        // GET: api/FoodsDrinks
        public IHttpActionResult Get()
        {
            try
            {
                ApiContext<foods_drinks> fooddrinkContext = new ApiContext<foods_drinks>(db.foods_drinks);
                var data = fooddrinkContext.SelectProperties(f => new
                {

                    f.id,
                    f.name,
                    f.price,
                    f.stock_quantity,
                    f.image_url,
                    f.category,
                }).ToList();


                int totalItem = fooddrinkContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    totalItem,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<foods_drinks> fooddrinkContext = new ApiContext<foods_drinks>(db.foods_drinks);
                var data = fooddrinkContext.Filter(f=>f.id == id).SelectProperties(f => new
                {

                    f.id,
                    f.name,
                    f.price,
                    f.stock_quantity,
                    f.image_url,
                    f.category,
                }).ToList();


                int totalItem = fooddrinkContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    totalItem,
                    data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/FoodsDrinks
        public IHttpActionResult Post([FromBody] foods_drinks foods_Drinks)
        {
            try
            {
                if (foods_Drinks == null)
                {
                    return BadRequest("foods_Drinks data cannot be null");
                }

                db.foods_drinks.Add(foods_Drinks);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "foods_Drinks added successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/FoodsDrinks/5
        public IHttpActionResult Put(int id, [FromBody] foods_drinks foods_Drinks)
        {
            try
            {
                if (foods_Drinks == null)
                {
                    return BadRequest("Invalid foods_Drinks data.");
                }

                var existingfoods_Drinks = db.foods_drinks.Find(id);
                if (existingfoods_Drinks == null)
                {
                    return NotFound();
                }

                existingfoods_Drinks.name = foods_Drinks.name;
                existingfoods_Drinks.price = foods_Drinks.price;
                existingfoods_Drinks.category = foods_Drinks.category;
                existingfoods_Drinks.stock_quantity = foods_Drinks.stock_quantity;
                existingfoods_Drinks.image_url = foods_Drinks.image_url;

                db.SaveChanges();

                return Ok(new { status = "success", message = "foods_Drinks updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/FoodsDrinks/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var foods_Drinks = db.foods_drinks.FirstOrDefault(f => f.id == id);
                if (foods_Drinks == null)
                {
                    return NotFound();
                }

                db.foods_drinks.Remove(foods_Drinks);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "foods_Drinks deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}