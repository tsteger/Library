using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    public class DefaultController : Controller
    {

        public IActionResult Index()
        {
           var rete = new ViewModel
           {
               Message = "My View"
           };
            return View(rete);
        }
    }
}