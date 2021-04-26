using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using MimeKit;
using MimeKit.Text;
using TenBlogNet.WpfApp.Converters;
using TenBlogNet.WpfApp.Widget;

namespace TenBlogNet.WpfApp.UserControls
{
    /// <summary>
    ///     ContactFeedback.xaml 的交互逻辑
    /// </summary>
    public partial class ContactFeedback
    {
        public ContactFeedback()
        {
            InitializeComponent();
        }

        private async void ContactFeedback_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var path = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/Resources/MarkdownSample.md";
                var markdown = await File.ReadAllTextAsync(path);
                EditSourceTextBox.Text = markdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void LinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Control control) return;
            var tag = control.Tag.ToString();
            Process.Start(new ProcessStartInfo("cmd", $"/c start {tag}") { CreateNoWindow = true });
        }

        private async void SendMailButton_OnClick(object sender, RoutedEventArgs e)
        {
            var view = new SimpleProgressDialog();
            await DialogHost.Show(view, "RootDialog", SendEmailEventHandler);
        }

        private async void SendEmailEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            try
            {
                var converter = new FlowDocumentToHtmlConverter();
                var html = converter.ConvertBack(MarkdownScrollViewer.Document, typeof(string), null, null)?.ToString();

                var tempDirectory =
                    Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty, "Temp");
                if (!Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);
                var emlFileAbsPath = Path.Combine(tempDirectory, "EmailToSend.eml");

                var mimeMessage = new MimeMessage
                {
                    From = { new MailboxAddress("Ten", "tanwucheng@outlook.com") },
                    To = { new MailboxAddress("anonymous", "example@example.com") },
                    Subject = "示例邮件标题：联系与反馈",
                    Body = new TextPart(TextFormat.Html)
                    {
                        Text = html
                    }
                };

                await using var fileStream = File.Open(emlFileAbsPath, FileMode.Create);
                var binaryWriter = new BinaryWriter(fileStream);
                //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                binaryWriter.Write(Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));

                await mimeMessage.WriteToAsync(FormatOptions.Default, fileStream);

                DialogHost.CloseDialogCommand.Execute(null, null);

                var psi = new ProcessStartInfo
                {
                    FileName = emlFileAbsPath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                DialogHost.CloseDialogCommand.Execute(null, null);

                var messageDialog = new SimpleMessageDialog("错误", "发送邮件发生异常: " + ex.Message);
                await DialogHost.Show(messageDialog, "RootDialog");
            }
        }

        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var progressDialog = new SimpleProgressDialog();
            await DialogHost.Show(progressDialog, "RootDialog", SaveDraftEventHandler);
        }

        private async void SaveDraftEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            try
            {
                await Task.Delay(1000);

                var tempDirectory =
                    Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty, "Temp");
                if (!Directory.Exists(tempDirectory)) Directory.CreateDirectory(tempDirectory);

                var emailDraftAbsPath = Path.Combine(tempDirectory, "EmailDraft.md");

                await using var fileStream = File.Open(emailDraftAbsPath, FileMode.Create);
                await using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                await streamWriter.WriteAsync(EditSourceTextBox.Text);

                DialogHost.CloseDialogCommand.Execute(null, null);

                var messageDialog = new SimpleMessageDialog("提示", "保存草稿成功");
                await DialogHost.Show(messageDialog, "RootDialog");
            }
            catch (Exception ex)
            {
                DialogHost.CloseDialogCommand.Execute(null, null);

                var messageDialog = new SimpleMessageDialog("错误", "保存草稿发生异常: " + ex.Message);
                await DialogHost.Show(messageDialog, "RootDialog");
            }
        }

        private async void LoadDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            var progressDialog = new SimpleProgressDialog();
            await DialogHost.Show(progressDialog, "RootDialog", LoadDraftEventHandler);
        }

        private async void LoadDraftEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            try
            {
                await Task.Delay(1000);

                var fileName =
                    Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty, "Temp", "EmailDraft.md");
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    var draft = await File.ReadAllTextAsync(fileName);
                    EditSourceTextBox.Text = draft;

                    DialogHost.CloseDialogCommand.Execute(null, null);
                }
                else
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    var messageDialog = new SimpleMessageDialog("错误", "草稿不存在，请加载范文");
                    await DialogHost.Show(messageDialog, "RootDialog");
                }
            }
            catch (Exception ex)
            {
                DialogHost.CloseDialogCommand.Execute(null, null);

                var messageDialog = new SimpleMessageDialog("错误", "加载草稿发生异常: " + ex.Message);
                await DialogHost.Show(messageDialog, "RootDialog");
            }
        }

        private async void LoadModalButton_OnClick(object sender, RoutedEventArgs e)
        {
            var progressDialog = new SimpleProgressDialog();
            await DialogHost.Show(progressDialog, "RootDialog", LoadModelEventHandler);
        }

        private async void LoadModelEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            try
            {
                await Task.Delay(1000);

                var path = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/Resources/MarkdownSample.md";
                var markdown = await File.ReadAllTextAsync(path);
                EditSourceTextBox.Text = markdown;
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception ex)
            {
                DialogHost.CloseDialogCommand.Execute(null, null);

                var messageDialog = new SimpleMessageDialog("错误", "加载范文发生异常: " + ex.Message);
                await DialogHost.Show(messageDialog, "RootDialog");
            }
        }
    }
}