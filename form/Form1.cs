using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace rab_sn
{
    public partial class Form1 : Form
    {
        bool dostup = false;
        public static string id_comp;
        public MySqlConnection ms;
        string sd()
        {
            string hash_PC = "null";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    hash_PC = queryObj["ProcessorId"].ToString();
                    //queryObj["ProcessorId"]
                    var command = ms.CreateCommand();
                    command.CommandText = "SELECT * FROM `users` WHERE `hash_wind` = '" + queryObj["ProcessorId"] + "'";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //INSERT INTO `users` (`id_users`, `hash_wind`, `name_comp`) VALUES (NULL, '213123f', '1');
                            id_comp = reader[2].ToString();
                            dostup = true;
                        }
                    }
                    command.Cancel();

                    command = ms.CreateCommand();
                    command.CommandText = "SELECT id_humans,fam,im,otc,dr,numphone FROM `humans` WHERE `id_comp` = " + id_comp + " ORDER BY `id_humans` ASC";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                        }
                    }
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            }
            return hash_PC;
        }

        public Form1()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Database = "otdel_profosmotr",
                UserID = "mysql",
                Password = "mysql",
                SslMode = MySqlSslMode.None,
            };
            InitializeComponent();
            ms = new MySqlConnection(builder.ConnectionString);
            ms.Open();
            string hash =sd();
            if (!dostup)
            {
                Clipboard.SetData(DataFormats.Text, (Object)hash);
                MessageBox.Show("Вы не зарегестированы в системе, ваш код скопирован в буфер обмена.");
                Environment.Exit(1);
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            Form2 fm = new Form2();
            fm.ShowDialog();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            Update fm = new Update(dataGridView1.SelectedRows[0].Cells[1].Value.ToString(), dataGridView1.SelectedRows[0].Cells[2].Value.ToString(), dataGridView1.SelectedRows[0].Cells[3].Value.ToString(), dataGridView1.SelectedRows[0].Cells[4].Value.ToString(), dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), dataGridView1.SelectedRows[0].Cells[5].Value.ToString(),dataGridView1,dataGridView2);
            fm.ShowDialog();
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1 )
            {
                if (MessageBox.Show("Вы хотите удалить - " + dataGridView1.SelectedRows[0].Cells[1].Value + " " + dataGridView1.SelectedRows[0].Cells[2].Value + "\nна отправку профосмотров?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    var commands = ms.CreateCommand();
                    var command = ms.CreateCommand();
                    try
                    {
                        commands.CommandText = "DELETE FROM `humans` WHERE `humans`.`id_humans` = " + dataGridView1.SelectedRows[0].Cells[0].Value;
                        commands.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        commands.CommandText = "DELETE FROM `humans_vred` WHERE `humans_vred`.`id_humans` = " + dataGridView1.SelectedRows[0].Cells[0].Value;
                        commands.ExecuteNonQuery();
                        commands.CommandText = "DELETE FROM `humans` WHERE `humans`.`id_humans` = " + dataGridView1.SelectedRows[0].Cells[0].Value;
                        commands.ExecuteNonQuery();

                    }
                    dataGridView1.Rows.Clear();
                    command.CommandText = "SELECT id_humans,fam,im,otc,dr,numphone FROM `humans` WHERE `id_comp` = " + id_comp + " ORDER BY `id_humans` ASC";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                        }
                    }
                    dataGridView1.Rows[0].Selected = true;
                }
            }
            else
            {
               MessageBox.Show("Выберите только одного человека!");
               
            }
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {

        }

        private void metroButton6_Click(object sender, EventArgs e)
        {

        }

        private void неОтправленныеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ms.Close();
            Environment.Exit(1);
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView2.Rows.Clear();
                var command = ms.CreateCommand();
                command.CommandText = "SELECT nazv, punkt FROM `humans`,`vred`, `humans_vred` WHERE `humans`.`id_comp` = " + id_comp + " and `humans`.id_humans = '" + dataGridView1.SelectedRows[0].Cells[0].Value + "' and `humans`.id_humans = `humans_vred`.id_humans and `humans_vred`.`id_vred`" +
                    "= `vred`.`id`";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader[0], reader[1]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        public void UpdateDbGr()
        {

            dataGridView1.Rows.Clear();
            var command = ms.CreateCommand();
            command.CommandText = "SELECT id_humans,fam,im,otc,dr,numphone FROM `humans` WHERE `id_comp` = " + Form1.id_comp + " ORDER BY `id_humans` ASC";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                }
            }
            dataGridView1.Rows[0].Selected = true;
            try
            {
                dataGridView2.Rows.Clear();
                command = ms.CreateCommand();
                command.CommandText = "SELECT nazv, punkt FROM `humans`,`vred`, `humans_vred` WHERE `humans`.`id_comp` = " + Form1.id_comp + " and `humans`.id_humans = '" + dataGridView1.SelectedRows[0].Cells[0].Value + "' and `humans`.id_humans = `humans_vred`.id_humans and `humans_vred`.`id_vred`" +
                    "= `vred`.`id`";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader[0], reader[1]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void metroButton8_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();

                var command = ms.CreateCommand();
                command.CommandText = "SELECT id_humans,fam,im,otc,dr,numphone FROM `humans` WHERE `id_comp` = " + id_comp + " ORDER BY `id_humans` ASC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                    }
                }
            }
            catch (Exception)
            {

            }
            try
            {
                dataGridView2.Rows.Clear();
                var command = ms.CreateCommand();
                command.CommandText = "SELECT nazv, punkt FROM `humans`,`vred`, `humans_vred` WHERE `humans`.`id_comp` = " + id_comp + " and `humans`.id_humans = '" + dataGridView1.SelectedRows[0].Cells[0].Value + "' and `humans`.id_humans = `humans_vred`.id_humans and `humans_vred`.`id_vred`" +
                    "= `vred`.`id`";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(reader[0], reader[1]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
