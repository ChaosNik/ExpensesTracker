using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Resources;
using System.Windows.Media.Imaging;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isMinus;
        public List<Unos> tabelaPrihodi = new List<Unos>();
        public List<Unos> tabelaRashodi = new List<Unos>();
        public MainWindow()
        {
            InitializeComponent();
            Util.AddIconToButton(buttonPlusMinus, "Minus", 85);
            isMinus = true;
            //rectanglePlus.Visibility = Visibility.Hidden;
            datePickerDatum.SelectedDate = DateTime.Now;

            labelDanUkupno.Background = Brushes.DarkGray;
            labelMjesecUkupno.Background = Brushes.DarkGray;
            labelGodinaUkupno.Background = Brushes.DarkGray;
            RefreshText();
            Kategorije();
        }
        
        private void RefreshText()
        {
            List<Unos> unosi = Util.Context.Unos.ToList<Unos>();
            MjeseciGodisnjiObracun(unosi);
            PrihodiRashodi(unosi);
        }

        private void Kategorije()
        {            
            List<Kategorija> kategorije = Util.Context.Kategorija.ToList<Kategorija>();
            kategorije.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));
            foreach (Kategorija x in kategorije)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = x.Naziv;
                item.Tag = x;
                comboBoxKategorija.Items.Add(item);
            }
            List<Potkategorija> potkategorije = Util.Context.Potkategorija.ToList<Potkategorija>();
            potkategorije.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));
            foreach (Potkategorija x in potkategorije)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = x.Naziv;
                item.Tag = x;
                comboBoxPotkategorija.Items.Add(item);
            }

        }

        private void MjeseciGodisnjiObracun(List<Unos> unosi)
        {
            double sumMjesecPlus = 0;
            double sumMjesecMinus = 0;
            double sumGodinaPlus = 0;
            double sumGodinaMinus = 0;
            DateTime selectedDate = datePickerDatum.SelectedDate.Value.Date;
            
            /**Izracunavanje za mjesec i za gorinu**/
            foreach (Unos x in unosi)
            {
                DateTime datum = x.DatumVrijeme;
                if (selectedDate.Year == datum.Year)
                {
                    if (selectedDate.Month == datum.Month &&
                        selectedDate.Day >= datum.Day)
                    {
                        if (Util.plus.Equals(x.Vrsta)) sumGodinaPlus += x.Iznos;
                        else sumGodinaMinus += x.Iznos;

                        if (Util.plus.Equals(x.Vrsta)) sumMjesecPlus += x.Iznos;
                        else sumMjesecMinus += x.Iznos;
                    }
                    else if (selectedDate.Month > datum.Month)
                    {
                        if (Util.plus.Equals(x.Vrsta)) sumGodinaPlus += x.Iznos;
                        else sumGodinaMinus += x.Iznos;
                    }
                }
            }

            /**Suma za sve dane u mjesecu**/
            List<Tuple<string, double, double>> mjesecNumbers =
                new List<Tuple<string, double, double>>();
            for (int i = 0; i < selectedDate.Day; ++i) mjesecNumbers.Add(new Tuple<string, double, double>((i + 1) + ".", 0d, 0d));
            foreach (Unos x in unosi)
            {
                DateTime datum = x.DatumVrijeme;
                if (selectedDate.Year == datum.Year &&
                    selectedDate.Month == datum.Month &&
                    selectedDate.Day >= datum.Day)
                {
                    mjesecNumbers[datum.Day - 1] =
                        new Tuple<string, double, double>
                        (
                            x.DatumVrijeme.Day + ".",
                            Util.plus.Equals(x.Vrsta) ? mjesecNumbers[datum.Day - 1].Item2 + (double)x.Iznos : mjesecNumbers[datum.Day - 1].Item2,
                            Util.minus.Equals(x.Vrsta) ? mjesecNumbers[datum.Day - 1].Item3 + (double)x.Iznos : mjesecNumbers[datum.Day - 1].Item3
                        );
                }
            }
            List<Tuple<string, string, string>> mjesec =
                new List<Tuple<string, string, string>>();
            foreach (var x in mjesecNumbers)
                mjesec.Add
                (
                    new Tuple<string, string, string>
                    (
                        x.Item1,
                        Util.BrojUTekst(x.Item2),
                        Util.BrojUTekst(x.Item3)
                    )
                );

            /**Povazivanje sa WPF-om**/
            dataGridMjesec.ItemsSource = mjesec;
            dataGridMjesec.ScrollIntoView(dataGridMjesec.Items.GetItemAt(mjesec.Count - 1));

            Util.PlusMinusUkupno(textBoxMjesecPlus, textBoxMjesecMinus, labelMjesecUkupno, sumMjesecPlus, sumMjesecMinus);
            Util.PlusMinusUkupno(textBoxGodinaPlus, textBoxGodinaMinus, labelGodinaUkupno, sumGodinaPlus, sumGodinaMinus);
        }
        private void PrihodiRashodi(List<Unos> unosi)
        {
            tabelaPrihodi = new List<Unos>();
            tabelaRashodi = new List<Unos>();
            double dnevniPrihodi = 0;
            double dnevniRashodi = 0;
            foreach (Unos x in unosi)
            {
                DateTime datum = x.DatumVrijeme;
                if (datePickerDatum.SelectedDate.Value.Date.Equals(x.DatumVrijeme.Date))
                {
                    if (Util.plus.Equals(x.Vrsta))
                    {
                        dnevniPrihodi += x.Iznos;
                        tabelaPrihodi.Add(x);
                    }
                    else
                    {
                        dnevniRashodi += x.Iznos;
                        tabelaRashodi.Add(x);
                    }
                }
                Util.PlusMinusUkupno(textBoxDanPlus, textBoxDanMinus, labelDanUkupno, dnevniPrihodi, dnevniRashodi);
            }
            dataGridUnosiPrihodi.ItemsSource = tabelaPrihodi;
            if (tabelaPrihodi.Count != 0)
                dataGridUnosiPrihodi.ScrollIntoView(dataGridUnosiPrihodi.Items.GetItemAt(tabelaPrihodi.Count - 1));

            dataGridUnosiRashodi.ItemsSource = tabelaRashodi;
            if (tabelaRashodi.Count != 0)
                dataGridUnosiRashodi.ScrollIntoView(dataGridUnosiRashodi.Items.GetItemAt(tabelaRashodi.Count - 1));
        }

        private void BrisanjeUnosaIzDataGrida(DataGrid dg)
        {
            Unos izabrano = (Unos)dg.SelectedItem;
            Util.Context.Unos.Remove(izabrano);
            Util.Context.SaveChanges();

            RefreshText();
        }

        private void ComboBoxKategorija_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxKategorija.SelectedItem == null)
                comboBoxKategorija.SelectedItem = comboBoxKategorija.Items[0];
            comboBoxPotkategorija.Items.Clear();
            List<Potkategorija> potkategorije = Util.Context.Potkategorija.ToList<Potkategorija>();
            foreach (Potkategorija x in potkategorije)
                if(((Kategorija)((ComboBoxItem)comboBoxKategorija.SelectedItem).Tag).IdKategorije.Equals(x.IdKategorije))
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = x.Naziv;
                    item.Tag = x;
                    comboBoxPotkategorija.Items.Add(item);
                }
            comboBoxPotkategorija.IsEnabled = true;
        }
        private void ComboBoxPotkategorija_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxPotkategorija.SelectedItem == null)
                comboBoxPotkategorija.SelectedItem = comboBoxPotkategorija.Items[0];
        }
        private void ButtonPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            if (isMinus)
            {
                //rectanglePlus.Visibility = Visibility.Visible;
                //rectangleMinus.Fill = Brushes.Green;
                Util.AddIconToButton(buttonPlusMinus, "Plus", 85);
                textBoxIznos.Background = Brushes.LightGreen;
                labelRashodi.Background = Brushes.White;
                labelPrihodi.Background = Brushes.LightGreen;
            }
            else
            {
                //rectanglePlus.Visibility = Visibility.Hidden;
                //rectangleMinus.Fill = Brushes.Red;
                Util.AddIconToButton(buttonPlusMinus, "Minus", 85);
                textBoxIznos.Background = Brushes.Salmon;
                labelRashodi.Background = Brushes.Salmon;
                labelPrihodi.Background = Brushes.White;
            }
            isMinus = !isMinus;
        }

        private void ButtonDodaj_Click(object sender, RoutedEventArgs e)
        {
            textBoxIznos.Text = textBoxIznos.Text.Replace(",", ".");

            Unos unos = new Unos();
            unos.DatumVrijeme = datePickerDatum.SelectedDate.Value;

            double outparam;
            bool success = double.TryParse(textBoxIznos.Text, out outparam);
            if (success)
            {
                unos.Iznos = outparam;
                unos.IdPotkategorije = ((Potkategorija)(((ComboBoxItem)comboBoxPotkategorija.SelectedItem).Tag)).IdPotkategorije;
                unos.Opis = textBoxOpis.Text;

                unos.Vrsta = isMinus ? Util.minus : Util.plus;
                Util.Context.Unos.Add(unos);
                Util.Context.SaveChanges();

                List<Unos> unosi = Util.Context.Unos.ToList<Unos>();
                MjeseciGodisnjiObracun(unosi);
                PrihodiRashodi(unosi);

                textBoxIznos.Text = "";
                textBoxOpis.Text = "";
            }
            else
            {
                textBoxIznos.Text = "";
                MessageBox.Show("Ponovo unesite iznos!", "Broj nije dobro napisan!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonPonisti_Click(object sender, RoutedEventArgs e)
        {
            textBoxIznos.Text = "";
            comboBoxKategorija.Text = "";
            comboBoxPotkategorija.Text = "";
            textBoxOpis.Text = "";
            RefreshText();
        }

        public void Rashodi_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            BrisanjeUnosaIzDataGrida(dataGridUnosiRashodi);
        }

        public void Prihodi_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            BrisanjeUnosaIzDataGrida(dataGridUnosiPrihodi);
        }

        public void Mjesec_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            var izabrano = (Tuple<string, string, string>)dataGridMjesec.SelectedItem;
            int dan = int.Parse(izabrano.Item1.TrimEnd('.'));
            DateTime datum = new DateTime(DateTime.Now.Year, DateTime.Now.Month, dan);
            datePickerDatum.SelectedDate = datum;

            RefreshText();
        }

        public void MenuUređivanjeKategorijeIPotkategorije_Click(object sender, RoutedEventArgs e)
        {
            new Kategorije(this).Show();
            this.Hide();
        }
        public void MenuIzvještajiMjesečni_Click(object sender, RoutedEventArgs e)
        {
            new MjesecniIzvjestaj(this).Show();
            this.Hide();
        }
        public void MenuIzvještajiGodišnji_Click(object sender, RoutedEventArgs e)
        {
            new GodisnjiIzvjestaj(this).Show();
            this.Hide();
        }
        public void MenuIzvještajiIntervalski_Click(object sender, RoutedEventArgs e)
        {
            new IntervalskiIzvjestaj(this).Show();
            this.Hide();
        }

        private void DatePickerDatum_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RefreshText();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Util.ClearTemps();
        }

        private void MenuPretraživanje_Click(object sender, RoutedEventArgs e)
        {
            new Pretrazivanje(this).Show();
            this.Hide();
        }
    }
}
