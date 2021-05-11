using System.Drawing;
using static IronOcr.OcrResult;

namespace Ocr.Server.ConsoleApp.Class
{
    public class Answer
    {
        public Bitmap Bitmap { get; set; }

        public string Text { get; private set; }

        private OcrPage _ocrPage;
        public OcrPage OcrPage
        {
            get => _ocrPage;
            set
            {
                _ocrPage = value;
                if (value != null)
                {
                    Text = value.Text.ToLower();
                }
            }
        }

        public int Count { get; set; }
    }
}
