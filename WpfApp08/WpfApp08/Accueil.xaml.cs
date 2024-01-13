
using System.Windows;

namespace WpfApp08
{
    public partial class Accueil : Window
    {
        public Accueil()
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
        private void Button_patients(object sender, RoutedEventArgs e)
        {
            CRUDPatient PageAcceuil = new CRUDPatient();
            PageAcceuil.Show();
            this.Close();
        }
        private void Button_rendez_vous(object sender, RoutedEventArgs e)
        {
            CRUDRendezVous Pagesite = new CRUDRendezVous();
            Pagesite.Show();
            this.Close();
        }

       
    }
}