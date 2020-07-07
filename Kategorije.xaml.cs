using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DnevnikTroskova
{
    /// <summary>
    /// Interaction logic for Kategorije.xaml
    /// </summary>
    public partial class Kategorije : Window
    {
        public List<Kategorija> kategorije = new List<Kategorija>();
        public List<Potkategorija> potkategorije = new List<Potkategorija>();
        public List<Potkategorija> izabranePotkategorije = new List<Potkategorija>();
        private MainWindow main;
        public Kategorije(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            Refresh();

            Util.AddIconToButton(buttonKategorijaGore, "Up", 60);
            Util.AddIconToButton(buttonKategorijaDole, "Down", 60);
            Util.AddIconToButton(buttonPotkategorijaGore, "Up", 60);
            Util.AddIconToButton(buttonPotkategorijaDole, "Down", 60);
        }
        private void SveKategorije()
        {
            kategorije = Util.Context.Kategorija.ToList<Kategorija>();
            kategorije.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));
            dataGridKategorije.ItemsSource = kategorije;
        }
        private void SvePotkategorije()
        {
            Util.Context.SaveChanges();
            potkategorije = Util.Context.Potkategorija.ToList<Potkategorija>();
            izabranePotkategorije = new List<Potkategorija>();
            if ((Kategorija)dataGridKategorije.SelectedItem == null) dataGridKategorije.SelectedIndex = 0;
            foreach (Potkategorija x in potkategorije)
                if (((Kategorija)dataGridKategorije.SelectedItem).IdKategorije.Equals(x.IdKategorije))
                    izabranePotkategorije.Add(x);
            izabranePotkategorije.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));
            dataGridPotkategorije.ItemsSource = izabranePotkategorije;
        }
        private void Refresh()
        {
            Util.Context.SaveChanges();
            SveKategorije();
            dataGridKategorije.SelectedIndex = 0;
            SvePotkategorije();
            dataGridPotkategorije.SelectedIndex = 0;
            textKategorija.Text = "";
            textPotkategorija.Text = "";
        }
        public void Kategorije_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            ButtonObrisiKategoriju_Click(sender, e);
        }
        public void Potkategorije_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            ButtonObrisiPotkategoriju_Click(sender, e);
        }
        public void ButtonDodajKategoriju_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            if (!kategorije.Exists(x => x.Naziv.Equals(textKategorija)))
            {
                Kategorija x = new Kategorija();
                x.Pozicija = 0;
                foreach (Kategorija y in dataGridKategorije.Items)
                    if (y.Pozicija > x.Pozicija) x.Pozicija = y.Pozicija;
                ++x.Pozicija;
                x.Naziv = textKategorija.Text;
                Util.Context.Kategorija.Add(x);
                Util.Context.SaveChanges();
            }
            else MessageBox.Show("Kategorija sa tim nazivom već postoji!", "UPOZORENJE!", MessageBoxButton.OK, MessageBoxImage.Warning);
            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
        }
        public void ButtonDodajPotkategoriju_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            if (kategorija != null)
            {
                if (!izabranePotkategorije.Exists(x => x.Naziv.Equals(textPotkategorija)))
                {
                    Potkategorija x = new Potkategorija();
                    x.Pozicija = 0;
                    foreach (Potkategorija y in dataGridKategorije.Items)
                        if (y.Pozicija > x.Pozicija) x.Pozicija = y.Pozicija;
                    ++x.Pozicija;
                    x.IdKategorije = kategorija.IdKategorije;
                    x.Naziv = textPotkategorija.Text;
                    Util.Context.Potkategorija.Add(x);
                    Util.Context.SaveChanges();
                }
                else MessageBox.Show("Potkategorija sa tim nazivom već postoji!", "UPOZORENJE!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
        }
        public void Kategorije_Selection_Changed(object sender, RoutedEventArgs e)
        {
            SvePotkategorije();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.Show();
        }
        private void ButtonObrisiKategoriju_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            bool count = true;
            foreach (var x in potkategorije) if (x.IdKategorije == kategorija.IdKategorije) count = false;
            if (count) Util.Context.Kategorija.Remove(kategorija);
            Refresh();
            dataGridKategorije.SelectedItem = dataGridKategorije.Items[0];
        }
        private void ButtonObrisiPotkategoriju_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            Potkategorija potkategorija = (Potkategorija)dataGridPotkategorije.SelectedItem;
            bool count = true;
            foreach (var x in Util.Context.Unos.ToList<Unos>())
                if (x.IdPotkategorije == potkategorija.IdPotkategorije) count = false;
            if (count) Util.Context.Potkategorija.Remove(potkategorija);
            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
        }

        private void ButtonKategorijaGore_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            if (kategorija.Pozicija == 1) return;
            foreach (Kategorija y in dataGridKategorije.Items)
                if (y.Pozicija == kategorija.Pozicija - 1)
                {
                    y.Pozicija++;
                    kategorija.Pozicija--;
                    break;
                }

            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
        }

        private void ButtonKategorijaDole_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            int maxPozicija = 0;
            foreach (Kategorija y in dataGridKategorije.Items)
                if (y.Pozicija > maxPozicija) maxPozicija = y.Pozicija;
            if (kategorija.Pozicija == maxPozicija) return;
            foreach (Kategorija y in dataGridKategorije.Items)
                if (y.Pozicija == kategorija.Pozicija + 1)
                {
                    y.Pozicija--;
                    kategorija.Pozicija++;
                    break;
                }

            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
        }

        private void ButtonPotkategorijaGore_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            Potkategorija potkategorija = (Potkategorija)dataGridPotkategorije.SelectedItem;
            if (potkategorija.Pozicija == 1) return;
            int index = dataGridPotkategorije.SelectedIndex;
            foreach (Potkategorija y in dataGridPotkategorije.Items)
                if (y.Pozicija == potkategorija.Pozicija - 1)
                {
                    y.Pozicija++;
                    potkategorija.Pozicija--;
                    break;
                }

            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
            dataGridPotkategorije.SelectedIndex = index - 1;
        }

        private void ButtonPotkategorijaDole_Click(object sender, RoutedEventArgs e)
        {
            Kategorija kategorija = (Kategorija)dataGridKategorije.SelectedItem;
            Potkategorija potkategorija = (Potkategorija)dataGridPotkategorije.SelectedItem;
            int maxPozicija = 0;
            foreach (Potkategorija y in dataGridPotkategorije.Items)
                if (y.Pozicija > maxPozicija) maxPozicija = y.Pozicija;
            if (potkategorija.Pozicija == maxPozicija) return;
            int index = dataGridPotkategorije.SelectedIndex;
            foreach (Potkategorija y in dataGridPotkategorije.Items)
                if (y.Pozicija == potkategorija.Pozicija + 1)
                {
                    y.Pozicija--;
                    potkategorija.Pozicija++;
                    Console.Out.WriteLine("" + potkategorija.Naziv + " " + y.Naziv);
                    break;
                }

            Refresh();
            dataGridKategorije.SelectedItem = kategorija;
            dataGridPotkategorije.SelectedIndex = index + 1;
        }
    }
}
