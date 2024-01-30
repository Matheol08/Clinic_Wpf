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
using System.Globalization;
using System.Windows.Input;

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
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
       
            if (DataGrid1.SelectedItem != null)
            {
               
                var selectedRowData = (RendezVous)DataGrid1.SelectedItem;

              
                InfoRDV secondWindow = new InfoRDV(selectedRowData);
                secondWindow.ShowDialog();
            }
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

                    string apiUrl = $"https://localhost:7152/api/RendezVous/recherchePatientsEtMedecins?patientNom={selectedNomPatient}&medecinNom={selectedNomMedecin}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }

                else if (ComboPatients.SelectedItem != null)
                {
                    var selectedPatient = (Patients)ComboPatients.SelectedItem;
                    string selectedPatientNom = selectedPatient.Nom;

                    string apiUrl = $"https://localhost:7152/api/RendezVous/recherchePatient?PatientNom={selectedPatientNom}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }

                else if (ComboMedecins.SelectedItem != null)
                {
                    var selectedMedecins = (Medecins)ComboMedecins.SelectedItem;
                    string selectedNomMedecins = selectedMedecins.Nom;

                    string apiUrl = $"https://localhost:7152/api/RendezVous/rechercheMedecin?medecinNom={selectedNomMedecins}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    HandleApiResponse(response);
                }



                string searchTerm = Datedebut.Text;
                string searchTerm2 = Datefin.Text;

                if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchTerm2))
                {

                    // Formater les dates dans le format attendu par l'API (jj/mm/aaaa)
                    DateTime dateDebutFormatted = DateTime.ParseExact(searchTerm, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dateFinFormatted = DateTime.ParseExact(searchTerm2, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // Construire l'URL de l'API avec les dates formatées correctement
                    string apiUrl = $"https://localhost:7152/api/RendezVous/DateRange?startDate={dateDebutFormatted.ToString("yyyy/MM/dd")}&endDate={dateFinFormatted.ToString("yyyy/MM/dd")}";

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
            Datedebut.SelectedDate = null;
            Datefin.SelectedDate = null;

        }





        private async void ChargerRDV()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/RendezVous";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var rdv = JsonConvert.DeserializeObject<List<RendezVous>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = rdv;

                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom Patient", Binding = new Binding("Patients.Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom Patient", Binding = new Binding("Patients.Prenom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom Medecin", Binding = new Binding("Medecins.Nom") });
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
                    string apiUrl = "https://localhost:7152/api/Patients";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Patients = JsonConvert.DeserializeObject<Patients[]>(json);


                        ComboPatients.ItemsSource = Patients;
                        ComboPatients.DisplayMemberPath = "Nom";
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
                    string apiUrl = "https://localhost:7152/api/Medecins";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var medecins = JsonConvert.DeserializeObject<Medecins[]>(json);

                        ComboMedecins.ItemsSource = medecins;
                        ComboMedecins.DisplayMemberPath = "Nom";
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