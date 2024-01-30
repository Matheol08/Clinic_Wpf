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
using WpfApp08.Models2;
using WpfApp08.Models3;


namespace WpfApp08
{
    public partial class Ajouter_RendezVous : Window
    {
        public ObservableCollection<RendezVous> RendezVous { get; set; }
        public Ajouter_RendezVous()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            ChargerLesMedecins();
            ChargerLesPatients();
            RendezVous = new ObservableCollection<RendezVous>();
        }
        private async void ChargerLesPatients()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7152/api/Patients";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var Patients = JsonConvert.DeserializeObject<Patients[]>(json);

                        
                        Combo2.ItemsSource = Patients;
                        Combo2.DisplayMemberPath = "Nom";
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
                        var Medecins = JsonConvert.DeserializeObject<Medecins[]>(json);

                        Combo1.ItemsSource = Medecins;
                        Combo1.DisplayMemberPath = "Nom";
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
                if (string.IsNullOrEmpty(Combo1.Text) ||
                    string.IsNullOrEmpty(Combo2.Text) ||
                    Combo1.SelectedItem == null ||
                    Combo2.SelectedItem == null ||
                    string.IsNullOrEmpty(date1.Text) ||
                    string.IsNullOrEmpty(date2.Text) ||
                    string.IsNullOrEmpty(Text1.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs requis.");
                }
                else
                {
                    int IdPatient = ((Patients)Combo2.SelectedItem).IdPatient;
                    int MedecinId = ((Medecins)Combo1.SelectedItem).IdMedecin;

                    DateTime startDateTime = DateTime.Parse(date1.Text);
                    DateTime endDateTime = DateTime.Parse(date2.Text);

                    bool isDoctorAvailable = await CheckExistingAppointments(MedecinId, startDateTime, endDateTime);

                    if (!isDoctorAvailable)
                    {
                        MessageBox.Show("Le médecin est déjà en rendez-vous pendant ces horaires.");
                        return;
                    }

                    RendezVous nouveau_RendezVous = new RendezVous
                    {
                        IdPatient = IdPatient,
                        MedecinId = MedecinId,
                        DateDebut = startDateTime,
                        DateFin = endDateTime,
                        InfosComplementaires = Text1.Text
                    };

                    bool updateSuccess = await EnvoyerDonneesAvecAPI(nouveau_RendezVous);

                    if (updateSuccess)
                    {
                        MessageBox.Show("Mise à jour réussie !");
                        CRUDRendezVous pageAcceuil = new CRUDRendezVous();
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


        private async Task<bool> CheckExistingAppointments(int medecinId, DateTime startDateTime, DateTime endDateTime)
        {
            try
            {
                string apiUrl = $"https://localhost:7152/api/RendezVous/MedecinEnRendezVous?medecinId={medecinId}&startDateTime={startDateTime}&endDateTime={endDateTime}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        bool isAvailable = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
                        return isAvailable;
                    }
                    else
                    {
                        Console.WriteLine($"Erreur de requête API : {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
                return false;
            }
        }





        private async Task<bool> EnvoyerDonneesAvecAPI(RendezVous nouveau_RendezVous)
        {
            try
            {
                string apiUrl = "https://localhost:7152/api/RendezVous";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(nouveau_RendezVous);
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
            CRUDRendezVous pageAcceuil = new CRUDRendezVous();
            pageAcceuil.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
