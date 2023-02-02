using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesseractOCR.Enums;

namespace ImageTextOCR
{
    static class SupportedLanguages
    {
        public static List<Language> Languages = new List<Language>()
            {
                Language.English,
                Language.Russian,
            };
    }
}
