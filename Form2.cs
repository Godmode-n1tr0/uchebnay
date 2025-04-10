using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace proverka
{
    public partial class Form2 : Form
    {
        string[,] a = new string[3, 3];
        Random rnd = new Random();
        private string currentFilePath;
        Form1 t1;
        string[,] question = new string[10, 6];
        int curr_question = 0;
        public Form2()
        {
            InitializeComponent();
        }
        private void Fio()
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox4.Text))
                {
                    bool proverka = true;
                    string text = textBox1.Text + textBox2.Text + textBox3.Text;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (char.IsDigit(text[i]))
                        {
                            proverka = false;
                        }
                    }
                    if (proverka == true)
                    {
                        t1 = new Form1(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                        t1.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show($"Вы ввели неправильно фио, попробуйте ещё раз!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show($"Строки не могу быть пустыми");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке вопросов:\n{ex.Message}");
            }
        }
        private void Createmas()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].HeaderText = "Фио студента";
            dataGridView1.Columns[1].HeaderText = "Группа";
            dataGridView1.Columns[2].HeaderText = "Результат";
            using (StreamReader file = new StreamReader("stats.txt"))
            {
                do
                {
                    string[] result = new string[3];
                    for (int j = 0; j < 3; j++)
                    {
                        result[j] = file.ReadLine();
                    }
                    dataGridView1.Rows.Add(result[0], result[1], result[2]);
                }
                while (!file.EndOfStream);
            }
        }
        private void ZagruzkaVop()
        {
            listBox1.Items.Clear();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            List<string> vopros = new List<string>();
                            vopros.Add(line);
                            if (vopros[0].Contains("?"))
                            {
                                listBox1.Items.Add(vopros[0]);
                            }
                        }

                    }
                }
            }
        }
        private void Savefile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFilePath = saveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(currentFilePath, false, Encoding.UTF8))
                {
                    foreach (string question in listBox1.Items)
                    {
                        writer.WriteLine(question);
                        writer.WriteLine("##");
                    }
                }
                MessageBox.Show("Файл успешно сохранен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dobavlenie()
        {

        }
        private void Udalenie()
        {

        }
        private void Reformat()
        {
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Fio();

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Createmas();
            textBox5.Text = listBox1.SelectedItem.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ZagruzkaVop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Savefile();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
