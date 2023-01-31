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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string? SelectedPath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilePathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"Image Files|{SupportedExtensions.ExplorerFilter}";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = openFileDialog.FileName;
                DetailsBox.Text = $"File selected:\n{SelectedPath}";
                GetInfoButton.IsEnabled = true;
            }
        }

        private void FolderPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog.SelectedPath + Path.DirectorySeparatorChar;
                DetailsBox.Text = $"Folder selected:\n{SelectedPath}";
                GetInfoButton.IsEnabled = true;
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
                DetailsBox.Text += $"\nNumber of files: {files.Count}";
                var separateThread = new Thread(() => ProcessFiles(files));
                separateThread.Start();
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
                DetailsBox.Text += $"\n\nComplited! Extract Time: {stopwatch.ElapsedMilliseconds / 1000} seconds";
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