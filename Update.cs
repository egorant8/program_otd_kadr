using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rab_sn
{
    public partial class Update : Form
    {
        static DataGridView dts = new DataGridView();
        static DataGridView dts1 = new DataGridView();

        Form1 fm = new Form1();
        public Update(string fam, string im, string otc, string dr, string id_humans, string numphone, DataGridView dw1, DataGridView dw2)
        {
            dts = dw1;
            dts1 = dw2;
            InitializeComponent();
            metroTextBox1.Text = fam;
            maskedTextBox1.Text = numphone;
            metroTextBox2.Text = im;
            metroTextBox3.Text = otc;
            DateTime dtr = Convert.ToDateTime(dr);
            dateTimePicker1.Value = dtr;
            var command = fm.ms.CreateCommand();
            command.CommandText = "SELECT * FROM `vred`,`humans_vred` where `id_humans` = " + id_humans + " and `humans_vred`.id_vred=`vred`.id";
            int i = 0;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    checkedListBox1.Invoke(new MethodInvoker(delegate {

                        checkedListBox1.Items.Add(reader[1]);
                        checkedListBox1.SetItemChecked(i, true);
                    }));
                    i++;
                }
                reader.Close();
            }
            command.Cancel();
            command = fm.ms.CreateCommand();
            command.CommandText = "SELECT * FROM `vred`";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    bool bv = false;
                    foreach (var item in checkedListBox1.Items)
                    {
                        if (item.ToString() == reader[1].ToString())
                        {
                            bv = true;
                        }
                    }
                    if (!bv)
                    {
                        checkedListBox1.Items.Add(reader[1]);
                    }
                    
                }
                reader.Close();
            }
            command.Cancel();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string id_humans = "";
            DateTime dt = dateTimePicker1.Value;
            var command = fm.ms.CreateCommand();
            command.CommandText = "SELECT * FROM `humans` WHERE `fam` = '" + metroTextBox1.Text + "' AND `im` = '" + metroTextBox2.Text + "' AND `otc` = '" + metroTextBox3.Text
                    + "' and `dr` = '" + dt.ToString("yyyy-MM-d") + "'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    id_humans = reader[0].ToString();
                    break;
                }
                reader.Close();
            }
            command.Cancel();

            //



            var commands = fm.ms.CreateCommand();
            commands.CommandText = "DELETE FROM `humans_vred` WHERE `humans_vred`.`id_humans` = " + id_humans;
            commands.ExecuteNonQuery();
            commands = fm.ms.CreateCommand();
            commands.CommandText = "UPDATE `humans` SET `fam` = '" + metroTextBox1.Text + "', `im` = '" + metroTextBox2.Text + "', `otc` = '" + metroTextBox3.Text
                    + "', `dr` = '" + dt.ToString("yyyy-MM-d") + "', numphone='"+maskedTextBox1.Text+"'  WHERE `humans`.`id_humans` = " + id_humans;
            commands.ExecuteNonQuery();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                commands = fm.ms.CreateCommand();
                commands.CommandText = "INSERT INTO `humans_vred` (`id`, `id_vred`, `id_humans`) VALUES (NULL, '" + id_vred(item.ToString()) + "', '" + id_humans + "');";
                commands.ExecuteNonQuery();
            }


            dts.Rows.Clear();
            command = fm.ms.CreateCommand();
            command.CommandText = "SELECT id_humans,fam,im,otc,dr,numphone FROM `humans` WHERE `id_comp` = " + Form1.id_comp + " ORDER BY `id_humans` ASC";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dts.Rows.Add(reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]);
                }
            }
            dts.Rows[0].Selected = true;
            try
            {
                dts1.Rows.Clear();
                command = fm.ms.CreateCommand();
                command.CommandText = "SELECT nazv, punkt FROM `humans`,`vred`, `humans_vred` WHERE `humans`.`id_comp` = " + Form1.id_comp + " and `humans`.id_humans = '" + dts.SelectedRows[0].Cells[0].Value + "' and `humans`.id_humans = `humans_vred`.id_humans and `humans_vred`.`id_vred`" +
                    "= `vred`.`id`";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dts1.Rows.Add(reader[0], reader[1]);
                    }
                }
            }
            catch (Exception)
            {

            }
            MessageBox.Show("Успешно изменён.");


            this.Close();
        }
        string id_vred(string name)
        {
            var command1 = fm.ms.CreateCommand();
            command1.CommandText = "SELECT * FROM `vred` where nazv='" + name + "'";
            string id = "";
            using (var reader1 = command1.ExecuteReader())
            {
                while (reader1.Read())
                {
                    //INSERT INTO `users` (`id_users`, `hash_wind`, `name_comp`) VALUES (NULL, '213123f', '1');

                    id = reader1[0].ToString();
                    break;
                }
                reader1.Close();
            }
            command1.Cancel();
            return id;
        }
    }
}
