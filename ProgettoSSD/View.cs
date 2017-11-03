using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace ProgettoSSD
{
    public partial class View : Form
    {
        /*public View()
        {
            InitializeComponent();
        }*/

        Controller C = new Controller();
        public View()
        {
            InitializeComponent();
            C.FlushText += viewEventHandler; // associo il codice all'handler nella applogic
            string sdb = ConfigurationManager.AppSettings["isSQLite"];
            if (sdb == "1")
                rbSQLite.Checked = true;
            else
                rbSQLServer.Checked = true;
        }
        private void viewEventHandler(object sender, string textToWrite)
        {
            txtConsole.AppendText(textToWrite + Environment.NewLine);
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            C.doSomething(); // logica applicative }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            C.readView(rbSQLite.Checked, clientID.Text);
        }

        private void btnReadFactory_Click(object sender, EventArgs e)
        {
            C.readViaFactory(rbSQLite.Checked, clientID.Text);
        }

        private void btnReadEF_Click(object sender, EventArgs e)
        {
            C.readViaEF(rbSQLite.Checked, clientID.Text);
        }

        private void btnPrevisione_Click(object sender, EventArgs e)
        {
            C.previsione(rbSQLite.Checked, clientID.Text);
        }
    }

}
