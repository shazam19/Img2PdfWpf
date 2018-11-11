using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ImagesToPdfConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertOnClick(object sender, RoutedEventArgs e)
        {
            var fileNames = GetSimplifiedFileNames();
            var destinationPdfFilePath = GetDestinationPdfFilePath();

            if (!VerifyInputs(fileNames, destinationPdfFilePath))
            {
                return;
            };

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "img2pdf",
                Arguments =
                    $"{fileNames} -o {destinationPdfFilePath}",
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.Exited += ProcessOnExited;

            if (!process.Start())
            {
                LblStatus.Text = "Failed to start process";
            }
        }

        private void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            //SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            //var d = SynchronizationContext.Current.po;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LblStatus.Text = "Process done running";
            }));
        }

        private string GetDestinationPdfFilePath()
        {
            var destFolder = GetDestinationFolder();
            var destinationPdfName = GetDestinationPdfName();

            return Path.Combine(destFolder, destinationPdfName);
        }

        private string GetDestinationPdfName()
        {
            var fileName = DestinationPdfName.Text;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            return $"{fileNameWithoutExtension}.pdf";
        }

        private bool VerifyInputs(string fileNames, string destFolder)
        {
            if (string.IsNullOrEmpty(fileNames))
            {
                LblStatus.Text = "No files to convert";
                return false;
            }

            if (string.IsNullOrEmpty(destFolder))
            {
                LblStatus.Text = "Destination path cannot be empty";
                return false;
            }

            return true;
        }

        private void VerifyInputs()
        {
            throw new System.NotImplementedException();
        }

        private string GetSimplifiedFileNames()
        {
            var sourceFolder = SourceFolderName.Text;

            if (!Directory.Exists(sourceFolder))
            {
                LblStatus.Text = "Source File doesn't exist";
                return null;
            }

            var files = Directory.GetFiles(sourceFolder);

            var sb = new StringBuilder();

            //files.(x => sb.AppendFormat($"\"{x}\" "));
            //sb.AppendFormat($"\"{0}\" ", files.Select(x => x));

            foreach (var file in files)
            {
                sb.AppendFormat($"\"{file}\" ");
            }

            sb.Length -= 1;

            return sb.ToString();
        }

        private string GetDestinationFolder()
        {
            if (!Directory.Exists(DestinationFolderName.Text))
            {
                Directory.CreateDirectory(DestinationFolderName.Text);
            }

            return DestinationFolderName.Text;
        }

        private void ChooseDestinationFolder(object sender, RoutedEventArgs e)
        {
            DestinationFolderName.Text = GetFolderName();
        }

        private void ChooseSourceFolder(object sender, RoutedEventArgs e)
        {
            SourceFolderName.Text = GetFolderName();
        }

        private string GetFolderName()
        {
            var diag = new CommonOpenFileDialog {IsFolderPicker = true};

            if (diag.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var name = diag.FileName;
                return name;
            }

            return null;
        }
    }
}
