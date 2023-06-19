using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Nikitin_autoremont
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void hide_all_windows()
        {
            panelAuthorizationMain.Visible = false;
            panelSidebarMechanic.Visible = false;
            panelMainMechanic.Visible = false;
            panelSidebarCashier.Visible = false;
            panelOrdersCashier.Visible = false;
            panelOrdersAdministrator.Visible = false;
            panelServicesAdministrator.Visible = false;
            panelStorageAdministrator.Visible = false;
            panelWorkersAdministrator.Visible = false;
            panelAuthorizationAdministrator.Visible = false;
            panelSidebarAdministrator.Visible = false;
            panelSidebarAccountant.Visible = false;
            panelMainAccountant.Visible = false;
            panelMainCashier.Visible = false;
        }

        private void buttonAuthorizationEnter_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM authorization WHERE `Login` = @login and `Password` = @password", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cmd.Parameters.AddWithValue("login",textBoxAuthorizationLogin.Text);
            cmd.Parameters.AddWithValue("password", textBoxAuthorizationPassword.Text);
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            string levelOfAccess;
            if (table.Rows.Count > 0)
            {
                hide_all_windows();
                string welcomemsg;
                using (MySqlCommand fetchNames = new MySqlCommand("SELECT * FROM workers WHERE `ID_of_worker`="+Convert.ToString(table.Rows[0].ItemArray[3]))){
                    fetchNames.Connection = conn;
                    fetchNames.CommandType = CommandType.Text;
                    using (DataTable tableNames = new DataTable())
                    {
                        using (MySqlDataAdapter sdaNames = new MySqlDataAdapter(fetchNames))
                        {
                            sdaNames.Fill(tableNames);
                            welcomemsg = "Добро пожаловать, " + Convert.ToString(tableNames.Rows[0].ItemArray[2]) + " " + Convert.ToString(tableNames.Rows[0].ItemArray[3]);
                        }
                    }
                }
                textBoxAuthorizationLogin.Text = "";
                textBoxAuthorizationPassword.Text = "";
                levelOfAccess = Convert.ToString(table.Rows[0].ItemArray[4]);
                MessageBox.Show(welcomemsg, "Приветствие", MessageBoxButtons.OK);
                switch (levelOfAccess)
                {
                    case "0": panelSidebarAdministrator.Visible = true; panelMainAdministrator.Visible = true; panelMainAdministrator.BringToFront(); break;
                    case "1": panelSidebarMechanic.Visible = true; panelMainMechanic.Visible = true; panelMainMechanic.BringToFront(); break;
                    case "2": panelSidebarCashier.Visible = true; panelMainCashier.Visible = true; panelMainCashier.BringToFront(); break;
                    case "3": panelSidebarAccountant.Visible = true; panelMainAccountant.Visible = true; panelMainAccountant.BringToFront(); break;
                    default: ; break;
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль, пожалуйста, попробуйте снова или обратитесь к администратору", "Ошибка авторизации", MessageBoxButtons.OK);
            }
        }

        private void buttonShowPassword_Click(object sender, EventArgs e)
        {
            if (textBoxAuthorizationPassword.PasswordChar == '*')
            {
                textBoxAuthorizationPassword.PasswordChar = '\0';
            }
            else
            {
                textBoxAuthorizationPassword.PasswordChar = '*';
            }
        }

        public void logout()
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                hide_all_windows();
                panelAuthorizationMain.Visible = true;
                panelAuthorizationMain.BringToFront();
            }
        }
        private void buttonLogOutMechanic_Click(object sender, EventArgs e)
        {
            logout();
        }

        private void textBoxAuthorizationPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && panelAuthorizationMain.Visible == true)
            {
                buttonAuthorizationEnter.PerformClick();
            }
        }

        private void buttonOrdersMechanic_Click(object sender, EventArgs e)
        {
            panelMainMechanic.Visible = true;
            dataGridViewOrdersMechanic.Visible = true;
            buttonConfirmComplitionMechanic.Visible = true;
            dataGridViewStorageMechanic.Visible = false;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT orders.ID_of_order, orders.Name_of_client as `Имя клиента`, orders.Surname_of_client as `Фамилия клиента`, orders.Phone_number_of_client as `Номер телефона клиента`, orders.Car_number as `Номер автомобиля`, orders.Date_of_complition as `Крайний срок сдачи заказа`, services.Name_of_service as `Услуги` FROM orders INNER JOIN `orders-services` ON orders.ID_of_order=`orders-services`.ID_of_order INNER JOIN services ON `orders-services`.ID_of_service=services.ID_of_service WHERE orders.Is_order_complete = 0", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            dataGridViewOrdersMechanic.DataSource = table;
            dataGridViewOrdersMechanic.Columns[0].Visible = false;
        }

        private void buttonStorageMechanic_Click(object sender, EventArgs e)
        {
            dataGridViewOrdersMechanic.Visible = false;
            buttonConfirmComplitionMechanic.Visible = false;
            dataGridViewStorageMechanic.Visible = true;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT Name_of_part as `Название детали`, Remaining_amount as `Осталось на складе:` FROM `storage`", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            dataGridViewStorageMechanic.DataSource = table;
        }

        private void buttonConfirmComplitionMechanic_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrdersMechanic.Rows.Count == 0)
            {
                MessageBox.Show("В данный момент нет заказов", "Ошибка", MessageBoxButtons.OK);
            }
            else
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите подтвердить выполнение заказа? Это действие нельзя отменить", "Подтверждение", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    MySqlConnection conn = DBUtils.GetDBConnection();
                    MySqlCommand cmd = new MySqlCommand("UPDATE orders set Is_order_complete=1 where ID_of_order = " + Convert.ToString(dataGridViewOrdersMechanic.Rows[dataGridViewOrdersMechanic.CurrentCell.RowIndex].Cells[0].Value), conn);
                    MySqlCommand getparts = new MySqlCommand("SELECT `storage-service`.ID_of_part, `storage-service`.Required_amount FROM `storage-service` INNER JOIN `orders-services` ON `storage-service`.ID_of_service = `orders-services`.ID_of_service WHERE ID_of_order = " + Convert.ToString(dataGridViewOrdersMechanic.CurrentRow.Cells[0].Value), conn);
                    DataTable tableservices = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(getparts);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    adapter.Fill(tableservices);
                    foreach (DataRow row in tableservices.Rows)
                    {
                        MySqlCommand minusparts = new MySqlCommand("UPDATE storage SET remaining_amount = remaining_amount - "+Convert.ToString(row[1])+" WHERE ID_of_part = "+Convert.ToString(row[0]), conn);
                        minusparts.ExecuteNonQuery();
                    }
                    conn.Close();
                    buttonOrdersMechanic.PerformClick();
                }
            }
        }

        private void buttonLogOutCashier_Click(object sender, EventArgs e)
        {
            logout();
        }

        public class StringItemWithOldID
        {
            public int OldID;
            public string text;
            public override string ToString()
            {
                return this.text;
            }
        }

        private void buttonOrdersCashier_Click(object sender, EventArgs e)
        {
            dataGridViewStorageServicesCashier.Visible = false;
            panelOrdersCashier.Visible = true;
            textBoxNameCashier.Text = "";
            textBoxSurnameCashier.Text = "";
            maskedTextBoxCarnCashier.Text = "";
            maskedTextBoxPhoneCashier.Text = "";
            dateTimePickerCashier.Value = DateTime.Today;
            checkedListBoxServicesCashier.Items.Clear();
            checkedListBoxWorkersCashier.Items.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_service,Name_of_service FROM services", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checkedListBoxServicesCashier.Items.Add(new StringItemWithOldID()
                            {
                                OldID = Convert.ToInt16(row[0]),
                                text = Convert.ToString(row[1])
                            });
                        }
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_worker,CONCAT(Name,' ',Surname) FROM workers WHERE Profession = 'Механик'", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checkedListBoxWorkersCashier.Items.Add(new StringItemWithOldID()
                            {
                                OldID = Convert.ToInt16(row[0]),
                                text = Convert.ToString(row[1])
                            });
                        }
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_order, Name_of_client as `Имя клиента`, Surname_of_client as `Фамилия клиента`, Phone_number_of_client as `Номер телефона клиента`, Car_number as `Номер автомобиля`, Date_of_complition as `Крайний срок сдачи заказа` FROM orders", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewOrdersCashier.DataSource = dt;
                        dataGridViewOrdersCashier.Columns[0].Visible = false;
                    }
                }
            }
        }

        private void dataGridViewOrdersCashier_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (int i in checkedListBoxServicesCashier.CheckedIndices)
            {
                checkedListBoxServicesCashier.SetItemCheckState(i, CheckState.Unchecked);
            }
            foreach (int i in checkedListBoxWorkersCashier.CheckedIndices)
            {
                checkedListBoxWorkersCashier.SetItemCheckState(i, CheckState.Unchecked);
            }
            if (Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value) != "")
            {
                textBoxNameCashier.Text = Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[1].Value);
                textBoxSurnameCashier.Text = Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[2].Value);
                maskedTextBoxCarnCashier.Text = Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[4].Value);
                dateTimePickerCashier.Value = Convert.ToDateTime(dataGridViewOrdersCashier.CurrentRow.Cells[5].Value);
                maskedTextBoxPhoneCashier.Text = Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[3].Value);
                MySqlConnection conn = DBUtils.GetDBConnection();
                using (MySqlCommand cmd = new MySqlCommand("SELECT CONCAT(workers.`Name`,' ',workers.Surname) as `Работники` FROM `orders-workers` INNER JOIN workers ON `orders-workers`.ID_of_responsible=workers.ID_of_worker WHERE ID_of_order="+dataGridViewOrdersCashier.Rows[dataGridViewOrdersCashier.CurrentCell.RowIndex].Cells[0].Value, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int i = 0; i < checkedListBoxWorkersCashier.Items.Count; i++){
                                    if (Convert.ToString(row.ItemArray[0]) == Convert.ToString(checkedListBoxWorkersCashier.Items[i]))
                                    {
                                        checkedListBoxWorkersCashier.SetItemChecked(i,true);
                                    }
                                }  
                            }
                        }
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand("SELECT services.Name_of_service as `Название услуги` FROM `orders-services` INNER JOIN services ON `orders-services`.ID_of_service=services.ID_of_service WHERE ID_of_order=" + dataGridViewOrdersCashier.Rows[dataGridViewOrdersCashier.CurrentCell.RowIndex].Cells[0].Value, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int i = 0; i < checkedListBoxServicesCashier.Items.Count; i++)
                                {
                                    if (Convert.ToString(row.ItemArray[0]) == Convert.ToString(checkedListBoxServicesCashier.Items[i]))
                                    {
                                        checkedListBoxServicesCashier.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                textBoxNameCashier.Text = "";
                textBoxSurnameCashier.Text = "";
                maskedTextBoxCarnCashier.Text = "";
                dateTimePickerCashier.Value = DateTime.Today;
                maskedTextBoxPhoneCashier.Text = "";
                checkedListBoxServicesCashier.ClearSelected();
                checkedListBoxWorkersCashier.ClearSelected();
                foreach (int i in checkedListBoxServicesCashier.CheckedIndices)
                {
                    checkedListBoxServicesCashier.SetItemCheckState(i, CheckState.Unchecked);
                }
                foreach (int i in checkedListBoxWorkersCashier.CheckedIndices)
                {
                    checkedListBoxWorkersCashier.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }


        private void buttonSaveCashier_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE orders SET Date_of_complition = @date, Name_of_client = @name, Surname_of_client = @surname, Phone_number_of_client = @phone, Is_order_complete = '0', Car_number = @carn WHERE ID_of_order = @ID", conn);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                cmd.Parameters.AddWithValue("date", (dateTimePickerCashier.Value));
                cmd.Parameters.AddWithValue("name", textBoxNameCashier.Text);
                cmd.Parameters.AddWithValue("surname", textBoxSurnameCashier.Text);
                cmd.Parameters.AddWithValue("phone", maskedTextBoxPhoneCashier.Text);
                cmd.Parameters.AddWithValue("carn", maskedTextBoxCarnCashier.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                MySqlCommand clearservices = new MySqlCommand("DELETE FROM `orders-services` WHERE ID_of_order = @id",conn);
                clearservices.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                clearservices.ExecuteNonQuery();
                MySqlCommand clearworkers = new MySqlCommand("DELETE FROM `orders-workers` WHERE ID_of_order = @id",conn);
                clearworkers.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                clearworkers.ExecuteNonQuery();
                foreach (StringItemWithOldID item in checkedListBoxServicesCashier.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-services` VALUES (@order,@service)", conn);
                    cmdlist.Parameters.AddWithValue("order", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                    cmdlist.Parameters.AddWithValue("service", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                foreach (StringItemWithOldID item in checkedListBoxWorkersCashier.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-workers` VALUES (@order,@worker)", conn);
                    cmdlist.Parameters.AddWithValue("order", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                    cmdlist.Parameters.AddWithValue("worker", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                conn.Close();
                buttonOrdersCashier.PerformClick();
            }
        }

        private void buttonCreateCashier_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать новый заказ используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes){
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO orders (Date_of_complition, Name_of_client, Surname_of_client, Phone_number_of_client, Is_order_complete, Car_number) VALUES (@date, @name, @surname, @phone, '0', @carn)",conn);
                cmd.Parameters.AddWithValue("date", (dateTimePickerCashier.Value));
                cmd.Parameters.AddWithValue("name", textBoxNameCashier.Text);
                cmd.Parameters.AddWithValue("surname", textBoxSurnameCashier.Text);
                cmd.Parameters.AddWithValue("phone", maskedTextBoxPhoneCashier.Text);
                cmd.Parameters.AddWithValue("carn", maskedTextBoxCarnCashier.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                foreach (StringItemWithOldID item in checkedListBoxServicesCashier.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-services` VALUES (@order,@service)", conn);
                    cmdlist.Parameters.AddWithValue("order", cmd.LastInsertedId.ToString());
                    cmdlist.Parameters.AddWithValue("service", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                foreach (StringItemWithOldID item in checkedListBoxWorkersCashier.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-workers` VALUES (@order,@worker)", conn);
                    cmdlist.Parameters.AddWithValue("order", cmd.LastInsertedId.ToString());
                    cmdlist.Parameters.AddWithValue("worker", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                conn.Close();
                buttonOrdersCashier.PerformClick();
            }
        }

        private void buttonDeleteCashier_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM orders WHERE ID_of_order = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersCashier.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonOrdersCashier.PerformClick();
            }
        }

        private void buttonStorageCashier_Click(object sender, EventArgs e)
        {
            dataGridViewStorageServicesCashier.Visible = true;
            panelOrdersCashier.Visible = false;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT Name_of_part as `Название детали`, Remaining_amount as `Осталось на складе:` FROM storage", conn);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridViewStorageServicesCashier.DataSource = dt;
            conn.Close();
        }

        private void buttonServicesCashier_Click(object sender, EventArgs e)
        {
            dataGridViewStorageServicesCashier.Visible = true;
            panelOrdersCashier.Visible = false;
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT Name_of_service as `Название услуги`, Price as `Стоимость руб.`, Recommended_time_days as `Среднее время выполнения (Дней)` FROM services", conn);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridViewStorageServicesCashier.DataSource = dt;
            conn.Close();
        }

        private void buttonLogOutAccountant_Click(object sender, EventArgs e)
        {
            logout();
        }

        private void buttonOrdersAccountant_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT orders.Is_order_complete as `Выполнен ли заказ`, orders.Name_of_client as `Имя клиента`, orders.Surname_of_client as `Фамилия клиента`, orders.Phone_number_of_client as `Номер телефона клиента`, orders.Car_number as `Номер автомобиля`, orders.Date_of_complition as `Крайний срок сдачи заказа`, services.Name_of_service as `Услуги`, services.Price as `Цена` FROM orders INNER JOIN `orders-services` ON orders.ID_of_order=`orders-services`.ID_of_order INNER JOIN services ON `orders-services`.ID_of_service=services.ID_of_service", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            dataGridViewAccountant.DataSource = table;
        }

        private void buttonStorageAccountant_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT Name_of_part as `Название детали`, Remaining_amount as `Осталось на складе:` FROM `storage`", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            dataGridViewAccountant.DataSource = table;
        }

        private void buttonWorkersAccountant_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT Profession as `Профессия`, Name as `Имя`, Surname as `Фамилия`, Salary as `Зарплата` FROM workers", conn);
            conn.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            dataGridViewAccountant.DataSource = table;
        }

        private void buttonLogOutAdministrator_Click(object sender, EventArgs e)
        {
            logout();
        }

        private void buttonOrdersCreateAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать новый заказ используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO orders (Date_of_complition, Name_of_client, Surname_of_client, Phone_number_of_client, Is_order_complete, Car_number) VALUES (@date, @name, @surname, @phone, '0', @carn)", conn);
                cmd.Parameters.AddWithValue("date", (dateTimePickerOrdersAdministrator.Value));
                cmd.Parameters.AddWithValue("name", textBoxOrdersNameAdministrator.Text);
                cmd.Parameters.AddWithValue("surname", textBoxOrdersSurnameAdministrator.Text);
                cmd.Parameters.AddWithValue("phone", maskedTextBoxOrdersPhoneAdministrator.Text);
                cmd.Parameters.AddWithValue("carn", maskedTextBoxOrdersCarnAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                foreach (StringItemWithOldID item in checkedListBoxOrdersServicesAdministrator.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-services` VALUES (@order,@service)", conn);
                    cmdlist.Parameters.AddWithValue("order", cmd.LastInsertedId.ToString());
                    cmdlist.Parameters.AddWithValue("service", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                foreach (StringItemWithOldID item in checkedListBoxOrdersWorkersAdministrator.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-workers` VALUES (@order,@worker)", conn);
                    cmdlist.Parameters.AddWithValue("order", cmd.LastInsertedId.ToString());
                    cmdlist.Parameters.AddWithValue("worker", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                conn.Close();
                buttonOrdersAdministrator.PerformClick();
            }
        }

        private void dataGridViewOrdersAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (int i in checkedListBoxOrdersServicesAdministrator.CheckedIndices)
            {
                checkedListBoxOrdersServicesAdministrator.SetItemCheckState(i, CheckState.Unchecked);
            }
            foreach (int i in checkedListBoxOrdersWorkersAdministrator.CheckedIndices)
            {
                checkedListBoxOrdersWorkersAdministrator.SetItemCheckState(i, CheckState.Unchecked);
            }
            if (Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxOrdersNameAdministrator.Text = Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[1].Value);
                textBoxOrdersSurnameAdministrator.Text = Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[2].Value);
                maskedTextBoxOrdersCarnAdministrator.Text = Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[4].Value);
                dateTimePickerOrdersAdministrator.Value = Convert.ToDateTime(dataGridViewOrdersAdministrator.CurrentRow.Cells[5].Value);
                maskedTextBoxOrdersPhoneAdministrator.Text = Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[3].Value);
                MySqlConnection conn = DBUtils.GetDBConnection();
                using (MySqlCommand cmd = new MySqlCommand("SELECT CONCAT(workers.`Name`,' ',workers.Surname) as `Работники` FROM `orders-workers` INNER JOIN workers ON `orders-workers`.ID_of_responsible=workers.ID_of_worker WHERE ID_of_order=" + dataGridViewOrdersAdministrator.Rows[dataGridViewOrdersAdministrator.CurrentCell.RowIndex].Cells[0].Value, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int i = 0; i < checkedListBoxOrdersWorkersAdministrator.Items.Count; i++)
                                {
                                    if (Convert.ToString(row.ItemArray[0]) == Convert.ToString(checkedListBoxOrdersWorkersAdministrator.Items[i]))
                                    {
                                        checkedListBoxOrdersWorkersAdministrator.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand("SELECT services.Name_of_service as `Название услуги` FROM `orders-services` INNER JOIN services ON `orders-services`.ID_of_service=services.ID_of_service WHERE ID_of_order=" + dataGridViewOrdersAdministrator.Rows[dataGridViewOrdersAdministrator.CurrentCell.RowIndex].Cells[0].Value, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int i = 0; i < checkedListBoxOrdersServicesAdministrator.Items.Count; i++)
                                {
                                    if (Convert.ToString(row.ItemArray[0]) == Convert.ToString(checkedListBoxOrdersServicesAdministrator.Items[i]))
                                    {
                                        checkedListBoxOrdersServicesAdministrator.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                textBoxOrdersNameAdministrator.Text = "";
                textBoxOrdersSurnameAdministrator.Text = "";
                maskedTextBoxOrdersCarnAdministrator.Text = "";
                dateTimePickerOrdersAdministrator.Value = DateTime.Today;
                maskedTextBoxOrdersPhoneAdministrator.Text = "";
                checkedListBoxOrdersServicesAdministrator.ClearSelected();
                checkedListBoxOrdersWorkersAdministrator.ClearSelected();
                foreach (int i in checkedListBoxOrdersServicesAdministrator.CheckedIndices)
                {
                    checkedListBoxOrdersServicesAdministrator.SetItemCheckState(i, CheckState.Unchecked);
                }
                foreach (int i in checkedListBoxOrdersWorkersAdministrator.CheckedIndices)
                {
                    checkedListBoxOrdersWorkersAdministrator.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        private void buttonOrdersAdministrator_Click(object sender, EventArgs e)
        {
            panelOrdersAdministrator.Visible = true;
            panelServicesAdministrator.Visible = false;
            panelStorageAdministrator.Visible = false;
            panelWorkersAdministrator.Visible = false;
            panelAuthorizationAdministrator.Visible = false;
            textBoxOrdersNameAdministrator.Text = "";
            textBoxOrdersSurnameAdministrator.Text = "";
            maskedTextBoxOrdersCarnAdministrator.Text = "";
            maskedTextBoxOrdersPhoneAdministrator.Text = "";
            dateTimePickerOrdersAdministrator.Value = DateTime.Today;
            checkedListBoxOrdersServicesAdministrator.Items.Clear();
            checkedListBoxOrdersWorkersAdministrator.Items.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_service,Name_of_service FROM services", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checkedListBoxOrdersServicesAdministrator.Items.Add(new StringItemWithOldID()
                            {
                                OldID = Convert.ToInt16(row[0]),
                                text = Convert.ToString(row[1])
                            });
                        }
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_worker,CONCAT(Name,' ',Surname) FROM workers WHERE Profession = 'Механик'", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            checkedListBoxOrdersWorkersAdministrator.Items.Add(new StringItemWithOldID()
                            {
                                OldID = Convert.ToInt16(row[0]),
                                text = Convert.ToString(row[1])
                            });
                        }
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_order, Name_of_client as `Имя клиента`, Surname_of_client as `Фамилия клиента`, Phone_number_of_client as `Номер телефона клиента`, Car_number as `Номер автомобиля`, Date_of_complition as `Крайний срок сдачи заказа` FROM orders", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewOrdersAdministrator.DataSource = dt;
                        dataGridViewOrdersAdministrator.Columns[0].Visible = false;
                    }
                }
            }
        }

        private void buttonOrdersSaveAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE orders SET Date_of_complition = @date, Name_of_client = @name, Surname_of_client = @surname, Phone_number_of_client = @phone, Is_order_complete = '0', Car_number = @carn WHERE ID_of_order = @ID", conn);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                cmd.Parameters.AddWithValue("date", (dateTimePickerOrdersAdministrator.Value));
                cmd.Parameters.AddWithValue("name", textBoxOrdersNameAdministrator.Text);
                cmd.Parameters.AddWithValue("surname", textBoxOrdersSurnameAdministrator.Text);
                cmd.Parameters.AddWithValue("phone", maskedTextBoxOrdersPhoneAdministrator.Text);
                cmd.Parameters.AddWithValue("carn", maskedTextBoxOrdersCarnAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                MySqlCommand clearservices = new MySqlCommand("DELETE FROM `orders-services` WHERE ID_of_order = @id", conn);
                clearservices.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                clearservices.ExecuteNonQuery();
                MySqlCommand clearworkers = new MySqlCommand("DELETE FROM `orders-workers` WHERE ID_of_order = @id", conn);
                clearworkers.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                clearworkers.ExecuteNonQuery();
                foreach (StringItemWithOldID item in checkedListBoxOrdersServicesAdministrator.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-services` VALUES (@order,@service)", conn);
                    cmdlist.Parameters.AddWithValue("order", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                    cmdlist.Parameters.AddWithValue("service", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                foreach (StringItemWithOldID item in checkedListBoxOrdersWorkersAdministrator.CheckedItems)
                {
                    MySqlCommand cmdlist = new MySqlCommand("INSERT INTO `orders-workers` VALUES (@order,@worker)", conn);
                    cmdlist.Parameters.AddWithValue("order", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                    cmdlist.Parameters.AddWithValue("worker", item.OldID.ToString());
                    cmdlist.ExecuteNonQuery();
                }
                conn.Close();
                buttonOrdersAdministrator.PerformClick();
            }
        }

        private void buttonOrdersDeleteAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM orders WHERE ID_of_order = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewOrdersAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonOrdersAdministrator.PerformClick();
            }
        }

        private void buttonStorageAdministrator_Click(object sender, EventArgs e)
        {
            panelOrdersAdministrator.Visible = false;
            panelServicesAdministrator.Visible = false;
            panelStorageAdministrator.Visible = true;
            panelWorkersAdministrator.Visible = false;
            panelAuthorizationAdministrator.Visible = false;
            textBoxStorageNameAdministrator.Text = "";
            textBoxStorageAmountAdministrator.Text = "";
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_part, Name_of_part as `Название детали`, Remaining_amount as `Осталось на складе:` FROM `storage`", conn))
            {
                cmd.CommandType = CommandType.Text;
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewStorageAdministrator.DataSource = dt;
                        dataGridViewStorageAdministrator.Columns[0].Visible = false;
                    }
                }
            }
        }

        private void dataGridViewStorageAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dataGridViewStorageAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxStorageNameAdministrator.Text = Convert.ToString(dataGridViewStorageAdministrator.CurrentRow.Cells[1].Value);
                textBoxStorageAmountAdministrator.Text = Convert.ToString(dataGridViewStorageAdministrator.CurrentRow.Cells[2].Value);
            }
            else
            {
                textBoxStorageNameAdministrator.Text = "";
                textBoxStorageAmountAdministrator.Text = "";
            }
        }

        private void buttonStorageCreateAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать новую деталь используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO storage (Name_of_part, Remaining_amount) VALUES (@name,@amount)", conn);
                cmd.Parameters.AddWithValue("name", textBoxStorageNameAdministrator.Text);
                cmd.Parameters.AddWithValue("amount", textBoxStorageAmountAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonStorageAdministrator.PerformClick();
            }
        }

        private void buttonStorageSaveAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранную деталь?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE storage SET Name_of_part = @name, Remaining_amount = @amount WHERE ID_of_part = @ID", conn);
                cmd.Parameters.AddWithValue("name", textBoxStorageNameAdministrator.Text);
                cmd.Parameters.AddWithValue("amount", textBoxStorageAmountAdministrator.Text);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewStorageAdministrator.CurrentRow.Cells[0].Value));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonStorageAdministrator.PerformClick();
            }
        }

        private void buttonStorageDeleteAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранную деталь?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM storage WHERE ID_of_part = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewStorageAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonStorageAdministrator.PerformClick();
            }
        }

        private void buttonServicesAdministrator_Click(object sender, EventArgs e)
        {
            panelOrdersAdministrator.Visible = false;
            panelServicesAdministrator.Visible = true;
            panelStorageAdministrator.Visible = false;
            panelWorkersAdministrator.Visible = false;
            panelAuthorizationAdministrator.Visible = false;
            textBoxServicesNameAdministrator.Text = "";
            textBoxServicesPriceAdministrator.Text = "";
            textBoxServicesTimeAdministrator.Text = "";
            textBoxServicesServiceIDAdministrator.Text = "";
            textBoxServicesPartIDAdministrator.Text = "";
            textBoxServicesAmountAdministrator.Text = "";
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_service as `Номер услуги`, Name_of_service as `Название услуги`, Price as `Стоимость`, Recommended_time_days as `Рекомендуемый срок работ` FROM services", conn))
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewServicesServicesAdministrator.DataSource = dt;
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_part as `Номер детали`, Name_of_part as `Название детали`, Remaining_amount as `Оставшееся количество` FROM storage", conn))
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewServicesPartsAdministrator.DataSource = dt;
                    }
                }
            }
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_link, ID_of_service as `Номер услуги`, ID_of_part as `Номер детали`, Required_amount as `Необходимое количество` FROM `storage-service` ORDER BY ID_of_service ASC", conn))
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewServicesServicesPartsAdministrator.DataSource = dt;
                        dataGridViewServicesServicesPartsAdministrator.Columns[0].Visible = false;
                    }
                }
            }
        }

        private void dataGridViewServicesServicesAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxServicesNameAdministrator.Text = Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[1].Value);
                textBoxServicesPriceAdministrator.Text = Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[2].Value);
                textBoxServicesTimeAdministrator.Text = Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[3].Value);
            }
            else
            {
                textBoxServicesNameAdministrator.Text = "";
                textBoxServicesPriceAdministrator.Text = "";
                textBoxServicesTimeAdministrator.Text = "";
            }
        }

        private void dataGridViewServicesServicesPartsAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxServicesServiceIDAdministrator.Text = Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[1].Value);
                textBoxServicesPartIDAdministrator.Text = Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[2].Value);
                textBoxServicesAmountAdministrator.Text = Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[3].Value);
            }
            else
            {
                textBoxServicesServiceIDAdministrator.Text = "";
                textBoxServicesPartIDAdministrator.Text = "";
                textBoxServicesAmountAdministrator.Text = "";
            }
        }

        private void buttonServicesCreateServiceAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать новую услугу используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO services (Name_of_service, Price, Recommended_time_days) VALUES (@name,@price,@days)", conn);
                cmd.Parameters.AddWithValue("name", textBoxServicesNameAdministrator.Text);
                cmd.Parameters.AddWithValue("price", textBoxServicesPriceAdministrator.Text);
                cmd.Parameters.AddWithValue("days", textBoxServicesTimeAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonServicesSaveServiceAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранную услугу?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE services SET Name_of_service = @name, Price = @price, Recommended_time_days = @days WHERE ID_of_service = @ID", conn);
                cmd.Parameters.AddWithValue("name", textBoxServicesNameAdministrator.Text);
                cmd.Parameters.AddWithValue("price", textBoxServicesPriceAdministrator.Text);
                cmd.Parameters.AddWithValue("days", textBoxServicesTimeAdministrator.Text);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[0].Value));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonServicesDeleteServiceAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранную услугу?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM services WHERE ID_of_service = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewServicesServicesAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonServicesCreateLinkAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать новую связь используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO `storage-service` (ID_of_service,ID_of_part,Required_amount) VALUES (@serviceID,@partID,@amount)", conn);
                cmd.Parameters.AddWithValue("serviceID", textBoxServicesServiceIDAdministrator.Text);
                cmd.Parameters.AddWithValue("partID", textBoxServicesPartIDAdministrator.Text);
                cmd.Parameters.AddWithValue("amount", textBoxServicesAmountAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonServicesAlterLinkAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранную связь?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE `storage-service` SET ID_of_service = @serviceID, ID_of_part = @partID, Required_amount = @amount WHERE ID_of_link = @ID", conn);
                cmd.Parameters.AddWithValue("serviceID", textBoxServicesServiceIDAdministrator.Text);
                cmd.Parameters.AddWithValue("partID", textBoxServicesPartIDAdministrator.Text);
                cmd.Parameters.AddWithValue("amount", textBoxServicesAmountAdministrator.Text);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[0].Value));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonServicesDeleteLinktAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранную связь?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM `storage-service` WHERE ID_of_link = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewServicesServicesPartsAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonServicesAdministrator.PerformClick();
            }
        }

        private void buttonWorkersAdministrator_Click(object sender, EventArgs e)
        {
            panelOrdersAdministrator.Visible = false;
            panelServicesAdministrator.Visible = false;
            panelStorageAdministrator.Visible = false;
            panelWorkersAdministrator.Visible = true;
            panelAuthorizationAdministrator.Visible = false;
            textBoxWorkersNameAdministrator.Text = "";
            textBoxWorkersSurnameAdministrator.Text = "";
            textBoxWorkersProfessionAdministrator.Text = "";
            textBoxWorkersSalaryAdministrator.Text = "";
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT ID_of_worker, Name as `Имя`, Surname as `Фамилия`, Profession as `Должность`, Salary as `Зарплата` FROM workers", conn))
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewWorkersAdministrator.DataSource = dt;
                        dataGridViewWorkersAdministrator.Columns[0].Visible = false;
                    }
                }
            }
        }

        private void dataGridViewWorkersAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxWorkersNameAdministrator.Text = Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[1].Value);
                textBoxWorkersSurnameAdministrator.Text = Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[2].Value);
                textBoxWorkersProfessionAdministrator.Text = Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[3].Value);
                textBoxWorkersSalaryAdministrator.Text = Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[4].Value);
            }
            else
            {
                textBoxWorkersNameAdministrator.Text = "";
                textBoxWorkersSurnameAdministrator.Text = "";
                textBoxWorkersProfessionAdministrator.Text = "";
                textBoxWorkersSalaryAdministrator.Text = "";
            }
        }

        private void buttonWorkersCreateAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать нового работника используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO workers (Name, Surname, Profession, Salary) VALUES (@name,@surname,@profession,@salary)", conn);
                cmd.Parameters.AddWithValue("name", textBoxWorkersNameAdministrator.Text);
                cmd.Parameters.AddWithValue("surname", textBoxWorkersSurnameAdministrator.Text);
                cmd.Parameters.AddWithValue("profession", textBoxWorkersProfessionAdministrator.Text);
                cmd.Parameters.AddWithValue("salary", textBoxWorkersSalaryAdministrator.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonWorkersAdministrator.PerformClick();
            }
        }

        private void buttonWorkersSaveAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранного работника?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE workers SET Name = @name, Surname = @surname, Profession = @profession, Salary = @salary WHERE ID_of_worker = @ID", conn);
                cmd.Parameters.AddWithValue("name", textBoxWorkersNameAdministrator.Text);
                cmd.Parameters.AddWithValue("surname", textBoxWorkersSurnameAdministrator.Text);
                cmd.Parameters.AddWithValue("profession", textBoxWorkersProfessionAdministrator.Text);
                cmd.Parameters.AddWithValue("salary", textBoxWorkersSalaryAdministrator.Text);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[0].Value));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonWorkersAdministrator.PerformClick();
            }
        }

        private void buttonWorkersDeleteAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранного работника?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM workers WHERE ID_of_worker = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewWorkersAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonWorkersAdministrator.PerformClick();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxWorkersAccessAdministrator.Items.Add(new StringItemWithOldID()
            {
                OldID = Convert.ToInt16(0),
                text = Convert.ToString("Администратор")
            });
            comboBoxWorkersAccessAdministrator.Items.Add(new StringItemWithOldID()
            {
                OldID = Convert.ToInt16(1),
                text = Convert.ToString("Механик")
            });
            comboBoxWorkersAccessAdministrator.Items.Add(new StringItemWithOldID()
            {
                OldID = Convert.ToInt16(2),
                text = Convert.ToString("Кассир")
            });
            comboBoxWorkersAccessAdministrator.Items.Add(new StringItemWithOldID()
            {
                OldID = Convert.ToInt16(3),
                text = Convert.ToString("Бухгалтер")
            });
            MySqlConnection conn = DBUtils.GetDBConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT ID_of_worker, CONCAT(Profession, ' - ', Name, ' ', Surname) FROM workers", conn);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                comboBoxWorkersWorkersAdministrator.Items.Add(new StringItemWithOldID()
                {
                    OldID = Convert.ToInt16(row[0]),
                    text = Convert.ToString(row[1])
                });
            }
         }

        private void buttonAuthorizationAdministrator_Click(object sender, EventArgs e)
        {
            panelOrdersAdministrator.Visible = false;
            panelServicesAdministrator.Visible = false;
            panelStorageAdministrator.Visible = false;
            panelWorkersAdministrator.Visible = false;
            panelAuthorizationAdministrator.Visible = true;
            textBoxAuthorizationLoginAdministrator.Text = "";
            textBoxWorkersSurnameAdministrator.Text = "";
            comboBoxWorkersAccessAdministrator.SelectedIndex = -1;
            comboBoxWorkersWorkersAdministrator.SelectedIndex = -1;
            MySqlConnection conn = DBUtils.GetDBConnection();
            using (MySqlCommand cmd = new MySqlCommand("SELECT authorization.ID_of_user, authorization.Level_of_access, authorization.ID_of_worker, CONCAT(workers.Name, ' ', workers.Surname) as `Работник`, workers.Profession as `Должность`, authorization.Login as `Логин`, authorization.Password as `Пароль` FROM authorization INNER JOIN workers ON workers.ID_of_worker = authorization.ID_of_worker ORDER BY ID_of_user ASC", conn))
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        dataGridViewAuthorizationAdministrator.DataSource = dt;
                        dataGridViewAuthorizationAdministrator.Columns[0].Visible = false;
                        dataGridViewAuthorizationAdministrator.Columns[1].Visible = false;
                        dataGridViewAuthorizationAdministrator.Columns[2].Visible = false;
                    }
                }
            }
        }

        private void buttonAuthorizationCreateAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите создать нового пользователя используя введенные данные?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO authorization (Login, Password, ID_of_worker, Level_of_access) VALUES (@login,@password,@workerID,@access)", conn);
                cmd.Parameters.AddWithValue("login", textBoxAuthorizationLoginAdministrator.Text);
                cmd.Parameters.AddWithValue("password", textBoxAuthorizationPasswordAdministrator.Text);
                cmd.Parameters.AddWithValue("workerID", ((StringItemWithOldID)comboBoxWorkersWorkersAdministrator.SelectedItem).OldID);
                cmd.Parameters.AddWithValue("access", ((StringItemWithOldID)comboBoxWorkersAccessAdministrator.SelectedItem).OldID);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonAuthorizationAdministrator.PerformClick();
            }
        }

        private void dataGridViewAuthorizationAdministrator_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[0].Value) != "")
            {
                textBoxAuthorizationLoginAdministrator.Text = Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[5].Value);
                textBoxAuthorizationPasswordAdministrator.Text = Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[6].Value);
                for (int i = 0; i < comboBoxWorkersAccessAdministrator.Items.Count; i++)
                {
                    if (Convert.ToString(((StringItemWithOldID)comboBoxWorkersAccessAdministrator.Items[i]).OldID) == Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[1].Value))
                    {
                        comboBoxWorkersAccessAdministrator.SelectedIndex = i;
                        
                        break;
                    }
                }
                for (int i = 0; i < comboBoxWorkersWorkersAdministrator.Items.Count; i++)
                {
                    if (Convert.ToString(((StringItemWithOldID)comboBoxWorkersWorkersAdministrator.Items[i]).OldID) == Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[2].Value))
                    {
                        comboBoxWorkersWorkersAdministrator.SelectedIndex = i;

                        break;
                    }
                }
            }
            else
            {
                textBoxAuthorizationLoginAdministrator.Text = "";
                textBoxAuthorizationPasswordAdministrator.Text = "";
                comboBoxWorkersAccessAdministrator.SelectedIndex = -1;
                comboBoxWorkersWorkersAdministrator.SelectedIndex = -1;
            }
        }

        private void buttonAuthorizationSaveAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите изменить выбранного пользователя?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("UPDATE authorization SET `Login` = @login, `Password` = @password, `ID_of_worker` = @workerID, `Level_of_access` = @access WHERE `ID_of_user` = @ID", conn);
                cmd.Parameters.AddWithValue("login", textBoxAuthorizationLoginAdministrator.Text);
                cmd.Parameters.AddWithValue("password", textBoxAuthorizationPasswordAdministrator.Text);
                cmd.Parameters.AddWithValue("workerID", ((StringItemWithOldID)comboBoxWorkersWorkersAdministrator.SelectedItem).OldID);
                cmd.Parameters.AddWithValue("access", ((StringItemWithOldID)comboBoxWorkersAccessAdministrator.SelectedItem).OldID);
                cmd.Parameters.AddWithValue("ID", Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[0].Value));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                buttonAuthorizationAdministrator.PerformClick();
            }
        }

        private void buttonAuthorizationDeleteAdministrator_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены что хотите удалить выбранного пользователя?", "Подтверждение", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM authorization WHERE ID_of_user = @id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("id", Convert.ToString(dataGridViewAuthorizationAdministrator.CurrentRow.Cells[0].Value));
                cmd.ExecuteNonQuery();
                buttonAuthorizationAdministrator.PerformClick();
            }
        }
    }
}
