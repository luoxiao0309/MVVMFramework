﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonFramework.MVVM
{
    /// <summary>
    /// 弱引用
    /// </summary>
    public static class WeakEventHandlerManager
    {
        private static readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        public static void CallWeakReferenecHandlers(object sender, List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                var calllees = new EventHandler[handlers.Count];
                int count = 0;

                count = CleanupOldHandlers(handlers, calllees, count);

                for (int i = 0; i < count; i++)
                {
                    CallHandler(sender, calllees[i]);
                }
            }
        }

        private static void CallHandler(object sender, EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                if (syncContext != null)
                {
                    syncContext.Post((o) => eventHandler(sender, EventArgs.Empty), null);
                }
                else
                {
                    eventHandler(sender, EventArgs.Empty);
                }
            }
        }

        private static int CleanupOldHandlers(List<WeakReference> handlers, EventHandler[] callees, int count)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                WeakReference reference = handlers[i];
                EventHandler handler = reference.Target as EventHandler;

                if (handler == null)
                {
                    handlers.RemoveAt(i);
                }
                else
                {
                    callees[count] = handler;
                    count++;
                }
            }

            return count;
        }

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler,
            int defaultListSize)
        {
            if (handler == null)

            {
                handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler existingHandler = reference.Target as EventHandler;
                    if ((existingHandler == null) || (existingHandler == handler))
                    {
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}
