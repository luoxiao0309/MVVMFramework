using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeControl
{
    public interface IWindow
    {
        void AddNode(string name, Point position);
    }
}
