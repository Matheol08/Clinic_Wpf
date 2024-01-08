using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp08.Models4;
namespace WpfApp08
{
    public partial class CRUDSpecialite : Window
    {
         public ObservableCollection<Specialites> Specialites { get; set; }
        public CRUDSpecialite()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            ResizeMode = ResizeMode.NoResize;
            Chargerlesservices();
            Specialites = new ObservableCollection<Specialites>();
            DataGrid1.ItemsSource = Specialites;
        }



        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Nom_Service.Text) )
            {
                MessageBox.Show("Veuillez remplir le champ service.");
            }
            else
            {

                string Nouveasite = Nom_Service.Text;

                Specialites nouveauSite = new Specialites
                {
                    Libelle = Nouveasite,

                };

                Specialites.Add(nouveauSite);

                bool updateSuccess = await EnvoyerDonneesAvecAPI(nouveauSite);

                if (updateSuccess)
                {
                    MessageBox.Show("Mise à jour réussie !");
                }
                else
                {
                    MessageBox.Show("Échec de la mise à jour.");
                }
            }
        }
        private async Task<bool> EnvoyerDonneesAvecAPI(Specialites Specialites)
        {
            try
            {

                string apiUrl = "https://localhost:7152/api/services";


                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(Specialites);

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);
                    CRUDSpecialite Pagesite = new CRUDSpecialite();
                    Pagesite.Show();
                    this.Close();
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erreur lors de la mise à jour : {ex.Message}");
                return false;
            }
        }
        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            Specialites serviceSelectionne = (Specialites)DataGrid1.SelectedItem;

            if (serviceSelectionne != null)
            {
                int serviceId = serviceSelectionne.IdSpecialite;

                bool isAssigned = await VerifierAssignationSalaries(serviceId);

                if (isAssigned)
                {
                    MessageBox.Show("Échec de la suppression. Le service est assigné à des salariés.");
                }
                else
                {
                    bool deleteSuccess = await SupprimerDonneesAvecAPI(serviceId);

                    if (deleteSuccess)
                    {
                        MessageBox.Show("Suppression réussie !");
                        Specialites.Remove(serviceSelectionne);
                    }
                    else
                    {
                        MessageBox.Show("Échec de la suppression.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne à supprimer.");
            }
        }

       
        private async Task<bool> VerifierAssignationSalaries(int serviceId)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AnnuaireContext>();
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Annuaire;Trusted_Connection=True;TrustServerCertificate=true");

                using (var context = new AnnuaireContext(optionsBuilder.Options))
                {
                    int count = await context.Specialites.CountAsync(s => s.IdSpecialite == serviceId);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de l'assignation des salariés : {ex.Message}");
                return false;
            }
        }
        private async Task<bool> SupprimerDonneesAvecAPI(int serviceId)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/services/{serviceId}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDSpecialite Pagesite = new CRUDSpecialite();
                    Pagesite.Show();
                    this.Close();
                    return response.IsSuccessStatusCode;
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression : {ex.Message}");
                return false;
            }
        }


        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            var editedRow = e.Row.Item as Specialites;


            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {

            Specialites serviceSelectionne = (Specialites)DataGrid1.SelectedItem;

            if (serviceSelectionne != null)
            {

                int serviceID = serviceSelectionne.IdSpecialite;


                bool updateSuccess = await MettreAJourDonneesAvecAPI(serviceID, serviceSelectionne);

                if (updateSuccess)
                {

                    MessageBox.Show("Mise à jour réussie !");
                }
                else
                {

                    MessageBox.Show("Échec de la mise à jour.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne à mettre à jour.");
            }
        }


        private async Task<bool> MettreAJourDonneesAvecAPI(int serviceID, Specialites Service)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/services/{serviceID}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(Service);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(apiUrl, content);
                   
                    return response.IsSuccessStatusCode;
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour : {ex.Message}");
                return false;
            }
        }

        private void Actualiser(object sender, RoutedEventArgs e)
        {
            Chargerlesservices();
        }

        private async void Chargerlesservices()
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
                        var service = JsonConvert.DeserializeObject<Specialites[]>(json);

                        DataGrid1.Columns.Clear();
                        DataGrid1.ItemsSource = service;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Service", Binding = new Binding("Nom_Service") });
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
            private void RetourButton_Click(object sender, RoutedEventArgs e)
            {
                Administrateur Pagesite = new Administrateur();
                Pagesite.Show();
                this.Close();
            }

        

    }
}
