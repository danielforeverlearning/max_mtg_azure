using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using max_mtg_azure.Models;

using System.Net;

namespace max_mtg_azure.Controllers
{
    public class HomeController : Controller
    {
        private Deck red_deck = new Deck();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public ActionResult ShuffleView()
        {
            string mypath  = string.Format("~/images/red_deck.csv");
            string urlpath = Url.Content(mypath);
            string csvpath = "./wwwroot" + urlpath;

            red_deck.ReadDeckCSV(csvpath, "red_deck");
            red_deck.Shuffle();
            int cardsdrawn = red_deck.Draw(7);
            List<DistinctCard> dchand = red_deck.SeeHand;

            List<string> piclist = new List<string>();
            foreach (DistinctCard dc in dchand)
            {
                string picstr = string.Format("~/images/{0}/{1}", red_deck.GetDeckName, dc.pic_filename);
                string urlstr = Url.Content(picstr);
                piclist.Add(urlstr);
            }

            //piclist.Add(Url.Content("~/images/red_deck/land_mountain.jpg"));
            ViewData["piclist"] = piclist;
            ViewData["Message"] = "klugecsvpath-->ReadDeckCSV-->Shuffle-->Draw-->SeeHand";

            return View();

            /****
            List<string> piclist2 = new List<string>();
            string mytestpic = Url.Content("~/images/red_deck/land_mountain.jpg");
            piclist2.Add(mytestpic);
            ViewData["piclist"] = piclist2;
            ViewData["Message"] = mytestpic;

            return View();
            ********/
        }

        public IActionResult LobbyView()
        {
            string hoststr = Request.HttpContext.Request.Host.Host;
            int? hostport = Request.HttpContext.Request.Host.Port;
            IPAddress clientip = Request.HttpContext.Connection.RemoteIpAddress;
            int clientport = Request.HttpContext.Connection.RemotePort;
     
            string hostportstr;
            if (hostport == null)
                hostportstr = "null";
            else
                hostportstr = hostport.ToString();

            string str1 = string.Format("hoststr={0} hostportstr={1}", hoststr, hostportstr);
            ViewData["Message1"] = str1;

            string str2 = string.Format("clientip={0} clientport={1}", clientip, clientport);
            ViewData["Message2"] = str2;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
