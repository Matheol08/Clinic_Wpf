using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp08.Models1;
using WpfApp08.Models4;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

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
                optionsBuilder.UseSqlServer("Server=DESKTOP-4QTSD8Q;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true;");

                using (var context = new ClinicContext(optionsBuilder.Options))
                {
                    // Vérifie s'il existe des rendez-vous assignés à ce médecin
                    bool isAssigned = await context.RendezVous.AnyAsync(r => r.MedecinId == IdMedecin);

                    return isAssigned;
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
                try
                {
                    string nouveauNom = medecinSelectionne.Nom;
                    string nouveauPrenom = medecinSelectionne.Prenom;
                    Specialites nouvelleSpecialite = medecinSelectionne.Specialites;

                    Specialites specialiteExistante = await VerifierSpecialiteExistante(nouvelleSpecialite);

                    if (specialiteExistante != null)
                    {
                        medecinSelectionne.SpecialiteId = specialiteExistante.IdSpecialite;

                        bool updateSuccess = await MettreAJourDonneesAvecAPI(medecinSelectionne.IdMedecin, medecinSelectionne);

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
                        MessageBox.Show("La spécialité modifiée n'existe pas.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la mise à jour : {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne à mettre à jour.");
            }
        }



        private async Task<Specialites> VerifierSpecialiteExistante(Specialites nouvelleSpecialite)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ClinicContext>();
                optionsBuilder.UseSqlServer("Server=DESKTOP-4QTSD8Q;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true;");
                using (var context = new ClinicContext(optionsBuilder.Options))

                {
                    Specialites specialiteExistante = await context.Specialites
                        .FirstOrDefaultAsync(s => s.Libelle == nouvelleSpecialite.Libelle);

                    return specialiteExistante;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification de la spécialité : {ex.Message}");
                return null;
            }
        }

        private async Task<bool> MettreAJourDonneesAvecAPI(int id, Medecins medecin)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/Medecins/{id}";

                int nouvelleSpecialiteId = medecin.SpecialiteId;

                medecin.Specialites = null;

              
                medecin.SpecialiteId = nouvelleSpecialiteId;

                using (HttpClient client = new HttpClient())
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    var jsonData = System.Text.Json.JsonSerializer.Serialize(medecin, options);
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
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom_Specialite", Binding = new Binding("Specialites.Libelle") });
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
