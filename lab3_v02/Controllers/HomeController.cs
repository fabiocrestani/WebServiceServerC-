using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lab3_v02.Models;
using System.Text.Encodings.Web;

namespace lab3_v02.Controllers {
    public class HomeController : Controller {
        /// <summary>
        /// GET: /HelloWorld/ 
        /// </summary>
        /// <returns></returns>
        public string Index() {
            return "";
        }

        //public IActionResult Error() {
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
