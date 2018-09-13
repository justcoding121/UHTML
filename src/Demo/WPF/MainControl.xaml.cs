using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UHtml.Demo.Common;
using UHtml.WPF;

namespace UHtml.Demo.WPF
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl
    {
        #region Fields and Consts

        /// <summary>
        /// timer to update the rendered html when html in editor changes with delay
        /// </summary>
        private readonly Timer updateHtmlTimer;

        /// <summary>
        /// used ignore html editor updates when updating separately
        /// </summary>
        private bool updateLock;

        /// <summary>
        /// In IE view if to show original html or the html generated from the html control
        /// </summary>
        private bool useGeneratedHtml;

        #endregion

        public MainControl()
        {
            InitializeComponent();

            //_htmlPanel.RenderError += OnRenderError;
            //_htmlPanel.LinkClicked += OnLinkClicked;
           //_htmlPanel.StylesheetLoad += HtmlRenderingHelper.OnStylesheetLoad;
            //_htmlPanel.ImageLoad += HtmlRenderingHelper.OnImageLoad;
            _htmlPanel.LoadComplete += (sender, args) => _htmlPanel.ScrollToElement("C4");

            LoadSamples();

            updateHtmlTimer = new Timer(OnUpdateHtmlTimerTick);
        }


        /// <summary>
        /// In IE view if to show original html or the html generated from the html control
        /// </summary>
        public bool UseGeneratedHtml
        {
            get { return useGeneratedHtml; }
            set { useGeneratedHtml = value; }
        }

        public string GetHtml()
        {
            return useGeneratedHtml ? _htmlPanel.GetHtml() : GetHtmlEditorText();
        }


        #region Private methods

        /// <summary>
        /// Loads the tree of document samples
        /// </summary>
        private void LoadSamples()
        {
            var cssTestSamplesRoot = new TreeViewItem();
            cssTestSamplesRoot.Header = "Css Test Samples";
            _samplesTreeView.Items.Add(cssTestSamplesRoot);

            bool varFirstCategory = true;
            foreach (var group in SamplesLoader.CssTestSamples.GroupBy(x => x.Category))
            {
                var groupTreeItem = new TreeViewItem();
                groupTreeItem.Header = group.Key;
                cssTestSamplesRoot.Items.Add(groupTreeItem);


                foreach (var sample in group.ToList())
                {
                    AddTreeItem(groupTreeItem, sample);
                }

                if (varFirstCategory)
                {
                    groupTreeItem.IsExpanded = true;
                    ((TreeViewItem)groupTreeItem.Items[0]).IsSelected = true;
                    varFirstCategory = false;
                }
            }

            cssTestSamplesRoot.IsExpanded = true;

        }

        /// <summary>
        /// Add an html sample to the tree and to all samples collection
        /// </summary>
        private void AddTreeItem(TreeViewItem root, HtmlSample sample)
        {
            var html = sample.Html.Replace("$$Release$$", _htmlPanel.GetType().Assembly.GetName().Version.ToString());

            var node = new TreeViewItem();
            node.Header = sample.Name;
            node.Tag = new HtmlSample(sample.Name, sample.Category, sample.FullName, html);
            root.Items.Add(node);
        }

        /// <summary>
        /// On tree view node click load the html to the html panel and html editor.
        /// </summary>
        private void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = ((TreeViewItem)e.NewValue);
            var sample = item.Tag as HtmlSample;
            if (sample != null)
            {
                updateLock = true;
                Cursor = Cursors.Wait;

                try
                {
                    _htmlPanel.AvoidImagesLateLoading = !sample.FullName.Contains("Many images");
                    _htmlPanel.Text = sample.Html;
                    _htmlEditor.Text = sample.Html;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Failed to render HTML");
                }

                Cursor = Cursors.Arrow;
                updateLock = false;

            }
        }

        /// <summary>
        /// On text change in the html editor update 
        /// </summary>
        private void OnHtmlEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!updateLock)
            {
                updateHtmlTimer.Change(1000, int.MaxValue);
            }
        }

        /// <summary>
        /// Update the html renderer with text from html editor.
        /// </summary>
        private void OnUpdateHtmlTimerTick(object state)
        {
            Dispatcher.BeginInvoke(new Action<Object>(o =>
            {
                updateLock = true;

                try
                {
                    _htmlPanel.Text = GetHtmlEditorText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Failed to render HTML");
                }

                updateLock = false;
            }), state);
        }

        /// <summary>
        /// Reload the html shown in the html editor by running coloring again.
        /// </summary>
        private void OnRefreshLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!updateLock)
            {
                updateHtmlTimer.Change(1000, int.MaxValue);
            }
        }

        /// <summary>
        /// Show error raised from html renderer.
        /// </summary> 
        //private void OnRenderError(object sender, RoutedEvenArgs<HtmlRenderErrorEventArgs> args)
        //{
        //    Dispatcher.BeginInvoke(new Action(() => MessageBox.Show(args.Data.Message + (args.Data.Exception != null ? "\r\n" + args.Data.Exception : null), "Error in Html Renderer", MessageBoxButton.OK)));
        //}

        /// <summary>
        /// On specific link click handle it here.
        /// </summary>
        //private void OnLinkClicked(object sender, RoutedEvenArgs<HtmlLinkClickedEventArgs> args)
        //{
        //    if (args.Data.Link == "SayHello")
        //    {
        //        MessageBox.Show("Hello you!");
        //        args.Data.Handled = true;
        //    }
        //    else if (args.Data.Link == "ShowSampleForm")
        //    {
        //        var w = new SampleWindow();
        //        var window = Window.GetWindow(this);
        //        if (window != null)
        //        {
        //            w.Owner = window;
        //            w.Width = window.Width * 0.8;
        //            w.Height = window.Height * 0.8;
        //            w.ShowDialog();
        //        }
        //        args.Data.Handled = true;
        //    }
        //}

  

        /// <summary>
        /// Get the html text from the html editor control.
        /// </summary>
        private string GetHtmlEditorText()
        {
            return _htmlEditor.Text;
        }

        #endregion
    }
}