using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace CommonFramework.Helper
{
    /// <summary>
    /// Different helper methods for WPF.
    /// </summary>
    public static class Helper
    {
        public static Window FindParentWindow(FrameworkElement element)
        {
            if (element.Parent == null)
            {
                return null;
            }

            Window window = element.Parent as Window;
            if (window != null)
            {
                return window;
            }

            FrameworkElement parentElement = element.Parent as FrameworkElement;
            if (parentElement != null)
            {
                return FindParentWindow(parentElement);
            }

            return null;
        }

        public static FrameworkElement FindParentWithDataContextAndName<DataContextT>(FrameworkElement childElement, string name)
            where DataContextT : class
        {
            FrameworkElement parent = (FrameworkElement)childElement.Parent;
            if (parent != null)
            {
                DataContextT data = parent.DataContext as DataContextT;
                if (data != null)
                {
                    if (parent.Name == name)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithDataContextAndName<DataContextT>(parent, name);
                if (parent != null)
                {
                    return parent;
                }
            }

            parent = (FrameworkElement)childElement.TemplatedParent;
            if (parent != null)
            {
                DataContextT data = parent.DataContext as DataContextT;
                if (data != null)
                {
                    if (parent.Name == name)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithDataContextAndName<DataContextT>(parent, name);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }

        public static FrameworkElement FindParentWithDataContext<DataContextT>(FrameworkElement childElement)
            where DataContextT : class
        {
            if (childElement.Parent != null)
            {
                DataContextT data = ((FrameworkElement)childElement.Parent).DataContext as DataContextT;
                if (data != null)
                {
                    return (FrameworkElement)childElement.Parent;
                }

                FrameworkElement parent = FindParentWithDataContext<DataContextT>((FrameworkElement)childElement.Parent);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent != null)
            {
                DataContextT data = ((FrameworkElement)childElement.TemplatedParent).DataContext as DataContextT;
                if (data != null)
                {
                    return (FrameworkElement)childElement.TemplatedParent;
                }

                FrameworkElement parent = FindParentWithDataContext<DataContextT>((FrameworkElement)childElement.TemplatedParent);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }

        public static ParentT FindVisualParentWithType<ParentT>(FrameworkElement childElement)
            where ParentT : class
        {
            FrameworkElement parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            if (parentElement != null)
            {
                ParentT parent = parentElement as ParentT;
                if (parent != null)
                {
                    return parent;
                }

                return FindVisualParentWithType<ParentT>(parentElement);
            }

            return null;
        }

        public static ParentT FindParentWithType<ParentT>(FrameworkElement childElement)
            where ParentT : class
        {
            if (childElement.Parent != null)
            {
                ParentT parent = childElement.Parent as ParentT;
                if (parent != null)
                {
                    return parent;
                }

                parent = FindParentWithType<ParentT>((FrameworkElement)childElement.Parent);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent != null)
            {
                ParentT parent = childElement.TemplatedParent as ParentT;
                if (parent != null)
                {
                    return parent;
                }

                parent = FindParentWithType<ParentT>((FrameworkElement)childElement.TemplatedParent);
                if (parent != null)
                {
                    return parent;
                }
            }

            FrameworkElement parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            if (parentElement != null)
            {
                ParentT parent = parentElement as ParentT;
                if (parent != null)
                {
                    return parent;
                }

                return FindParentWithType<ParentT>(parentElement);
            }

            return null;
        }

        public static ParentT FindParentWithTypeAndDataContext<ParentT>(FrameworkElement childElement, object dataContext)
            where ParentT : FrameworkElement
        {
            if (childElement.Parent != null)
            {
                ParentT parent = childElement.Parent as ParentT;
                if (parent != null)
                {
                    if (parent.DataContext == dataContext)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithTypeAndDataContext<ParentT>((FrameworkElement)childElement.Parent, dataContext);
                if (parent != null)
                {
                    return parent;
                }
            }

            if (childElement.TemplatedParent != null)
            {
                ParentT parent = childElement.TemplatedParent as ParentT;
                if (parent != null)
                {
                    if (parent.DataContext == dataContext)
                    {
                        return parent;
                    }
                }

                parent = FindParentWithTypeAndDataContext<ParentT>((FrameworkElement)childElement.TemplatedParent, dataContext);
                if (parent != null)
                {
                    return parent;
                }
            }

            FrameworkElement parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
            if (parentElement != null)
            {
                ParentT parent = parentElement as ParentT;
                if (parent != null)
                {
                    return parent;
                }

                return FindParentWithType<ParentT>(parentElement);
            }

            return null;
        }

        public static T HitTestHighestForDataContext<T>(FrameworkElement rootElement, Point point)
            where T : class
        {
            FrameworkElement hitFrameworkElement = null;
            return HitTestHighestForDataContext<T>(rootElement, point, out hitFrameworkElement);
        }

        public static T HitTestHighestForDataContext<T>(FrameworkElement rootElement,
                                                  Point point, out FrameworkElement hitFrameworkElement)
            where T : class
        {
            hitFrameworkElement = null;

            FrameworkElement hitElement = null;
            T hitData = HitTestForDataContext<T, FrameworkElement>(rootElement, point, out hitElement);
            if (hitData == null)
            {
                return null;
            }

            hitFrameworkElement = hitElement;

            //
            // Find the highest level parent below root element that still matches the data context.
            while (hitElement != null && hitElement != rootElement &&
                   hitElement.DataContext == hitData)
            {
                hitFrameworkElement = hitElement;

                if (hitElement.Parent != null)
                {
                    hitElement = hitElement.Parent as FrameworkElement;
                    continue;
                }

                if (hitElement.TemplatedParent != null)
                {
                    hitElement = hitElement.TemplatedParent as FrameworkElement;
                    continue;
                }

                break;
            }

            return hitData;
        }

        public static DataContextT HitTestForDataContextAndName<DataContextT, ElementT>(FrameworkElement rootElement,
                                          Point point, string name, out ElementT hitFrameworkElement)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            DataContextT data = null;
            hitFrameworkElement = null;
            ElementT frameworkElement = null;

            VisualTreeHelper.HitTest(
                    rootElement,
                    // Hit test filter.
                    null,
                    // Hit test result.
                    delegate (HitTestResult result)
                    {
                        frameworkElement = result.VisualHit as ElementT;
                        if (frameworkElement != null)
                        {
                            data = frameworkElement.DataContext as DataContextT;
                            if (data != null)
                            {
                                if (frameworkElement.Name == name)
                                {
                                    return HitTestResultBehavior.Stop;
                                }
                            }
                        }

                        return HitTestResultBehavior.Continue;
                    },

                    new PointHitTestParameters(point));

            hitFrameworkElement = frameworkElement;
            return data;
        }

        public static DataContextT HitTestForDataContext<DataContextT, ElementT>(FrameworkElement rootElement,
                                          Point point, out ElementT hitFrameworkElement)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            DataContextT data = null;
            hitFrameworkElement = null;
            ElementT frameworkElement = null;

            VisualTreeHelper.HitTest(
                    rootElement,
                    // Hit test filter.
                    null,
                    // Hit test result.
                    delegate (HitTestResult result)
                    {
                        frameworkElement = result.VisualHit as ElementT;
                        if (frameworkElement != null)
                        {
                            data = frameworkElement.DataContext as DataContextT;
                            return data != null ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
                        }

                        return HitTestResultBehavior.Continue;
                    },

                    new PointHitTestParameters(point));

            hitFrameworkElement = frameworkElement;
            return data;
        }

        public static T FindAncestor<T>(FrameworkElement element) where T : class
        {
            if (element.Parent != null)
            {
                T ancestor = element.Parent as T;
                if (ancestor != null)
                {
                    return ancestor;
                }

                FrameworkElement parent = element.Parent as FrameworkElement;
                if (parent != null)
                {
                    return FindAncestor<T>(parent);
                }
            }

            if (element.TemplatedParent != null)
            {
                T ancestor = element.TemplatedParent as T;
                if (ancestor != null)
                {
                    return ancestor;
                }

                FrameworkElement parent = element.TemplatedParent as FrameworkElement;
                if (parent != null)
                {
                    return FindAncestor<T>(parent);
                }
            }

            DependencyObject visualParent = VisualTreeHelper.GetParent(element);
            if (visualParent != null)
            {
                T visualAncestor = visualParent as T;
                if (visualAncestor != null)
                {
                    return visualAncestor;
                }

                FrameworkElement visualElement = visualParent as FrameworkElement;
                if (visualElement != null)
                {
                    return FindAncestor<T>(visualElement);
                }
            }

            return null;
        }

        public static Point TransformPointToAncestor<T>(FrameworkElement element, Point point) where T : Visual
        {
            T ancestor = FindAncestor<T>(element);
            if (ancestor == null)
            {
                throw new ApplicationException("Find to find '" + typeof(T).Name + "' for element '" + element.GetType().Name + "'.");
            }

            return TransformPointToAncestor(ancestor, element, point);
        }

        public static Point TransformPointToAncestor(Visual ancestor, FrameworkElement element, Point point)
        {
            return element.TransformToAncestor(ancestor).Transform(point);
        }

        public static ElementT FindElementWithName<ElementT>(Visual rootElement, string name)
            where ElementT : FrameworkElement
        {
            FrameworkElement rootFrameworkElement = rootElement as FrameworkElement;
            if (rootFrameworkElement != null)
            {
                rootFrameworkElement.UpdateLayout();
            }

            int numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (int i = 0; i < numChildren; ++i)
            {
                Visual childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                ElementT typedChildElement = childElement as ElementT;
                if (typedChildElement != null)
                {
                    if (typedChildElement.Name == name)
                    {
                        return typedChildElement;
                    }
                }

                ElementT foundElement = FindElementWithName<ElementT>(childElement, name);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public static ElementT FindElementWithDataContextAndName<DataContextT, ElementT>(Visual rootElement, DataContextT data, string name)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            Trace.Assert(rootElement != null);

            FrameworkElement rootFrameworkElement = rootElement as FrameworkElement;
            if (rootFrameworkElement != null)
            {
                rootFrameworkElement.UpdateLayout();
            }

            int numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (int i = 0; i < numChildren; ++i)
            {
                Visual childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                ElementT typedChildElement = childElement as ElementT;
                if (typedChildElement != null &&
                    typedChildElement.DataContext == data)
                {
                    if (typedChildElement.Name == name)
                    {
                        return typedChildElement;
                    }
                }

                ElementT foundElement = FindElementWithDataContextAndName<DataContextT, ElementT>(childElement, data, name);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public static ElementT FindElementWithType<ElementT>(Visual rootElement)
            where ElementT : FrameworkElement
        {
            if (rootElement == null)
            {
                throw new ArgumentNullException("rootElement");
            }

            FrameworkElement rootFrameworkElement = rootElement as FrameworkElement;
            if (rootFrameworkElement != null)
            {
                rootFrameworkElement.UpdateLayout();
            }

            //
            // Check each child.
            //
            int numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (int i = 0; i < numChildren; ++i)
            {
                Visual childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                ElementT typedChildElement = childElement as ElementT;
                if (typedChildElement != null)
                {
                    return typedChildElement;
                }
            }

            //
            // Check sub-trees.
            //
            for (int i = 0; i < numChildren; ++i)
            {
                Visual childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                ElementT foundElement = FindElementWithType<ElementT>(childElement);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public static ElementT FindElementWithDataContext<DataContextT, ElementT>(Visual rootElement, DataContextT data)
            where DataContextT : class
            where ElementT : FrameworkElement
        {
            if (rootElement == null)
            {
                throw new ArgumentNullException("rootElement");
            }

            FrameworkElement rootFrameworkElement = rootElement as FrameworkElement;
            if (rootFrameworkElement != null)
            {
                rootFrameworkElement.UpdateLayout();
            }

            int numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (int i = 0; i < numChildren; ++i)
            {
                Visual childElement = (Visual)VisualTreeHelper.GetChild(rootElement, i);

                ElementT typedChildElement = childElement as ElementT;
                if (typedChildElement != null &&
                    typedChildElement.DataContext == data)
                {
                    return typedChildElement;
                }

                ElementT foundElement = FindElementWithDataContext<DataContextT, ElementT>(childElement, data);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public static DataTemplateT FindTemplateForType<DataTemplateT>(Type type, FrameworkElement element)
            where DataTemplateT : class
        {
            object resource = element.TryFindResource(new DataTemplateKey(type));
            DataTemplateT dataTemplate = resource as DataTemplateT;
            if (dataTemplate != null)
            {
                return dataTemplate;
            }

            if (type.BaseType != null &&
                type.BaseType != typeof(object))
            {
                dataTemplate = FindTemplateForType<DataTemplateT>(type.BaseType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            foreach (Type interfaceType in type.GetInterfaces())
            {
                dataTemplate = FindTemplateForType<DataTemplateT>(interfaceType, element);
                if (dataTemplate != null)
                {
                    return dataTemplate;
                }
            }

            return null;
        }

        public static FrameworkElement CreateVisual(Type type, FrameworkElement element, object dataContext)
        {
            DataTemplate template = FindTemplateForType<DataTemplate>(type, element);
            if (template == null)
            {
                throw new ApplicationException("Failed to find DataTemplate for type " + type.Name);
            }

            FrameworkElement visual = (FrameworkElement)template.LoadContent();
            visual.Resources = element.Resources;
            visual.DataContext = dataContext;
            return visual;
        }

        public static void InitaliseElement(FrameworkElement element)
        {
            element.UpdateLayout();
            element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
        }

        public static ICollection<T> FindTypedElements<T>(DependencyObject rootElement) where T : DependencyObject
        {
            List<T> foundElements = new List<T>();
            FindTypedElements(rootElement, foundElements);
            return foundElements;
        }

        private static void FindTypedElements<T>(DependencyObject rootElement, List<T> foundElements) where T : DependencyObject
        {
            int numChildren = VisualTreeHelper.GetChildrenCount(rootElement);
            for (int i = 0; i < numChildren; ++i)
            {
                DependencyObject childElement = VisualTreeHelper.GetChild(rootElement, i);
                if (childElement is T)
                {
                    foundElements.Add((T)childElement);
                    continue;
                }

                FindTypedElements<T>(childElement, foundElements);
            }
        }

        public static void DumpVisualTree(Visual root)
        {
            DumpVisualTree(root, 0);
        }

        private static void DumpVisualTree(Visual root, int indentLevel)
        {
            string indentStr = new string(' ', indentLevel * 2);
            Trace.Write(indentStr);
            Trace.Write(root.GetType().Name);

            FrameworkElement rootElement = root as FrameworkElement;
            if (rootElement != null)
            {
                if (rootElement.DataContext != null)
                {
                    Trace.Write(" [");
                    Trace.Write(rootElement.DataContext.GetType().Name);
                    Trace.Write("]");
                }
            }

            Trace.WriteLine("");

            int numChildren = VisualTreeHelper.GetChildrenCount(root);
            if (numChildren > 0)
            {
                Trace.Write(indentStr);
                Trace.WriteLine("{");

                for (int i = 0; i < numChildren; ++i)
                {
                    Visual child = (Visual)VisualTreeHelper.GetChild(root, i);
                    DumpVisualTree(child, indentLevel + 1);
                }

                Trace.Write(indentStr);
                Trace.WriteLine("}");
            }
        }


    }
}
