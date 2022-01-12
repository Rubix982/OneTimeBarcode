using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ExpiringBarcode.Controllers
{
    [Authorize]
    public class MemberController : BaseAPIController
    {
        public string Get()
        {
            var memberId = UserManager.FindById(User.Identity.GetUserId()).MembershipNumber;
            return memberId;
        }
    }
}