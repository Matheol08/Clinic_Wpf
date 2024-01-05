using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;


//namespace WPF_Annuaire
//{
//    public partial class MainWindow : Window
//    {

//        public MainWindow()
//        {
//            //InitializeComponent();

//            // Obtenez les données de la base de données
//            List<Personne> personnes = GetPersonnesFromDatabase();

            
//        }
//        public List<Personne> GetPersonnesFromDatabase()
//        {
//            List<Personne> personnes = new List<Personne>();

//            // Chaîne de connexion à votre base de données
//            string connectionString = "Server=.\\SQLExpress;Database=Annuaire;Trusted_Connection=True;TrustServerCertificate=true";


//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                string sqlQuery = "SELECT * FROM Salaries";

//                connection.Open();

//                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
//                {
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Personne personne = new Personne
//                            {
//                                Nom = reader["Nom"].ToString(),
//                                Prenom = reader["Prenom"].ToString(),
//                                Telephone_fixe = reader["Telephone_fixe"].ToString(),
//                                Telephone_portable = reader["Telephone_portable"].ToString(),
//                                Email = reader["Email"].ToString(),
//                                IDService = reader["IDService"].ToString(),
//                                IDSite = reader["IDSite"].ToString()
//                            };

//                            personnes.Add(personne);


//                        }
//                    }
//                }
//            }

//            return personnes;
//        }

//        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//        {

//        }

//        //private void RechercherButton_Click(object sender, RoutedEventArgs e)
//        //{

//        //}
//    }

    //public string ConvertIDSite(string IDSite)
    //{
    //    // Logique de conversion pour afficher une autre valeur que 1
    //    if (IDSite == "1")
    //    {
    //        return "AutreValeurQue1";
    //    }

    //    return "HGHJ"; // Retourne la valeur d'origine si elle n'est pas égale à 1
    //}



    // Classe de modèle de données pour une personne

//}



