using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.Configuration;

namespace ProgettoSSD
{
    class Model
    {
        public delegate void viewEventHandler(object sender, string textToWrite); // questo gestisce l'evento
        public event viewEventHandler FlushText; // questo genera l'evanto
        public void doSomathing()
        {
            for (int i = 0; i < 10; i++)
            {
                FlushText(this, $"i={i}");
            }
        }

        public void readViaFactory(string connString, string factory, string idCliente)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(factory);
            using (DbConnection conn = dbFactory.CreateConnection()) {
                try
                { 
                    conn.ConnectionString = connString;
                    conn.Open();
                    DbDataAdapter dbAdapter = dbFactory.CreateDataAdapter();
                    DbCommand dbCommand = conn.CreateCommand();

                    //query parametrizzata -> così l'utente non può fare sql injection
                    string queryText = "SELECT id, codice, descr from ordini where idcliente =  @id";
                    dbCommand.CommandText = queryText;

                    IDbDataParameter param = dbCommand.CreateParameter();
                    param.DbType = DbType.Int32;
                    param.ParameterName = "@id";
                    param.Value = Int32.Parse(idCliente);
                    dbCommand.Parameters.Add(param);
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                            FlushText(this, "ID = " + reader["id"] + ", CODICE = " + reader["codice"] + ", DESCRIZIONE = " + reader["descr"]);
                    }

                } catch (Exception ex) { 
                    FlushText(this,"Error: " + ex.Message);
                } finally { 
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }

        //LA STRINGA DI CONNESSIONE NON DOVREBBE STARE DENTRO AL MODEL! => gliela passiamo
        //Però la gestione della connessione deve essere fatta dentro al MODEL
        public void readView(string sqLiteConnString, bool isSQLite, string idCliente)
        {
            /*è uguale che stiamo lavorando lato SQLite o SQLSErver, cambia solo la riga di connessione*/

            IDbConnection conn = null;
            try
            {
                if (isSQLite)
                    conn = new SQLiteConnection(sqLiteConnString);
                else
                {
                    //ci connettiamo al server che in realtà è la nostra macchina 
                    //=> deve un po' variare la stringa (connectionString = @"Data Source=tcp:137.204.74.181;)
                    //LE STRINGHE DI CONNESSIONE NON SI SCRIVONO DENTRO AL CODICE!
                    conn = new SqlConnection(sqLiteConnString);
                }
                
                conn.Open();
                IDbCommand com = conn.CreateCommand();
                //query parametrizzata -> così l'utente non può fare sql injection
                string queryText = "SELECT id, codice, descr from ordini where idcliente =  @id";
                com.CommandText = queryText;

                IDbDataParameter param = com.CreateParameter();
                param.DbType = DbType.Int32;
                param.ParameterName = "@id";
                param.Value = Int32.Parse(idCliente);
                com.Parameters.Add(param);
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                        FlushText(this, "ID = " + reader["id"] + ", CODICE = " + reader["codice"] + ", DESCRIZIONE = " + reader["descr"]);
                }

                /*IDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //FlushText(this, reader["ordini.id"] + " " + reader["ordini.idcliente"] + " " + reader["ordini.codice"] + " " + reader["ordini.descr"]);
                    FlushText(this, "ID = " + reader["id"] + ", CODICE = " + reader["codice"] + ", DESCRIZIONE = " + reader["descr"]);
                }
                reader.Close();*/
                conn.Close();
            } catch (Exception ex)
            {
                FlushText(this, ex.Message);
            } finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }


        //il metodo ritorna al controller una lista di ordini
        //è il controller che si occupa di cambiargli formato
        public List<ordini> readViaEF(int idCliente)
        {
            string queryText = "SELECT * from ordini where idcliente = " + idCliente;
            //testDbEntities context = new testDbEntities();
            List<ordini> ordList = new List<ordini>();
            /*foreach (ordini o in context.ordini)
            {
                if (o.idcliente.Equals(idCliente))
                    ordList.Add(o);

            }*/

            using (var context = new testDbEntities())
            {
                ordList = context.ordini.SqlQuery(queryText).ToList();
            }

            return ordList;

        }


    }
}
