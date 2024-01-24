using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading.Tasks;
using WpfApp08.Models4;
using WpfApp08.Models1;
using WpfApp08.Models3;
using WpfApp08.Models2;

namespace WpfApp08
{
    public partial class Utilisateur : Window
    {
        private ClinicContext dbContext;
        public Utilisateur()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ResizeMode = ResizeMode.NoResize;
            ChargerLesPatients();
            ChargerRDV();
            ChargerLesMedecins();
            dbContext = new ClinicContext(new DbContextOptions<ClinicContext>());

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 PageAcceuil = new Window1();
            PageAcceuil.Show();
            this.Close();
        }
        private void Actualiser(object sender, RoutedEventArgs e)
        {
            ChargerRDV();
        }

        private async void Bouton_Rechercher(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                if (ComboPatients.SelectedItem != null && ComboMedecins.SelectedItem != null)
                {
                    var selectedPatient = (Patients)ComboPatients.SelectedItem;
                    var selectedMedecin = (Medecins)ComboMedecins.SelectedItem;

                    string selectedNomPatient = selectedPatient.Nom;
                    string selectedNomMedecin = selectedMedecin.Nom;

                    string apiUrl = $"https://localhost:7152/api/salaries/rechercheSiteEtService?ville={selectedNomPatient}&nomMedecin={selectedNomMedecin}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }
                else if (ComboPatients.SelectedItem != null)
                {
                    var selectedSite = (Specialites)ComboPatients.SelectedItem;
                    string selectedPatient = selectedSite.Libelle;

                    string apiUrl = $"https://localhost:7152/api/salaries/rechercheSite?ville={selectedPatient}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }
                else if (ComboMedecins.SelectedItem != null)
                {
                    var selectedService = (Medecins)ComboMedecins.SelectedItem;
                    string selectedNomService = selectedService.Nom;

                    string apiUrl = $"https://localhost:7152/api/salaries/rechercheService?Nom_Service={selectedNomService}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }


                string searchTerm = Datedebut.Text;
                string searchTerm2 = Datefin.Text;

                    if (!string.IsNullOrEmpty(searchTerm)|| !string.IsNullOrEmpty(searchTerm2))
                    {
                        string apiUrl = $"https://localhost:7152/api/salaries/rechercherdvpardate?searchTerm2={searchTerm2}?searchTerm={searchTerm}";

                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        HandleApiResponse(response);
                    }
                
            }
        }
    

        private async Task HandleApiResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                var results = JsonConvert.DeserializeObject<List<RendezVous>>(json);

                DataGrid1.ItemsSource = results;
            }
            else
            {
                MessageBox.Show("Erreur lors de la requête API.");
            }

            ComboPatients.SelectedItem = null;
            ComboMedecins.SelectedItem = null;
        }





        private async void ChargerRDV()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/RDV";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var rdv = JsonConvert.DeserializeObject<List<RendezVous>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = rdv;

                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Nom.Patients") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Prenom.Patients") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Medecin", Binding = new Binding("Nom.Medecins") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Date Debut", Binding = new Binding("DateDebut") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Date Fin", Binding = new Binding("DateFin") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Information Complementaire", Binding = new Binding("InfosComplementaires") });

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
        private async void ChargerLesPatients()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/patients";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Patients = JsonConvert.DeserializeObject<Patients[]>(json);


                        ComboPatients.ItemsSource = Patients;
                        ComboPatients.DisplayMemberPath = "Patients";
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
        private async void ChargerLesMedecins()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/medecins";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var medecins = JsonConvert.DeserializeObject<Medecins[]>(json);

                        ComboMedecins.ItemsSource = medecins;
                        ComboMedecins.DisplayMemberPath = "Nom_Medecins";
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

       
    }
}