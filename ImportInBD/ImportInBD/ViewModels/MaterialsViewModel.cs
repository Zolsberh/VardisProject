using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImportInBD.Commands;
using ImportInBD.Entities;

namespace ImportInBD.ViewModels
{
    internal class MaterialsViewModel : INotifyPropertyChanged
    {
        Material? selectedMaterial;
        public ObservableCollection<Material> Materials { get; set; }
        public List<Material> searchCollection { get; set; }
        List<Material>? searchMaterialsById = null;
        List<Material>? searchMaterialsByName = null;
        RelayCommand? acceptCommand;
        RelayCommand? cancelCommand;
        RelayCommand? searchByNameCommand;
        RelayCommand? searchByArticleCommand;


        public MaterialsViewModel()
        {
            Materials = [];
            searchCollection = [];
        }


        public Material SelectedMaterial
        {
            get => selectedMaterial ?? new();
            set
            {
                selectedMaterial = value;
                OnPropertyChanged("SelectedMaterial");
            }
        }

        public RelayCommand AcceptCommand =>
           acceptCommand ??= new RelayCommand((win) =>
           {
               Window window = (Window)win;
               if (window != null)
               {
                   window.DialogResult = true;
               }
               searchMaterialsById = null;
               searchMaterialsByName = null;
           });

        public RelayCommand CancelCommand =>
            cancelCommand ??= new RelayCommand((win) =>
            {
                searchMaterialsById = null;
                searchMaterialsByName = null;

                Window window = (Window)win;
                window?.Close();
            });

        public RelayCommand SearchByNameCommand =>
            searchByNameCommand ??= new RelayCommand((obj) =>
            {
                TextBox textBox = (TextBox)obj;
                string search = textBox.Text.ToUpper();
                if (search != null)
                {
                    Func<Material, bool> predicate = (material) => material.MaterialName.Contains(search, StringComparison.CurrentCultureIgnoreCase);
                    SearchBy(ref searchMaterialsById, ref searchMaterialsByName, predicate);

                }
                if (string.IsNullOrEmpty(search))
                {
                    searchMaterialsByName = null;
                }
            });


        public RelayCommand SearchByArticleCommand =>
            searchByArticleCommand ??= new RelayCommand((obj) =>
            {
                TextBox textBox = (TextBox)obj;
                string search = textBox.Text;

                if (search != null)
                {
                    Func<Material, bool> predicate = (material) => material.MaterialID.ToString().StartsWith(search, StringComparison.CurrentCultureIgnoreCase);
                    SearchBy(ref searchMaterialsByName, ref searchMaterialsById, predicate);
                }
                if (string.IsNullOrEmpty(search))
                {
                    searchMaterialsById = null;
                }
            });


        //Поиск материалов по строке поиска
        private void SearchBy(ref List<Material> search1, ref List<Material> search2, Func<Material, bool> predicate)
        {
            if (search1 is null)
            {
                search2 = searchCollection.Where(predicate).ToList();
            }
            else
            {
                search2 = search1.Where(predicate).ToList();
            }

            Materials.Clear();
            foreach (Material material in search2)
            {
                Materials.Add(material);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string pop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pop));
        }
    }
}
