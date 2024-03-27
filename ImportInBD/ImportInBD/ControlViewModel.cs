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
    internal class ControlViewModel : INotifyPropertyChanged
    {
        private ControlOrderMaterial selsectedMaterial;
        private string? connectionString;
        private RelayCommand selectFromBD;
        private RelayCommand acceptCommand;
        private RelayCommand deleteControlMaterialCommand;
        private RelayCommand cancelCommand;

        public ObservableCollection<ControlOrderMaterial> MaterialsOfControl { get; set; }

        public ControlViewModel() 
        {

            connectionString = AccessController.GetConnectionStringToAccess();
            MaterialsOfControl = new ObservableCollection<ControlOrderMaterial>();
        }

        public ControlOrderMaterial SelectedMaterial
        {
            get
            {
                return selsectedMaterial;
            }
            set 
            {
                selsectedMaterial = value;
                OnPropertyChanged("SelectedMaterial");
            }
        }

        public RelayCommand SelectFromBD => selectFromBD ??
            (selectFromBD = new RelayCommand(obj =>
            {
                List<Material> materials = AccessController.GetMaterialsFromAccessAll(connectionString);
                MaterialsWindow materialsWindow = new MaterialsWindow();
                MaterialsViewModel materialsViewModel = new MaterialsViewModel() { Materials = new ObservableCollection<Material>(list: materials) };
                materialsWindow.DataContext = materialsViewModel;

                int index = MaterialsOfControl.IndexOf(SelectedMaterial);

                if (materialsWindow.ShowDialog() == true)
                {
                    ControlOrderMaterial newMaterial = MaterialsOfControl[index];
                    MaterialsOfControl.Remove(SelectedMaterial);
                    newMaterial.FromAccessID = materialsViewModel.SelectedMaterial.MaterialID;
                    newMaterial.NameFromAccess = materialsViewModel.SelectedMaterial.MaterialName;
                    newMaterial.UnitFromAccess = materialsViewModel.SelectedMaterial.MaterialUnit;
                    MaterialsOfControl.Insert(index, newMaterial);    
                }
            }));


        public RelayCommand DeleteControlMaterialCommand => deleteControlMaterialCommand ??
           (deleteControlMaterialCommand = new RelayCommand(obj =>
           {
               MaterialsOfControl.Remove(SelectedMaterial);
           }));


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


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string pop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pop));
            }
        }
    }
}
