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
using System.Threading;

namespace ImageTextOCR
{
    public partial class MainWindow : Window
    {
        public string? SelectedPath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            if (DownloadedLanguages.Languages.Count == 0)
                BlockApp("You need language data in root directory.");
        }

        private void BlockApp(string message)
        {
            DetailsBox.Text = $"{message}\nPlease check the project page on GitHub.";
            FilePathButton.IsEnabled = false;
            FolderPathButton.IsEnabled = false;
        }

        private void FilePathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"Image Files|{SupportedExtensions.ExplorerFilter}";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = openFileDialog.FileName;
                DetailsBox.Text = $"File selected:\n{SelectedPath}";
                ExtractTextButton.IsEnabled = true;
            }
        }

        private void FolderPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar;
                DetailsBox.Text = $"Folder selected:\n{SelectedPath}";
                ExtractTextButton.IsEnabled = true;
            }
        }

        private void ExtractTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SelectedPath))
            {
                throw new ArgumentException("To extract the text, the path must be selected");
            }
            else
            {
                var files = new List<FileInfo>();
                if (SelectedPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    var directoryInfo = new DirectoryInfo(SelectedPath);
                    var directoryFiles = directoryInfo.GetFiles();
                    files.AddRange(directoryFiles);
                }
                else
                {
                    files.Add(new FileInfo(SelectedPath));
                }
                files = files.Where(file => SupportedExtensions.Extensions.Contains(file.Extension)).ToList();
                DetailsBox.Text += $"\n\nNumber of files: {files.Count}\n\nPlease, wait...";
                var separateThread = new Thread(() => ProcessFiles(files));
                separateThread.Start();
                ExtractTextButton.IsEnabled = false;
            }
        }

        private void ProcessFiles(List<FileInfo> files)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.ForEach(files, ExtractText);
            stopwatch.Stop();
            Dispatcher.Invoke(() =>
            {
                DetailsBox.Text += $"\n\nComplited! Extract Time: {stopwatch.ElapsedMilliseconds / (double)1000} seconds";
            });
        }

        private void ExtractText(FileInfo file)
        {
            var imageInfo = new ImageInfo(file.FullName);
            var saveDirectory = file.DirectoryName + Path.DirectorySeparatorChar + "ExtractedText" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            var textFile = saveDirectory + file.Name.Split('.')[0] + ".txt";
            using (StreamWriter writer = new StreamWriter(textFile, false, Encoding.Default))
            {
                writer.WriteLine(imageInfo.Text);
            }
        }
    }
}