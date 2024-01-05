using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp08
{
   
    public partial class AdminMDP : Window
    {
        public AdminMDP()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ResizeMode = ResizeMode.NoResize;
        }
        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 PageAcceuil = new Window1();
            PageAcceuil.Show();
            this.Close();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string valeurSaisie = passwordBox.Password;

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://localhost:7152/api/Admin/verificationAdmin?idadmin=1&valeurSaisie={valeurSaisie}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (responseContent == "Mot de passe correct.")
                    {


                        Administrateur PageAcceuil = new Administrateur();
                        PageAcceuil.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Mot de passe incorrect.");
                        
                    }
                }
                else
                {
                    MessageBox.Show("Erreur lors de la requête API.");
                }
            }
            passwordBox.Password = "";
        }





        public class VerificationResponse
        {
            public bool IsCorrect { get; set; }
            public string Message { get; set; }
        }


    }
}
