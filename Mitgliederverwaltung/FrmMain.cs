using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using My.Async;
using Shared;

namespace Mitgliederverwaltung
{
    public partial class FrmMain : Form
    {
        private AsyncTask mAsyncTask;
        private BindingSource mBindingSource = new BindingSource();

        private void CreateMenu()
        {
            MenuStrip strip = new MenuStrip();
            strip.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            ToolStripMenuItem itemSelect = new ToolStripMenuItem("&Neu Abfragen");
            ToolStripMenuItem itemInsert = new ToolStripMenuItem("&Anlegen");
            ToolStripMenuItem itemUpdate = new ToolStripMenuItem("&Bearbeiten");
            ToolStripMenuItem itemDelete = new ToolStripMenuItem("&Löschen");
            ToolStripMenuItem itemClose = new ToolStripMenuItem("&Schliessen");
            itemClose.Alignment = ToolStripItemAlignment.Right;

            strip.Items.Add(new ToolStripSeparator());
            strip.Items.Add(itemSelect);
            strip.Items.Add(new ToolStripSeparator());
            strip.Items.Add(itemInsert);
            strip.Items.Add(itemUpdate);
            strip.Items.Add(itemDelete);
            strip.Items.Add(new ToolStripSeparator());

            ToolStripSeparator sep1 = new ToolStripSeparator();
            sep1.Alignment = ToolStripItemAlignment.Right;
            strip.Items.Add(sep1);

            strip.Items.Add(itemClose);

            ToolStripSeparator sep2 = new ToolStripSeparator();
            sep2.Alignment = ToolStripItemAlignment.Right;
            strip.Items.Add(sep2);

            itemSelect.Click += (s, e) =>
            {
                if (mAsyncTask.IsBusy)
                    return;

                Mitglied mg = mBindingSource.Current as Mitglied;
                int id = mg == null ? 0 : mg.Id;
                Read(id);
            };

            itemInsert.Click += (s, e) =>
            {
                if (mAsyncTask.IsBusy)
                    return;

                DlgSave dlg = new DlgSave();
                dlg.SetHeaderText("M i t g l i e d  A n l e g e n");
                dlg.SetData(null);
                dlg.Save = Save;
                dlg.ShowDialog();
            };

            itemUpdate.Click += (s, e) =>
            {
                if (mAsyncTask.IsBusy)
                    return;

                Mitglied mitglied = mBindingSource.Current as Mitglied;

                if (mitglied == null)
                {
                    string msg = "Es wurde kein Mitglied ausgewählt";
                    MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DlgSave dlg = new DlgSave();
                dlg.SetHeaderText("M i t g l i e d  B e a r b e i t e n");
                dlg.SetData(mitglied);
                dlg.Save = Save;
                dlg.ShowDialog();
            };

            itemDelete.Click += (s, e) =>
            {
                if (mAsyncTask.IsBusy)
                    return;

                Mitglied mitglied = mBindingSource.Current as Mitglied;

                if (mitglied == null)
                {
                    string msg = "Es wurde kein Mitglied ausgewählt";
                    MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Delete(mitglied);
            };

            itemClose.Click += (s, e) =>
            {
                Close();
            };

            this.Controls.Add(strip);
        }

        private void ConfigGrid()
        {
            Grid.AutoGenerateColumns = false;
            Grid.ColumnHeadersHeight = (int)(Grid.ColumnHeadersDefaultCellStyle.Font.GetHeight() * 2);

            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Name";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DataPropertyName = "Name";
            column.Name = "Name";
            column.ReadOnly = true;
            column.Width = 180;
            Grid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Ort";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DataPropertyName = "Ort";
            column.Name = "Ort";
            column.ReadOnly = true;
            column.Width = 180;
            Grid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Strasse";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.DataPropertyName = "Strasse";
            column.Name = "Strasse";
            column.ReadOnly = true;
            column.Width = 220;
            Grid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Geb.Datum";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DataPropertyName = "Geburtsdatum";
            column.Name = "Geburtsdatum";
            column.ReadOnly = true;
            column.Width = 100;
            column.DefaultCellStyle.Format = "dd.MM.yyyy";
            Grid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Geschl.";
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DataPropertyName = "Geschlecht";
            column.Name = "Geschlecht";
            column.ReadOnly = true;
            column.Width = 65;
            Grid.Columns.Add(column);
        }

        private void ConfigBindings()
        {
            mBindingSource.DataSource = typeof(Mitglied);

            LblCaptionName.DataBindings.Add("Text", mBindingSource, "Name");
            LblNotiz.DataBindings.Add("Text", mBindingSource, "Notiz");

            Grid.DataSource = mBindingSource;

            LboTelefon.DataSource = mBindingSource;
            LboTelefon.DisplayMember = "LstTelefon.Nummer";

            mBindingSource.ListChanged += (s, e) =>
            {
                int count = mBindingSource.Count;

                if (count == 1)
                    LblInfoCount.Text = mBindingSource.Count.ToString("F0") + " Mitglied";
                else
                    LblInfoCount.Text = mBindingSource.Count.ToString("F0") + " Mitglieder";
            };
        }

        private void ConfigAsyncTask()
        {
            mAsyncTask = new AsyncTask(this);
        }

        private void Read(int id)
        {
            Action<AsyncResult<ReadResult>> executing = (res) =>
            {
                res.Item = Queries.Read();
            };

            Action<AsyncResult<ReadResult>> executed = (res) =>
            {
                Cursor = Cursors.Default;
                LblInfo.Text = string.Empty;

                if (res.Exception != null)
                {
                    MessageBox.Show(res.Exception.Message);
                    return;
                }

                Grid.DataSource = null;

                for (int i = 0; i < res.Item.Mitglieder.Count; i++)
                {
                    Mitglied mitglied = res.Item.Mitglieder[i];
                    mBindingSource.Add(mitglied);

                    if (mitglied.Id == id)
                        mBindingSource.Position = i;
                }

                Grid.DataSource = mBindingSource;

                string m = res.Item.Maennlich.ToString("N2", Config.Culture);
                string w = res.Item.Weiblich.ToString("N2", Config.Culture);
                LblInfo.Text = "männlich: " + m + " %" + "   " + "weiblich: " + w + " %";

            };

            Cursor = Cursors.WaitCursor;
            LblInfo.Text = "Daten werden abgefragt ...";
            LblNotiz.Text = string.Empty;
            mBindingSource.Clear();
            LblInfoCount.Text = string.Empty;

            Application.DoEvents();

            mAsyncTask.Execute(executing, executed);
        }

        private void Save(DlgSave dlg)
        {
            Mitglied mitglied;

            if (!dlg.TryGetData(out mitglied))
                return;

            dlg.Close();

            int id;

            try
            {
                Queries.Save(mitglied, out id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Read(id);
        }

        private void Delete(Mitglied mitglied)
        {
            string msg = "Mitglied '" + mitglied.Name + "' wird unwiderruflich gelöscht!";
            DialogResult result = MessageBox.Show(msg, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (result != DialogResult.OK)
                return;

            int previous = 0;

            try
            {
                mBindingSource.MovePrevious();
                Mitglied temp = mBindingSource.Current as Mitglied;

                if (temp != null)
                    previous = temp.Id;
            }
            catch { }

            try
            {
                int affected;
                Queries.Delete(mitglied.Id, out affected);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Read(previous);
        }

        public FrmMain()
        {
            InitializeComponent();

            CreateMenu();
            ConfigGrid();
            ConfigBindings();
            ConfigAsyncTask();

            Read(0);
        }
    }
}
