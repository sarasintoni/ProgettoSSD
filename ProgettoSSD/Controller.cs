using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace ProgettoSSD
{
    class Controller
    {
        Model W = new Model();
        public delegate void viewEventHandler(object sender, string textToWrite); // questo gestisce l'evento
        public event viewEventHandler FlushText; // questo genera l'evanto

        public Controller() {
            W.FlushText += controllerViewEventHandler;
        }

        private void controllerViewEventHandler(object sender, string textToWrite){
            FlushText(this, textToWrite);
        }

        public void doSomething(){
            W.doSomathing();
        }

        public void readView(bool isSQLLite, string idCliente)
        {
            string connString;
            //dovrà connettersi a DB divesi (lo decide l'utente)

            //@"Data Source=H:\dbGAP.sqlite;Version=3;"; //è molto dipendendente dalla tecnologia
            if (isSQLLite)
            {
                connString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
            } else {
                connString = ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;

            }

            W.readView(connString, isSQLLite, idCliente);
        }

        public void readViaFactory(bool isSQLLite, string idCliente)
        {
            string fact, connString;
            //dovrà connettersi a DB divesi (lo decide l'utente)

            //@"Data Source=H:\dbGAP.sqlite;Version=3;"; //è molto dipendendente dalla tecnologia
            if (isSQLLite)
            {
                connString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
                fact = "System.Data.SQLite";
            }
            else
            {
                connString = ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;
                fact = "System.Data.SqlClient";

            }

            W.readViaFactory(connString, fact, idCliente);
        }

        public void readViaEF(bool isSQLLite, string idCliente)
        {
            if (isSQLLite)
            {
                FlushText(this, "Database not supported");
            }
            else
            {
                List<ordini> ordList = W.readViaEF(Convert.ToInt32(idCliente));
                string jData = JsonConvert.SerializeObject(ordList);
                FlushText(this, jData);
            }
        }
    }
}
