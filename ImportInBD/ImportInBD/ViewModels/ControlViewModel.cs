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
using ImportInBD.Commands;
using ImportInBD.ContolEntities;
using ImportInBD.Controllers;
using ImportInBD.Entities;

namespace ImportInBD.ViewModels
{
    internal class ControlViewModel : INotifyPropertyChanged
    {
        private ControlOrderMaterial? selsectedMaterial;
        private readonly string? connectionString;
        readonly List<Material> materials;
        private RelayCommand? selectFromBD;
        private RelayCommand? acceptCommand;
        private RelayCommand? deleteControlMaterialCommand;
        private RelayCommand? cancelCommand;

        public ObservableCollection<ControlOrderMaterial> MaterialsOfControl { get; set; }

        public ControlViewModel()
        {

            connectionString = AccessController.GetConnectionString();
            materials = AccessController.GetMaterialsAll(connectionString);
            MaterialsOfControl = [];
        }

        public ControlOrderMaterial SelectedMaterial
        {
            get => selsectedMaterial ?? new();
            set
            {
                selsectedMaterial = value;
                OnPropertyChanged("SelectedMaterial");
            }
        }

        public RelayCommand SelectFromBD => selectFromBD ??= new RelayCommand(obj =>
            {
                MaterialsWindow materialsWindow = new();
                MaterialsViewModel materialsViewModel = new()
                {
                    Materials = new(list: materials),
                    searchCollection = materials
                };

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
            });


        public RelayCommand DeleteControlMaterialCommand => deleteControlMaterialCommand ??= new RelayCommand(obj =>
           {
               MaterialsOfControl.Remove(SelectedMaterial);
           });


        public RelayCommand AcceptCommand =>
            acceptCommand ??= new RelayCommand((win) =>
            {
                Window window = (Window)win;
                if (window != null)
                {
                    window.DialogResult = true;
                }
            });

        public RelayCommand CancelCommand =>
            cancelCommand ??= new RelayCommand((win) =>
            {
                Window window = (Window)win;
                window?.Close();
            });


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string pop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pop));
        }
    }
}
