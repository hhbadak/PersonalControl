using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalControl
{
    public partial class @default : Form
    {
        DataModel dm = new DataModel();
        public @default()
        {
            InitializeComponent();
        }

        private void default_Load(object sender, EventArgs e)
        {
            cb_employee.ValueMember = "ID";
            cb_employee.DisplayMember = "definition";
            cb_employee.DataSource = dm.getPersonalRecord();
            loadGrid();
            dgv_list.Visible = false;
        }

        private void loadGrid()
        {
            var result = dm.LogEntryListBySelectedDate(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }
        private void loadGridOcak()
        {
            // Personel seçilmediyse, işlem yapmadan metodu sonlandır
            if (cb_employee.SelectedValue == null)
            {
                return;
            }

            // Ay bilgisini al
            int selectedMonth = 1; // Ocak ayını temsil eden değer

            // Seçili öğe varsa devam et
            if (cb_moonControl.SelectedItem != null)
            {
                // Seçili öğeyi al
                var selectedItem = cb_moonControl.SelectedItem;

                // Ay ismini al
                string selectedMonthName = selectedItem.ToString();

                // Ay ismini ayın numerik değerine dönüştür
                selectedMonth = DateTime.ParseExact(selectedMonthName, "MMMM", CultureInfo.CurrentCulture).Month;

                // Veritabanından verileri al
                var result = dm.LogEntryListBySelectedOcak(new DataAccessLayer.Product
                {
                    SelectedMonth = selectedMonth,
                    CastingPersonalID = (int)cb_employee.SelectedValue
                });

                // DataGridView'e sorgu sonucunu yükle
                dgv_list.DataSource = result;

                // DataGridView'i güncelle
                dgv_list.Columns["Type"].HeaderText = "Durum";
                dgv_list.Columns["Count"].HeaderText = "Sayı";
                dgv_list.Columns["Count"].DefaultCellStyle.Format = "N0 Adet";
                dgv_list.Columns["Definition"].HeaderText = "Ürün Kodu";
            }
            else
            {
                MessageBox.Show("Geçerli bir tarih seçiniz.");
            }
        }

        private void loadGridSubat()
        {
            var result = dm.LogEntryListBySelectedSubat(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridMart()
        {
            var result = dm.LogEntryListBySelectedMart(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridNisan()
        {
            var result = dm.LogEntryListBySelectedNisan(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridMayis()
        {
            var result = dm.LogEntryListBySelectedMayis(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridHaziran()
        {
            var result = dm.LogEntryListBySelectedHaziran(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridTemmuz()
        {
            var result = dm.LogEntryListBySelectedTemmuz(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridAgustos()
        {
            var result = dm.LogEntryListBySelectedAgustos(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridEylul()
        {
            var result = dm.LogEntryListBySelectedEylul(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridEkim()
        {
            var result = dm.LogEntryListBySelectedEkim(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridKasim()
        {
            var result = dm.LogEntryListBySelectedKasim(
                new DataAccessLayer.Product
                {
                    CastingDate = Convert.ToDateTime(dtp_dayCaontrol.Value.ToShortDateString()),
                    CastingPersonalID = cb_employee.SelectedValue != null ? (int)cb_employee.SelectedValue : 0
                });

            dgv_list.DataSource = result;
            var rt = result.OrderByDescending(r => r.ID).ToList();
            DataTable dt = new DataTable();

            dt.Columns.Add("Durum");
            dt.Columns.Add("Sayı");
            dt.Columns.Add("Ürün Kodu");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["Durum"] = rt[i].Type;
                r["Sayı"] = rt[i].Count + " Adet";
                r["Ürün Kodu"] = rt[i].Definition;


                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void loadGridAralik()
        {
            // Personel seçilmediyse, işlem yapmadan metodu sonlandır
            if (cb_employee.SelectedValue == null)
            {
                return;
            }

            // Ay bilgisini al
            int selectedMonth = 1; // Ocak ayını temsil eden değer

            // Seçili öğe varsa devam et
            if (cb_moonControl.SelectedItem != null)
            {
                // Seçili öğeyi al
                var selectedItem = cb_moonControl.SelectedItem;

                // Ay ismini al
                string selectedMonthName = selectedItem.ToString();

                // Ay ismini ayın numerik değerine dönüştür
                selectedMonth = DateTime.ParseExact(selectedMonthName, "MMMMyyyy", CultureInfo.CurrentCulture).Month;

                // Veritabanından verileri al
                var result = dm.LogEntryListBySelectedOcak(new DataAccessLayer.Product
                {
                    SelectedMonth = selectedMonth,
                    CastingPersonalID = (int)cb_employee.SelectedValue
                });

                // DataGridView'e sorgu sonucunu yükle
                dgv_list.DataSource = result;
                var rt = result.OrderByDescending(r => r.ID).ToList();
                DataTable dt = new DataTable();

                dt.Columns.Add("Durum");
                dt.Columns.Add("Sayı");
                dt.Columns.Add("Ürün Kodu");

                for (int i = 0; i < rt.Count; i++)
                {
                    DataRow r = dt.NewRow();

                    r["Durum"] = rt[i].Type;
                    r["Sayı"] = rt[i].Count + " Adet";
                    r["Ürün Kodu"] = rt[i].Definition;


                    dt.Rows.Add(r);
                }
                dgv_list.DataSource = dt;
            }
        }

        private void btn_list_Click(object sender, EventArgs e)
        {
            if (cb_employee.SelectedValue != null)
            {
                Product p = new Product();
                p.CastingPersonalID = (int)cb_employee.SelectedValue; // SelectedValue kullanarak ID'yi atama
                loadGrid();
                dgv_list.Visible = true;
            }
            if (cb_moonControl.SelectedIndex == 0)
            {

            }
        }

        private void cb_moonControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMonth = cb_moonControl.SelectedItem.ToString();

            switch (selectedMonth)
            {
                case "Ocak":
                    loadGridOcak();
                    dgv_list.Visible = true;
                    break;
                case "Şubat":
                    loadGridSubat();
                    dgv_list.Visible = true;
                    break;
                case "Mart":
                    loadGridMart();
                    dgv_list.Visible = true;
                    break;
                case "Nisan":
                    loadGridNisan();
                    dgv_list.Visible = true;
                    break;
                case "Mayıs":
                    loadGridMayis();
                    dgv_list.Visible = true;
                    break;
                case "Haziran":
                    loadGridHaziran();
                    dgv_list.Visible = true;
                    break;
                case "Temmuz2023":
                    loadGridTemmuz();
                    dgv_list.Visible = true;
                    break;
                case "Ağustos2023":
                    loadGridAgustos();
                    dgv_list.Visible = true;
                    break;
                case "Eylül2023":
                    loadGridEylul();
                    dgv_list.Visible = true;
                    break;
                case "Ekim2023":
                    loadGridEkim();
                    dgv_list.Visible = true;
                    break;
                case "Kasım2023":
                    loadGridKasim();
                    dgv_list.Visible = true;
                    break;
                case "Aralık2023":
                    loadGridAralik();
                    dgv_list.Visible = true;
                    break;
                default:
                    // Eğer seçilen ay ile eşleşen bir işlem yoksa, bir hata mesajı gösterilebilir veya varsayılan bir işlem yapılabilir
                    MessageBox.Show("Geçersiz ay seçimi!");
                    break;
            }
        }
    }
}
