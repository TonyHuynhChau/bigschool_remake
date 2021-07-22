using bigschool.Models;
using Microsoft.AspNet.Identity;
using System.Linq;

using System.Web.Http;



namespace bigschool.Controllers
{
    public class AttendancesController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult Attend(Course attendanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if (context.Attendances.Any(p => p.Attendee == userID && p.CourseId == attendanceDto.Id))
            {
                //return BadRequest("The attendance already exists!");
                context.Attendances.Remove(context.Attendances.SingleOrDefault(p => p.Attendee == userID && p.CourseId == attendanceDto.Id));
                context.SaveChanges();
                return Ok("cancel");
            }
            var attendance = new Attendance()
            {
                CourseId = attendanceDto.Id,
                Attendee = User.Identity.GetUserId()
            };

            context.Attendances.Add(attendance);
            context.SaveChanges();
            return Ok();
        }
    }
}