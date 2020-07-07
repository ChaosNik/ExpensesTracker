using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for IntervalskiIzvjestaj.xaml
    /// </summary>
    public partial class IntervalskiIzvjestaj : Window
    {

        private MainWindow main;
        public TreeGridViewItem mjesecni;
        public TreeGridViewItem godisnji;
        private DateTime datumOd;
        private DateTime datumDo;
        public IntervalskiIzvjestaj(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            datePickerOd.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            datePickerDo.SelectedDate = DateTime.Now;
            
            Refresh();
            //DataContext = godisnji;
        }
        private void Refresh()
        {
            datumOd = datePickerOd.SelectedDate.Value;
            datumDo = datePickerDo.SelectedDate.Value;
            mjesecni = new TreeGridViewItem();
            foreach (var mjesec in Util.Mjeseci())
            {
                TreeGridViewItem m = Util.Stablo(x => x.CompareTo(datumOd) == 1 && x.CompareTo(datumDo) == -1 && x.Month == mjesec.Item1);
                m.Naziv = Util.BrojUTekst(mjesec.Item1, 2) + ". - " + mjesec.Item2;
                mjesecni.Items.Add(m);
                mjesecni.Prihodi += m.Prihodi;
                mjesecni.Rashodi += m.Rashodi;
            }
            godisnji = Util.Stablo(x => x.CompareTo(datumOd) == 1 && x.CompareTo(datumDo) == -1);
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
        private void ButtonStampajGodisnjiUkupni_Click(object sender, RoutedEventArgs e)
        {
            string naziv = "Izvještaj za period od " + Util.DateTimeToString(datumOd) + " do " + Util.DateTimeToString(datumDo);
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(mjesecni, 1), naziv), naziv + ".pdf");
        }
        private void ButtonStampajGodisnjiDetaljni_Click(object sender, RoutedEventArgs e)
        {
            string naziv = "Detaljni izvještaj za period od " + Util.DateTimeToString(datumOd) + " do " + Util.DateTimeToString(datumDo);
            Util.PrintPDF(Util.NapraviPDF(Util.OgraniciStablo(godisnji, 2), naziv), naziv + ".pdf");
        }
        private void DatePickerOd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void DatePickerDo_CalendarClosed(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
