using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnevnikTroskova
{
    public class TreeGridViewItem
    {
        public string Naziv { get; set; } = "";
        public double Prihodi { get; set; } = 0;
        public double Rashodi { get; set; } = 0;
        public object Tag { get; set; } = null;
        public double Ukupno { get { return Math.Round(Prihodi - Rashodi, 2); } }
        public List<TreeGridViewItem> Items { get; set; }
        public TreeGridViewItem()
        {
            Items = new List<TreeGridViewItem>();
        }
        public TreeGridViewItem(string Naziv)
        {
            this.Naziv = this.Naziv = Naziv;
            Items = new List<TreeGridViewItem>();
        }
        public TreeGridViewItem(string Naziv, double Prihodi, double Rashodi)
        {
            this.Naziv = Naziv;
            this.Prihodi = Prihodi;
            this.Rashodi = Rashodi;
            Items = new List<TreeGridViewItem>();
        }
        public TreeGridViewItem(string Naziv, double Prihodi, double Rashodi, object Tag)
        {
            this.Naziv = Naziv;
            this.Prihodi = Prihodi;
            this.Rashodi = Rashodi;
            this.Tag = Tag;
            Items = new List<TreeGridViewItem>();
        }
    }
}
