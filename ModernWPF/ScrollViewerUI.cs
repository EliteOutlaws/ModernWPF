﻿using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    /// <summary>
    /// Contains various attached properties for <see cref="ScrollViewer"/> using the modern theme.
    /// </summary>
    public static class ScrollViewerUI
    {

        #region HScroll attached dp

        /// <summary>
        /// Attached propert to allow horizontal scroll by mouse wheel if possible.
        /// </summary>
        public static readonly DependencyProperty HScrollOnWheelProperty =
            DependencyProperty.RegisterAttached
            (
                "HScrollOnWheel",
                typeof(bool),
                typeof(ScrollViewerUI),
                new PropertyMetadata(true, OnHScrollOnWheelPropertyChanged)
            );
        /// <summary>
        /// Gets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetHScrollOnWheel(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(HScrollOnWheelProperty);
        }
        /// <summary>
        /// Sets the HScrollOnWheel property for this object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> then h-scroll logic will be used if possible.</param>
        public static void SetHScrollOnWheel(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(HScrollOnWheelProperty, value);
        }

        private static void OnHScrollOnWheelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            var scroller = d as ScrollViewer;
            if (scroller == null)// dpo is ItemsControl)
            {
                //scroller = ((ItemsControl)dpo).TryGetScrollerViewer();
                scroller = d.FindInVisualTree<ScrollViewer>();
            }

            // animated scroll viewer handles this already so do nothing
            if (scroller != null && !(scroller is AnimatedScrollViewer))
            {
                if ((bool)e.NewValue)
                {
                    scroller.PreviewMouseWheel -= scroller_PreviewMouseWheel;
                    scroller.PreviewMouseWheel += scroller_PreviewMouseWheel;
                }
                else
                {
                    scroller.PreviewMouseWheel -= scroller_PreviewMouseWheel;
                }
            }
        }

        static void scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;

            // only allow h-scroll if not doing v-scroll

            if (scroller != null)
            {
                if (e.Delta < 0)
                {
                    if (!scroller.CanVScrollDown() && scroller.CanHScrollRight())
                    {
                        //scroller.LineRight();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + 48);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (!scroller.CanVScrollUp() && scroller.CanHScrollLeft())
                    {
                        //scroller.LineLeft();
                        scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - 48);
                        e.Handled = true;
                    }
                }
            }
        }

        #endregion

        #region over content dp

        /// <summary>
        /// Gets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetOverContent(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (bool)obj.GetValue(OverContentProperty);
        }

        /// <summary>
        /// Sets whether the scrollbar will cover the content.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value flag.</param>
        public static void SetOverContent(DependencyObject obj, bool value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(OverContentProperty, value);
        }

        /// <summary>
        /// DP on whether the scrollbar will cover the content.
        /// </summary>
        public static readonly DependencyProperty OverContentProperty =
            DependencyProperty.RegisterAttached("OverContent", typeof(bool), typeof(ScrollViewerUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        #endregion

    }
}
