using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for MjesecniIzvjestaj.xaml
    /// </summary>
    public partial class MjesecniIzvjestaj : Window
    {
        private MainWindow main;
        public TreeGridViewItem mjesecni;
        public MjesecniIzvjestaj(MainWindow main)
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
            foreach(var m in Util.Mjeseci())
            {
                ComboBoxItem g = new ComboBoxItem();
                g.Content = Util.BrojUTekst(m.Item1, 2) + " - " + m.Item2;
                g.Tag = m;
                comboBoxMjesec.Items.Add(g);
            }
            comboBoxGodina.SelectedItem = comboBoxGodina.Items[DateTime.Now.Year - Util.PocetnaGodina];
            comboBoxMjesec.SelectedItem = comboBoxMjesec.Items[DateTime.Now.Month - 1];
            Refresh();
            //DataContext = godisnji;
        }
        private void Refresh()
        {
            DateTime datum =
                new DateTime
                (
                    (int)(((ComboBoxItem)comboBoxGodina.SelectedItem).Tag),
                    ((Tuple<int, string>)(((ComboBoxItem)comboBoxMjesec.SelectedItem).Tag)).Item1,
                    1
                );
            mjesecni = Util.Stablo(x => x.Year == datum.Year && x.Month == datum.Month);
            DataContext = new
            {
                Mjesecni = mjesecni.Items
            };
            Util.PlusMinusUkupno(textBoxPrihodiGodina, textBoxRashodiGodina, labelUkupnoGodina, mjesecni.Prihodi, mjesecni.Rashodi);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }
        private void ComboBoxGodina_DropDownClosed(object sender, EventArgs e)
        {
            Refresh();
        }
        private void ButtonStampajMjesecniUkupni_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int, string> n = (Tuple<int, string>)(((ComboBoxItem)comboBoxMjesec.SelectedItem).Tag);
            string naziv = "Izvještaj za " +
                n.Item2 + " (" + n.Item1 + ". mjesec) " +
                ((ComboBoxItem)comboBoxGodina.SelectedItem).Content.ToString() + ". godine";
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(mjesecni, 1), naziv), naziv + ".pdf");
        }
        private void ButtonStampajMjesecniDetaljni_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int, string> n = (Tuple<int, string>)(((ComboBoxItem)comboBoxMjesec.SelectedItem).Tag);
            string naziv = "Detaljni izvještaj za " +
                n.Item2 + " (" + n.Item1 + ". mjesec) " +
                ((ComboBoxItem)comboBoxGodina.SelectedItem).Content.ToString() + ". godine";
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(mjesecni, 2), naziv), naziv + ".pdf");
        }
    }
}
