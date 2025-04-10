using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proverka
{
    public class Question
    {
        public string Text { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
        public List<string> CorrectAnswers { get; set; } = new List<string>();
        public string ImagePath { get; set; }
        public static List<Question> AllQuestions { get; } = new List<Question>();

        public bool HasImage => !string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath);
        public bool IsMultipleChoice => CorrectAnswers.Count > 1;

        public static void LoadFromFile(string filePath)
        {
            try
            {
                AllQuestions.Clear();
                string[] lines = File.ReadAllLines(filePath);
                int i = 0;

                while (i < lines.Length)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) { i++; continue; }

                    var question = new Question { Text = lines[i++] };

                    while (i < lines.Length && !lines[i].StartsWith("##") && question.Answers.Count < 5)
                    {
                        if (!string.IsNullOrWhiteSpace(lines[i]))
                            question.Answers.Add(lines[i]);
                        i++;
                    }

                    if (i < lines.Length && lines[i].StartsWith("##"))
                    {
                        question.CorrectAnswers.AddRange(lines[i].Substring(2).Split('^'));
                        i++;
                    }

                    question.ImagePath = i < lines.Length ? lines[i++] : "";

                    AllQuestions.Add(question);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки вопросов: {ex.Message}");
            }
        }

        public static void ShuffleQuestions(Random random)
        {
            AllQuestions.OrderBy(q => random.Next()).ToList();
        }

        public static void ShuffleAnswers(Random random)
        {
            foreach (var question in AllQuestions)
            {
                var correctTexts = question.CorrectAnswers.ToList();
                question.Answers = question.Answers.OrderBy(a => random.Next()).ToList();

                question.CorrectAnswers = question.Answers
                    .Where(a => correctTexts.Contains(a))
                    .ToList();
            }
        }
    }
}