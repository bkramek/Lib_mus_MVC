using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using StronaMuzy.Models;
using PagedList;

namespace StronaMuzy.Controllers
{
    //[Authorize(Roles = "admin")]
    public class AdminLoginsController : Controller
    {
        private MusicListEntities2 db = new MusicListEntities2();

        // GET: AdminLogins
        public ActionResult Index(string sort, string searchBox, int? page, string filter)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.UsernameSort = String.IsNullOrEmpty(sort) ? "username" : "";
            ViewBag.FirstnameSort = String.IsNullOrEmpty(sort) ? "firstname" : "";
            ViewBag.LastnameSort = String.IsNullOrEmpty(sort) ? "lastname" : "";
            ViewBag.EmailSort = String.IsNullOrEmpty(sort) ? "email" : "";
            ViewBag.RoleSort = String.IsNullOrEmpty(sort) ? "role" : "";

            ViewBag.DateSort = sort == "date" ? "desc" : "date";


            if (searchBox != null)
            {
                page = 1;
            }
            else
            {
                searchBox = filter;
            }

            ViewBag.Filter = searchBox;

            var logins = from s in db.Logins.Include(s => s.Role)
                         select s;
            if (!String.IsNullOrEmpty(searchBox))
            {
                logins = logins.Where(s => s.Username.Contains(searchBox));
            }

                switch (sort)
                {
                    case "username":
                        logins = logins.OrderBy(s => s.Username);
                        break;
                    case "email":
                        logins = logins.OrderBy(s => s.Email);
                        break;
                    case "date":
                        logins = logins.OrderBy(s => s.Date);
                        break;
                    case "desc":
                        logins = logins.OrderByDescending(s => s.Date);
                        break;
                    case "firstname":
                        logins = logins.OrderBy(s => s.Firstname);
                        break;
                    case "lastname":
                        logins = logins.OrderBy(s => s.Lastname);
                        break;
                    default:
                        logins = logins.OrderBy(s => s.Role.Name);
                        break;
                }
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(logins.ToPagedList(pageNumber, pageSize));


            } 

        // GET: AdminLogins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // GET: AdminLogins/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Roles, "id_Role", "Name");
            return View();
        }

        // POST: AdminLogins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_Role,Username,Password,Email,Firstname,Lastname")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Logins.Add(login);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_Role = new SelectList(db.Roles, "id", "Name", login.id_Role);
            return View(login);
        }

        // GET: AdminLogins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_Role = new SelectList(db.Roles, "id", "Name", login.id_Role);
            return View(login);
        }

        // POST: AdminLogins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_Role,Username,Password,Email,Firstname,Lastname")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", login.id_Role);
            return View(login);
        }

        // GET: AdminLogins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = db.Logins.Find(id);
            if (login == null)
            {
                return HttpNotFound();
            }
            return View(login);
        }

        // POST: AdminLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Login login = db.Logins.Find(id);
            db.Logins.Remove(login);
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
    }
}
