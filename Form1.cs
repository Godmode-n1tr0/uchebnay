using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace proverka
{
    public partial class Form1 : Form
    {
        Form2 t2;
        private List<System.Windows.Forms.RadioButton> radioButtons;
        private List<System.Windows.Forms.CheckBox> checkBoxes;
        private int currentQuestionIndex = 0;
        private int correctAnswers = 0;
        private Random random = new Random();
        private List<int> questionOrder = new List<int>();
        public Form1(string surname, string name, string lastname, string group)
        {
            InitializeComponent();
            InitializeControls();
            SetupInitialState();
            label2.Text = surname;
            label3.Text = lastname;
            label4.Text = name;
            label5.Text = group;
        }

        private void InitializeControls()
        {
            radioButtons = new List<System.Windows.Forms.RadioButton> { radioButton1, radioButton2, radioButton3, radioButton4, radioButton5 };
            checkBoxes = new List<System.Windows.Forms.CheckBox> { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };
        }

        private void SetupInitialState()
        {
            HideAllOptions();
            radioButton1.Checked = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Question.LoadFromFile("questions.txt");
                if (Question.AllQuestions.Count == 0)
                {
                    MessageBox.Show("Файл вопросов пуст", "Ошибка");
                    Close();
                    return;
                }

                Question.ShuffleQuestions(random);
                Question.ShuffleAnswers(random);
                ShowCurrentQuestion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                Close();
            }
        }

        private void HideAllOptions()
        {
            radioButtons.ForEach(rb => { rb.Visible = false; rb.Checked = false; });
            checkBoxes.ForEach(cb => { cb.Visible = false; cb.Checked = false; });
        }

        private void ShowCurrentQuestion()
        {
            if (currentQuestionIndex < 0 || currentQuestionIndex >= Question.AllQuestions.Count) return;

            var question = Question.AllQuestions[currentQuestionIndex];
            textBoxQuestion.Text = question.Text;

            if (question.IsMultipleChoice)
                ShowCheckBoxAnswers(question);
            else
                ShowRadioButtonAnswers(question);

            UpdateNavigation();
            UpdateProgress();
            LoadImage(question.ImagePath);
        }

        private void ShowRadioButtonAnswers(Question question)
        {
            HideAllOptions();
            for (int i = 0; i < question.Answers.Count && i < radioButtons.Count; i++)
            {
                radioButtons[i].Text = question.Answers[i];
                radioButtons[i].Visible = true;
            }
            radioButtons[0].Checked = true;
        }

        private void ShowCheckBoxAnswers(Question question)
        {
            HideAllOptions();
            for (int i = 0; i < question.Answers.Count && i < checkBoxes.Count; i++)
            {
                checkBoxes[i].Text = question.Answers[i];
                checkBoxes[i].Visible = true;
            }
        }

        private void LoadImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    pictureBox.Image = Image.FromFile(imagePath);
                    pictureBox.Visible = true;
                }
                else
                {
                    pictureBox.Image = null;
                    pictureBox.Visible = false;
                }
            }
            catch
            {
                pictureBox.Visible = false;
            }
        }

        private void UpdateNavigation()
        {
            buttonNext.Text = currentQuestionIndex == Question.AllQuestions.Count - 1 ? "Завершить" : "Далее";
        }
        private void UpdateProgress()
        {
            labelProgress.Text = $"Вопрос {currentQuestionIndex + 1} из {Question.AllQuestions.Count}";
            labelScore.Text = $"Правильно: {correctAnswers}";
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            CheckAnswer();

            if (currentQuestionIndex < Question.AllQuestions.Count - 1)
            {
                currentQuestionIndex++;
                ShowCurrentQuestion();
            }
            else
            {
                ShowResults();
            }
        }

        private void CheckAnswer()
        {
            var question = Question.AllQuestions[currentQuestionIndex];
            bool isCorrect = false;

            if (question.IsMultipleChoice)
            {
                var selected = checkBoxes
                    .Where(c => c.Visible && c.Checked)
                    .Select(c => c.Text)
                    .ToList();
                isCorrect = selected.Count == question.CorrectAnswers.Count &&
                           selected.All(a => question.CorrectAnswers.Contains(a));
            }
            else
            {
                var selected = radioButtons.FirstOrDefault(r => r.Visible && r.Checked);
                isCorrect = selected != null && question.CorrectAnswers.Contains(selected.Text);
            }

            if (isCorrect) correctAnswers++;
        }

        private void ShowResults()
        {
            double percentage = (double)correctAnswers / Question.AllQuestions.Count * 100;
            this.Hide();
            using (StreamWriter file = File.AppendText("stats.txt"))
            {
                file.WriteLine($"{label2.Text} {label3.Text} {label4.Text}");
                file.WriteLine($"{label5.Text}");
                file.WriteLine($"{percentage}%");
            }

        }
    }
}
