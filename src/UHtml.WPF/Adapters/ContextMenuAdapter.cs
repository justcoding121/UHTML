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
        private readonly ContextMenu contextMenu;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public ContextMenuAdapter()
        {
            contextMenu = new ContextMenu();
        }

        public override int ItemsCount
        {
            get { return contextMenu.Items.Count; }
        }

        public override void AddDivider()
        {
            contextMenu.Items.Add(new Separator());
        }

        public override void AddItem(string text, bool enabled, EventHandler onClick)
        {
            ArgChecker.AssertArgNotNullOrEmpty(text, "text");
            ArgChecker.AssertArgNotNull(onClick, "onClick");

            var item = new MenuItem();
            item.Header = text;
            item.IsEnabled = enabled;
            item.Click += new RoutedEventHandler(onClick);
            contextMenu.Items.Add(item);
        }

        public override void RemoveLastDivider()
        {
            if (contextMenu.Items[contextMenu.Items.Count - 1].GetType() == typeof(Separator))
                contextMenu.Items.RemoveAt(contextMenu.Items.Count - 1);
        }

        public override void Show(RControl parent, RPoint location)
        {
            contextMenu.PlacementTarget = ((ControlAdapter)parent).Control;
            contextMenu.PlacementRectangle = new Rect(Utils.ConvertRound(location), Size.Empty);
            contextMenu.IsOpen = true;
        }

        public override void Dispose()
        {
            contextMenu.IsOpen = false;
            contextMenu.PlacementTarget = null;
            contextMenu.Items.Clear();
        }
    }
}