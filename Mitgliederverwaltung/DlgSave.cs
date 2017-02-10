using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;
using My.Parsing;

namespace Mitgliederverwaltung
{
    public partial class DlgSave : Form
    {
        public Action<DlgSave> Save = null;
        private BindingSource mBindingSource = new BindingSource();
        private int mId = -1;

        public DlgSave()
        {
            InitializeComponent();

            ConfigMove();
            ConfigGrid();
            ConfigTbo();
            ConfigButtons();

            mBindingSource.DataSource = typeof(Telefon);

            Grid.DataSource = mBindingSource;
        }

        public void SetHeaderColor(Color color)
        {
            LblCaptionHeader.BackColor = color;
        }

        public void SetHeaderText(string text)
        {
            LblCaptionHeader.Text = text == null ? string.Empty : text;
        }

        public void SetData(Mitglied mitglied)
        {
            mId = -1;

            if (mitglied == null)
                return;

            mId = mitglied.Id;

            TboName.Text = mitglied.Name;
            TboOrt.Text = mitglied.Ort;
            TboStrasse.Text = mitglied.Strasse;
            TboGeburtsdatum.Text = mitglied.Geburtsdatum == null ? string.Empty : mitglied.Geburtsdatum.Value.ToString("dd.MM.yyyy", Config.Culture);
            TboGeschlecht.Text = mitglied.Geschlecht;
            TboNotiz.Text = mitglied.Notiz;

            foreach (var telefon in mitglied.LstTelefon)
                mBindingSource.Add(telefon.GetClone());
        }

        public bool TryGetData(out Mitglied mitglied)
        {
            mitglied = new Mitglied();

            string formatted;
            object value = null;

            ParseResult result;

            // Id
            mitglied.Id = mId;
            //

            // Name
            result = Config.ParserEntryNotEmpty.TryParse(TboName.Text, out formatted);

            TboName.Text = formatted;

            if (result != ParseResult.Success)
            {
                string msg = "Fehlende Eingabe für Name";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TboName.Focus();
                TboName.SelectAll();
                return false;
            }

            mitglied.Name = formatted;
            // --

            // Ort
            result = Config.ParserEntryNotEmpty.TryParse(TboOrt.Text, out formatted);

            TboOrt.Text = formatted;

            if (result != ParseResult.Success)
            {
                string msg = "Fehlende Eingabe für Ort";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TboOrt.Focus();
                TboOrt.SelectAll();
                return false;
            }

            mitglied.Ort = formatted;
            // --

            // Strasse
            result = Config.ParserEntryNotEmpty.TryParse(TboStrasse.Text, out formatted);

            TboStrasse.Text = formatted;

            if (result != ParseResult.Success)
            {
                string msg = "Fehlende Eingabe für Strasse";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TboStrasse.Focus();
                TboStrasse.SelectAll();
                return false;
            }

            mitglied.Strasse = formatted;
            // --

            // Geburtsdatum
            result = Config.ParserGeburtsdatum.TryParse(TboGeburtsdatum.Text, out value, out formatted);

            if (result != ParseResult.Success)
            {
                string msg = "Ungültige Eingabe für Geburtsdatum (Format: 'TT.MM.JJJJ')";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TboGeburtsdatum.Focus();
                TboGeburtsdatum.SelectAll();
                return false;
            }

            TboGeburtsdatum.Text = formatted;
            mitglied.Geburtsdatum = Convert.ToDateTime(value);
            // --

            // Geschlecht
            result = Config.ParserGeschlecht.TryParse(TboGeschlecht.Text, out formatted);

            if (result != ParseResult.Success)
            {
                string msg = "Ungültige Eingabe für Geschlecht (Format: 'M' / 'W')";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TboGeschlecht.Focus();
                TboGeschlecht.SelectAll();
                return false;
            }

            TboGeschlecht.Text = formatted;
            mitglied.Geschlecht = formatted;
            // -- 

            // Notiz
            TboNotiz.Text = TboNotiz.Text.Trim();
            mitglied.Notiz = TboNotiz.Text;
            // --

            // Telefon
            foreach (var entry in mBindingSource)
            {
                Telefon telefon = entry as Telefon;
                telefon.Nummer = telefon.Nummer.Trim();

                if (telefon.Nummer != string.Empty)
                    mitglied.LstTelefon.Add(new Telefon(telefon.Nummer));
            }
            // --

            return true;
        }

        private void ConfigMove()
        {
            bool mMouseUp = true;
            int mX = 0;
            int mY = 0;

            foreach (Control c in Controls)
            {
                if (c is DataGridView || c is Button || c is TextBox)
                    continue;

                c.MouseDown += (s, e) =>
                {
                    mMouseUp = false;
                    mX = e.X;
                    mY = e.Y;
                };

                c.MouseUp += (s, e) =>
                {
                    mMouseUp = true;
                };

                c.MouseMove += (s, e) =>
                {
                    if (mMouseUp)
                        return;

                    Left += e.X - mX;
                    Top += e.Y - mY;
                };
            }
        }

        private void ConfigGrid()
        {
            Grid.AutoGenerateColumns = false;
            Grid.ColumnHeadersHeight = (int)(Grid.ColumnHeadersDefaultCellStyle.Font.GetHeight() * 2);

            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Telefonnummer";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DataPropertyName = "Nummer";
            column.Name = "Nummer";
            column.Width = 140;
            Grid.Columns.Add(column);
        }

        private void ConfigTbo()
        {
            foreach (Control ctr in this.Controls)
            {
                TextBox tbo = ctr as TextBox;

                if (tbo == null)
                    continue;

                tbo.Validated += (s, e) =>
                {
                    tbo.Text = tbo.Text.Trim();
                };
            }

            TboGeburtsdatum.Validated += (s, e) =>
            {
                string formatted;

                if (Config.ParserGeburtsdatum.TryParse(TboGeburtsdatum.Text, out formatted) == ParseResult.Success)
                    TboGeburtsdatum.Text = formatted;
            };
        }

        private void ConfigButtons()
        {
            BtnEscape.Click += (s, e) =>
            {
                Close();
            };

            BtnSave.Click += (s, e) =>
            {
                Save?.Invoke(this);

                //if (Save != null)
                //    Save(this);
            };
        }
    }
}
