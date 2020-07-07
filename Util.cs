using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace DnevnikTroskova
{
    static class Util
    {
        public const int PocetnaGodina = 2019;
        private const String precision = "{0:N2}";
        public const String plus = "PLUS";
        public const String minus = "MINUS";
        public const double delta = 0.0001;
        public static DnevnikTroskovaEntities Context { get; } = new DnevnikTroskovaEntities();
        private static List<string> tempFiles = new List<string>();
        private static List<Tuple<int, string>> mjeseci { get; } = new List<Tuple<int, string>>();
        static Util()
        {
            mjeseci.Add(new Tuple<int, string>(1, "Januar"));
            mjeseci.Add(new Tuple<int, string>(2, "Februar"));
            mjeseci.Add(new Tuple<int, string>(3, "Mart"));
            mjeseci.Add(new Tuple<int, string>(4, "April"));
            mjeseci.Add(new Tuple<int, string>(5, "Maj"));
            mjeseci.Add(new Tuple<int, string>(6, "Juni"));
            mjeseci.Add(new Tuple<int, string>(7, "Juli"));
            mjeseci.Add(new Tuple<int, string>(8, "Avgust"));
            mjeseci.Add(new Tuple<int, string>(9, "Septembar"));
            mjeseci.Add(new Tuple<int, string>(10, "Oktobar"));
            mjeseci.Add(new Tuple<int, string>(11, "Novembar"));
            mjeseci.Add(new Tuple<int, string>(12, "Decembar"));
        }
        public static void ClearTemps()
        {
            foreach (var f in tempFiles) File.Delete(f);
        }
        public static List<Tuple<int, string>> Mjeseci()
        {
            return mjeseci;
        }
        public static string BrojUTekst(double broj)
        {
            return string.Format(precision, Math.Round((double)broj, 2));
        }
        public static string BrojUTekst(int broj, int brojCifara)
        {
            string result = "";
            int temp = broj;
            while (temp>0)
            {
                brojCifara--;
                temp /= 10;
            }
            while (brojCifara-->0) result += "0";
            return result + ((broj!=0)?(""+broj):"");
        }
        public static void AddIconToButton(Button button, string name, int size)
        {
            button.Content =
                new Image
                {
                    Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/" + name + ".png")),
                    Width = size,
                    Height = size,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
        }
        public static string DateTimeToString(DateTime x)
        {
            return
                BrojUTekst(x.Day, 2) + "." +
                BrojUTekst(x.Month, 2) + "." +
                x.Year + ".";
        }
        public static void ColorLabel(Label label, double plus, double minus)
        {
            if (Math.Abs(plus) > Math.Abs(minus)) label.Background = Brushes.Green;
            else if (Math.Abs(plus) < Math.Abs(minus)) label.Background = Brushes.Red;
            else label.Background = Brushes.DarkGray;
        }
        public static void BrojUTextBox(TextBox textBox, double broj, string prefix)
        {
            textBox.Text = prefix + Util.BrojUTekst(Math.Abs(broj));
        }
        public static void BrojULabel(Label label, double plus, double minus)
        {
            double broj = Math.Abs(plus) - Math.Abs(minus);
            label.Content = (broj > 0 ? "+" : "-") + Util.BrojUTekst(Math.Abs(broj));
        }
        public static string SelectedComboBoxItemToString(ComboBox comboBox)
        {
            return (string)(((ComboBoxItem)comboBox.SelectedItem).Content);
        }
        public static void PlusMinusUkupno(TextBox textBoxPlus, TextBox textBoxMinus, Label labelUkupno, double plus, double minus)
        {
            BrojUTextBox(textBoxPlus, plus, "+");
            BrojUTextBox(textBoxMinus, minus, "-");
            BrojULabel(labelUkupno, plus, minus);
            ColorLabel(labelUkupno, plus, minus);
        }
        public static void PrintPDF(PdfDocument pdf, string naziv)
        {
            pdf.Save(naziv);
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = naziv //put the correct path here
            };
            p.Start();
            tempFiles.Add(naziv);
        }
        public static PdfDocument NapraviPDF(TreeGridViewItem stablo, string naslov)
        {
            string html = "";
            /**Naslov**/
            html += "<h1 align = 'center'> " + naslov + "</h1>";
            /**Tabela**/
            html += "<table style='width: 550px; page-break-inside: avoid;' align='center'><tbody>";
            html += "<tr>";
            html += "<td style='width: 100px; color: white; background-color: black;'>Naziv</td>";
            html += "<td style='width: 20px; color: white; background-color: black;'>Prihodi</td>";
            html += "<td style='width: 20px; color: white; background-color: black;'>Rashodi</td>";
            html += "<td style='width: 20px; color: white; background-color: black;'>Ukupno</td>";
            html += "</tr>";
            html += TabelaHTML(stablo, -10);
            html += "</tbody></table>";
            /**Kreiranje**/
            return PdfGenerator.GeneratePdf(html, PageSize.A4);
        }
        public static string TabelaHTML(TreeGridViewItem item, int numberOfSpaces)
        {
            string space = "";
            for (int i = 0; i < numberOfSpaces; ++i) space += "&nbsp;";
            string naziv = space + item.Naziv;
            if (numberOfSpaces >= 0) 
                naziv = naziv.Substring(0, (70 + (6 * numberOfSpaces)) < item.Naziv.Length ? (70 + (6 * numberOfSpaces)) : naziv.Length);

            string html = "";
            html += "<tr style='break-inside: avoid; page-break-inside: avoid;'>";
            html += "<td style='break-inside: avoid; page-break-inside: avoid;width: 100px;' align='left'>" + naziv + "</td>";
            html += "<td style='break-inside: avoid; page-break-inside: avoid;width: 20px;' align='right'>" + BrojUTekst(item.Prihodi) + "</td>";
            html += "<td style='break-inside: avoid; page-break-inside: avoid;width: 20px;' align='right'>" + BrojUTekst(item.Rashodi) + "</td>";
            html += "<td style='break-inside: avoid; page-break-inside: avoid;width: 20px;' align='right'>" + BrojUTekst(item.Ukupno) + "</td>";
            html += "</tr>";

            foreach (var i in item.Items) html += TabelaHTML(i, numberOfSpaces + 10);
            return html;
        }
        public static TreeGridViewItem Stablo(Func<DateTime, bool> uslov)
        {
            TreeGridViewItem root = new TreeGridViewItem();
            root.Naziv = "Ukupno";
            List<Kategorija> kategorije = Context.Kategorija.ToList<Kategorija>();
            foreach (var kategorija in kategorije)
            {
                TreeGridViewItem k = new TreeGridViewItem(kategorija.Naziv);
                k.Tag = kategorija;
                foreach (var potkategorija in kategorija.Potkategorija)
                {
                    TreeGridViewItem p = new TreeGridViewItem(potkategorija.Naziv);
                    p.Tag = potkategorija;
                    foreach (var unos in potkategorija.Unos)
                    {
                        if (uslov(unos.DatumVrijeme))
                        {
                            TreeGridViewItem u;
                            if (unos.Vrsta == "PLUS")
                            {
                                u = new TreeGridViewItem(Util.DateTimeToString(unos.DatumVrijeme) + " - " + unos.Opis.Substring(0, 30 < unos.Opis.Length ? 20 : unos.Opis.Length), unos.Iznos, 0);
                                u.Tag = unos;
                                p.Prihodi += unos.Iznos;
                            }
                            else
                            {
                                u = new TreeGridViewItem(Util.DateTimeToString(unos.DatumVrijeme) + " - " + unos.Opis.Substring(0, 30 < unos.Opis.Length ? 20 : unos.Opis.Length), 0, unos.Iznos);
                                u.Tag = unos;
                                p.Rashodi += unos.Iznos;
                            }
                            p.Items.Add(u);
                        }
                    }
                    p.Items.Sort((x, y) => (((Unos)x.Tag)).DatumVrijeme.CompareTo(((Unos)y.Tag).DatumVrijeme));

                    k.Prihodi += p.Prihodi;
                    k.Rashodi += p.Rashodi;
                    k.Items.Add(p);
                }
                k.Items.Sort((x, y) => (((Potkategorija)x.Tag)).Pozicija.CompareTo(((Potkategorija)y.Tag).Pozicija));

                root.Prihodi += k.Prihodi;
                root.Rashodi += k.Rashodi;
                root.Items.Add(k);
            }
            root.Items.Sort((x, y) => (((Kategorija)x.Tag)).Pozicija.CompareTo(((Kategorija)y.Tag).Pozicija));
            return root;
        }

        public static TreeGridViewItem OgraniciStablo(TreeGridViewItem stablo, int brojNivoa)
        {
            TreeGridViewItem root = new TreeGridViewItem();
            root.Naziv = "Ukupno";
            root.Prihodi = stablo.Prihodi;
            root.Rashodi = stablo.Rashodi;
            root.Items = new List<TreeGridViewItem>();
            if (brojNivoa >= 1)
            {
                foreach (var k in stablo.Items)
                {
                    TreeGridViewItem kat = new TreeGridViewItem();
                    kat.Naziv = k.Naziv;
                    kat.Prihodi = k.Prihodi;
                    kat.Rashodi = k.Rashodi;
                    kat.Tag = k.Tag;
                    kat.Items = new List<TreeGridViewItem>();
                    if (brojNivoa >= 2)
                    {
                        foreach (var p in k.Items)
                        {
                            TreeGridViewItem pot = new TreeGridViewItem();
                            pot.Naziv = p.Naziv;
                            pot.Prihodi = p.Prihodi;
                            pot.Rashodi = p.Rashodi;
                            pot.Tag = p.Tag;
                            pot.Items = new List<TreeGridViewItem>();
                            if(brojNivoa>=3)
                            {
                                foreach(var u in p.Items)
                                {
                                    TreeGridViewItem uno = new TreeGridViewItem();
                                    uno.Naziv = u.Naziv;
                                    uno.Prihodi = u.Prihodi;
                                    uno.Rashodi = u.Rashodi;
                                    uno.Tag = u.Tag;
                                    uno.Items = new List<TreeGridViewItem>();

                                    pot.Items.Add(uno);
                                }
                            }
                            kat.Items.Add(pot);
                        }
                    }
                    root.Items.Add(kat);
                }
            }
            return root;
        }
    }
}
