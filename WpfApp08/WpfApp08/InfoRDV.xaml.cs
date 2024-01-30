using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp08.Models3;

namespace WpfApp08
{
  
    public partial class InfoRDV : Window
    {
        public InfoRDV(RendezVous selectedRendezVous)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            Text1.Text = selectedRendezVous.InfosComplementaires;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur pageAcceuil = new Utilisateur();
            pageAcceuil.Show();
            this.Close();
        }

    }
}
