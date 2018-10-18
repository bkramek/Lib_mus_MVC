using StronaMuzy.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace StronaMuzy.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        #region Index method.  
        /// <summary>  
        /// Index method.   
        /// </summary>  
        /// <returns>Returns - Index view</returns>  
        public ActionResult Index()
        {
            return this.View();
        }
        #endregion
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        private MusicListEntities2 db = new MusicListEntities2();
        ///////////////////////////////////////////////////////////////////////////////////
        // GET: AdminLogins/Edit/5
        public ActionResult UserAcc()

        {


            string uname = User.Identity.Name;
            var obj = db.Logins.Where(x => x.Username.Equals(uname)).FirstOrDefault();
            int userID = obj.id;
            Login login = db.Logins.Find(userID);


            ViewBag.id_Role = new SelectList(db.Roles, "id_Role", "Name", login.id_Role);
            return View(login);
        }

        // POST: AdminLogins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAcc([Bind(Include = "id,id_Role,Username,Password,Email,Firstname,Lastname")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return View("~/Views/Home/Index.cshtml");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "id_Role", "Name", login.id_Role);
            return View("~/Views/Home/Index.cshtml");
        }/// <summary>


        /// ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="disposing"></param>
    }
}