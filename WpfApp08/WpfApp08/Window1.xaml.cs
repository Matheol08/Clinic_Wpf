﻿
using System.Windows;
using System.Windows.Input;
namespace WpfApp08
{
        public partial class Window1 : Window
        {
          public Window1()
          {
                InitializeComponent();
            // Centrer la fenêtre
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            
            ResizeMode = ResizeMode.NoResize;

            PreviewKeyDown += MainWindow_PreviewKeyDown;
            Loaded += (sender, e) => Focus();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)//raccourci admin
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.A)
            {
                Administrateur nouvelleFenetre = new Administrateur(); //remettre la fenetre pour le mot de passe

                nouvelleFenetre.Show();
                this.Close();
            }
        }
        private void OuvrirNouvelleFenetre_Click(object sender, RoutedEventArgs e)
        {
            Utilisateur nouvelleFenetre = new Utilisateur();

            nouvelleFenetre.Show();
            this.Close();
        }
        private void rdv(object sender, RoutedEventArgs e)
        {
            Acceuil nouvelleFenetre = new Acceuil();

            nouvelleFenetre.Show();
            this.Close();
        }

    }

}