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
                int IdRendezVous = rendezVousSelectionne.IdRendezVous;

                bool deleteSuccess = await SupprimerDonneesAvecAPI(IdRendezVous);

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

        private async Task<bool> SupprimerDonneesAvecAPI(int IdRendezVous)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/rendezvous/{IdRendezVous}";

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
            var editedRow = e.Row.Item as RendezVous;
            var nouvelleValeur = (e.EditingElement as TextBox).Text;
        }

        private async void MAJ_Click(object sender, RoutedEventArgs e)
        {
            RendezVous rendezVousSelectionne = (RendezVous)DataGrid1.SelectedItem;

            if (rendezVousSelectionne != null)
            {
                int IdRendezVous = rendezVousSelectionne.IdRendezVous;

                bool updateSuccess = await MettreAJourDonneesAvecAPI(IdRendezVous, rendezVousSelectionne);

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

        private async Task<bool> MettreAJourDonneesAvecAPI(int IdRendezVous, RendezVous rendezVous)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/rendezvous/{IdRendezVous}";

                using (HttpClient client = new HttpClient())
                {
                    var jsonData = JsonConvert.SerializeObject(rendezVous);
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
                    string apiUrl = "https://localhost:7152/api/rendezvous";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var rendezVous = JsonConvert.DeserializeObject<List<RendezVous>>(json);

                        DataGrid1.Columns.Clear();

                        DataGrid1.ItemsSource = rendezVous;
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Nom", Binding = new Binding("Patients.Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Prenom", Binding = new Binding("Patients.Prenom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Medecin", Binding = new Binding("Medecins.Nom") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "DateDebut", Binding = new Binding("DateDebut") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "DateFin", Binding = new Binding("DateFin") });
                        DataGrid1.Columns.Add(new DataGridTextColumn { Header = "Infos", Binding = new Binding("InfoComplementaire") });

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
