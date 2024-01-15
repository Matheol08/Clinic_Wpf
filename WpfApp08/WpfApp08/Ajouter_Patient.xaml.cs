using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp08.Models1;
using WpfApp08.Models2;
using WpfApp08.Models3;


namespace WpfApp08
{
    public partial class Ajouter_Patient : Window
    {
        public ObservableCollection<Patients> Patients { get; set; }
        public Ajouter_Patient()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            Patients = new ObservableCollection<Patients>();
        }



        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Text1.Text) ||
                    string.IsNullOrEmpty(Text2.Text) ||
                    string.IsNullOrEmpty(Text3.Text) ||
                    string.IsNullOrEmpty(Text4.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs requis.");
                }
                else
                {
                    Patients nouvelAjoutPatients = new Patients
                    {
                        Nom = Text1.Text,
                        Prenom = Text2.Text,
                        Telephone = Text3.Text,
                        Email = Text4.Text
                    };

                    bool updateSuccess = await EnvoyerDonneesAvecAPI(nouvelAjoutPatients);

                    if (updateSuccess)
                    {
                        MessageBox.Show("Mise à jour réussie !");
                        CRUDRendezVous pageAcceuil = new CRUDRendezVous();
                        pageAcceuil.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Échec de la mise à jour.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }
    
        private async Task<bool> EnvoyerDonneesAvecAPI(Patients nouvelAjoutPatients)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/salaries";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(nouvelAjoutPatients);
                Console.WriteLine($"JSON Data: {jsonData}");

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("API call succeeded.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"API call failed. Status Code: {response.StatusCode}");

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during API call: {ex.Message}");
                MessageBox.Show($"Error during API call: {ex.Message}");
                return false;
            }
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CRUDRendezVous pageAcceuil = new CRUDRendezVous();
            pageAcceuil.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
