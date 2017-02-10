using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using My.Parsing;

namespace Shared
{
    public static class Config
    {
        public static string CONNECTION_STRING =
            "Data Source=(local);Initial Catalog=Mitgliederverwaltung;Integrated Security=true;";

        //public static string CONNECTION_STRING =
        //    "Data Source=DESKTOP-6IHHQK8;Initial Catalog=Mitgliederverwaltung;User Id=sa;Password=sa";

        public static readonly CultureInfo Culture = new CultureInfo("de-AT");

        public static readonly DateParser ParserGeburtsdatum = new DateParser(Culture.LCID, DateFormat.ShortDate_yyyy, false,
            (entry) => 
            {
                DateTime today = DateTime.Today;
                return entry <= today && entry >= today.AddYears(-150);
            });

        public static readonly StringParser ParserGeschlecht = new StringParser(Culture.LCID, true, false,
            (entry) => 
            {
                entry  = entry.ToUpper();
                return entry == "M" || entry == "W";
            });

        public static readonly StringParser ParserEntryNotEmpty = new StringParser(Culture.LCID, true, false,
            (entry) => 
            {
                return entry != string.Empty;
            });
    }

    public static class Queries
    {
        private static Telefon[] DeserializeTelefon(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return new Telefon[0];

            XmlSerializer serializer = new XmlSerializer(typeof(Telefon[]));

            using (TextReader reader = new StringReader(xml))
            {
                return (Telefon[])serializer.Deserialize(reader);
            }
        }

        private static string SerializeTelefon(IList<Telefon> list)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            XmlSerializer serializer = new XmlSerializer(typeof(List<Telefon>));

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, list);
                return writer.ToString();
            }
        }

        public static ReadResult Read()
        {
            ReadResult readResult = new ReadResult();

            using (SqlConnection con = new SqlConnection(Config.CONNECTION_STRING))
            {
                con.Open();
                SqlCommand com = con.CreateCommand();

                com.CommandText =
                    //"WAITFOR DELAY '00:00:00';" +
                    "SELECT Id,Name,Ort,Strasse,Geburtsdatum,Geschlecht,Notiz,Telefon " +
                    "FROM Mitglied ORDER BY Name,Id";

                using (SqlDataReader rdr = com.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Mitglied mitglied = new Mitglied();
                        mitglied.Id = Convert.ToInt32(rdr["Id"]);
                        mitglied.Name = Convert.ToString(rdr["Name"]);
                        mitglied.Ort = Convert.ToString(rdr["Ort"]);
                        mitglied.Strasse = Convert.ToString(rdr["Strasse"]);
                        mitglied.Geburtsdatum = Convert.ToDateTime(rdr["Geburtsdatum"]);
                        mitglied.Geschlecht = Convert.ToString(rdr["Geschlecht"]);
                        mitglied.Notiz = Convert.ToString(rdr["Notiz"]);
                        mitglied.LstTelefon.AddRange(DeserializeTelefon(Convert.ToString(rdr["Telefon"])));
                        readResult.Mitglieder.Add(mitglied);
                    }
                }

                com.CommandText =
                    "SELECT ISNULL(M,0),ISNULL((100-M), 0) W FROM " +
                    "(SELECT ROUND((COUNT(Case when Geschlecht='M' THEN Geschlecht END)/" +
                    "NULLIF(CONVERT(FLOAT, COUNT(Geschlecht)),0)*100),2) M FROM Mitglied)TBL";

                using (SqlDataReader rdr = com.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        readResult.Maennlich = Convert.ToDouble(rdr[0]);
                        readResult.Weiblich = Convert.ToDouble(rdr[1]);
                    }
                }
            }

            return readResult;
        }

        public static void Save(Mitglied mitglied, out int id)
        {
            string xmlTelefon = SerializeTelefon(mitglied.LstTelefon);

            SqlParameter pId = new SqlParameter("@id", SqlDbType.Int);
            SqlParameter pName = new SqlParameter("@name", SqlDbType.NVarChar, 50);
            SqlParameter pOrt = new SqlParameter("@ort", SqlDbType.NVarChar, 50);
            SqlParameter pStrasse = new SqlParameter("@strasse", SqlDbType.NVarChar, 50);
            SqlParameter pGeburtsdatum = new SqlParameter("@geburtsdatum", SqlDbType.Date);
            SqlParameter pGeschlecht = new SqlParameter("@geschlecht", SqlDbType.NVarChar, 1);
            SqlParameter pNotiz = new SqlParameter("@notiz", SqlDbType.NVarChar, mitglied.Notiz.Length);
            SqlParameter pTelefon = new SqlParameter("@telefon", SqlDbType.NVarChar, xmlTelefon.Length);

            SqlCommand com = new SqlCommand();
            com.Parameters.Add(pId).Value = mitglied.Id;
            com.Parameters.Add(pName).Value = mitglied.Name;
            com.Parameters.Add(pOrt).Value = mitglied.Ort;
            com.Parameters.Add(pStrasse).Value = mitglied.Strasse;
            com.Parameters.Add(pGeburtsdatum).Value = mitglied.Geburtsdatum.Value;
            com.Parameters.Add(pGeschlecht).Value = mitglied.Geschlecht;
            com.Parameters.Add(pNotiz).Value = mitglied.Notiz;
            com.Parameters.Add(pTelefon).Value = xmlTelefon;

            if (mitglied.Id > 0)
            {
                com.CommandText =
                    "UPDATE Mitglied SET " +
                    "Name=@name," +
                    "Ort=@ort," +
                    "Strasse=@strasse," +
                    "Geburtsdatum=@geburtsdatum," +
                    "Geschlecht=@geschlecht," +
                    "Notiz=@notiz," +
                    "Telefon=@telefon " +
                    "WHERE ID=@id;" +
                    "SELECT @id";
            }
            else
            {
                com.CommandText =
                  "INSERT INTO Mitglied" +
                  "(Name,Ort,Strasse,Geburtsdatum,Geschlecht,Notiz,Telefon)" +
                  "VALUES" +
                  "(@name,@ort,@strasse,@geburtsdatum,@geschlecht,@notiz,@telefon);" +
                  "SELECT @@IDENTITY";
            }

            using (SqlConnection con = new SqlConnection(Config.CONNECTION_STRING))
            {
                con.Open();
                com.Connection = con;
                id = Convert.ToInt32(com.ExecuteScalar());
            }
        }

        public static void Delete(int id, out int affected)
        {
            SqlParameter pId = new SqlParameter("@id", SqlDbType.Int);

            SqlCommand com = new SqlCommand();
            com.Parameters.Add(pId).Value = id;

            com.CommandText =
                "DELETE FROM Mitglied WHERE ID=@id";

            using (SqlConnection con = new SqlConnection(Config.CONNECTION_STRING))
            {
                con.Open();
                com.Connection = con;
                affected = com.ExecuteNonQuery();
            }
        }
    }

    public class Mitglied
    {
        private int mId = -1;
        private string mName = string.Empty;
        private string mOrt = string.Empty;
        private string mStrasse = string.Empty;
        private DateTime? mGeburtsdatum = null;
        private string mGeschlecht = string.Empty;
        private string mNotiz = string.Empty;

        public int Id
        {
            get { return mId; }
            set { mId = value; }
        }
        public string Name
        {
            get { return mName; }
            set { mName = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }
        public string Ort
        {
            get { return mOrt; }
            set { mOrt = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }
        public string Strasse
        {
            get { return mStrasse; }
            set { mStrasse = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }
        public DateTime? Geburtsdatum
        {
            get { return mGeburtsdatum; }
            set { mGeburtsdatum = value; }
        }
        public string Geschlecht
        {
            get { return mGeschlecht; }
            set { mGeschlecht = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }
        public string Notiz
        {
            get { return mNotiz; }
            set { mNotiz = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }
        public List<Telefon> LstTelefon { get; set; }

        public Mitglied()
        {
            LstTelefon = new List<Telefon>(5);
        }
    }

    public class Telefon
    {
        private string mNummer = string.Empty;

        public string Nummer
        {
            get { return mNummer; }
            set { mNummer = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }

        public Telefon() { }
  
        public Telefon(string nummer)
        {
            this.Nummer = nummer;
        }

        public Telefon GetClone()
        {
            return new Telefon(mNummer);
        }
    }

    public class ReadResult
    {
        public double Maennlich { get; set; }
        public double Weiblich { get; set; }
        public List<Mitglied> Mitglieder { get; private set; }

        public ReadResult()
        {
            Mitglieder = new List<Mitglied>(100);
        }
    }
}
