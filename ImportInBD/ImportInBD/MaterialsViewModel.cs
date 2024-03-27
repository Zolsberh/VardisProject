using ImportInBD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImportInBD
{
    internal class MaterialsViewModel : INotifyPropertyChanged
    {
        private Material selectedMaterial;
        public ObservableCollection<Material> Materials {  get; set; }
        private RelayCommand acceptCommand;
        private RelayCommand cancelCommand;

        public MaterialsViewModel() 
        {
            Materials = new ObservableCollection<Material>();
        }


        public Material SelectedMaterial 
        {  
            get 
            { 
                return selectedMaterial; 
            } 
            set 
            {
                selectedMaterial = value;
                OnPropertyChanged("SelectedMaterial");
            } }

        
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string pop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pop));
            }
        }

        public RelayCommand AcceptCommand => acceptCommand ??= new RelayCommand(Accept);

        private void Accept(object commandParameter)
        {
            Window window = (Window)commandParameter;
            window.DialogResult = true;
        }

        public RelayCommand CancelCommand => cancelCommand ??= new RelayCommand(Cancel);

        private void Cancel(object commandParameter) 
        {
            Window window = (Window)commandParameter;
            window.Close();
        }
    }
}
