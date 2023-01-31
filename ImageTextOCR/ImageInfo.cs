using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TesseractOCR;
using TesseractOCR.Enums;

namespace ImageTextOCR
{
    public class ImageInfo
    {
        static List<Language> languages = new List<Language>()
            {
                Language.English,
                Language.Russian,
            };

        public string FileName { get; private set; }
        public float MeanConfidence { get; private set; }
        public string Text { get; private set; }

        public ImageInfo(string fileName)
        {
            FileName = fileName;
            Text = string.Empty;
            GetData();
        }

        private void GetData()
        {
            using (var engine = new Engine($".{Path.DirectorySeparatorChar}tessdata", languages, EngineMode.Default))
            {
                using (var image = TesseractOCR.Pix.Image.LoadFromFile(FileName))
                {
                    using (var page = engine.Process(image))
                    {
                        MeanConfidence = page.MeanConfidence;
                        Text = page.Text;
                    }
                    engine.Dispose();
                }
            }
        }
    }
}
