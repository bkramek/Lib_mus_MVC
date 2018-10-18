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


    public class SongsController : Controller
    {

        private MusicListEntities2 db = new MusicListEntities2();

        // GET: Songs
        public ActionResult Index(string sort, string searchBox,int? page,string filter)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.AuthorSort = String.IsNullOrEmpty(sort) ? "author" : "";
            ViewBag.TittleSort = String.IsNullOrEmpty(sort) ? "tittle" : "";
            ViewBag.AlbumSort = String.IsNullOrEmpty(sort) ? "album" : "";
            ViewBag.LikesSort = String.IsNullOrEmpty(sort) ? "likes" : "";
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

            var songs = from s in db.Songs.Include(s => s.Scores)
                           select s;
            if (!String.IsNullOrEmpty(searchBox))
            {
                songs = songs.Where(s => s.Tittle.Contains(searchBox)
                                       || s.Author.Contains(searchBox));
            }
            switch (sort)
            {
                case "tittle":
                    songs = songs.OrderBy(s => s.Tittle);
                    break;
                case "author":
                    songs = songs.OrderBy(s => s.Author);
                    break;
                case "date":
                    songs = songs.OrderBy(s => s.Date);
                    break;
                case "desc":
                    songs = songs.OrderByDescending(s => s.Date);
                    break;
                case "album":
                    songs = songs.OrderBy(s => s.Album.Tittle);
                    break;
                case "likes":
                    songs = songs.OrderBy(s => s.Scores.Count);
                    break;
                default:
                    songs = songs.OrderByDescending(s =>s.Scores.Count);
                    break;
            }
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(songs.ToPagedList(pageNumber, pageSize));

        }
        // GET: Songs/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Id_Albums = new SelectList(db.Albums, "id", "Tittle");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_Songs,Tittle,Author,Link,Date,Id_Albums,Details")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Albums = new SelectList(db.Albums, "id", "Tittle", song.Id_Albums);
            return View(song);
        }

        // GET: Songs/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Albums = new SelectList(db.Albums, "id", "Tittle", song.Id_Albums);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_Songs,Tittle,Author,Link,Date,Id_Albums")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Albums = new SelectList(db.Albums, "id", "Tittle", song.Id_Albums);
            return View(song);
        }

        // GET: Songs/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            var deleteit = from DScore in db.Scores
                            where DScore.id_Songs == id
                            select DScore;
            foreach (var DScore in deleteit)
            {
                db.Scores.Remove(DScore);
            }
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult Like([Bind(Include = "id,id_Songs,Likes,id_User")] int? id, Score score)
        {
            Song song = db.Songs.Find(id);
            bool result;
            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userID = obj.id;
            score.id_User = userID;
            score.Likes = "Like";
            score.id_Songs = song.id;
            var add = (from DScore in db.Scores
                         where DScore.id_User == userID
                         where DScore.id_Songs == id
                         select DScore).Any();
            result = add;
            
            if (result == true)
            {
                return RedirectToAction("Index");
            }
            else
            {
                db.Scores.Add(score);
                db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            

        }
        [Authorize]
        public ActionResult Unlike(int? id, Score score)
        {
            

            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userID = obj.id;
            var deleteDit = from DScore in db.Scores
                            where DScore.id_User == userID where DScore.id_Songs == id
                            select DScore;
            foreach ( var DScore in deleteDit)
            {
                db.Scores.Remove(DScore);
            }
            
    
        
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        // GET: Albums/Create
        [Authorize]
        public ActionResult CreateAlbum()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAlbum([Bind(Include = "id,Tittle,Author,Date")] Album album, Song song)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(song);
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: Playlistas/Edit/5
        [Authorize]
        public ActionResult AddSongToList(int? id)
        {
            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userID = obj.id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pListViewModels = new PsongList
            {
                
                Sonk = db.Songs.Include(d => d.Playlistas).FirstOrDefault(d=> d.id ==id),
            };
            if (pListViewModels.Sonk == null)
                return HttpNotFound();
            var allPlaylistasList = db.Playlistas.ToList();

            pListViewModels.AllPlaylists = allPlaylistasList.Where(w => w.id_User==userID).Select(x => new SelectListItem
            {
                Text = x.id + " " + x.Name + " ",
                Value = x.id.ToString()
            });
          
         
            return View(pListViewModels);
            


        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSongToList(PsongList pListViewModels)
        {

            if (pListViewModels == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);



            if (ModelState.IsValid)
            {
                var update = db.Songs
                    .Include(i => i.Playlistas)
                    .FirstOrDefault(i => i.id == pListViewModels.Sonk.id);
                if (TryUpdateModel(update, "Playlist", new string[] { "id_User", "Name" }))
                {
                    var song = db.Songs.Where(m => pListViewModels.SelectedPlaylists.Contains(m.id)).ToList();
                    var upSong = new HashSet<int>(pListViewModels.SelectedPlaylists);
                    foreach (Playlista songE in db.Playlistas)
                    {
                        if (!upSong.Contains(songE.id))
                        {
                            update.Playlistas.Remove(songE);
                        }
                        else
                        {
                            update.Playlistas.Add(songE);
                        }
                    }

                    db.Entry(update).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(pListViewModels);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////
      

        [HttpGet]
        public ActionResult GetPosts(int? id)
        {
           
            
            
            IQueryable<PostsVM> Posts = db.Songs.Where(p=> p.id == id)
                                                 .Select(p => new PostsVM
                                                 {
                                                     PostID = p.id,
                                                     Tittle = p.Tittle,
                                                     Author = p.Author,
                                                     Link = p.Link,
                                                     Album = p.Album.Tittle,
                                                     Details = p.Details,
                                                     PostedDate = p.Date
                                                 }).AsQueryable();
            
            return View(Posts);
        }

        public PartialViewResult GetComments(int postId)
        {
            IQueryable<CommentsVM> comments = db.Comments.Where(c => c.Song.id == postId)
                                     .Select(c => new CommentsVM
                                     {
                                         ComID = c.id,
                                         CommentedDate = c.Date,
                                         CommentMsg = c.Msg,
                                         Users = new UserVM
                                         {
                                             UserID = c.Login.id,
                                             Username = c.Login.Username,
                                            
                                         }
                                     }).AsQueryable();

            return PartialView("~/Views/Shared/_MyComments.cshtml", comments);
        }

        [HttpPost]
        public ActionResult AddComment(CommentsVM comment, int postId)
        {
            //bool result = false;  
            Comment commentEntity = null;
            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userId = obj.id;

            var user = db.Logins.FirstOrDefault(u => u.id == userId);
            var post = db.Songs.FirstOrDefault(p => p.id == postId);

            if (comment != null)
            {

                commentEntity = new Comment
                {
                    Msg = comment.CommentMsg,
                    Date = comment.CommentedDate,
                };


                if (user != null && post != null)
                {
                    post.Comments.Add(commentEntity);
                    user.Comments.Add(commentEntity);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("GetComments", "Songs", new { postId = postId });
        }

        [HttpGet]
        public PartialViewResult GetSubComments(int ComID)
        {
            IQueryable<SubCommentsVM> subComments = db.ReplayComments.Where(sc => sc.Comment.id == ComID)
                                       .Select(sc => new SubCommentsVM
                                       {
                                           SubComID = sc.id,
                                           CommentMsg = sc.Msg,
                                           CommentedDate = sc.Date,
                                           User = new UserVM
                                           {
                                               UserID = sc.Login.id,
                                               Username = sc.Login.Username,
                                               
                                           }
                                       }).AsQueryable();

            return PartialView("~/Views/Shared/_MySubComments.cshtml", subComments);

        }

        [HttpPost]
        public ActionResult AddSubComment(SubCommentsVM subComment, int ComID)
        {
            ReplayComment subCommentEntity = null;
            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userId = obj.id;

            var user = db.Logins.FirstOrDefault(u => u.id == userId);
            var comment = db.Comments.FirstOrDefault(p => p.id == ComID);

            if (subComment != null)
            {

                subCommentEntity = new ReplayComment
                {
                    Msg = subComment.CommentMsg,
                    Date = subComment.CommentedDate,
                };


                if (user != null && comment != null)
                {
                    comment.ReplayComments.Add(subCommentEntity);
                    user.ReplayComments.Add(subCommentEntity);

                    db.SaveChanges();
                    //result = true;  
                }
            }

            return RedirectToAction("GetSubComments", "Songs", new { ComID = ComID });

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////

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
