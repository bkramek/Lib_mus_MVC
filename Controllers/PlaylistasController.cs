using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using StronaMuzy.Models;
using PagedList;
namespace StronaMuzy.Controllers
{
    [Authorize]
    public class PlaylistasController : Controller
    {
        private MusicListEntities2 db = new MusicListEntities2();

        // GET: Playlistas
     
        public ActionResult Index(string sort, string searchBox, int? page, string filter)
        {
            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userID = obj.id;

            var playlistas = db.Playlistas.Include(p => p.Login);

            ViewBag.CurrentSort = sort;
            ViewBag.PlaylistSort = String.IsNullOrEmpty(sort) ? "playlist" : "";
            ViewBag.UsernameSort = String.IsNullOrEmpty(sort) ? "username" : "";
       


            if (searchBox != null)
            {
                page = 1;
            }
            else
            {
                searchBox = filter;
            }

            ViewBag.Filter = searchBox;

            
            if (!String.IsNullOrEmpty(searchBox))
            {
                playlistas = playlistas.Where(s => s.Name.Contains(searchBox));
            }
            switch (sort)
            {
                case "playlist":
                    playlistas = playlistas.OrderBy(s => s.Name);
                    break;
                case "username":
                    playlistas = playlistas.OrderBy(s => s.Login.Username);
                    break;
     
                default:
                    playlistas = playlistas.OrderByDescending(s => s.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
         


            
            return View(playlistas.Where(s => s.id_User == userID).ToPagedList(pageNumber,pageSize));
        }

        // GET: Playlistas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlista playlista = db.Playlistas.Find(id);
            if (playlista == null)
            {
                return HttpNotFound();
            }
            return View(playlista);
        }

        // GET: Playlistas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Playlistas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_Playlist,id_User,Playlist")] Playlista playlista)
        {
            if (ModelState.IsValid)
            {
                string uname = User.Identity.Name;
                var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
                int userID = obj.id;
                playlista.id_User = userID;

                db.Playlistas.Add(playlista);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(playlista);
        }

        // GET: Playlistas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pListViewModels = new PListViewModels
            { 
                Playlist = db.Playlistas.Include(s => s.Songs).FirstOrDefault(s => s.id == id), // pla
            };
            if(pListViewModels.Playlist == null)
                return HttpNotFound();
            var allPlaylistasList = db.Songs.ToList();
            pListViewModels.AllSongs = allPlaylistasList.Select(x => new SelectListItem
            {
                Text = x.Tittle +" "+ x.Author+ " ",
                Value = x.id.ToString()
            });
         
            
           
            return View(pListViewModels);
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PListViewModels pListViewModels)
        {

            if (pListViewModels == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);



            if (ModelState.IsValid)
            {
                var update = db.Playlistas.Include(i => i.Songs).First(i => i.id == pListViewModels.Playlist.id);

                if (TryUpdateModel(update, "Playlist", new string[] { "id_User", "Playlist" }))
                {
                    var song = db.Playlistas.Where(m => pListViewModels.SelectedSongs.Contains(m.id)).ToList();
                    var upSong = new HashSet<int>(pListViewModels.SelectedSongs);
                    foreach (Song songE in db.Songs)
                    {
                        if (!upSong.Contains(songE.id))
                        {
                            update.Songs.Remove(songE);
                        }
                        else
                        {
                            update.Songs.Add(songE);
                        }
                    }

                    db.Entry(update).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
           
            return View(pListViewModels);
        }
        // GET: Playlistas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlista playlista = db.Playlistas.Find(id);
            if (playlista == null)
            {
                return HttpNotFound();
            }
            return View(playlista);
        }

        // POST: Playlistas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Playlista playlista = db.Playlistas.Find(id);
            
            db.Playlistas.Remove(playlista);
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
