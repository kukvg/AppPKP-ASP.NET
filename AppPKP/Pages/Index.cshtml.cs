using AppPKP.Pages.PKP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;
using static AppPKP.Pages.IndexModel;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace AppPKP.Pages
{
    public class IndexModel : PageModel
    {


        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage();
        }


        public List<PolaczeniaInfo> PolaczeniaList = new List<PolaczeniaInfo>();

        public string Godzina { get; set; }
        public string Odjazd { get; set; }
        public string Przyjazd { get; set; }

        public string polaczenieID { get; set;}

        public string nazwaUser { get; set; }

        public void OnPostSearch()
        {

            Godzina = Request.Form["Godzina"];
            Odjazd = Request.Form["Odjazd"];
            Przyjazd = Request.Form["Przyjazd"];
          

            
            try
            {
                string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection conn = new SqlConnection(bazaconnectString))
                {
                    conn.Open();
                    String polecenie = "SELECT * FROM polaczenia";
                    using (SqlCommand comm = new SqlCommand(polecenie, conn))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                PolaczeniaInfo polaczeniaInfo = new PolaczeniaInfo();
                                polaczeniaInfo.id = reader.GetInt32(0); // "" + ....
                                polaczeniaInfo.godzina = reader.GetTimeSpan(1).ToString();
                                polaczeniaInfo.wyjazd = reader.GetString(2);
                                polaczeniaInfo.przyjazd = reader.GetString(3);
                                polaczeniaInfo.droga = reader.GetString(4);

                               

                                TimeSpan ts1, ts2;
                                TimeSpan.TryParse(polaczeniaInfo.godzina, out ts1);
                                TimeSpan.TryParse(Godzina, out ts2);


                                int godzinaInt = (int)ts1.TotalSeconds;
                                int godzinaPageInt = (int)ts2.TotalSeconds;


                                

                                if (godzinaInt >= godzinaPageInt && polaczeniaInfo.wyjazd == Odjazd && polaczeniaInfo.przyjazd == Przyjazd)
                                {
                                    
                                    PolaczeniaList.Add(polaczeniaInfo);
                                    //completeMessage = "Znaleziono!";
                                    
                                }

                                PolaczeniaList = PolaczeniaList.OrderBy(td => TimeSpan.Parse(td.godzina)).ToList();


                              

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }



        }
        public void OnPostOrder()
        {

            polaczenieID = Request.Form["polaczenieID"];
            nazwaUser = Request.Form["nazwaUser"];

                if (int.TryParse(polaczenieID, out int polaczenieIDValue))
                {

                    try
                    {

                        string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                        using (SqlConnection conn = new SqlConnection(bazaconnectString))
                        {
                            conn.Open();
                            string polecenieinsert = "INSERT INTO bilety (nazwa_USER, polaczenieID) VALUES (@nazwaUser, @polaczenieID)";
                            using (SqlCommand comm = new SqlCommand(polecenieinsert, conn))
                            {
                                comm.Parameters.AddWithValue("@nazwaUser", nazwaUser);
                                comm.Parameters.AddWithValue("@polaczenieID", polaczenieIDValue);

                                comm.ExecuteNonQuery();
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Exception: " + ex.ToString());
                    }
                }
            

        }

        public void OnGet()
        {
            //try
            //{
            //    string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //    using (SqlConnection conn = new SqlConnection(bazaconnectString))
            //    {
            //        conn.Open();
            //        String polecenie = "INSERT INTO zamowienie (godzina, wyjazd, przyjazd, droga) VALUES (@godzina, @wyjazd, @przyjazd, @droga)";
            //        using (SqlCommand comm = new SqlCommand(polecenie, conn))
            //        {
            //            comm.Parameters.AddWithValue("@godzina", Imie);
            //            comm.Parameters.AddWithValue("@wyjazd", Nazwisko);
            //            comm.Parameters.AddWithValue("@przyjazd", Email);
            //            comm.Parameters.AddWithValue("@droga", hashedPassword);
                
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception: " + ex.ToString());
            //}
        }

       



        public class PolaczeniaInfo
        {
            public int id;
            public string godzina;
            public string wyjazd;
            public string przyjazd;
            public string droga;
        }
    }
}