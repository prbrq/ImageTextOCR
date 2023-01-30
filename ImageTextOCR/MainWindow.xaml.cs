using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ImageTextOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] SupportedExtensions { get; } = new string[] { ".jpg", "jpeg", ".png", ".bmp" };
        public string SelectedPath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilePathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"Image Files|{GetExtensionsFilter(SupportedExtensions)}";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = openFileDialog.FileName;
                DetailsBox.Text = $"File selected:\n{SelectedPath}";
            }
        }

        private string GetExtensionsFilter(string[] SupportedExtensions)
        {
            var result = String.Concat(SupportedExtensions.SelectMany(extension => "*" + extension + ";"));
            if (String.IsNullOrEmpty(result))
                throw new ArgumentException("Extensions are configured incorrectly");
            else
                return result;
        }

        private void FolderPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog.SelectedPath + "\\";
                DetailsBox.Text = $"Folder selected:\n{SelectedPath}";
            }
        }

        private void GetInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var files = new List<FileInfo>();
            var path = SelectedPath;
            if (path.EndsWith('\\'))
            {
                var dirInfo = new DirectoryInfo(path);
                var info = dirInfo.GetFiles();
                files.AddRange(info);
            }
            else
            {
               files.Add(new FileInfo(path));
            }
            GetInfo(files);
        }

        private void GetInfo(List<FileInfo> files)
        {
            var detailsText = DetailsBox.Text + "\n\n";
            var imageCount = 0;
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(files, ExtractText);
            sw.Stop();
            DetailsBox.Text += $"\n\nComplited! Extract Time: {sw.ElapsedMilliseconds / 1000} seconds";
        }

        private void ExtractText(FileInfo file)
        {
            if (SupportedExtensions.Contains(file.Extension))
            {
                var fileInfo = new FileInfo(file.FullName);
                if (fileInfo.DirectoryName != null)
                {
                    var imageInfo = new ImageInfo(fileInfo.FullName);
                    var saveDirectory = fileInfo.DirectoryName + Path.DirectorySeparatorChar + "ExtractedText" + Path.DirectorySeparatorChar;
                    if (!Directory.Exists(saveDirectory))
                        Directory.CreateDirectory(saveDirectory);
                    var textFile = saveDirectory + fileInfo.Name.Split('.')[0] + ".txt";
                    using (StreamWriter writer = new StreamWriter(textFile, false, System.Text.Encoding.Default))
                    {
                        writer.WriteLine(imageInfo.Text);
                    }
                }
            }
        }
    }
}