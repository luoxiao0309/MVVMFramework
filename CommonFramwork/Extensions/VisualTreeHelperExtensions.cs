using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CommonFramework.Extensions
{
    /// <summary>
    /// Provides useful extensions for working with the visual tree.
    /// VisualTree 视觉树
    /// </summary>
    /// <remarks>
    /// Since many of these extension methods are declared on types like
    /// DependencyObject high up in the class hierarchy, we've placed them in
    /// the Primitives namespace which is less likely to be imported for normal
    /// scenarios.
    /// </remarks>
    /// <QualityBand>Experimental</QualityBand>
    public static class VisualTreeHelperExtensions
    {
        public static T FindTemplate<T>(this Control root, string name)
            where T : Visual
        {
            var template = root.Template.FindName(name, root);
            Debug.Assert(template != null, $"{name}が実装されていません。");

            var result = template as T;
            Debug.Assert(result != null, $"{name}は{typeof(T).Name}で実装する必要があります。");

            return result;
        }

        public static T FindChildWithName<T>(this FrameworkElement root, string name)
            where T : FrameworkElement
        {
            return root.FindChild<T>(x => x.Name == name);
        }

        public static T FindChildWithDataContext<T>(this FrameworkElement root, object datContext)
            where T : FrameworkElement
        {
            return root.FindChild<T>(x => x.DataContext == datContext);
        }

        public static T FindChild<T>(this FrameworkElement root, Func<FrameworkElement, bool> compare = null)
            where T : FrameworkElement
        {
            if (compare is null)
                compare = x => true;

            var children = Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(root)).Select(x => VisualTreeHelper.GetChild(root, x)).OfType<FrameworkElement>().ToArray();

            foreach (var child in children)
            {
                if (child is T t && compare(child))
                    return t;

                t = child.FindChild<T>(compare);
                if (t != null)
                    return t;
            }
            return null;
        }

        public static bool ContainChildren(this FrameworkElement root, FrameworkElement @object)
        {
            if (@object is null)
                return false;

            if (root == @object)
                return true;

            var children = Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(root)).Select(x => VisualTreeHelper.GetChild(root, x)).OfType<FrameworkElement>().ToArray();

            foreach (var child in children)
            {
                if (child == @object)
                    return true;

                if (child.ContainChildren(@object))
                    return true;
            }
            return false;
        }

        public static bool HitTestCircle(this Visual root, Point center, double radius)
        {
            var result = false;
            VisualTreeHelper.HitTest(root, null,
                _ =>
                {
                    result = true; return HitTestResultBehavior.Stop;
                },
                new GeometryHitTestParameters(new EllipseGeometry(center, radius, radius)));
            return result;
        }

        public static bool HitTestRect(this Visual root, Point center, double width, double height)
        {
            var result = false;
            var rect = new Rect(center.X - width / 2, center.Y - height / 2, width, height);
            VisualTreeHelper.HitTest(root, null,
                _ =>
                {
                    result = true; return HitTestResultBehavior.Stop;
                },
                new GeometryHitTestParameters(new RectangleGeometry(rect)));
            return result;
        }

        public static bool HitTestRect(this Visual root, Rect rect)
        {
            var result = false;
            VisualTreeHelper.HitTest(root, null,
                _ =>
                {
                    result = true; return HitTestResultBehavior.Stop;
                },
                new GeometryHitTestParameters(new RectangleGeometry(rect)));
            return result;
        }

        public static IEnumerable<T> GetHitTestChildrenWithRect<T>(this Visual root, Rect rect) where T : Visual
        {
            var result = new List<DependencyObject>();
            VisualTreeHelper.HitTest(root, null,
                x =>
                {
                    result.Add(x.VisualHit);
                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(new RectangleGeometry(rect)));

            return result.Select(x => x.FindVisualParentWithType<T>())
                .Where(x => x != null)
                .Distinct();
        }

        /// <summary>
        /// Get the bounds of an element relative to another element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="otherElement">
        /// The element relative to the other element.
        /// </param>
        /// <returns>
        /// The bounds of the element relative to another element, or null if
        /// the elements are not related.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="otherElement"/> is null.
        /// </exception>
        public static Rect? GetBoundsRelativeTo(this FrameworkElement element, UIElement otherElement)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            else if (otherElement == null)
            {
                throw new ArgumentNullException("otherElement");
            }

            try
            {
#pragma warning disable IDE0018 // 内联变量声明
                Point origin, bottom;
#pragma warning restore IDE0018 // 内联变量声明
                GeneralTransform transform = element.TransformToVisual(otherElement);
                if (transform != null &&
                    transform.TryTransform(new Point(), out origin) &&
                    transform.TryTransform(new Point(element.ActualWidth, element.ActualHeight), out bottom))
                {
                    return new Rect(origin, bottom);
                }
            }
            catch (ArgumentException)
            {
                // Ignore any exceptions thrown while trying to transform
            }

            return null;
        }

        /// <summary>
        /// Perform an action when the element's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public static void InvokeOnLayoutUpdated(this FrameworkElement element, Action action)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            else if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            // Create an event handler that unhooks itself before calling the
            // action and then attach it to the LayoutUpdated event.
#pragma warning disable IDE0039 // 使用本地函数
            EventHandler handler = null;
            handler = (s, e) =>
            {
                element.LayoutUpdated -= handler;
                action();
            };
            element.LayoutUpdated += handler;
#pragma warning restore IDE0039 // 使用本地函数
        }

        /// <summary>
        /// Retrieves all the logical children of a framework element using a
        /// breadth-first search. For performance reasons this method manually
        /// manages the stack instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            Popup popup = parent as Popup;
            if (popup != null)
            {
                FrameworkElement popupChild = popup.Child as FrameworkElement;
                if (popupChild != null)
                {
                    yield return popupChild;
                }
            }

            // If control is an items control return all children using the
            // Item container generator.
            ItemsControl itemsControl = parent as ItemsControl;
            if (itemsControl != null)
            {
                foreach (FrameworkElement logicalChild in
                    Enumerable
                        .Range(0, itemsControl.Items.Count)
                        .Select(index => itemsControl.ItemContainerGenerator.ContainerFromIndex(index))
                        .OfType<FrameworkElement>())
                {
                    yield return logicalChild;
                }
            }

            string parentName = parent.Name;
            Queue<FrameworkElement> queue =
                new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                if (element.Parent == parent || element is UserControl)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                    {
                        queue.Enqueue(visualChild);
                    }
                }
            }
        }
        
        public static Visual FindDescendantByName(Visual element, string name)
        {
            if (element != null && (element is FrameworkElement) && (element as FrameworkElement).Name == name)
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = FindDescendantByName(visual, name);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        public static Visual FindDescendantByType(Visual element, Type type)
        {
            return FindDescendantByType(element, type, true);
        }

        public static Visual FindDescendantByType(Visual element, Type type, bool specificTypeOnly)
        {
            if (element == null)
                return null;

            if (specificTypeOnly ? (element.GetType() == type)
                : (element.GetType() == type) || (element.GetType().IsSubclassOf(type)))
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = FindDescendantByType(visual, type, specificTypeOnly);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        public static T FindDescendantByType<T>(Visual element) where T : Visual
        {
            Visual temp = FindDescendantByType(element, typeof(T));

            return (T)temp;
        }

        public static Visual FindDescendantWithPropertyValue(Visual element,
            DependencyProperty dp, object value)
        {
            if (element == null)
                return null;

            if (element.GetValue(dp).Equals(value))
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = FindDescendantWithPropertyValue(visual, dp, value);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        
    }
}
