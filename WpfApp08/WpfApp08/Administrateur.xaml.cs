
using System.Windows;

namespace WpfApp08
{
    public partial class Administrateur : Window
    {
        public Administrateur()
        {
            InitializeComponent();
            // Centrer la fenêtre
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Rendre la fenêtre non redimensionnable
            ResizeMode = ResizeMode.NoResize;
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 PageAcceuil = new Window1();
            PageAcceuil.Show();
            this.Close();
        }
        private void Button_site(object sender, RoutedEventArgs e)
        {
            CRUDSpecialite Pagesite = new CRUDSpecialite();
            Pagesite.Show();
            this.Close();
        }
        private void Button_service(object sender, RoutedEventArgs e)
        {
            CRUDMedecin Pagesite = new CRUDMedecin();
            Pagesite.Show();
            this.Close();
        }
        private void Button_utilisateur(object sender, RoutedEventArgs e)
        {
            CRUDRendezVous PageEmploye = new CRUDRendezVous();
            PageEmploye.Show();
            this.Close();
        }

       
    }
}