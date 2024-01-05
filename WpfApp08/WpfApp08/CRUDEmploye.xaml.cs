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

namespace WpfApp08
{

    public partial class CRUDEmploye : Window
    {
        public ObservableCollection<Salaries> Salaries { get; set; }
        public CRUDEmploye()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            ResizeMode = ResizeMode.NoResize;
            ChargerSalaries();
            Salaries = new ObservableCollection<Salaries>();
            DataGrid1.ItemsSource = Salaries;
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
            Salaries siteSelectionne = (Salaries)DataGrid1.SelectedItem;

            if (siteSelectionne != null)
            {

                int salariesId = siteSelectionne.IDSalaries;


                bool deleteSuccess =  await SupprimerDonneesAvecAPI(salariesId);

                if (deleteSuccess)
                {

                    MessageBox.Show("Suppression réussie !");

                    Salaries.Remove(siteSelectionne);
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
                    CRUDEmploye Pagesite = new CRUDEmploye();
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

            var editedRow = e.Row.Item as Salaries;


            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {

            Salaries SalariesSelectionne = (Salaries)DataGrid1.SelectedItem;

            if (SalariesSelectionne != null)
            {

                int IDSalaries = SalariesSelectionne.IDSalaries;


                bool updateSuccess = await MettreAJourDonneesAvecAPI(IDSalaries, SalariesSelectionne);

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


        private async Task<bool> MettreAJourDonneesAvecAPI(int IDSalaries, Salaries Salaries)
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
                        var salaries = JsonConvert.DeserializeObject<List<Salaries>>(json);

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
