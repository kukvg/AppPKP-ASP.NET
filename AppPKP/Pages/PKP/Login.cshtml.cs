using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using AppPKP.Pages.PKP;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using static AppPKP.Pages.IndexModel;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;


namespace AppPKP.Pages.PKP
{
    public class LoginModel : PageModel
    {
        public string mess = "";

        public string Email { get; set; }

        public string Haslo { get; set; }

        public bool sUser = false;

        List<string> emailList = new List<string>();
        List<string> hashedPasswordList = new List<string>();
        List<string> saltList = new List<string>();
        List<bool> suList = new List<bool>();

        public void OnPost()
        {
           

            Email = Request.Form["Email"];
            Haslo = Request.Form["Haslo"];



            try
            {
                string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection conn = new SqlConnection(bazaconnectString))
                {
                    conn.Open();
                    String polecenie = "SELECT email, haslo, sol, su FROM users";
                    using (SqlCommand comm = new SqlCommand(polecenie, conn))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                emailList.Add(reader.GetString(0));
                                hashedPasswordList.Add(reader.GetString(1));
                                saltList.Add(reader.GetString(2));
                                suList.Add(reader.GetBoolean(3));

                                

                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }


            string userInputPassword = Haslo;
            
            int id = -1;

            

            for (int i = 0; i < emailList.Count; i++)
            {
                
                if (Email == emailList[i])
                {
                    id = i;
                    string hashedPassword = HashPassword(userInputPassword, saltList[id]);
                    if (id != -1)
                    {
                        if (hashedPasswordList[id] == hashedPassword && emailList[id] == Email)
                        {
                            mess = "Pomyœlnie zalogowano!";

                            HttpContext.Session.SetString(ZmienneSesja.sesjaUser, @Email);
                            HttpContext.Session.SetString(ZmienneSesja.idSesji, Guid.NewGuid().ToString());
                           for(int j = 0; j < suList.Count-1;j++)
                            {
                                if (emailList[j] == Email && suList[j] == true)
                                {
                                    ZmienneSesja.sUser = true;
                                }
                            }
                                    

                            
                        }
                        else
                        {
                            mess = "Has³o jest niepoprawne";

                        }
                    }
                    break;
                }
                

            }
            








        }


        public string HashPassword(string password, string salt)
        {
            byte[] passowrdBytes = Encoding.UTF8.GetBytes(password); //konwersje hasla oraz soli na tablice bajtow 
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] hashedBytes;

            using (var sha256 = SHA256.Create()) //generacja skrotu sha-256
            {
                byte[] combinedBytes = new byte[passowrdBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(passowrdBytes, 0, combinedBytes, 0, passowrdBytes.Length); // skopiowanie tablicy bajtow hasla do polaczonej tablicy bajtow
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, passowrdBytes.Length, saltBytes.Length); // skopiowanie tablicy bajtow soli do polaczonej tablicy bajtow

                hashedBytes = sha256.ComputeHash(combinedBytes); // oblicznie skrotu shap-265 dla polaczonej tablicy
            }

            return Convert.ToBase64String(hashedBytes); // konwersja tablicy bajtow zaszyfrowanego hasla na Base64 i wypisanie jak string
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            app.UseSession();
        }

        [HttpGet]
        public void OnGet()
        {
            HttpContext.Session.Remove(ZmienneSesja.sesjaUser);
            HttpContext.Session.Remove(ZmienneSesja.idSesji);
            ZmienneSesja.sUser = false;
            RedirectToPage("/Index");
        }
    }
}
