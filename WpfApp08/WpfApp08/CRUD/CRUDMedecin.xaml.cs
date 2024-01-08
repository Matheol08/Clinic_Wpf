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
using WpfApp08.Models1;


namespace WpfApp08
{
    public partial class CRUDMedecin : Window
    {

        public ObservableCollection<Medecins> Medecins { get; set; }
        public CRUDMedecin()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            ResizeMode = ResizeMode.NoResize;
            Chargerlessites();
            Medecins = new ObservableCollection<Medecins>();
            DataGrid1.ItemsSource = Medecins;
        }



        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Nom.Text) )
            {
                MessageBox.Show("Veuillez remplir le champ Ville.");
            }
            else
            {

                string Nouveasite = Ville.Text;

                Medecins nouveauSite = new Medecins
                {
                    Nom = Nouveasite,
                    Prenom = ""
                };

                Medecins.Add(nouveauSite);

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
            private async Task<bool> EnvoyerDonneesAvecAPI(Medecins site)
            {
                try
                {

                    string apiUrl = "https://localhost:7152/api/sites";


                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(site);

                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(apiUrl, content);
                    CRUDMedecin Pagesite = new CRUDMedecin();
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
 

        private async Task<bool> SupprimerDonneesAvecAPI(int siteId)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/sites/{siteId}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDMedecin Pagesite = new CRUDMedecin();
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


        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            Medecins siteSelectionne = (Medecins)DataGrid1.SelectedItem;

            if (siteSelectionne != null)
            {
                int IdMedecin = siteSelectionne.IdMedecin;

                bool isAssigned = await VerifierAssignationSalaries(IdMedecin);

                if (isAssigned)
                {
                    MessageBox.Show("Échec de la suppression. Le site est assigné à des salariés.");
                }
                else
                {
                    bool deleteSuccess = await SupprimerDonneesAvecAPI(IdMedecin);

                    if (deleteSuccess)
                    {
                        MessageBox.Show("Suppression réussie !");
                        Medecins.Remove(siteSelectionne);
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

        private async Task<bool> VerifierAssignationSalaries(int siteId)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AnnuaireContext>();
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Annuaire;Trusted_Connection=True;TrustServerCertificate=true");

                using (var context = new AnnuaireContext(optionsBuilder.Options))
                {
                    int count = await context.Salaries.CountAsync(s => s.IDSite == siteId);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de l'assignation des salariés : {ex.Message}");
                return false;
            }
        }



        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            {
      
                var editedRow = e.Row.Item as Medecins;


                var nouvelleValeur = (e.EditingElement as TextBox).Text;
            }
        
            private async void MAJ_Click(object sender, RoutedEventArgs e)
            {

            Medecins siteSelectionne = (Medecins)DataGrid1.SelectedItem;

                if (siteSelectionne != null)
                {
      
                    int siteId = siteSelectionne.IdMedecin;

             
                    bool updateSuccess = await MettreAJourDonneesAvecAPI(siteId, siteSelectionne);

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
        

            private async Task<bool> MettreAJourDonneesAvecAPI(int IdMedecin, Medecins Medecins)
            {
                try
                {
                    string apiUrl = $"https://localhost:7152/api/sites/{IdMedecin}";

                    using (HttpClient client = new HttpClient())
                    {
                        var jsonData = JsonConvert.SerializeObject(Medecins);
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
        

        private void Button_Click(object sender, RoutedEventArgs e)
            {

            }
          
            
            private void RetourButton_Click(object sender, RoutedEventArgs e)
            {
                Administrateur Pagesite = new Administrateur();
                Pagesite.Show();
                this.Close();
            }

            private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            {

            }
            private void Actualiser(object sender, RoutedEventArgs e)
            {
                Chargerlessites();
            }
        private async void Chargerlessites()
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
                            var sites = JsonConvert.DeserializeObject<Medecins[]>(json);

                        DataGrid1.Columns.Clear();
                        DataGrid1.ItemsSource = sites;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Prenom") });

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

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}