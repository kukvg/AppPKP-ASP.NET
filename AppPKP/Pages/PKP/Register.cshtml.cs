using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static AppPKP.Pages.IndexModel;

namespace AppPKP.Pages.PKP
{
    public class RegisterModel : PageModel
    {
        public string mess = "";
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        //[EmailAddress(ErrorMessage = "Zly format adresu email")]
        public string Email { get; set; }

        public string Haslo { get; set; }

        public string PowtHaslo { get; set; }


        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
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


        public void OnPost()
        {
            Imie = Request.Form["Imie"];
            Nazwisko = Request.Form["Nazwisko"];
            Email = Request.Form["Email"];
            Haslo = Request.Form["Haslo"];
            PowtHaslo = Request.Form["PowtHaslo"];

            string salt = GenerateSalt();
            string hashedPassword  = HashPassword(Haslo, salt);



            if (Imie.Length != 0 && Nazwisko.Length != 0 && Email.Length != 0 && Haslo.Length != 0 && PowtHaslo.Length != 0 && Haslo == PowtHaslo)
            {

               
                try
                {
                    string bazaconnectString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                    using (SqlConnection conn = new SqlConnection(bazaconnectString))
                    {
                        conn.Open();
                        String polecenie = "INSERT INTO users (imie, nazwisko, email, haslo, sol) VALUES (@Imie, @Nazwisko, @Email, @Haslo, @sol)";
                        using (SqlCommand comm = new SqlCommand(polecenie, conn))
                        {

                            comm.Parameters.AddWithValue("@Imie", Imie);
                            comm.Parameters.AddWithValue("@Nazwisko", Nazwisko);
                            comm.Parameters.AddWithValue("@Email", Email);
                            comm.Parameters.AddWithValue("@Haslo", hashedPassword);
                            comm.Parameters.AddWithValue("@sol", salt);


                            comm.ExecuteNonQuery();
                            mess = "Pomyslnie zarejestrowano!";
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
                mess = "WprowadŸ wszystkie pola, has³o musi byæ identyczne!";
                return;
            }
        }
        public void OnGet()
        {
        }
    }
}
