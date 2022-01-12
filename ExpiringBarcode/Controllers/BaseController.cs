using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ExpiringBarcode.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpiringBarcode.Controllers
{
    public class BaseController : Controller
    {
        protected DbContext db;
        private readonly UserStore<ApplicationUser> _userStore;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public BaseController()
        {
            this.db = new ApplicationDbContext();
            this._userStore = new UserStore<ApplicationUser>(db);
            this._userManager = new ApplicationUserManager(_userStore);
        }
    }
}