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
            ChargerLesSpecialites();
            Specialites = new ObservableCollection<Specialites>();
            DataGrid1.ItemsSource = Specialites;
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Nom_Spe.Text))
            {
                MessageBox.Show("Veuillez remplir le champ Specialité.");
            }
            else
            {
                string NouvelleSpecialite = Nom_Spe.Text;

                Specialites nouvelleSpecialite = new Specialites
                {
                    Libelle = NouvelleSpecialite,
                };

                Specialites.Add(nouvelleSpecialite);

                bool updateSuccess = await EnvoyerDonneesAvecAPI(nouvelleSpecialite);

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

        private async Task<bool> EnvoyerDonneesAvecAPI(Specialites specialite)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/specialites";

                string jsonData = JsonConvert.SerializeObject(specialite);

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
            Specialites specialiteSelectionnee = (Specialites)DataGrid1.SelectedItem;

            if (specialiteSelectionnee != null)
            {
                int specialiteId = specialiteSelectionnee.IdSpecialite;

                bool isAssigned = await VerifierAssignationMedecin(specialiteId);

                if (isAssigned)
                {
                    MessageBox.Show("Échec de la suppression. La Spécialité est assigné à un Médecin.");
                }
                else
                {
                    bool deleteSuccess = await SupprimerDonneesAvecAPI(specialiteId);

                    if (deleteSuccess)
                    {
                        MessageBox.Show("Suppression réussie !");
                        Specialites.Remove(specialiteSelectionnee);
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

        private async Task<bool> VerifierAssignationMedecin(int specialiteId)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ClinicContext>();
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true");

                using (var context = new ClinicContext(optionsBuilder.Options))
                {
                    int count = await context.Specialites.CountAsync(s => s.IdSpecialite == specialiteId);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de l'assignation des salariés : {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SupprimerDonneesAvecAPI(int specialiteId)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/specialites/{specialiteId}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDSpecialite Pagespe = new CRUDSpecialite();
                    Pagespe.Show();
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

        //private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    var editedRow = e.Row.Item as Specialites;
        //    var nouvelleValeur = (e.EditingElement as TextBox).Text;
        //}

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {
            Specialites specialiteSelectionnee = (Specialites)DataGrid1.SelectedItem;

            if (specialiteSelectionnee != null)
            {
                int specialiteID = specialiteSelectionnee.IdSpecialite;

                bool updateSuccess = await MettreAJourDonneesAvecAPI(specialiteID, specialiteSelectionnee);

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

        private async Task<bool> MettreAJourDonneesAvecAPI(int specialiteID, Specialites specialite)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/specialites/{specialiteID}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(specialite);
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
            ChargerLesSpecialites();
        }

        private async void ChargerLesSpecialites()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/specialites";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var specialites = JsonConvert.DeserializeObject<Specialites[]>(json);

                        DataGrid1.Columns.Clear();
                        DataGrid1.ItemsSource = specialites;
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
