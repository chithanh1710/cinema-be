using CINEMA_BE.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CINEMA_BE.Controllers.Api
{
    public class ProblemsController : ApiController
    {
        // GET: api/Problems
        QL_RCP_Entities db = new QL_RCP_Entities();
      
        public IHttpActionResult Get(string q = "", int page = 1, int pageSize = 10)
        {
            try
            {
                ApiContext<problem> problemContext = new ApiContext<problem>(db.problems);
                string search = Util.RemoveDiacritics(q);

                var data = problemContext.Filter(p => p.name.Contains(search)).SortBy(p => p.id, false).Pagination(page, pageSize).SelectProperties(p => new
                {
                    p.id,
                    p.status,
                    p.name,
                    p.description,
                    p.date_start,
                    p.date_end,
                    staff = new{p.staff.id, p.staff.name},
                }).ToList();

                if (data == null || !data.Any())
                {
                    return NotFound();
                }

                int totalItem = problemContext.TotalItem();

                return Ok(new
                {
                    status = "success",
                    search = q,
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

        
        public IHttpActionResult Get(int id)
        {
            try
            {
                ApiContext<problem> problemContext = new ApiContext<problem>(db.problems);

                var data = problemContext.Filter(p => p.id == id).SelectProperties(p => new
                {
                    p.id,
                    p.status,
                    p.name,
                    p.description,
                    p.date_start,
                    p.date_end,
                    staff = new { p.staff.id, p.staff.name },
                }).ToList();

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

        
        public IHttpActionResult Post([FromBody] problem Problem)
        {
            try
            {
                if (Problem == null)
                {
                    return BadRequest("Problem data cannot be null");
                }
                
                var existingStaff = db.staffs.FirstOrDefault(s => s.id == Problem.id_staff);
                if (existingStaff == null)
                {
                    return BadRequest("Staff not found");
                }
                Problem.staff = existingStaff;
                db.problems.Add(Problem);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Problem added successfully",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        public IHttpActionResult Put(int id, [FromBody] problem Problem)
        {
            try
            {
                if (Problem == null)
                {
                    return BadRequest("Invalid actor data.");
                }

                var existingProblem = db.problems.Find(id);
                if (existingProblem == null)
                {
                    return NotFound();
                }

                
                existingProblem.name = Problem.name;
                existingProblem.status = Problem.status;
                existingProblem.description = Problem.description;
                existingProblem.date_start = Problem.date_start;
                existingProblem.date_end = Problem.date_end;
                existingProblem.expense = Problem.expense;

                var existingStaff = db.staffs.FirstOrDefault(s => s.id == Problem.id_staff);
                if (existingStaff == null)
                {
                    return BadRequest("Staff not found");
                }
                
                existingProblem.staff = existingStaff;
                
                db.SaveChanges();

                return Ok(new { status = "success", message = "Problem updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var Problem = db.problems.Find(id);
                if (Problem == null)
                {
                    return NotFound();
                }

                db.problems.Remove(Problem);
                db.SaveChanges();

                return Ok(new
                {
                    status = "success",
                    message = "Problem deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
