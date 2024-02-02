using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp08.Models1;
using WpfApp08.Models3;
using WpfApp08.Models4;
using WpfApp08.Models2;
namespace WpfApp08
{
    public partial class CRUDRendezVous : Window
    {
        public ObservableCollection<RendezVous> RendezVous { get; set; }

        public CRUDRendezVous()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            ChargerRendezVous();
            RendezVous = new ObservableCollection<RendezVous>();
            DataGrid1.ItemsSource = RendezVous;
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_RendezVous PageRendezVous = new Ajouter_RendezVous();
            PageRendezVous.Show();
            this.Close();
        }

        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            RendezVous rendezVousSelectionne = (RendezVous)DataGrid1.SelectedItem;

            if (rendezVousSelectionne != null)
            {
                int id = rendezVousSelectionne.IdRendezVous;

                bool deleteSuccess = await SupprimerDonneesAvecAPI(id);

                if (deleteSuccess)
                {
                    MessageBox.Show("Suppression réussie !");
                    RendezVous.Remove(rendezVousSelectionne);
                }
                else
                {
                    MessageBox.Show("Échec de la suppression.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne à supprimer.");
            }
        }

        private async Task<bool> SupprimerDonneesAvecAPI(int id)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/RendezVous/{id}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDRendezVous PageRendezVous = new CRUDRendezVous();
                    PageRendezVous.Show();
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

        private void Actualiser(object sender, RoutedEventArgs e)
        {
            ChargerRendezVous();
        }

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedRow = e.Row.Item as ajoutRendezVous;
            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }
        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {
            RendezVous rendezVousSelectionne = (RendezVous)DataGrid1.SelectedItem;

            if (rendezVousSelectionne != null)
            {
                try
                {
                    Patients nouveauPatient = rendezVousSelectionne.Patients;
                    Medecins nouveauMedecin = rendezVousSelectionne.Medecins;

                    Patients patientExistant = await VerifierPatientExistant(nouveauPatient);
                    Medecins medecinExistant = await VerifierMedecinExistant(nouveauMedecin);

                    if (patientExistant != null && medecinExistant != null)
                    {
                        rendezVousSelectionne.PatientId = patientExistant.IdPatient;
                        rendezVousSelectionne.MedecinId = medecinExistant.IdMedecin;

                        bool updateSuccess = await MettreAJourDonneesAvecAPI(rendezVousSelectionne.IdRendezVous, rendezVousSelectionne);

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
                        MessageBox.Show("Le patient ou le médecin modifié n'existe pas.");
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

        private async Task<Patients> VerifierPatientExistant(Patients nouveauPatient)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ClinicContext>();
                optionsBuilder.UseSqlServer("Server=DESKTOP-4QTSD8Q;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true;");
                using (var context = new ClinicContext(optionsBuilder.Options))
                {
                    Patients patientExistant = await context.Patients
                        .FirstOrDefaultAsync(p => p.Nom == nouveauPatient.Nom);

                    return patientExistant;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification du patient : {ex.Message}");
                return null;
            }
        }

        private async Task<Medecins> VerifierMedecinExistant(Medecins nouveauMedecin)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ClinicContext>();
                optionsBuilder.UseSqlServer("Server=DESKTOP-4QTSD8Q;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true;");
                using (var context = new ClinicContext(optionsBuilder.Options))
                {
                    Medecins medecinExistant = await context.Medecins
                        .FirstOrDefaultAsync(m => m.Nom == nouveauMedecin.Nom);

                    return medecinExistant;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification du médecin : {ex.Message}");
                return null;
            }
        }

        private async Task<bool> MettreAJourDonneesAvecAPI(int id, RendezVous rendezVous)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/RendezVous/{id}";

                int nouveauPatientId = rendezVous.PatientId;
                int nouveauMedecinId = rendezVous.MedecinId;

                rendezVous.Patients = null;
                rendezVous.Medecins = null;

                rendezVous.PatientId = nouveauPatientId;
                rendezVous.MedecinId = nouveauMedecinId;

                using (HttpClient client = new HttpClient())
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    var jsonData = System.Text.Json.JsonSerializer.Serialize(rendezVous, options);
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


        private async void ChargerRendezVous()
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
                        var rendezVous = JsonConvert.DeserializeObject<List<RendezVous>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = rendezVous;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom Patient", Binding = new Binding("Patients.Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Medecin", Binding = new Binding("Medecins.Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "DateDebut", Binding = new Binding("DateDebut") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "DateFin", Binding = new Binding("DateFin") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Infos", Binding = new Binding("InfosComplementaires") });

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Accueil pageAcceuil = new Accueil();
            pageAcceuil.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
