using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            dt.Columns.Add("ID");
            dt.Columns.Add("Döküm Adeti");
            dt.Columns.Add("Ürün Kodu");
            dt.Columns.Add("Döküm Hata");
            dt.Columns.Add("Rötuş Fire");
            dt.Columns.Add("Kalite");
            dt.Columns.Add("Dökümcü Sicil No");
            dt.Columns.Add("Döküm Tarihi");

            for (int i = 0; i < rt.Count; i++)
            {
                DataRow r = dt.NewRow();

                r["ID"] = rt[i].ID;
                r["Döküm Adeti"] = rt[i].Count;
                r["Ürün Kodu"] = rt[i].Code;
                r["Döküm Hata"] = rt[i].Fault;
                r["Rötuş Fire"] = rt[i].RetouchFire;
                r["Kalite"] = rt[i].Quality;
                r["Dökümcü Sicil No"] = rt[i].CastingPersonalID; // CastingPersonal
                r["Döküm Tarihi"] = rt[i].CastingDate; // CastDate

                dt.Rows.Add(r);
            }
            dgv_list.DataSource = dt;

        }

        private void btn_list_Click(object sender, EventArgs e)
        {
            if (cb_employee.SelectedValue != null)
            {
                Product p = new Product();
                p.CastingPersonalID = (int)cb_employee.SelectedValue; // SelectedValue kullanarak ID'yi atama
                loadGrid();
            }
        }
    }
}
