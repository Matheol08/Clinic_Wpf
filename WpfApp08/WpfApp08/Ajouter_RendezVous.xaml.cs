using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public partial class Ajouter_RendezVous : Window
    {
        public ObservableCollection<RendezVous> RendezVous { get; set; }
        public Ajouter_RendezVous()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            ChargerLesServices();
            ChargerLesSites();
            RendezVous = new ObservableCollection<RendezVous>();
        }
        private async void ChargerLesSites()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/sites";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Patients = JsonConvert.DeserializeObject<Patients[]>(json);

                        
                        Combo2.ItemsSource = Patients;
                        Combo2.DisplayMemberPath = "Ville";
                    }
                    else
                    {
                        MessageBox.Show($"Erreur lors de la récupération des données : {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }
        private async void ChargerLesServices()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/services";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Medecins = JsonConvert.DeserializeObject<Medecins[]>(json);

                        Combo1.ItemsSource = Medecins;
                        Combo1.DisplayMemberPath = "Nom_Service";
                    }
                    else
                    {
                        MessageBox.Show($"Erreur lors de la récupération des données : {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Combo1.Text) ||
                    string.IsNullOrEmpty(Combo2.Text) ||
                    Combo1.SelectedItem == null ||
                    Combo2.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs requis.");
                }
                else
                {
                    int idService = ((Medecins)Combo1.SelectedItem).IdPatient;
                    int idSite = ((Patients)Combo2.SelectedItem).MedecinId;

                    Ajouter_RendezVous nouveau_RendezVous = new Ajouter_RendezVous
                    {
                        IdPatient = Combo1.Text,
                        MedecinId = Combo2.Text,
                        Telephone_fixe = Text3.Text,
                        Telephone_portable = Text4.Text
                    };

                    bool updateSuccess = await EnvoyerDonneesAvecAPI(nouveau_RendezVous);

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




        private async Task<bool> EnvoyerDonneesAvecAPI(nouveau_RendezVous)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/salaries";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(nouveau_RendezVous);
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
