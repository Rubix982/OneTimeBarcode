using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ExpiringBarcode.Controllers
{
    public class TokenController : ApiController
    {
        public void Index()
        {
            Console.WriteLine("This is Token");
        }
    }
}
