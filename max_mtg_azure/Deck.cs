using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace max_mtg_azure
{

    public class DistinctCard
    {
        public int id;
        public string pic_filename;
        public int card_count;
        public string card_type;
        public string mana_cost;
        public int effects;
    }

    public class Deck
    {
        //private string myconnectionstr = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-max_mtg_no_azure-20180604112333;Integrated Security=True";
        private string myconnectionstr = @"Data Source=66.126.21.120\MSSQLLocalDB;Initial Catalog=aspnet-max_mtg_no_azure-20180604112333;Integrated Security=True";
        private string DeckName = null;
        private List<DistinctCard> DistinctCards = null;
        private int[] ActualCards = null; //contains index to DistinctCards
        private int total_deckcount = 0;
        private int current_deckcount = 0;
        private List<DistinctCard> hand = null;
        private List<DistinctCard> graveyard = null;


        private void EraseDeck()
        {
            if (DistinctCards == null)
                DistinctCards = new List<DistinctCard>();
            else
                DistinctCards.Clear();

            if (ActualCards != null)
            {
                ActualCards = null; //When garbage collector does GC.Collect(), the heap from this object will be freed
            }

            if (hand == null)
                hand = new List<DistinctCard>();
            else
                hand.Clear();

            if (graveyard == null)
                graveyard = new List<DistinctCard>();
            else
                graveyard.Clear();

            total_deckcount = 0;
            current_deckcount = 0;
            DeckName = null;
        }

        public void ReadDeckCSV(string deckcsvpath, string deckname)
        {
            //you need to pay microsoft for sqlserver database on azure but i am too cheap to buy it, use csv file instead.

            EraseDeck();

            DeckName = deckname;

            
            StreamReader deckrdr = new StreamReader(deckcsvpath);
            Char delimiter = ',';
            string line = deckrdr.ReadLine(); //first line is header row
            line = deckrdr.ReadLine();
            while (line != null)
            {
                DistinctCard mycard = new DistinctCard();
                string[] substrings = line.Split(delimiter);
                mycard.id = int.Parse(substrings[0]);
                mycard.pic_filename = substrings[1];
                mycard.card_count = int.Parse(substrings[2]);
                mycard.card_type = substrings[3];
                mycard.mana_cost = substrings[4];
                mycard.effects = int.Parse(substrings[5]);

                total_deckcount += mycard.card_count;

                DistinctCards.Add(mycard);

                line = deckrdr.ReadLine();
            }

            int distinctcardindex = 0;
            int actualcardindex = 0;
            current_deckcount = total_deckcount;
            ActualCards = new int[total_deckcount];
            foreach (DistinctCard dc in DistinctCards)
            {
                for (int ii = 0; ii < dc.card_count; ii++)
                {
                    ActualCards[actualcardindex] = distinctcardindex;
                    actualcardindex++;
                }

                distinctcardindex++;
            }

            deckrdr.Close();
        }

        public string ReadDeckSQL(string deckname)
        {
            //you need to pay microsoft for sqlserver database on azure but i am too cheap to buy it, use csv file instead.

            EraseDeck();

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(myconnectionstr);
                if (conn == null)
                    return ("ohmygosh conn is null");

                conn.Open();
            }
            catch (Exception ex)
            {
                string str = string.Format("ReadDeck exception caught: MESSAGE={0}", ex.Message);
                if (ex.InnerException != null && ex.InnerException.Message != null)
                    str += string.Format(" INNER_EXCEPTION_MESSAGE={0}", ex.InnerException.Message);
                return str;
            }



            /****
            string querystring = string.Format("select * from {0}", deckname);
            SqlCommand cmd = new SqlCommand(querystring, conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            DeckName = deckname;

            while (rdr.Read())
            {
                DistinctCard mycard = new DistinctCard();
                mycard.id = (int)rdr[0];
                mycard.pic_filename = rdr[1].ToString();
                mycard.card_count = (int)rdr[2];
                mycard.card_type = rdr[3].ToString();
                mycard.mana_cost = rdr[4].ToString();
                mycard.effects = (int)rdr[5];

                total_deckcount += mycard.card_count;

                DistinctCards.Add(mycard);
            }

            int distinctcardindex = 0;
            int actualcardindex = 0;
            current_deckcount = total_deckcount;
            ActualCards = new int[total_deckcount];
            foreach (DistinctCard dc in DistinctCards)
            {
                for (int ii = 0; ii < dc.card_count; ii++)
                {
                    ActualCards[actualcardindex] = distinctcardindex;
                    actualcardindex++;
                }

                distinctcardindex++;
            }

            rdr.Close();
            conn.Close();
            ****/
            return "OK";
        }

        public int Shuffle()
        {
            Random rnd = new Random();
            int[] myrand = new int[current_deckcount];
            for (int ii = 0; ii < current_deckcount; ii++)
            {
                myrand[ii] = rnd.Next(1, 5001);
            }

            //So now you got 2 arrays: ActualCards and myrandlist
            //ok ok let us just do bubble-sort, i know it is O(n^2) ok ok, i am just lazy and tired to learn something new, please
            //dang dude you could have just used List<int>.Sort() method gosh
            int loopcount = 0;
            int lastcardindex = current_deckcount - 1;
            bool didswap = true;

            while (didswap)
            {
                didswap = false;
                loopcount++;
                for (int cc = 0; cc <= lastcardindex; cc++)
                {
                    if (cc == lastcardindex) //no pairs of cards left
                    {
                        break;
                    }

                    int AA = myrand[cc];
                    int BB = myrand[cc + 1];
                    if (BB < AA)
                    {
                        didswap = true;
                        myrand[cc] = BB;
                        myrand[cc + 1] = AA;

                        int temp = ActualCards[cc];
                        ActualCards[cc] = ActualCards[cc + 1];
                        ActualCards[cc + 1] = temp;
                    }
                }
                lastcardindex--;
            }

            return loopcount;
        }

        public int Draw(int drawcount)
        {
            int actualcardsdrawncount = 0;
            while (actualcardsdrawncount < drawcount)
            {
                if (current_deckcount > 0)
                {
                    int dcindex = ActualCards[current_deckcount - 1]; //bottom of array is actually top of physical card deck
                    current_deckcount--;
                    hand.Add(DistinctCards[dcindex]);
                    actualcardsdrawncount++;
                }
                else
                    break;
            }
            return actualcardsdrawncount;
        }

        public List<DistinctCard> SeeHand
        {
            get
            {
                return this.hand;
            }
        }

        public string GetDeckName
        {
            get
            {
                return this.DeckName;
            }
        }

    }
}
