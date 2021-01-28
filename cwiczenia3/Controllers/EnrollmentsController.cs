using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cwiczenia3.DAL;
using cwiczenia3.dto.request;
using cwiczenia3.dto.response;
using cwiczenia3.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cwiczenia3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService service = new SqlServerDbService();

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response;
            try
            {
                response = service.EnrollStudent(request);
                if (response == null) 
                    return BadRequest("Response is null");
            }
            catch (BadRequestException e) { return BadRequest(e.Message); }
            catch (NotFoundException e) { return NotFound(e.Message); }

            return Created($"api/students/enrollments", response);
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromotionRequest request)
        {
            PromotionResponse response;
            try
            {
                response = service.PromoteStudents(request);
                if (response == null)
                {
                    return BadRequest($"Studies {request.Studies} , {request.Semester} not found.");
                }
            }
            catch (BadRequestException e) { return BadRequest(e.Message); }
            catch (NotFoundException e) { return NotFound(e.Message); }

            return Created($"api/students", response);
        }
    }
}
