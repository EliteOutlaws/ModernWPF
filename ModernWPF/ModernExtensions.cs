﻿using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Permissions;
using System.Windows.Threading;

namespace ModernWPF
{
    /// <summary>
    /// Extension methods for using this lib.
    /// </summary>
    public static class ModernExtensions
    {
        /// <summary>
        /// Try to the get <see cref="ScrollViewer" /> from an <see cref="ItemsControl" />.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static ScrollViewer TryGetScrollViewer(this ItemsControl control)
        {
            if (control != null && VisualTreeHelper.GetChildrenCount(control) > 0)
            {
                Decorator border = VisualTreeHelper.GetChild(control, 0) as Decorator;
                if (border != null)
                {
                    return border.Child as ScrollViewer;
                }
            }
            return null;
        }


        /// <summary>
        /// Finds the first specified object type in visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static T FindInVisualTree<T>(this DependencyObject control) where T : DependencyObject
        {
            if (control != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(control);

                for (int i = 0; i < count; i++)
                {
                    var c = VisualTreeHelper.GetChild(control, i);
                    var casted = c as T;
                    if (casted != null)
                    {
                        return casted;
                    }
                    else if (c != null)
                    {
                        var subHit = FindInVisualTree<T>(c);
                        if (subHit != null) { return subHit; }
                    }
                }
            }
            return null;
        }

        internal static bool ProcessInVisualTree<T>(this DependencyObject control, Predicate<T> callback) where T : DependencyObject
        {
            if (control != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(control);

                for (int i = 0; i < count; i++)
                {
                    var c = VisualTreeHelper.GetChild(control, i);
                    var casted = c as T;
                    if (casted != null)
                    {
                        var result = callback(casted);
                        if (result) { return true; }
                    }
                    if (c != null)
                    {
                        var subResult = ProcessInVisualTree<T>(c, callback);
                        if (subResult) { return true; }
                    }
                }
            }
            return false;
        }



        internal static bool CanVScrollDown(this ScrollViewer scroller)
        {
            return scroller.ScrollableHeight > 0 && scroller.VerticalOffset < scroller.ScrollableHeight;
        }
        internal static bool CanHScrollRight(this ScrollViewer scroller)
        {
            return scroller.ScrollableWidth > 0 && scroller.HorizontalOffset < scroller.ScrollableWidth;
        }

        internal static bool CanVScrollUp(this ScrollViewer scroller)
        {
            return scroller.ScrollableHeight > 0 && scroller.VerticalOffset > 0;
        }
        internal static bool CanHScrollLeft(this ScrollViewer scroller)
        {
            return scroller.ScrollableWidth > 0 && scroller.HorizontalOffset > 0;
        }





        // from msdn http://msdn.microsoft.com/library/system.windows.threading.dispatcher.pushframe.aspx

        /// <summary>
        /// Simulate the famous DoEvents() method from winform days.
        /// </summary>
        /// <param name="application">The application.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Application application)
        {
            application.Dispatcher.DoEvents();
        }

        /// <summary>
        /// Simulate the famous DoEvents() method from winform days.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Dispatcher dispatcher)
        {
            DispatcherFrame frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
    }
}
