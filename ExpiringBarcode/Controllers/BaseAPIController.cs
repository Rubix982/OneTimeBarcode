﻿using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using ExpiringBarcode.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExpiringBarcode.Controllers
{
    public class BaseAPIController : ApiController
    {
        protected DbContext db;
        private readonly UserStore<ApplicationUser> _userStore;
        private ApplicationUserManager _userManager;

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        public BaseAPIController()
        {
            this.db = new ApplicationDbContext();
            this._userStore = new UserStore<ApplicationUser>(db);
            this._userManager = new ApplicationUserManager(_userStore);
        }

    //    public BaseAPIController(ApplicationUserManager userManager,
    //ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
    //    {
    //        UserManager = userManager;
    //        AccessTokenFormat = accessTokenFormat;
    //    }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}
