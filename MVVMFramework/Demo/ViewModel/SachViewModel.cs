using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CommonFramework.MVVM;
using MVVMFramework.Model;

namespace MVVMFramework.ViewModel
{
    public class SachViewModel
    {
        private ObservableCollection<SachModel> listSach;

        public ObservableCollection<SachModel> ListSach
        {
            get { return listSach; }
            set { listSach = value; }
        }
        public SachViewModel()
        {
            LoadSach();
            ModifyCommand = new RelayCommandType<UIElementCollection>((p) => p != null, (p) => ModifySach(p));
            DeleteCommand = new RelayCommandType<object>((p) => p != null, (p) => DeleteSach(p));
            AddCommand = new RelayCommandType<UIElementCollection>((p) => p != null, (p) => AddSach(p));
        }
        private void AddSach(UIElementCollection p)
        {
            int masach = 0;
            string tensach = "";
            bool ismasach = false;

            foreach (var item in p)
            {
                TextBox a = item as TextBox;
                if (a == null)
                    continue;
                switch (a.Name)
                {
                    case "txbMa":
                        ismasach = Int32.TryParse(a.Text, out masach);
                        break;
                    case "txbTen":
                        tensach = a.Text;
                        break;
                }

            }
            if (!ismasach || string.IsNullOrEmpty(tensach))
            {
                return;
            }
            SachModel sach = new SachModel() { MaSach = masach, TenSach = tensach };
            SachDAL.AddSach(sach);
            LoadSach();
        }
        private void DeleteSach(object p)
        {
            SachDAL.DeleteSach(p as SachModel);
            LoadSach();
        }
        private void ModifySach(UIElementCollection p)
        {
            int masach = 0;
            string tensach = "";
            bool ismasach = false;

            foreach (var item in p)
            {
                TextBox a = item as TextBox;
                if (a == null)
                    continue;
                switch (a.Name)
                {
                    case "txbMa":
                        ismasach = Int32.TryParse(a.Text, out masach);
                        break;
                    case "txbTen":
                        tensach = a.Text;
                        break;

                }

            }
            if (!ismasach || string.IsNullOrEmpty(tensach))
            {
                return;
            }
            SachModel sach = new SachModel() { MaSach = masach, TenSach = tensach };
            SachDAL.ModifySach(sach);
            LoadSach();
        }
        public ICommand DeleteCommand { get; set; }

        public ICommand AddCommand { get; set; }

        public ICommand ModifyCommand { get; set; }
        void LoadSach()
        {
            listSach = SachDAL.getListSachFromDatabase();
        }
    }
}
