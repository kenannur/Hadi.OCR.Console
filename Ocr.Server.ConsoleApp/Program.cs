using HtmlAgilityPack;
using IronOcr;
using IronOcr.Languages;
using Ocr.Server.ConsoleApp.Class;
using Ocr.Server.ConsoleApp.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static IronOcr.OcrResult;

namespace Ocr.Server.ConsoleApp
{
    class Program
    {
        static string imageFilePath = @"C:\Users\Kenan Nur\Desktop\hadi\07-11-2018\5.png";
        static readonly byte blackWhiteConvertGradeForQuestion = 240;
        static readonly byte blackWhiteConvertGradeForAnswers = 230;

        static void Main(string[] args)
        {
            Bitmap bitmap = new Bitmap(Image.FromFile(imageFilePath));
            Question question = GetQuestion(bitmap);
            List<string> resultWordList = GetGoogleResults(question);
            List<Answer> answerList = GetAnswers(bitmap, resultWordList);

            Console.WriteLine(answerList.All(ans => ans.Count == 0) ? "NOT FOUND" : answerList.OrderByDescending(x => x.Count).First().Text);
            Console.ReadLine();
        }

        private static List<Answer> GetAnswers(Bitmap bitmap, List<string> resultWordList)
        {
            Answer a = new Answer
            {
                Bitmap = bitmap.Clone(new Rectangle(85, 540, 580, 85), bitmap.PixelFormat)
            };
            a.Bitmap.ToBlackWhite(blackWhiteConvertGradeForAnswers);
            a.Bitmap.Save(imageFilePath.Split('.')[0] + "_A.png");

            Answer b = new Answer
            {
                Bitmap = bitmap.Clone(new Rectangle(85, 680, 580, 85), bitmap.PixelFormat)
            };
            b.Bitmap.ToBlackWhite(blackWhiteConvertGradeForAnswers);
            b.Bitmap.Save(imageFilePath.Split('.')[0] + "_B.png");

            Answer c = new Answer
            {
                Bitmap = bitmap.Clone(new Rectangle(85, 820, 580, 85), bitmap.PixelFormat)
            };
            c.Bitmap.ToBlackWhite(blackWhiteConvertGradeForAnswers);
            c.Bitmap.Save(imageFilePath.Split('.')[0] + "_C.png");

            List<Answer> answerList = new List<Answer>() { a, b, c };

            var advancedOcrForAnswers = new AdvancedOcr
            {
                Language = Turkish.OcrLanguagePack
            };
            OcrResult advancedResultForAnswers = advancedOcrForAnswers.ReadMultiThreaded(new List<Bitmap>() { a.Bitmap, b.Bitmap, c.Bitmap });
            a.OcrPage = advancedResultForAnswers.Pages[0];
            b.OcrPage = advancedResultForAnswers.Pages[1];
            c.OcrPage = advancedResultForAnswers.Pages[2];

            foreach (Answer answer in answerList)
            {
                if (answer.OcrPage != null && answer.OcrPage.Words != null)
                {
                    foreach (OcrWord word in answer.OcrPage.Words)
                    {
                        word.Text = word.Text.ToLower();
                        answer.Count += resultWordList.Count(x => x == word.Text);
                    }
                }
            }
            answerList.ForEach(answer => Console.WriteLine(answer.Text + " - " + answer.Count.ToString()));
            return answerList;
        }

        private static List<string> GetGoogleResults(Question question)
        {
            string searchDataForGoogle = string.Join("+", question.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            string searchUrl = "http://www.google.com/search?q=" + searchDataForGoogle;
            Process chromeProcess = Process.Start(searchUrl);

            HtmlDocument htmlDocument = new HtmlWeb().Load(searchUrl);
            List<string> resultWordList = new List<string>();
            var searchDiv = htmlDocument.GetElementbyId("search");
            if (searchDiv != null && searchDiv.FirstChild != null && searchDiv.FirstChild.FirstChild != null && searchDiv.FirstChild.FirstChild.ChildNodes != null)
            {
                var list = searchDiv.FirstChild.FirstChild.ChildNodes;
                foreach (HtmlNode item in list)
                {
                    var headerNode = item.ChildNodes.FirstOrDefault(x => x.Name == "h3");
                    if (headerNode != null)
                    {
                        var headerText = headerNode.InnerText.CleanString().ToLower();
                        var words = headerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        resultWordList.AddRange(words);
                    }

                    var bodyNode = item.ChildNodes.FirstOrDefault(x => x.Name == "div");
                    if (bodyNode != null)
                    {
                        var bodyTextNode = bodyNode.ChildNodes.FirstOrDefault(x => x.Name == "span");
                        if (bodyTextNode != null)
                        {
                            var bodyText = bodyTextNode.InnerText.CleanString().ToLower();
                            var words = bodyText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            resultWordList.AddRange(words);
                        }
                    }
                }
            }

            return resultWordList;
        }

        private static Question GetQuestion(Bitmap bitmap)
        {
            Question question = new Question
            {
                Bitmap = bitmap.Clone(new Rectangle(10, 250, 730, 275), bitmap.PixelFormat)
            };
            question.Bitmap.ToBlackWhite(blackWhiteConvertGradeForQuestion);
            question.Bitmap.Save(imageFilePath.Split('.')[0] + "_Q.png");
            AdvancedOcr advancedOcr = new AdvancedOcr()
            {
                Language = Turkish.OcrLanguagePack,
            };
            question.Text = advancedOcr.Read(question.Bitmap).Text;

            int indexOfQuestionMark = question.Text.IndexOf('?');
            if (indexOfQuestionMark != -1)
                question.Text = question.Text.Remove(indexOfQuestionMark);

            question.Text = question.Text.Replace(Environment.NewLine, " ");
            Question.ExcludeCharacterList.ForEach(str => question.Text = question.Text.Replace(str, string.Empty));
            question.Text = question.Text.TrimStart().TrimEnd().ToLower();
            Console.WriteLine(question.Text + Environment.NewLine);
            return question;
        }
    }
}
