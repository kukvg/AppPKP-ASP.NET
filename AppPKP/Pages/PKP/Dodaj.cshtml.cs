using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static AppPKP.Pages.IndexModel;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using AppPKP.Pages.PKP;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Identity.Client;

namespace AppPKP.Pages.PKP
{
    public class DodajModel : PageModel
    {

        public string nazwaUser { get; set; }
        public string Godzina { get; set; }
        public string Odjazd { get; set; }
        public string Przyjazd { get; set; }

        public string Trasa { get; set; }

        public bool isSu { get; set; }

        public string mess = "";


        protected void PageLoad(object sender, EventArgs e)
        {

              
            
        }


        public void OnGet()
        {

        }

        public void OnPost()
        {
            isSu = ZmienneSesja.sUser;

            Godzina = Request.Form["Godzina"];
            Odjazd = Request.Form["Odjazd"];
            Przyjazd = Request.Form["Przyjazd"];
            Trasa = Request.Form["Trasa"];
            if (isSu == true)
            {
                if (Godzina.Length != 0 && Odjazd.Length != 0 && Przyjazd.Length != 0 && Trasa.Length != 0)
                {


                    try
                    {
                        string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                        using (SqlConnection conn = new SqlConnection(bazaconnectString))
                        {
                            conn.Open();
                            String polecenie = "INSERT INTO polaczenia (godzina, wyjazd, przyjazd, droga) VALUES (@Godzina, @Odjazd, @Przyjazd, @Trasa)";
                            using (SqlCommand comm = new SqlCommand(polecenie, conn))
                            {

                                comm.Parameters.AddWithValue("@Godzina", Godzina);
                                comm.Parameters.AddWithValue("@Odjazd", Odjazd);
                                comm.Parameters.AddWithValue("@Przyjazd", Przyjazd);
                                comm.Parameters.AddWithValue("@Trasa", Trasa);


                                comm.ExecuteNonQuery();
                                mess = "Pomyslnie dodano polaczenie!";
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.ToString());
                    }
                }
                else
                {
                    mess = "Wprowadz wszystkie pola!";
                    return;
                }
            }
            else
            {

                mess = "Brak uprawnien";
            }

        }
    }
}
