using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SotaLogger
{
    public partial class MainForm : Form
    {
        List<qso> allQsos = new List<qso>();
        private qso[] tmpArray;
        public string mysqlInsert = "";
        public string csvExport = "";
        public MySqlConnection connection;


        public MainForm()
        {
            InitializeComponent();
        }

        private void dbConnectButton_Click(object sender, EventArgs e)
        {
            string dbName = "";
            switch (myCallComboBox.Text)
            {
                case "M0BLF/P": { dbName = "log4om_m0blf"; break; }
                case "MW0BLF/P": { dbName = "log4om_gw"; break; }
                case "MM0BLF/P": { dbName = "log4om_gm"; break; }
                case "F/M0BLF/P": { dbName = "log4om_f"; break; }
                case "FP/M0BLF/P": { dbName = "log4om_fp"; break; }
                case "ON/M0BLF/P": { dbName = "log4om_on"; break; }
                case "TF/M0BLF/P": { dbName = "log4om_tf"; break; }
            }
            if (dbName.Length > 1)
            {
                string server = Properties.Settings.Default.dbServer;
                string uid = Properties.Settings.Default.dbUsername;
                string password = Properties.Settings.Default.dbPassword;
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" + dbName + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
                connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    toolStripStatusLabel.Text = "Connected";
                }
                catch (MySqlException ex)
                {
                    //When handling errors, you can your application's response based 
                    //on the error number.
                    //The two most common error numbers when connecting are as follows:
                    //0: Cannot connect to server.
                    //1045: Invalid user name and/or password.
                    switch (ex.Number)
                    {
                        case 0:
                            MessageBox.Show("Cannot connect to server.");
                            break;

                        case 1045:
                            MessageBox.Show("Invalid username/password, please try again");
                            break;
                    }

                    toolStripStatusLabel.Text = "Failed connecting to database";
                }
            }
            else { MessageBox.Show("Database doesn't exist"); }
        }

        private void addQsoButton_Click(object sender, EventArgs e)
        {
            qso tmpQso = new qso();
            tmpQso.timeOn = dateTimePicker1.Value.Date;
            int hours = int.Parse(utcTextBox.Text.Substring(0, 2));
            int minutes = 0;
            if (utcTextBox.Text.Contains(':') )
            {
                minutes = int.Parse(utcTextBox.Text.Substring(3, 2));
            }
            else
            {
                minutes = int.Parse(utcTextBox.Text.Substring(2, 2));
            }
            tmpQso.timeOn = tmpQso.timeOn.AddHours((double)hours);
            tmpQso.timeOn = tmpQso.timeOn.AddMinutes((double)minutes);
            tmpQso.callsign = theirCallTextBox.Text.ToUpper();
            tmpQso.myCall = myCallComboBox.Text;
            tmpQso.rstTx = int.Parse(rstTxTextBox.Text);
            tmpQso.rstRx = int.Parse(rstRxTextBox.Text);
            tmpQso.sotaRef = sotaTextBox.Text;
            tmpQso.sotaRefRx = sotaRxTextBox.Text;
            tmpQso.grid = gridTextbox.Text;
            tmpQso.mode = modeComboBox.Text;
            tmpQso.equipmentString = equipmentComboBox.Text;
            tmpQso.parseEquipmentString();
            tmpQso.setFrequencyFromBandMode();
            allQsos.Add(tmpQso);
            drawQsoTable();
            if (sotaTextBox.Text.Length > 1)
            {
                databaseSaveButton.Enabled = true;
                exportCsvButton.Enabled = true;
            }
            mysqlInsert = mysqlInsert + " " + tmpQso.AddToMySqlString();
            if (csvExport.Length > 1)
            {
                csvExport = csvExport + "\r\n" + tmpQso.AddToCsvExport();
            }
            else
            {
                csvExport = tmpQso.AddToCsvExport();
            }
            utcTextBox.Focus();
            theirCallTextBox.Text = "";
        }

        private void drawQsoTable()
        {
            tmpArray = allQsos.ToArray();
            int line = 0;
            for (int i= tmpArray.Length; i> 0; i--)
            {
                if (i<= tmpArray.Length)
                {
                    qso tmpQso = tmpArray[ (i-1) ];
                    if (line ==7)
                    {
                        qso1Call.Text = tmpQso.callsign;
                        qso1Equipment.Text = tmpQso.equipmentString;
                        qso1Mode.Text = tmpQso.mode;
                        qso1RstRx.Text = tmpQso.rstRx.ToString();
                        qso1RstTx.Text = tmpQso.rstTx.ToString();
                        qso1UTC.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso1SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 6)
                    {
                        qso2Call.Text = tmpQso.callsign;
                        qso2Equipment.Text = tmpQso.equipmentString;
                        qso2Mode.Text = tmpQso.mode;
                        qso2RstRx.Text = tmpQso.rstRx.ToString();
                        qso2RstTx.Text = tmpQso.rstTx.ToString();
                        qso2Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso2SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 5)
                    {
                        qso3Call.Text = tmpQso.callsign;
                        qso3Equipment.Text = tmpQso.equipmentString;
                        qso3Mode.Text = tmpQso.mode;
                        qso3RstRx.Text = tmpQso.rstRx.ToString();
                        qso3RstTx.Text = tmpQso.rstTx.ToString();
                        qso3Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso3SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 4)
                    {
                        qso4Call.Text = tmpQso.callsign;
                        qso4Equipment.Text = tmpQso.equipmentString;
                        qso4Mode.Text = tmpQso.mode;
                        qso4RstRx.Text = tmpQso.rstRx.ToString();
                        qso4RstTx.Text = tmpQso.rstTx.ToString();
                        qso4Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso4SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 3)
                    {
                        qso5Call.Text = tmpQso.callsign;
                        qso5Equipment.Text = tmpQso.equipmentString;
                        qso5Mode.Text = tmpQso.mode;
                        qso5RstRx.Text = tmpQso.rstRx.ToString();
                        qso5RstTx.Text = tmpQso.rstTx.ToString();
                        qso5Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso5SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 2)
                    {
                        qso6Call.Text = tmpQso.callsign;
                        qso6Equipment.Text = tmpQso.equipmentString;
                        qso6Mode.Text = tmpQso.mode;
                        qso6RstRx.Text = tmpQso.rstRx.ToString();
                        qso6RstTx.Text = tmpQso.rstTx.ToString();
                        qso6Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso6SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 1)
                    {
                        qso7Call.Text = tmpQso.callsign;
                        qso7Equipment.Text = tmpQso.equipmentString;
                        qso7Mode.Text = tmpQso.mode;
                        qso7RstRx.Text = tmpQso.rstRx.ToString();
                        qso7RstTx.Text = tmpQso.rstTx.ToString();
                        qso7Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso7SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    else if (line == 0)
                    {
                        qso8Call.Text = tmpQso.callsign;
                        qso8Equipment.Text = tmpQso.equipmentString;
                        qso8Mode.Text = tmpQso.mode;
                        qso8RstRx.Text = tmpQso.rstRx.ToString();
                        qso8RstTx.Text = tmpQso.rstTx.ToString();
                        qso8Utc.Text = tmpQso.timeOn.ToString("HH:mm");
                        qso8SotaRx.Text = tmpQso.sotaRefRx;
                        Application.DoEvents();
                    }
                    line++;
                }
            }
        }

        private void databaseSaveButton_Click(object sender, EventArgs e)
        {
            Console.Write(mysqlInsert);
            MySqlCommand cmd = new MySqlCommand(mysqlInsert, connection);
            cmd.ExecuteNonQuery();
            toolStripStatusLabel.Text = "Saved in database OK";
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            if (connection.State.Equals(ConnectionState.Open))
            {
                connection.Close();
            }
        }

        private void exportCsvButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV Export|*.csv";
            saveFileDialog1.Title = "Save CSV Export";
            saveFileDialog1.FileName = "export.csv";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());
                writer.Write(csvExport);
                writer.Dispose();
                writer.Close();
            }
            toolStripStatusLabel.Text = "CSV file saved OK";
        }   
    }
}
