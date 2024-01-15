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
using System.Windows.Data;
using WpfApp08.Models2;
namespace WpfApp08
{
    public partial class CRUDPatient : Window
    {
        public ObservableCollection<Patients> Patients { get; set; }

        public CRUDPatient()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            ChargerPatients();
            Patients = new ObservableCollection<Patients>();
            DataGrid1.ItemsSource = Patients;
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_Patient PagePatient = new Ajouter_Patient();
            PagePatient.Show();
            this.Close();
        }

        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            Patients patientSelectionne = (Patients)DataGrid1.SelectedItem;

            if (patientSelectionne != null)
            {
                int IdMedecin = patientSelectionne.IdPatient;

                bool deleteSuccess = await SupprimerDonneesAvecAPI(IdMedecin);

                if (deleteSuccess)
                {
                    MessageBox.Show("Suppression réussie !");
                    Patients.Remove(patientSelectionne);
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

        private async Task<bool> SupprimerDonneesAvecAPI(int IdMedecin)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/patients/{IdMedecin}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDPatient PagePatient = new CRUDPatient();
                    PagePatient.Show();
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
            ChargerPatients();
        }

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedRow = e.Row.Item as Patients;
            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {
            Patients patientSelectionne = (Patients)DataGrid1.SelectedItem;

            if (patientSelectionne != null)
            {
                int IdMedecin = patientSelectionne.IdPatient;

                bool updateSuccess = await MettreAJourDonneesAvecAPI(IdMedecin, patientSelectionne);

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

        private async Task<bool> MettreAJourDonneesAvecAPI(int IdMedecin, Patients patient)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/patients/{IdMedecin}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(patient);
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

        private async void ChargerPatients()
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
                        var patients = JsonConvert.DeserializeObject<List<Patients>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = patients;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Prenom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Ville", Binding = new Binding("Ville") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "TelephoneFixe", Binding = new Binding("Telephone_fixe") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "TelephonePortable", Binding = new Binding("Telephone_portable") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Binding("Email") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom_Service", Binding = new Binding("Service_Employe.Nom_Service") });
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
            Administrateur pageAccueil = new Administrateur();
            pageAccueil.Show();
            this.Close();
        }
    }
}
