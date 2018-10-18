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
    public class AlbumsController : Controller
    {
        private MusicListEntities2 db = new MusicListEntities2();

        // GET: Albums
        public ActionResult Index(string sort, string searchBox, int? page, string filter)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.AuthorSort = String.IsNullOrEmpty(sort) ? "author" : "";
            ViewBag.TittleSort = String.IsNullOrEmpty(sort) ? "tittle" : "";
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
           
            var albums = from s in db.Albums
                        select s;
            if (!String.IsNullOrEmpty(searchBox))
            {
                albums = albums.Where(s => s.Tittle.Contains(searchBox)
                                       || s.Author.Contains(searchBox));
            }
            switch (sort)
            {
                case "tittle":
                    albums = albums.OrderBy(s => s.Tittle);
                    break;
                case "author":
                    albums = albums.OrderBy(s => s.Author);
                    break;
                case "date":
                    albums = albums.OrderBy(s => s.Date);
                    break;
                case "desc":
                    albums = albums.OrderByDescending(s => s.Date);
                    break;
                default:
                    albums = albums.OrderByDescending(s => s.Tittle);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(albums.ToPagedList(pageNumber, pageSize));

        
    }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Albums,Tittle,Author,Date")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Albums,Tittle,Author,Date")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
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
