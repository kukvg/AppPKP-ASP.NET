using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using static AppPKP.Pages.IndexModel;
using System.Security.Cryptography;
using System.Text;
using AppPKP.Pages.PKP;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Reflection.PortableExecutable;

namespace AppPKP.Pages.PKP
{

    public class UserModel : PageModel
    {
        public string mess { get; set; }

        public List<BiletyInfo> BiletyList = new List<BiletyInfo>();

        public string nazwaUser { get; set; }

        public int id { get; set; }
        public string godzina { get; set; }
        public string wyjazd { get; set; }
        public string przyjazd { get; set; }
        public string droga { get; set; }


        protected void PageLoad(object sender, EventArgs e)
        {
        }
        public void OnGet()
        {

            try
            {
                string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection conn = new SqlConnection(bazaconnectString))
                {
                    conn.Open();

                    string polecenie = "SELECT b.id, b.nazwa_USER, p.godzina, p.wyjazd, p.przyjazd, p.droga FROM bilety b INNER JOIN polaczenia p ON b.polaczenieID = p.id WHERE b.nazwa_USER = @nazwaUser";

              

                    using (SqlCommand comm = new SqlCommand(polecenie, conn))
                    {
                        nazwaUser = HttpContext.Session.GetString(ZmienneSesja.sesjaUser);
                        comm.Parameters.AddWithValue("@nazwaUser", nazwaUser);

                        //mess = nazwaUser;

                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    BiletyInfo biletyInfo = new BiletyInfo();
                                    biletyInfo.id = reader.GetInt32(0);
                                    biletyInfo.nazwaUser = reader.GetString(1);
                                    biletyInfo.godzina = reader.GetTimeSpan(2).ToString();
                                    biletyInfo.wyjazd = reader.GetString(3);
                                    biletyInfo.przyjazd = reader.GetString(4);
                                    biletyInfo.droga = reader.GetString(5);

                                    BiletyList.Add(biletyInfo);
                                }
                                catch (Exception ex)
                                {
                                    // Obs³u¿ wyj¹tek lub zaloguj go do konsoli, aby dowiedzieæ siê, co posz³o nie tak.
                                    Console.WriteLine("Exception while processing data: " + ex.ToString());
                                    mess = ex.ToString();
                                }
                            }


                            //mess = "test";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }



    public class BiletyInfo
    {

        public string nazwaUser;

        //public BiletyInfo(string nazwaUser)
        //{
        //    this.nazwaUser = nazwaUser;
        //}

        public int id;
        public string godzina;
        public string wyjazd;
        public string przyjazd;
        public string droga;
    }

}
