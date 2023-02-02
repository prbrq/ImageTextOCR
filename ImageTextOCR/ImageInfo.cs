using TesseractOCR;
using TesseractOCR.Enums;

namespace ImageTextOCR
{
    public class ImageInfo
    {
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
            using (var engine = new Engine($".", DownloadedLanguages.Languages, EngineMode.Default))
            {
                using (var image = TesseractOCR.Pix.Image.LoadFromFile(FileName))
                {
                    using (var page = engine.Process(image))
                    {
                        MeanConfidence = page.MeanConfidence;
                        Text = page.Text;
                    }
                }
            }
        }
    }
}
