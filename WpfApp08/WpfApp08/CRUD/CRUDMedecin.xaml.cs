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
            ChargerLesMedecins();
            Medecins = new ObservableCollection<Medecins>();
            DataGrid1.ItemsSource = Medecins;
        }
        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_Medecin pageIdMedecin = new Ajouter_Medecin();
            pageIdMedecin.Show();
            this.Close();
        }



        private async Task<bool> SupprimerDonneesAvecAPI(int id)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/Medecins/{id}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDMedecin PageMedecin = new CRUDMedecin();
                    PageMedecin.Show();
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
            Medecins medecinSelectionne = (Medecins)DataGrid1.SelectedItem;

            if (medecinSelectionne != null)
            {
                int IdMedecin = medecinSelectionne.IdMedecin;

                bool isAssigned = await VerifierAssignationRDV(IdMedecin);

                if (isAssigned)
                {
                    MessageBox.Show("Échec de la suppression. Le médecin est assigné à des rendez-vous.");
                }
                else
                {
                    bool deleteSuccess = await SupprimerDonneesAvecAPI(IdMedecin);

                    if (deleteSuccess)
                    {
                        MessageBox.Show("Suppression réussie !");
                        Medecins.Remove(medecinSelectionne);
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

        private async Task<bool> VerifierAssignationRDV(int IdMedecin)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ClinicContext>();
                //optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true");

                using (var context = new ClinicContext(optionsBuilder.Options))
                {
                    int count = await context.Medecins.CountAsync(s => s.IdMedecin == IdMedecin);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de l'assignation des rendez-vous : {ex.Message}");
                return false;
            }
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {
            Medecins medecinSelectionne = (Medecins)DataGrid1.SelectedItem;

            if (medecinSelectionne != null)
            {
                int IdMedecin = medecinSelectionne.IdMedecin;

                bool updateSuccess = await MettreAJourDonneesAvecAPI(IdMedecin, medecinSelectionne);

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

        private async Task<bool> MettreAJourDonneesAvecAPI(int id, Medecins medecin)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/Medecins/{id}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(medecin);
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

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            Administrateur PageMedecin = new Administrateur();
            PageMedecin.Show();
            this.Close();
        }

        private void Actualiser(object sender, RoutedEventArgs e)
        {
            ChargerLesMedecins();
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

                        DataGrid1.Columns.Clear();
                        DataGrid1.ItemsSource = medecins;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Prenom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom_Specialite", Binding = new Binding("Medecins.Libelle") });
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
