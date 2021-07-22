using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bigschool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace bigschool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        [HttpGet]
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse, FormCollection f)
        {
            BigSchoolContext context = new BigSchoolContext();
            var count = context.Courses.Count() + 1;
            var time = String.Format("{0:MM/dd/yyyy}", f["DateTime"]);
            objCourse.DateTime = DateTime.Parse(time);
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("create", objCourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;
            objCourse.Id = count;

            context.Courses.Add(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }

        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name; //Name la cot da them vao Aspnetuser
            }
            return View(courses);
        }

        public ActionResult Edit(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            var userId = User.Identity.GetUserId();
            var course = context.Courses.Single(c => c.Id == id && c.LecturerId == userId);
            var viewModel = new Course
            {
                Id = course.Id,
                LecturerId = course.LecturerId,
                Place = course.Place,
                DateTime = course.DateTime,
                CategoryId = course.CategoryId
            };
            viewModel.ListCategory = context.Categories.ToList();
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course objCourse, FormCollection f)
        {
            BigSchoolContext context = new BigSchoolContext();
            var time = String.Format("{0:MM/dd/yyyy}", f["DateTime"]);
            objCourse.DateTime = DateTime.Parse(time);

            var id = context.Courses.SingleOrDefault(p => p.Id == objCourse.Id);

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            id.LecturerId = user.Id;
            id.Place = objCourse.Place;
            id.DateTime = objCourse.DateTime;
            id.CategoryId = objCourse.CategoryId;

            context.SaveChanges();

            return RedirectToAction("Mine", "Courses");
        }

        [Authorize]
        public ActionResult Delete(Course objCourse)
        {
            //try 
            //{
                BigSchoolContext context = new BigSchoolContext();
                var deletecourse = context.Courses.SingleOrDefault(p => p.Id == objCourse.Id);
                context.Courses.Remove(deletecourse);
                context.SaveChanges();
                return RedirectToAction("Mine", "Courses");
            //}
            //catch (Exception ex)
            //{
            //    return View("Mine", "Courses") ;
            //}
        }

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();
            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}