using System.Collections.Generic;
using System.Drawing;

namespace Ocr.Server.ConsoleApp.Class
{
    public class Question
    {
        private static List<string> _excludeCharacterList;
        public static List<string> ExcludeCharacterList
        {
            get
            {
                if (_excludeCharacterList == null)
                {
                    _excludeCharacterList = new List<string>()
                    {
                        "/", "[", "]", "?", "!", "{", "}", "(", ")", "\\", "&", ",", ".", ":", ";", "|", "-", "%", "^", "~", "#", "$", "*", "?", "_", "\"", "“", "”", "â"
                    };
                }
                return _excludeCharacterList;
            }
        }

        public Bitmap Bitmap { get; set; }

        public string Text { get; set; }
    }
}
