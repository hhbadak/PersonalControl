﻿using System;
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
                cmd.CommandText = @"SELECT
    Tanim,
    MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
    MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
    MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
    MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
    MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
    MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
    MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
FROM
(
    SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
    FROM
    (
        SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
        FROM UT_D_Urunler u
        JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
        WHERE u.DokumcuId = @employee AND u.DokumTarih = @selectedDate
        GROUP BY kl.tanim
        UNION ALL
        SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
        FROM Products p
        LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
        LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
        WHERE p.CastPersonal = @employee AND p.CastDate = @selectedDate
        GROUP BY kl.kaliteAd, kol.tanim, p.Quality
        UNION ALL
        SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
        FROM RotusTakip rt
        JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
        JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
        JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
        WHERE u.DokumTarih = @selectedDate AND u.DokumcuId = @employee
        GROUP BY kl.tanim, rh.HataTanim
		UNION ALL
		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
        FROM UT_D_Urunler u
        JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
        JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
        WHERE u.DokumTarih = @selectedDate AND (Durum = 'Fire') AND u.DokumcuId = @employee
        GROUP BY kl.tanim, dh.HataTanim
        UNION ALL
		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
        FROM UT_D_Urunler u
        JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
        JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
        WHERE u.DokumTarih = @selectedDate AND (Durum = 'iptal') AND u.DokumcuId = @employee
        GROUP BY kl.tanim, dh.HataTanim
    ) AS TumUrunler
    GROUP BY Tür, Tanim
) AS SourceTable
GROUP BY Tanim
ORDER BY Tanim ASC;

";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@selectedDate", p.CastingDate);
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedOcak(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-01-01' AND u.DokumTarih < '2024-02-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-01-01' AND p.CastDate < '2024-02-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-01-01' AND u.DokumTarih < '2024-02-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-01-01' AND u.DokumTarih < '2024-02-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-01-01' AND u.DokumTarih < '2024-02-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedSubat(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-02-01' AND u.DokumTarih < '2024-03-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-02-01' AND p.CastDate < '2024-03-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-02-01' AND u.DokumTarih < '2024-03-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-02-01' AND u.DokumTarih < '2024-03-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-02-01' AND u.DokumTarih < '2024-03-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedMart(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-03-01' AND u.DokumTarih < '2024-04-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-03-01' AND p.CastDate < '2024-04-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-03-01' AND u.DokumTarih < '2024-04-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-03-01' AND u.DokumTarih < '2024-04-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-03-01' AND u.DokumTarih < '2024-04-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedNisan(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-04-01' AND u.DokumTarih < '2024-05-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-04-01' AND p.CastDate < '2024-05-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-04-01' AND u.DokumTarih < '2024-05-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-04-01' AND u.DokumTarih < '2024-05-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-04-01' AND u.DokumTarih < '2024-05-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedMayis(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-05-01' AND u.DokumTarih < '2024-06-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-05-01' AND p.CastDate < '2024-06-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-05-01' AND u.DokumTarih < '2024-06-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-05-01' AND u.DokumTarih < '2024-06-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-05-01' AND u.DokumTarih < '2024-06-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedHaziran(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-06-01' AND u.DokumTarih < '2024-07-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-06-01' AND p.CastDate < '2024-07-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-06-01' AND u.DokumTarih < '2024-07-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-06-01' AND u.DokumTarih < '2024-07-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-06-01' AND u.DokumTarih < '2024-07-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedTemmuz(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2024-07-01' AND u.DokumTarih < '2024-08-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2024-07-01' AND p.CastDate < '2024-08-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2024-07-01' AND u.DokumTarih < '2024-08-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-07-01' AND u.DokumTarih < '2024-08-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2024-07-01' AND u.DokumTarih < '2024-08-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedAgustos(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2023-08-01' AND u.DokumTarih < '2023-09-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2023-08-01' AND p.CastDate < '2023-09-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2023-08-01' AND u.DokumTarih < '2023-09-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-08-01' AND u.DokumTarih < '2023-09-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-08-01' AND u.DokumTarih < '2023-09-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedEylul(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2023-09-01' AND u.DokumTarih < '2023-10-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2023-09-01' AND p.CastDate < '2023-10-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2023-09-01' AND u.DokumTarih < '2023-10-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-09-01' AND u.DokumTarih < '2023-10-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-09-01' AND u.DokumTarih < '2023-10-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedEkim(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2023-10-01' AND u.DokumTarih < '2023-11-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2023-10-01' AND p.CastDate < '2023-11-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2023-10-01' AND u.DokumTarih < '2023-11-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-10-01' AND u.DokumTarih < '2023-11-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-10-01' AND u.DokumTarih < '2023-11-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedKasim(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2023-11-01' AND u.DokumTarih < '2023-12-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2023-11-01' AND p.CastDate < '2023-12-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2023-11-01' AND u.DokumTarih < '2023-12-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-11-01' AND u.DokumTarih < '2023-12-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-11-01' AND u.DokumTarih < '2023-12-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }

        public List<Product> LogEntryListBySelectedAralik(Product p)
        {
            List<Product> results = new List<Product>();
            try
            {
                cmd.CommandText = @"SELECT
                        Tanim,
                        MAX(CASE WHEN Tür = '.Döküm' THEN ToplamAdet ELSE 0 END) AS ['.Döküm'],
                        MAX(CASE WHEN Tür = '1.Kalite' THEN ToplamAdet ELSE 0 END) AS ['1.Kalite'],
                        MAX(CASE WHEN Tür = '2.Kalite' THEN ToplamAdet ELSE 0 END) AS ['2.Kalite'],
                        MAX(CASE WHEN Tür = 'Extra' THEN ToplamAdet ELSE 0 END) AS ['Extra'],
                        MAX(CASE WHEN Tür = 'Iskarta' THEN ToplamAdet ELSE 0 END) AS ['Iskarta'],
                        MAX(CASE WHEN Tür = 'Rötüş Hatası' THEN ToplamAdet ELSE 0 END) AS ['Rötüş Hatası'],
                    	MAX(CASE WHEN Tür = 'Döküm Hatası' THEN ToplamAdet ELSE 0 END) AS ['Döküm Hatası'],
						MAX(CASE WHEN Tür = 'İptal Kalıp' THEN ToplamAdet ELSE 0 END) AS ['İptal Kalıp']
                    FROM
                    (
                        SELECT Tür, Tanim, SUM(Adet) AS ToplamAdet
                        FROM
                        (
                            SELECT '.Döküm' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            WHERE u.DokumcuId = @employee AND u.DokumTarih >= '2023-12-01' AND u.DokumTarih < '2024-01-01'
                            GROUP BY kl.tanim
                            UNION ALL
                            SELECT kl.kaliteAd AS Tür, COUNT(*) AS Adet, kol.tanim AS Tanim
                            FROM Products p
                            LEFT JOIN kod_liste kol ON kol.Kimlik = p.ProductCode
                            LEFT JOIN kalite_liste kl ON kl.Kimlik = p.Quality
                            WHERE p.CastPersonal = @employee AND p.CastDate >= '2023-12-01' AND p.CastDate < '2024-01-01'
                            GROUP BY kl.kaliteAd, kol.tanim, p.Quality
                            UNION ALL
                            SELECT 'Rötüş Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM RotusTakip rt
                            JOIN UT_D_Urunler u ON rt.Barkod = u.BarkodNo
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN RotusHatalari rh ON rh.Id = rt.RotusHata_ID
                            WHERE u.DokumTarih >= '2023-12-01' AND u.DokumTarih < '2024-01-01' AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, rh.HataTanim
                    		UNION ALL
                    		SELECT 'Döküm Hatası' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-12-01' AND u.DokumTarih < '2024-01-01' AND (Durum = 'Fire') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
							UNION ALL
                    		SELECT 'İptal Kalıp' AS Tür, COUNT(*) AS Adet, kl.tanim AS Tanim
                            FROM UT_D_Urunler u
                            JOIN kod_liste kl ON kl.Kimlik = u.TezgahKalipId
                            JOIN UT_DokumHatalari dh ON dh.Id = u.HataId
                            WHERE u.DokumTarih >= '2023-12-01' AND u.DokumTarih < '2024-01-01' AND (Durum = 'iptal') AND u.DokumcuId = @employee
                            GROUP BY kl.tanim, dh.HataTanim
                        ) AS TumUrunler
                        GROUP BY Tür, Tanim
                    ) AS SourceTable
                    GROUP BY Tanim
                    ORDER BY Tanim ASC;";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@employee", p.CastingPersonalID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product result = new Product();
                        result.Type = !reader.IsDBNull(0) ? reader.GetString(0) : " "; // Tür
                        result.Definition = !reader.IsDBNull(1) ? reader.GetInt32(1).ToString() : " "; // Tanim
                        result.Count = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // ToplamAdet
                        result.Count1 = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0; // ToplamAdet
                        result.Count2 = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0; // ToplamAdet
                        result.Count3 = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0; // ToplamAdet
                        result.Count4 = !reader.IsDBNull(6) ? reader.GetInt32(6) : 0; // ToplamAdet
                        result.Count5 = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0; // ToplamAdet
                        result.Count6 = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0; // ToplamAdet

                        results.Add(result);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ya da uygun şekilde yönetin
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { con.Close(); }
        }


    }

}