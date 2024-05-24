using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataModel
    {
        SqlConnection con; SqlCommand cmd;

        public DataModel()
        {
            con = new SqlConnection(ConnectionStrings.ConStr);
            cmd = con.CreateCommand();
        }

        public List<PersonalInfo> getPersonalRecord()
        {
            List<PersonalInfo> personal = new List<PersonalInfo>();
            try
            {
                cmd.CommandText = "SELECT Kimlik, Tanim FROM dokum_sicil_liste";
                cmd.Parameters.Clear();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PersonalInfo model = new PersonalInfo();
                    model.ID = Convert.ToInt32(reader["Kimlik"]);
                    model.Definition = reader["Tanim"].ToString();
                    personal.Add(model);
                }
                return personal;
            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedDate(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT 'Kayıp' AS Tür, COUNT(*) AS Adet, kl.Tanim AS Tanim
                            FROM UT_D_Urunler u
                            LEFT JOIN Products p ON u.BarkodNo = p.Barcode
                            LEFT JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE p.Barcode IS NULL
                                AND u.DokumcuId = @employee 
                                AND u.DokumTarih = @selectedDate
                            GROUP BY kl.Tanim

                            UNION ALL

                            SELECT COALESCE(h.HataTanim, 'Döküm') AS Tür, COUNT(*) AS Adet, kl.Tanim AS Tanim
                            FROM UT_D_Urunler u
                            LEFT JOIN UT_DokumHatalari h ON h.Id = u.HataId
                            LEFT JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee 
                                AND u.DokumTarih = @selectedDate
                            GROUP BY COALESCE(h.HataTanim, 'Döküm'), kl.Tanim

                            UNION ALL 

                            SELECT kl.Tanim AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            WHERE p.CastPersonal = @employee 
                                AND p.CastDate = @selectedDate
                            GROUP BY p.Quality, kl.Tanim, kol.tanim

                            UNION ALL

                            SELECT rh.HataTanim AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih = @selectedDate
                                AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim

                            UNION ALL

                            SELECT 'Dökülen Ürün Sayısı' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee
                                AND u.DokumTarih = @selectedDate
                            GROUP BY kl.tanim
                            
                            ORDER BY Tanim DESC";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@selectedDate", p.CastingDate);
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product result = new Product();
                    result.Type = reader.GetString(0); // Tür
                    result.Definition = !reader.IsDBNull(2) ? reader.GetString(2) : null; // Tanim
                    result.Quality = !reader.IsDBNull(1) ? reader.GetByte(1) : (byte)0; // Quality
                    result.Count = reader.GetInt32(1); // Adet
                    results.Add(result);
                }
                return results;
            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }

    }
}
