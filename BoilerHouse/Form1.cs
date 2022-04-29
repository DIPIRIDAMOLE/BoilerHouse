using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; // работа с COM портами
using System.Text.RegularExpressions; // регулярные выражения

namespace BoilerHouse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = Properties.Settings.Default.Theme; // получение сохраненной темы
            SetTheme(); // установка темы 
            
            string[] ports = SerialPort.GetPortNames(); // получение всех COM портов из винды

            comboBoxPorts.Text = "";
            comboBoxPorts.Items.Clear();

            // выключение кликабельности кнопок авто и ручной
            button2.Enabled = false;
            button3.Enabled = false;

            // если полученные из винды порты есть, то добавить их в выпадающий список
            if (ports.Length != 0)
            {
                comboBoxPorts.Items.AddRange(ports);
                comboBoxPorts.SelectedIndex = 0;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Подключиться")
            {
                try
                {
                    serialPort1.PortName = comboBoxPorts.Text; // берем из выпадающего списка имя порта, к которому будем подключаться
                    serialPort1.Open();
                    // делаем кнопку неактивной и меняем текст на отключиться
                    comboBoxPorts.Enabled = false;
                    button1.Text = "Отключиться";
                }
                catch
                {
                    MessageBox.Show("Ошибка при установке соединения! Проверьте подключение устройства и выберите нужный порт.");
                }
            }
            else if (button1.Text == "Отключиться")
            {
                serialPort1.Close();
                comboBoxPorts.Enabled = true;
                button1.Text = "Подключиться";
            }
        }


        // поток работы с ком портом
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    String read = serialPort1.ReadLine(); // принимаем приходящую строку из ком порта
                    MatchCollection match = Regex.Matches(read, @"\d+.\d+"); // создаем массив из значений из строки, которые подходят по паттерну  \d+.\d+ (число точка число)
                    // раскидывем значения по текстбоксам
                    textBox1.Text = match[0].Value;
                    textBox5.Text = match[1].Value;
                    textBox6.Text = match[2].Value;
                    textBox7.Text = match[3].Value;
                    textBox8.Text = match[4].Value;
                    textBox9.Text = match[5].Value;
                    textBox2.Text = match[6].Value;
                    textBox3.Text = match[7].Value;
                    textBox4.Text = match[8].Value;
                    textBox10.Text = match[9].Value;
                    // в конце строки приходит значение 1 или 0, которое показывает включен или выключен авто режим набора воды
                    if (match[10].Value == "1.00")
                    {
                        // если авто режим включен, то кнопку авто делаем неактивной, а кнопку ручной активной
                        button2.Enabled = true;
                        button3.Enabled = false;
                    }
                    else if (match[10].Value == "0.00")
                    {
                        // то же самое наоборот
                        button3.Enabled = true;
                        button2.Enabled = false;
                    }

                    // добавляем в лог во второй вкладке строку "время | строка" и делаем последнюю добавленную строку выбранной, чтобы лог сам проматывался
                    listBox1.Items.Add(DateTime.Now.ToString() + " | " + read);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }));
            else
            {
                // тут тоже самое
                String read = serialPort1.ReadLine();
                MatchCollection match = Regex.Matches(read, @"\d+.\d+");
                textBox1.Text = match[0].Value;
                textBox5.Text = match[1].Value;
                textBox6.Text = match[2].Value;
                textBox7.Text = match[3].Value;
                textBox8.Text = match[4].Value;
                textBox9.Text = match[5].Value;
                textBox2.Text = match[6].Value;
                textBox3.Text = match[7].Value;
                textBox4.Text = match[8].Value;
                textBox10.Text = match[9].Value;
                if (match[10].Value == "1.00")
                {
                    button2.Enabled = true;
                    button3.Enabled = false;
                }
                else if (match[10].Value == "0.00")
                {
                    button3.Enabled = true;
                    button2.Enabled = false;
                }

                listBox1.Items.Add(DateTime.Now.ToString() + " | " + read);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }

        }




        // если в ардуино полученная строка будет равна "a", то включается светодиод, иначе выключается

        private void button2_Click(object sender, EventArgs e)
        {
            // по нажатию кнопки авто происходит отправка в ком порт буквы б
            serialPort1.Write("b");
            button3.Enabled = true;
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // по нажатию кнопки авто происходит отправка в ком порт буквы а
            serialPort1.Write("a");
            button2.Enabled = true;
            button3.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // при смене темы происходит ее сохранение и установка
            if (comboBox1.SelectedIndex == 0)
                Properties.Settings.Default.Theme = 0;
            else
                Properties.Settings.Default.Theme = 1;
            SetTheme();
        }

        private void SetTheme()
        {
            if (Properties.Settings.Default.Theme == 1) // темная тема
            {
                this.BackColor = Color.Black;
                this.ForeColor = Color.White;
                tabPage1.BackColor = Color.FromArgb(30, 30, 30);
                tabPage2.BackColor = Color.FromArgb(30, 30, 30);
                tabPage3.BackColor = Color.FromArgb(30, 30, 30);
                listBox1.BackColor = Color.FromArgb(30, 30, 30);
                listBox1.ForeColor = Color.White;
                groupBox1.ForeColor = Color.White;
                groupBox2.ForeColor = Color.White;
                groupBox3.ForeColor = Color.White;
                groupBox4.ForeColor = Color.White;
                groupBox5.ForeColor = Color.White;
                groupBox6.ForeColor = Color.White;
                groupBox7.ForeColor = Color.White;
                button1.ForeColor = Color.Black;
                button2.ForeColor = Color.Black;
                button3.ForeColor = Color.Black;
                pictureBox2.Visible = false;
                pictureBox1.Visible = true;
            }
            else // светлая тема
            {
                this.BackColor = Color.LightSteelBlue;
                this.ForeColor = Color.Black;
                tabPage1.BackColor = Color.White;
                tabPage2.BackColor = Color.White;
                tabPage3.BackColor = Color.White;
                listBox1.BackColor = Color.White;
                listBox1.ForeColor = Color.Black;
                groupBox1.ForeColor = Color.Black;
                groupBox2.ForeColor = Color.Black;
                groupBox3.ForeColor = Color.Black;
                groupBox4.ForeColor = Color.Black;
                groupBox5.ForeColor = Color.Black;
                groupBox6.ForeColor = Color.Black;
                groupBox7.ForeColor = Color.Black;
                button1.ForeColor = Color.Black;
                button2.ForeColor = Color.Black;
                button3.ForeColor = Color.Black;
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // при закрытии приложения происходит сохранение настроек
            Properties.Settings.Default.Save();
        }
    }

}
