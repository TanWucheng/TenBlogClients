using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using TenBlogCoreLib.RssSubscriber;
using TenBlogNet.WpfApp.Domain;
using TenBlogNet.WpfApp.Models;
using TenBlogNet.WpfApp.ViewModels;
using TenBlogNet.WpfApp.Widget;

namespace TenBlogNet.WpfApp.UserControls
{
    /// <summary>
    ///     Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home
    {
        internal static Home HomeInstance;

        private readonly HomeViewModel _viewModel;

        public Home()
        {
            InitializeComponent();

            _viewModel = new HomeViewModel();
            DataContext = _viewModel;

            Init();

            HomeInstance = this;
        }

        public async void Init()
        {
            var view = new SimpleProgressDialog();
            await MainWindow.MainDialog.ShowDialog(view, DialogOpenedEventHandler);
            await BlogArticleWebView.EnsureCoreWebView2Async();
            BlogArticleWebView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
        }

        private async void DialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var entries = await RssSubscribeService.GetBlogEntries(path);
            var blogSearchItems = new List<BlogSearchModel>();
            foreach (var entry in entries)
            {
                List<CategoryViewModel> list = new();
                foreach (var category in entry.Categories)
                {
                    switch (category.Term)
                    {
                        case "Github":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.Github });
                                break;
                            }
                        case "笔记":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.BookOpen });
                                break;
                            }
                        case ".NET For Android":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.Android });
                                break;
                            }
                        case "Statement":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, PackIconKind = PackIconKind.MessageTextOutline });
                                break;
                            }
                        case ".NET":
                        case "Blazor":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, PackIconKind = PackIconKind.MicrosoftDotNet });
                                break;
                            }
                        case "HEXO":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.Blog });
                                break;
                            }
                        case "杂谈":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.Pencil });
                                break;
                            }
                        case "谚语":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.FileWordBox });
                                break;
                            }
                        case "教程":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.SchoolOnline });
                                break;
                            }
                        case "PWA":
                        case "Gulp":
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.LanguageJavascript });
                                break;
                            }
                        default:
                            {
                                list.Add(new CategoryViewModel { Category = category, PackIconKind = PackIconKind.CodeTags });
                                break;
                            }
                    }
                }

                blogSearchItems.Add(new BlogSearchModel { Link = entry.Link, Title = entry.Title });

                var code = entry.Title[..1];
                _viewModel.Items.Add(new EntryViewModel
                {
                    Code = code,
                    Entry = entry,
                    Categories = list
                });
            }

            if (MainWindow.RootWindow.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.BlogSearchItems = new ObservableCollection<BlogSearchModel>(blogSearchItems);
            }

            MainWindow.MainDialog.IsOpen = false;

            MainWindowSnackBarMessage.Show("获取成功", SnackBarMessageType.Success);
        }
    }
}