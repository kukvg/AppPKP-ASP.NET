using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using AppPKP.Pages.PKP;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using static AppPKP.Pages.IndexModel;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace AppPKP.Pages.PKP
{
    public class RozkladModel : PageModel
    {
       
        public List<RozkladInfo> RozkladList = new List<RozkladInfo>();
        public void OnGet()
        {

            try
            {
                string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection conn = new SqlConnection(bazaconnectString))
                {
                    conn.Open();
                    String polecenie = "SELECT * FROM polaczenia";
                    using (SqlCommand comm = new SqlCommand(polecenie, conn))
                    {
                        using(SqlDataReader reader = comm.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                RozkladInfo rozkladInfo = new RozkladInfo();
                                rozkladInfo.id = reader.GetInt32(0); // "" + ....
                                rozkladInfo.godzina = reader.GetTimeSpan(1).ToString();
                                rozkladInfo.wyjazd = reader.GetString(2);
                                rozkladInfo.przyjazd = reader.GetString(3);
                                rozkladInfo.droga = reader.GetString(4);

                                RozkladList.Add(rozkladInfo);

                                RozkladList = RozkladList.OrderBy(td => TimeSpan.Parse(td.godzina)).ToList();

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

        //public int tempSite = 1;


        //public void Poprzedni()
        //{

        //    if (tempSite > 1)
        //    {
        //        tempSite--;
        //    }
        //}
        //public void Kolejny()
        //{
        //   if(tempSite < RozkladList.Count)
        //    {
        //        tempSite++;
        //    }

        //}


    }
    public class RozkladInfo
    {
        public int id; // string
        public string godzina;
        public string wyjazd;
        public string przyjazd;
        public string droga;
    }
    
   
  
}
