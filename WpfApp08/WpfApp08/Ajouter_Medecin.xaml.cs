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
using WpfApp08.Models1;
using WpfApp08.Models4;

namespace WpfApp08
{
    public partial class Ajouter_Medecin : Window
    {
        public ObservableCollection<Medecins> Medecins { get; set; }
        public Ajouter_Medecin()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            ChargerLesSpe();
            //Medecins = new ObservableCollection<Ajouter_Medecin>();
        }
        private async void ChargerLesSpe()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/specialite";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Specialite = JsonConvert.DeserializeObject<Specialites[]>(json);

                        
                        Combo1.ItemsSource = Specialite;
                        Combo1.DisplayMemberPath = "Specialité";
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


        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Text1.Text) ||
                    string.IsNullOrEmpty(Text2.Text) ||
                    Combo1.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs requis.");
                }
                else
                {
                    int SpecialiteId = ((Specialites)Combo1.SelectedItem).IdSpecialite;

                    Medecins nouveauMedecin = new Medecins
                    {
                        Nom = Text1.Text,
                        Prenom = Text2.Text,
                        SpecialiteId = SpecialiteId,
                    };

                    bool updateSuccess = await EnvoyerDonneesAvecAPI(nouveauMedecin);

                    if (updateSuccess)
                    {
                        MessageBox.Show("Mise à jour réussie !");
                        CRUDMedecin pageAcceuil = new CRUDMedecin();
                        pageAcceuil.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Échec de la mise à jour.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }

        private async Task<bool> EnvoyerDonneesAvecAPI(Medecins nouveauMedecin)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/medecin";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(nouveauMedecin);
                Console.WriteLine($"JSON Data: {jsonData}");

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("API call succeeded.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"API call failed. Status Code: {response.StatusCode}");

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during API call: {ex.Message}");
                MessageBox.Show($"Error during API call: {ex.Message}");
                return false;
            }
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CRUDMedecin pageAcceuil = new CRUDMedecin();
            pageAcceuil.Show();
            this.Close();
        }

        
    }
}
