using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonFramework.Extensions
{
    public static class UIElementExtension
    {
        /// <summary>
        /// 移动UI元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="Deleta">变化量</param>
        public static void MoveElement(this UIElement element, Vector Deleta)
        {
            Canvas.SetLeft(element, Canvas.GetLeft(element) + Deleta.X);
            Canvas.SetTop(element, Canvas.GetTop(element) + Deleta.Y);
        }

        /// <summary>
        /// 获取UI元素的位置
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Point GetUIElementPosition(this UIElement element)
        {
            return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        }
    }
}
