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
        //any class object you place here at top of Controller class gets re-instantiated on every page request of a web-page, no persistency
        //aw shoot .......... i coded Deck to be persistent, i need to recode stuff and get a database and stuff ........ maybe i just use a file
        //aw shoot .......... i coded players to be persistent, i need to recode stuff and get a database and stuff ........ maybe i just use a file

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

            //any class object you place here within Controller class gets re-instantiated on every page request of a web-page, no persistency
            //aw shoot .......... i coded Deck to be persistent, i need to recode stuff and get a database and stuff ........ maybe i just use a file
            Deck red_deck = new Deck();
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
            ViewData["hoststr"] = str1;

            ViewData["RefreshSeconds"] = 5;

            string timestr = string.Format("Current time={0} ..... next refresh in 5 seconds", DateTime.Now.ToString("HH:mm:ss"));
            ViewData["TimeMessage"] = timestr;

            //any class object you place here at top of Controller class gets re-instantiated on every page request of a web-page, no persistency
            //aw shoot .......... i coded players to be persistent, i need to recode stuff and get a database and stuff ........ maybe i just use a file

            //private Dictionary<string, string> players = new Dictionary<string, string>(); //key=ip-port value=status (LOBBY, GAME)

            string clientstr = string.Format("{0}-{1}", clientip, clientport);
            
            //if (players.ContainsKey("clientstr"))
            //    clientstr = string.Format("{0} was found in players, can not play from this browser client until that player has finished game!", clientstr);
            //else
            //{
            //    players.Add(clientstr, "LOBBY");
            //}
            ViewData["clientstr"] = clientstr;

            //ViewData["players"] = players;


            //********* this stuff back to LobbyView.cshtml, i took it out because i could not get azure to update :(   *********************
            //<meta http-equiv="refresh" content="@ViewData["RefreshSeconds"]">

            //< h2 > @ViewData["TimeMessage"] </ h2 >
            //< h3 > @ViewData["hoststr"] </ h3 >
            //< h3 > @ViewData["clientstr"] </ h3 >
            //< p > players </ p >
            //< ul >
            //@{
            //foreach (KeyValuePair<string,string> kv in (Dictionary<string,string>)@ViewData["players"])
            //{
            //    string tempstr = string.Format("{0} {1}", kv.Key, kv.Value);
            //    <li>@tempstr</li>
            //}
            //}
            //</ ul >

            int dummy = 2;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
