







// 



using System;
using System.Windows;
using System.Windows.Controls;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;
using UHtml.WPF.Utilities;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF context menu for core.
    /// </summary>
    internal sealed class ContextMenuAdapter : RContextMenu
    {
        #region Fields and Consts

        /// <summary>
        /// the underline WPF context menu
        /// </summary>
        private readonly ContextMenu _contextMenu;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public ContextMenuAdapter()
        {
            _contextMenu = new ContextMenu();
        }

        public override int ItemsCount
        {
            get { return _contextMenu.Items.Count; }
        }

        public override void AddDivider()
        {
            _contextMenu.Items.Add(new Separator());
        }

        public override void AddItem(string text, bool enabled, EventHandler onClick)
        {
            ArgChecker.AssertArgNotNullOrEmpty(text, "text");
            ArgChecker.AssertArgNotNull(onClick, "onClick");

            var item = new MenuItem();
            item.Header = text;
            item.IsEnabled = enabled;
            item.Click += new RoutedEventHandler(onClick);
            _contextMenu.Items.Add(item);
        }

        public override void RemoveLastDivider()
        {
            if (_contextMenu.Items[_contextMenu.Items.Count - 1].GetType() == typeof(Separator))
                _contextMenu.Items.RemoveAt(_contextMenu.Items.Count - 1);
        }

        public override void Show(RControl parent, RPoint location)
        {
            _contextMenu.PlacementTarget = ((ControlAdapter)parent).Control;
            _contextMenu.PlacementRectangle = new Rect(Utils.ConvertRound(location), Size.Empty);
            _contextMenu.IsOpen = true;
        }

        public override void Dispose()
        {
            _contextMenu.IsOpen = false;
            _contextMenu.PlacementTarget = null;
            _contextMenu.Items.Clear();
        }
    }
}