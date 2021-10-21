using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rab_sn
{
    public partial class Form2 : Form
    {
        Form1 fm = new Form1();
        public Form2()
        {
            InitializeComponent();
            label1.Text = Form1.id_comp;
            Thread th = new Thread(sl);
            th.Start();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }
        void sl()
        {

            var command = fm.ms.CreateCommand();
            command.CommandText = "SELECT * FROM `vred`";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //INSERT INTO `users` (`id_users`, `hash_wind`, `name_comp`) VALUES (NULL, '213123f', '1');
                    checkedListBox1.Invoke(new MethodInvoker(delegate {

                        checkedListBox1.Items.Add(reader[1]);
                    }));
                }
                reader.Close();
            }
            command.Cancel();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*checkedListBox1.Items.Clear();
            var command = fm.ms.CreateCommand();
            command.CommandText = "SELECT * FROM `vred` where nazv like '" + textBox1.Text+"%'";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    checkedListBox1.Items.Add(reader[1]);
                }
            }*/
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker1.Value;
            if (metroTextBox1.Text == "" && metroTextBox2.Text == "" && metroTextBox3.Text == "" && dateTimePicker1.Value.ToString("d.MM.yyyy") == dt.ToString("d.MM.yyyy") &&checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Введите все поля");
            }
            else
            {
                //try
                {
                    if (true)
                    {

                    }
                    var command = fm.ms.CreateCommand();
                    command.CommandText = "SELECT * FROM `humans` WHERE `fam` = '" + metroTextBox1.Text + "' AND `im` = '" + metroTextBox2.Text + "' AND `otc` = '" + metroTextBox3.Text
                            + "' and `dr` = '" + dt.ToString("yyyy-MM-d") + "' and `id_status_proid` = 1";
                    bool found = false;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            found = true;
                        }
                        reader.Close();
                    }
                    command.Cancel();
                    if (found)
                    {
                        MessageBox.Show("Данный человек уже есть в базе.");
                    }
                    else
                    {
                        var commands = fm.ms.CreateCommand();
                        commands.CommandText = "INSERT INTO `humans` (`id_humans`, `fam`, `im`, `otc`, `dr`, `id_status_proid`, `numphone`,`id_comp`) VALUES (NULL, '" + metroTextBox1.Text + "', '" + metroTextBox2.Text + "', '" + metroTextBox3.Text
                            + "', '" + dt.ToString("yyyy-MM-d") + "', '1','" + maskedTextBox1.Text + "', "+label1.Text+");";
                        commands.ExecuteNonQuery();
                        vredAdd();
                        MessageBox.Show("Успешно добавлен человек");
                        metroTextBox1.Text = "";
                        metroTextBox2.Text = "";
                        metroTextBox3.Text = "";
                        DateTime dateTime = DateTime.Now;
                        dateTimePicker1.Value = dateTime;
                        maskedTextBox1.Text = "";

                    }
                }
                //catch (Exception ee)
                {
                    //MessageBox.Show(ee.Message);
                }
            }
           
        }

        void vredAdd()
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
            foreach (var item in checkedListBox1.CheckedItems)
            {
                var commands = fm.ms.CreateCommand();
                commands.CommandText = "INSERT INTO `humans_vred` (`id`, `id_vred`, `id_humans`) VALUES (NULL, '" + id_vred(item.ToString()) + "', '" + id_humans + "');";
                commands.ExecuteNonQuery();
            }
        }
        string id_vred(string name)
        {
            var command1 = fm.ms.CreateCommand();
            command1.CommandText = "SELECT * FROM `vred` where nazv='"+name+"'";
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
