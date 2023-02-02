using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesseractOCR.Enums;

namespace ImageTextOCR
{
    static class DownloadedLanguages
    {
        public static List<Language> Languages { get; set; } = new List<Language>();

        private static readonly Dictionary<string, Language> languageFiles = new Dictionary<string, Language>() 
        { 
            { "eng.traineddata", Language.English },
            { "rus.traineddata", Language.Russian },
            { "pol.traineddata", Language.Polish },
            { "jpn.traineddata", Language.Japanese },
        };

        static DownloadedLanguages()
        {
            foreach (var fileName in Directory.GetFiles(".", "*.traineddata").Select(name => name.Substring(2)))
            {
                if (languageFiles.ContainsKey(fileName))
                    Languages.Add(languageFiles[fileName]);
            }
        }
    }
}
