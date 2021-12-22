using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Web.Http;
using ExpiringBarcode.Models;
using Microsoft.AspNet.Identity;
using Shared;
using Shared.Models;

namespace ExpiringBarcode.Controllers
{
    [Authorize]
    public class BarcodeController : BaseAPIController
    {
        public BigInteger POST(requestSharedKeyBindingModel model)
        {
            DiffieHellman DH = new DiffieHellman();
            var key = DH.getFinalKey(BigInteger.Parse(model.Key));

            UserManager.FindById(User.Identity.GetUserId()).SharedBarcodeSecret = key;
            db.SaveChanges();
            return DH.GetMyPublic();
        }
    }
}
