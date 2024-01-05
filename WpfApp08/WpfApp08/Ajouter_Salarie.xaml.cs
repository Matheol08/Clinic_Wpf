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
using WPF_Salaries;
using WpfApp08.Models2;
using WpfApp08.Models3;


namespace WpfApp08
{
    public partial class Ajouter_Salarie : Window
    {
        public ObservableCollection<Salaries> Salaries { get; set; }
        public Ajouter_Salarie()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            ChargerLesServices();
            ChargerLesSites();
            Salaries = new ObservableCollection<Salaries>();
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
                        var sites = JsonConvert.DeserializeObject<Sites[]>(json);

                        
                        Combo2.ItemsSource = sites;
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
                        var services = JsonConvert.DeserializeObject<Service[]>(json);

                        Combo1.ItemsSource = services;
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
                if (string.IsNullOrEmpty(Text1.Text) ||
                    string.IsNullOrEmpty(Text2.Text) ||
                    string.IsNullOrEmpty(Text3.Text) ||
                    string.IsNullOrEmpty(Text4.Text) ||
                    Combo1.SelectedItem == null ||
                    Combo2.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs requis.");
                }
                else
                {
                    int idService = ((Service)Combo1.SelectedItem).IDService;
                    int idSite = ((Sites)Combo2.SelectedItem).IDSite;

                    Ajout_Salaries nouvelAjoutSalarie = new Ajout_Salaries
                    {
                        Nom = Text1.Text,
                        Prenom = Text2.Text,
                        Telephone_fixe = Text3.Text,
                        Telephone_portable = Text4.Text,
                        Email = Text5.Text,
                        IDService = idService,
                        IDSite = idSite
                    };

                    bool updateSuccess = await EnvoyerDonneesAvecAPI(nouvelAjoutSalarie);

                    if (updateSuccess)
                    {
                        MessageBox.Show("Mise à jour réussie !");
                        CRUDEmploye pageAcceuil = new CRUDEmploye();
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




        private async Task<bool> EnvoyerDonneesAvecAPI(Ajout_Salaries ajoutSalarie)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/salaries";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ajoutSalarie);
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
            CRUDEmploye pageAcceuil = new CRUDEmploye();
            pageAcceuil.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
