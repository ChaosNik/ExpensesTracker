using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for Izvještaj.xaml
    /// </summary>
    public partial class GodisnjiIzvjestaj : Window
    {
        private MainWindow main;
        public TreeGridViewItem mjesecni;
        public TreeGridViewItem godisnji;
        public GodisnjiIzvjestaj(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            for (int godina = Util.PocetnaGodina; godina <= DateTime.Now.Year; godina++)
            {
                ComboBoxItem g = new ComboBoxItem();
                g.Content = Util.BrojUTekst(godina, 4);
                g.Tag = godina;
                comboBoxGodina.Items.Add(g);
            }
            comboBoxGodina.SelectedItem = comboBoxGodina.Items[DateTime.Now.Year - Util.PocetnaGodina];
            Refresh();
            //DataContext = godisnji;
        }
        private void Refresh()
        {
            DateTime datum =
                new DateTime
                (
                    (int)(((ComboBoxItem)comboBoxGodina.SelectedItem).Tag),
                    1,
                    1
                );
            mjesecni = new TreeGridViewItem();
            foreach(var mjesec in Util.Mjeseci())
            {
                TreeGridViewItem m = Util.Stablo(x => x.Year == datum.Year && x.Month == mjesec.Item1);
                m.Naziv = Util.BrojUTekst(mjesec.Item1, 2) + ". - " + mjesec.Item2;
                mjesecni.Items.Add(m);
                mjesecni.Prihodi += m.Prihodi;
                mjesecni.Rashodi += m.Rashodi;
            }

            godisnji = Util.Stablo(x => x.Year == datum.Year);
            DataContext = new
            {
                Mjesecni = mjesecni.Items,
                Godisnji = godisnji.Items
            };
            Util.PlusMinusUkupno(textBoxPrihodiGodina, textBoxRashodiGodina, labelUkupnoGodina, godisnji.Prihodi, godisnji.Rashodi);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }

        private void ComboBoxGodina_DropDownClosed(object sender, EventArgs e)
        {
            Refresh();
        }
        private void ButtonStampajGodisnjiUkupni_Click(object sender, RoutedEventArgs e)
        {
            string naziv = "Izvještaj za " + ((ComboBoxItem)comboBoxGodina.SelectedItem).Content.ToString() + ". godinu";
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(mjesecni, 1), naziv), naziv + ".pdf");
        }
        private void ButtonStampajGodisnjiDetaljni_Click(object sender, RoutedEventArgs e)
        {
            string naziv = "Detaljni izvještaj za " + ((ComboBoxItem)comboBoxGodina.SelectedItem).Content.ToString() + ". godinu";
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(godisnji, 2), naziv), naziv + ".pdf");
        }
    }
}