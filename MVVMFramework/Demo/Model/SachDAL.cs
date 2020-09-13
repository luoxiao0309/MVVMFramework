using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFramework.Model
{
    public class SachDAL
    {
        private static ObservableCollection<SachModel> listSach = new ObservableCollection<SachModel>() {
            new SachModel() { MaSach = 1, TenSach = "Longman" },
            new SachModel() { MaSach = 2, TenSach = "Đắc nhân tâm" },
            new SachModel() { MaSach = 3, TenSach = "Toán 12" } };

        public static ObservableCollection<SachModel> getListSachFromDatabase()
        {
            return listSach;
        }
        public static void AddSach(SachModel sach)
        {
            listSach.Add(sach);
        }
        public static void DeleteSach(SachModel sach)
        {
            listSach.Remove(sach);
        }
        public static void ModifySach(SachModel sach)
        {
            foreach (var book in listSach)
            {
                if (book.MaSach == sach.MaSach)
                {
                    book.TenSach = sach.TenSach;
                }
            }
        }
    }
}
