using SchoolManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SchoolManagementSystem.Controllers
{
    public class AllocationController : Controller
    {
        private SMS_DBEntities db = new SMS_DBEntities();


        private List<Teacher> GetTeachers()
        {
            var teachers = db.Teachers.ToList();
            return teachers;
        }

        private List<Subject> GetSubjects()
        {
            var subject = db.Subjects.ToList();
            return subject;
        }

        private List<Student> GetStudents()
        {
            var student = db.Students.ToList();
            return student;
        }



        public ActionResult Allocation()
        {

            var students = GetStudents();
            ViewBag.StudentIdList = new SelectList(students, "StdID", "FirstName");

            var teachers = GetTeachers();
            ViewBag.TeacherIdList = new SelectList(teachers, "TeacherID", "FirstName");

            var subjects = GetSubjects(); // Assuming you have a method GetSubjects to fetch subjects
            ViewBag.Subjects = new SelectList(subjects, "Sub_code", "Name");

            var allocatedSubjects = db.Subjects.Include(s => s.Teachers).ToList();

            return View(allocatedSubjects);
        }

        // GET: Allocation/AllocatedSubjects
        public ActionResult AllocatedSubjects()
        {
            var allocatedSubjects = db.Subjects.Include(s => s.Teachers).ToList();
            return PartialView("_AllocatedSubjects", allocatedSubjects);
        }

        // POST: Allocation/AssignSubjectToTeacher
        [HttpPost]
        public ActionResult AssignSubjectToTeacher(string teacherId, string subjectCode)
        {
            try
            {
                var teacher = db.Teachers.Include(t => t.Subjects).FirstOrDefault(t => t.TeacherID == teacherId);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                var subject = db.Subjects.FirstOrDefault(s => s.Sub_code == subjectCode);
                if (subject == null)
                {
                    return Json(new { success = false, message = "Subject not found." });
                }

                teacher.Subjects.Add(subject);
                subject.Teachers.Add(teacher);

                db.SaveChanges();

                return Json(new { success = true, message = "Subject assigned to the teacher successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]

        public ActionResult RemoveSubjectFromTeacher(string teacherId, string subjectCode)
        {
            try
            {
                var teacher = db.Teachers.Include(t => t.Subjects).FirstOrDefault(t => t.TeacherID == teacherId);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                var subject = teacher.Subjects.FirstOrDefault(s => s.Sub_code == subjectCode);
                if (subject == null)
                {
                    return Json(new { success = false, message = "Subject not found for this teacher." });
                }

                teacher.Subjects.Remove(subject);
                db.SaveChanges();

                return Json(new { success = true, message = "Subject removed from teacher successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }




        //FOR STUDENTS
        public ActionResult AllocatedStudents()
        {
            try
            {
                var allocatedStudents = db.Students.Include(s => s.Subjects.Select(sub => sub.Teachers)).ToList();
                return PartialView("_AllocatedStudent", allocatedStudents);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an empty list if an error occurs
                ViewBag.ErrorMessage = "An error occurred while retrieving allocated students: " + ex.Message;
                return PartialView("_AllocatedStudent", new List<Student>());
            }
        }


        // POST: Allocation/AssignSubjectAndTeacherToStudent
        [HttpPost]
        public ActionResult AssignSubjectAndTeacherToStudent(string studentId, string subjectCode, string teacherId)
        {
            try
            {
                var student = db.Students.Include(s => s.Subjects).FirstOrDefault(s => s.StdID == studentId);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                var subject = db.Subjects.FirstOrDefault(s => s.Sub_code == subjectCode);
                if (subject == null)
                {
                    return Json(new { success = false, message = "Subject not found." });
                }

                var teacher = db.Teachers.FirstOrDefault(t => t.TeacherID == teacherId);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                // Assign subject and teacher to the student
                student.Subjects.Add(subject);
                subject.Students.Add(student);
                subject.Teachers.Add(teacher);

                db.SaveChanges();

                return Json(new { success = true, message = "Subject and teacher assigned to the student successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        public ActionResult GetTeachersForSubject(string subjectCode)
        {
            try
            {
                var teachers = db.Teachers
                    .Where(t => t.Subjects.Any(s => s.Sub_code == subjectCode))
                    .Select(t => new SelectListItem
                    {
                        Value = t.TeacherID,
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList();

                return PartialView("_teacherlist", teachers);
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }






    }
}