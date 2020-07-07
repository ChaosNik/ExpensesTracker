using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for Pretrazivanje.xaml
    /// </summary>
    public partial class Pretrazivanje : Window
    {
        private MainWindow main;
        public List<Tuple<string, string, string, string, string>> tabela = new List<Tuple<string, string, string, string, string>>();

        public Pretrazivanje(MainWindow main)
        {
            InitializeComponent();
            this.main = main;

            datePickerOd.SelectedDate = DateTime.Now;
            datePickerDo.SelectedDate = DateTime.Now;

            ComboBoxItem x = new ComboBoxItem();
            x.Content = "Sve";
            comboBoxKategorija.Items.Add(x);
            foreach (var kat in Util.Context.Kategorija)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = kat.Naziv;
                item.Tag = kat;
                comboBoxKategorija.Items.Add(item);
            }
            comboBoxKategorija.SelectedItem = comboBoxKategorija.Items[0];
            comboBoxPotkategorija.Items.Add("Sve");
            comboBoxPotkategorija.SelectedIndex = 0;

            DataContext = new { Tabela = tabela };
            Refresh();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }
        private void Refresh()
        {
            List<Unos> unosi = Util.Context.Unos.ToList<Unos>();
            unosi.Sort((x, y) => x.DatumVrijeme.CompareTo(y.DatumVrijeme));
            List<Unos> rezultat = new List<Unos>();
            foreach(var unos in unosi)
            {
                bool add = true;

                /**Način odabira datuma**/
                if("Dan".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                {
                    DateTime datumOd = datePickerOd.SelectedDate.Value;
                    if (unos.DatumVrijeme.CompareTo(datumOd) != 0) add = false;
                }
                else if("Mjesec".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                {
                    DateTime datumOd = datePickerOd.SelectedDate.Value;
                    if (unos.DatumVrijeme.Month!=datumOd.Month) add = false;
                }
                else if ("Godina".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                {
                    DateTime datumOd = datePickerOd.SelectedDate.Value;
                    if (unos.DatumVrijeme.Year != datumOd.Year) add = false;
                }
                else if ("Interval".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                {
                    DateTime datumOd = datePickerOd.SelectedDate.Value;
                    DateTime datumDo = datePickerDo.SelectedDate.Value;
                    if (unos.DatumVrijeme.CompareTo(datumOd) == -1) add = false;
                    if (unos.DatumVrijeme.CompareTo(datumDo) == 1) add = false;
                }

                /**Odabir kategorije i potkategorije**/
                if (comboBoxKategorija.Items.Count > 0)
                {
                    if (!"Sve".Equals(((ComboBoxItem)comboBoxKategorija.SelectedItem).Content))
                    {
                        var kategorija = (Kategorija)((ComboBoxItem)comboBoxKategorija.SelectedItem).Tag;
                        if (comboBoxPotkategorija.Items.Count > 0)
                        {
                            if ("Sve".Equals(((ComboBoxItem)comboBoxPotkategorija.SelectedItem).Content))
                            {
                                var potkategorija = (Potkategorija)((ComboBoxItem)comboBoxPotkategorija.SelectedItem).Tag;
                                if (!kategorija.IdKategorije.Equals(unos.Potkategorija.IdKategorije)) add = false;
                            }
                            else
                            {
                                var potkategorija = (Potkategorija)((ComboBoxItem)comboBoxPotkategorija.SelectedItem).Tag;
                                if (!potkategorija.IdPotkategorije.Equals(unos.Potkategorija.IdPotkategorije)) add = false;
                            }
                        }
                    }
                }

                /**Odabir opisa**/
                if (!unos.Opis.ToLower().Contains((textBoxOpis.Text.ToLower()))) add = false;

                /**Vrijednost**/
                if (textBoxIznosOd != null && textBoxIznosDo != null)
                {
                    if ("Vrijednost".Equals(((ComboBoxItem)comboBoxCijenaInterval.SelectedItem).Content))
                    {
                        double iznos = Math.Abs(double.Parse(textBoxIznosOd.Text));
                        if ((iznos != 0.00) && ((iznos - unos.Iznos) > Util.delta)) add = false;
                    }
                    else if ("Interval".Equals(((ComboBoxItem)comboBoxCijenaInterval.SelectedItem).Content))
                    {
                        double iznosOd = double.Parse(textBoxIznosOd.Text);
                        double iznosDo = double.Parse(textBoxIznosDo.Text);
                        if (iznosDo != 0.00)
                        {
                            if (iznosOd > unos.Iznos) add = false;
                            else if (iznosDo < unos.Iznos) add = false;
                        }
                    }

                    /**Vrsta vrijednosti**/
                    if("Prihodi".Equals(((ComboBoxItem)comboBoxPrihodiRashodi.SelectedItem).Content))
                    {
                        if (Util.minus.Equals(unos.Vrsta)) add = false;
                    }
                    else if ("Rashodi".Equals(((ComboBoxItem)comboBoxPrihodiRashodi.SelectedItem).Content))
                    {
                        if (Util.plus.Equals(unos.Vrsta)) add = false;
                    }
                }

                if (add) rezultat.Add(unos);
            }

            tabela = new List<Tuple<string, string, string, string, string>>();
            foreach (var unos in rezultat)
                tabela.Add
                    (
                        new Tuple<string, string, string, string, string>
                        (
                            Util.DateTimeToString(unos.DatumVrijeme),
                            unos.Potkategorija.Kategorija.Naziv,
                            unos.Potkategorija.Naziv,
                            Util.BrojUTekst(Util.plus.Equals(unos.Vrsta) ? unos.Iznos : (-unos.Iznos)),
                            unos.Opis
                        )
                    );
            DataContext = new { Tabela = tabela };
        }
        private void ComboBoxVrstaDatuma_DropDownClosed(object sender, EventArgs e)
        {
            if ("Dan".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                datePickerOd.SelectedDate = DateTime.Now;
            else if ("Mjesec".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                datePickerOd.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            else if ("Godina".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
                datePickerOd.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            else if ("Interval".Equals(((ComboBoxItem)(comboBoxVrstaDatuma.SelectedItem)).Content))
            {
                datePickerOd.SelectedDate = new DateTime(DateTime.Now.Year,1,1);
                datePickerDo.SelectedDate = DateTime.Now;
            }
            Refresh();
        }
        private void DatePickerOd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void DatePickerDo_CalendarClosed(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void ComboBoxKategorija_DropDownCLosed(object sender, SelectionChangedEventArgs e)
        {
            comboBoxPotkategorija.Items.Clear();
            ComboBoxItem cb = new ComboBoxItem();
            cb.Content = "Sve";
            comboBoxPotkategorija.Items.Add(cb);
            if (!"Sve".Equals(Util.SelectedComboBoxItemToString(comboBoxKategorija)))
            {
                foreach (var pot in ((Kategorija)(((ComboBoxItem)(comboBoxKategorija.SelectedItem)).Tag)).Potkategorija.ToList())
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = pot.Naziv;
                    item.Tag = pot;
                    comboBoxPotkategorija.Items.Add(item);
                }
            }
            comboBoxPotkategorija.SelectedItem = comboBoxPotkategorija.Items[0];
            Refresh();
        }
        private void ComboBoxPotkategorija_DropDownCLosed(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
        private void ComboBoxCijenaInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
        private void ComboBoxPrihodiRashodi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
        private void TextBoxOpis_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }
        private void ComboBoxCijenaInterval_DropDownClosed(object sender, EventArgs e)
        {
            Refresh();
        }
        private void ComboBoxPrihodiRashodi_DropDownClosed(object sender, EventArgs e)
        {
            Refresh();
        }
        private void TextBoxIznosOd_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = textBoxIznosOd.Text.Replace(",", ".");
            if ("".Equals(text)) textBoxIznosOd.Text = "0.00";
            double outparam;
            bool success = double.TryParse(text, out outparam);
            if (success)
            {
                Refresh();
            }
            else
            {
                MessageBox.Show("Ponovo unesite iznos!", "Broj nije dobro napisan!", MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxIznosOd.Text = "0.00";
            }
        }
        private void TextBoxIznosDo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = textBoxIznosDo.Text.Replace(",", ".");
            if ("".Equals(text)) textBoxIznosDo.Text = "0.00";
            double outparam;
            bool success = double.TryParse(text, out outparam);
            if (success)
            {
                Refresh();
            }
            else
            {
                MessageBox.Show("Ponovo unesite iznos!", "Broj nije dobro napisan!", MessageBoxButton.OK, MessageBoxImage.Error);
                textBoxIznosDo.Text = "0.00";
            }
        }
        private void DataGridUnosiPrihodi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Refresh();
        }
    }
}
