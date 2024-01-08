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
using WpfApp08.Models3;
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
            ChargerSalaries();
            RendezVous = new ObservableCollection<RendezVous>();
            DataGrid1.ItemsSource = RendezVous;
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_RendezVous Pagesite = new Ajouter_RendezVous();
            Pagesite.Show();
            this.Close();
        }

            
       
        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            // pour obtenir la ligne sélectionnée dans le DataGrid
            RendezVous siteSelectionne = (RendezVous)DataGrid1.SelectedItem;

            if (siteSelectionne != null)
            {

                int IdRendezVous = siteSelectionne.IdRendezVous;


                bool deleteSuccess =  await SupprimerDonneesAvecAPI(IdRendezVous);

                if (deleteSuccess)
                {

                    MessageBox.Show("Suppression réussie !");

                    RendezVous.Remove(siteSelectionne);
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

        private async Task<bool> SupprimerDonneesAvecAPI(int IdRendezVous)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/salaries/{IdRendezVous}";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(apiUrl);
                    CRUDRendezVous Pagesite = new CRUDRendezVous();
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

            var editedRow = e.Row.Item as RendezVous;


            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {

            RendezVous RDVSelectionne = (RendezVous)DataGrid1.SelectedItem;

            if (RDVSelectionne != null)
            {

                int IdRendezVous = RDVSelectionne.IdRendezVous;


                bool updateSuccess = await MettreAJourDonneesAvecAPI(IdRendezVous, RDVSelectionne);

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


        private async Task<bool> MettreAJourDonneesAvecAPI(int IDSalaries, RendezVous RendezVous)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/salaries/{IDSalaries}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(RendezVous);
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
                        var salaries = JsonConvert.DeserializeObject<List<RendezVous>>(json);

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
