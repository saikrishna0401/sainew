using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sample.Models;
using System.IO;
using System.Data.Entity;



namespace Sample.Controllers
{
    public class EmpoyeeController : Controller
    {
       // GET Empoyee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewAll()
        {
            return View(GetAllEmployee());
        }

        IEnumerable<Employees> GetAllEmployee()
        {
            using (JquerAjaxDBEntities db = new JquerAjaxDBEntities())
            {
                return db.Emp_Details.ToList<Employees>();
            }

        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Employees emp = new Employees();
            if (id != 0)
            {
                using (JquerAjaxDBEntities db = new JquerAjaxDBEntities())
                {
                    emp = db.Emp_Details.Where(x => x.EMpl_ID == id).FirstOrDefault<Employees>();
                }
            }
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Employees emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.image = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (JquerAjaxDBEntities db = new JquerAjaxDBEntities())
                {
                    if (emp.EMpl_ID == 0)
                    {
                        db.Emp_Details.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (JquerAjaxDBEntities db = new JquerAjaxDBEntities())
                {
                    Employees emp = db.Emp_Details.Where(x => x.EMpl_ID == id).FirstOrDefault<Employees>();
                    db.Emp_Details.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}