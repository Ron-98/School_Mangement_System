using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private SMS_DBEntities db = new SMS_DBEntities();

        // GET: Student
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Student/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StdID,FirstName,LastName,Gender,DOB,Address,ContactNo,Enable")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            

            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StdID,FirstName,LastName,Gender,DOB,Address,ContactNo,Enable")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        //GET: Student/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //POST: Student/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult RemoveStudent(Student s)
        {
            Student ss = db.Students.Where(x => x.StdID == s.StdID).FirstOrDefault();
            db.Students.Remove(ss);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //Delete
        //public ActionResult DeleteStudent(string id)
        //{
        //   try
        //   {
        //       var student = db.Students.Find(id);
        //        if (student == null)
        //      {
        //           return Json(new { success = false, message = "Student not found." });
        //        }

        //       db.Students.Remove(student);
        //        db.SaveChanges();

        //       return Json(new { success = true, message = "Student deleted successfully." });
        //   }
        //    catch (Exception ex)
        //      {
        //        return Json(new { success = false, message = "An error occurred while deleting the student: " + ex.Message });
        //   }
        //}


        public ActionResult StudentSubjects(string id)
        {
            var student = db.Students
                             .Include(s => s.Subjects.Select(sub => sub.Teachers))
                             .FirstOrDefault(s => s.StdID == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            // Explicitly load students for each subject
            foreach (var subject in student.Subjects)
            {
                db.Entry(subject).Collection(s => s.Students).Load();
            }

            return View("_Details",student);
        }


        public JsonResult IsStdIDAvailable(string stdID)
        {
            bool isAvailable = !db.Students.Any(s => s.StdID == stdID);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ToggleEnable(string id, bool enable)
        {
            try
            {
                var student = db.Students.Find(id);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                student.Enable = enable;
                db.SaveChanges();

                return Json(new { success = true, message = enable ? "Student enabled successfully." : "Student disabled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }





    }
}
