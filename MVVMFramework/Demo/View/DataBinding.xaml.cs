﻿using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMFramework.View
{
    public partial class DataBinding : INotifyPropertyChanged
    {
        public DataBinding()
        {
            DataContext = this;
            InitializeComponent();
        }


        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            BoundNumber = 0;
        }


        private int _boundNumber;
        
        public int BoundNumber
        {
            get { return _boundNumber; }
            set
            {
                if(_boundNumber != value)
                {
                    _boundNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
