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
            ChargerSalaries();
            Patients = new ObservableCollection<Patients>();
            DataGrid1.ItemsSource = Patients;
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_Salarie Pagesite = new Ajouter_Salarie();
            Pagesite.Show();
            this.Close();
        }

            
       
        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            // pour obtenir la ligne sélectionnée dans le DataGrid
            Patients siteSelectionne = (Patients)DataGrid1.SelectedItem;

            if (siteSelectionne != null)
            {

                int salariesId = siteSelectionne.IdPatient;


                bool deleteSuccess =  await SupprimerDonneesAvecAPI(salariesId);

                if (deleteSuccess)
                {

                    MessageBox.Show("Suppression réussie !");

                    Patients.Remove(siteSelectionne);
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

        private async Task<bool> SupprimerDonneesAvecAPI(int salariesId)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/salaries/{salariesId}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDPatient Pagesite = new CRUDPatient();
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
        private void Actualiser(object sender, RoutedEventArgs e)
        {
            ChargerSalaries();
        }

        private void DataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            var editedRow = e.Row.Item as Patients;


            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {

            Patients SalariesSelectionne = (Patients)DataGrid1.SelectedItem;

            if (SalariesSelectionne != null)
            {

                int IdPatient = SalariesSelectionne.IdPatient;


                bool updateSuccess = await MettreAJourDonneesAvecAPI(IdPatient, SalariesSelectionne);

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


        private async Task<bool> MettreAJourDonneesAvecAPI(int IDSalaries, Patients Salaries)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/salaries/{IDSalaries}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(Salaries);
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



        private async void ChargerSalaries()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/salaries";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var salaries = JsonConvert.DeserializeObject<List<Patients>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = salaries;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Prenom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Ville", Binding = new Binding("Sites.Ville") });
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
            Administrateur pageAcceuil = new Administrateur();
            pageAcceuil.Show();
            this.Close();
        }
    }
}
