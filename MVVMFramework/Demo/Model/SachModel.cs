using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFramework.Model
{
    public class SachModel
    {
        private int maSach;
        private string tenSach;

        public int MaSach
        {
            get { return maSach; }
            set { maSach = value; }
        }
        public string TenSach
        {
            get { return tenSach; }
            set { tenSach = value; }
        }
    }
}
