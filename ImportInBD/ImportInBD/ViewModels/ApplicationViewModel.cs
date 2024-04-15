using ImportInBD.Commands;
using ImportInBD.ContolEntities;
using ImportInBD.Controllers;
using ImportInBD.Entities;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Controls;

namespace ImportInBD.ViewModels
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        List<Manager>? managers;
        List<Constructor>? constructors;
        List<Contractor>? contractors;
        List<District>? districts;
        List<Status>? statuses;
        List<MaterialFromDBF>? materialFromDBFs;
        List<Material>? materialsFromAccess;
        AccountingByOrder? selectedAccountByOrder;
        Order? order;
        int index = -1;
        readonly List<Material> materials;

        RelayCommand? importCommand;
        RelayCommand? cancelCommand;
        RelayCommand? loadInDBCommand;
        RelayCommand? deleteAcccountByOrderCommand;
        RelayCommand? addMaterialCommand;
        RelayCommand? editNaterialCommand;
        RelayCommand? copyInsertContent;

        public List<Manager> Managers
        {
            get => managers ?? [];
            set
            {
                managers = value;
                OnPropertyChanged("Managers");

            }
        }

        public List<Constructor> Constructors
        {
            get => constructors ?? [];
            set
            {
                constructors = value;
                OnPropertyChanged("Constructors");
            }
        }

        public List<Contractor> Contractors
        {
            get => contractors ?? [];
            set
            {
                contractors = value;
                OnPropertyChanged("Contractors");
            }
        }

        public List<District> Districts
        {
            get => districts ?? [];
            set
            {
                districts = value;
                OnPropertyChanged("Districts");
            }
        }

        public List<Status> Statuses
        {
            get => statuses ?? [];
            set
            {
                statuses = value;
                OnPropertyChanged("Statuses");
            }
        }

        public AccountingByOrder SelectedAccountByOrder
        {
            get => selectedAccountByOrder ?? new();
            set
            {
                selectedAccountByOrder = value;
                OnPropertyChanged("SelectedAccountByOrder");
            }
        }

        public Order Order
        {
            get => order ?? new();
            set
            {
                order = value;
                OnPropertyChanged("Order");
            }
        }


        public ObservableCollection<AccountingByOrder> AccountingByOrders { get; set; }

        public string? connectionStringToAccess;


        public ApplicationViewModel()
        {
            connectionStringToAccess = AccessController.GetConnectionString();
            Managers = AccessController.GetManagers(connectionStringToAccess);
            Constructors = AccessController.GetConstructors(connectionStringToAccess);
            Contractors = AccessController.GetContractors(connectionStringToAccess);
            Districts = AccessController.GetDistricts(connectionStringToAccess);
            Statuses = AccessController.GetStatuses(connectionStringToAccess);
            AccountingByOrders = [];
            Order = new Order();
            materials = AccessController.GetMaterialsAll(connectionStringToAccess);
        }

        public RelayCommand ImportCommand => importCommand ??= new RelayCommand(obj =>
                    {
                        string? directory = "";
                        string fileName = "";
                        AccountingByOrders.Clear();

                        OpenFileDialog openFileDialog = new()
                        {
                            Filter = "Data base files(*.dbf)|*.dbf"
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            directory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                            fileName = openFileDialog.SafeFileName;

                            materialFromDBFs = DBFController.GetMaterialFromDBFs(directory, fileName);
                            materialsFromAccess = AccessController.GetMaterialsByDBF(connectionStringToAccess, materialFromDBFs);

                            ObservableCollection<ControlOrderMaterial> materialsOfControl = [];

                            foreach (MaterialFromDBF material in materialFromDBFs)
                            {
                                ControlOrderMaterial controlOrderMaterial = new()
                                {
                                    FromDBFID = material.CODE,
                                    NameFromDBF = material.NAME,
                                    UnitFromDBF = material.EDIZ,
                                    Count = material.KOLORDER
                                };
                                materialsOfControl.Add(controlOrderMaterial);
                            }

                            for (int i = 0; i < materialsOfControl.Count; ++i)
                            {
                                materialsOfControl[i].FromAccessID = materialsFromAccess[i].MaterialID;
                                materialsOfControl[i].NameFromAccess = materialsFromAccess[i].MaterialName;
                                materialsOfControl[i].UnitFromAccess = materialsFromAccess[i].MaterialUnit;
                            }

                            ControlWindow controlWindow = new()
                            {
                                DataContext = new ControlViewModel()
                                {
                                    MaterialsOfControl = materialsOfControl
                                }
                            };

                            if (controlWindow.ShowDialog() == true)
                            {
                                foreach (ControlOrderMaterial material in materialsOfControl)
                                {
                                    AccountingByOrder accountingByOrder = new()
                                    {
                                        MaterialId = material.FromAccessID
                                    };
                                    Material accountMaterial = new() { MaterialName = material.NameFromAccess ?? "" };
                                    accountingByOrder.MaterialName = accountMaterial;
                                    accountingByOrder.MaterialCount = material.Count;
                                    if (material.UnitFromAccess is not null)
                                        accountingByOrder.Ediz = material.UnitFromAccess.Name;
                                    accountingByOrder.District = new District();
                                    AccountingByOrders.Add(accountingByOrder);
                                }
                            }
                        }
                    });


        public RelayCommand CancelComand => cancelCommand ??= new RelayCommand(obj =>
            {
                AccountingByOrders.Clear();
            });

        public RelayCommand LoadInDBCommand => loadInDBCommand ??= new RelayCommand(obj =>
            {
                Order order = new()
                {
                    Name = Order.Name,
                    DeadLine = Order.DeadLine,
                    Manager = Order.Manager,
                    Contractor = Order.Contractor,
                    Constructor = Order.Constructor,
                    Status = Order.Status
                };

                MessageBoxResult result = MessageBox.Show("Вы действительно хотите сохранить данные?", "Сохранение в БД", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (AccessController.HasSameOrder(connectionStringToAccess, order.Name!, order.DeadLine))
                    {
                        MessageBox.Show("Такой заказ уже существует!");
                    }
                    else
                    {
                        if (IsDistrictNull().Item1 == true)
                        {
                            Material material = IsDistrictNull().Item2!;
                            MessageBox.Show($"Отсутствует участок для материала: {material.MaterialName}!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            if (order.Name != null && order.Manager != null && order.Contractor != null &&
                                order.Constructor != null && order.Status != null)
                            {
                                AccessController.InsertOrder(connectionStringToAccess, order.Name, order.DeadLine, order.Manager,
                                order.Contractor, order.Constructor, order.Status);
                                InsertToAccess();
                                MessageBox.Show("Данные успешно сохранены!");
                                AccountingByOrders.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Выбраны не все параметры!");
                            }
                        }
                    }
                }
            });


        public RelayCommand DeleteAcccountByOrderCommand => deleteAcccountByOrderCommand ??= new RelayCommand(obj =>
            {
                AccountingByOrders.Remove(SelectedAccountByOrder);
            });


        public RelayCommand AddMaterialCommand => addMaterialCommand ??= new RelayCommand(obj =>
            {
                MaterialsWindow materialsWindow = new();
                MaterialsViewModel materialsViewModel = new()
                {
                    Materials = new(list: materials),
                    searchCollection = materials
                };

                materialsWindow.DataContext = materialsViewModel;

                if (materialsWindow.ShowDialog() == true)
                {
                    AccountingByOrder accountingByOrder = new();
                    Material material = materialsViewModel.SelectedMaterial;
                    accountingByOrder.MaterialId = material.MaterialID;
                    accountingByOrder.MaterialName = material;
                    if (material.MaterialUnit is not null)
                        accountingByOrder.Ediz = material.MaterialUnit.Name;
                    accountingByOrder.District = new District();
                    AccountingByOrders.Add(accountingByOrder);
                }
            });

        public RelayCommand EditMaterialCommand => editNaterialCommand ??= new RelayCommand(obj =>
            {
                MaterialsWindow materialsWindow = new();
                MaterialsViewModel materialsViewModel = new()
                {
                    Materials = new(list: materials),
                    searchCollection = materials
                };

                materialsWindow.DataContext = materialsViewModel;

                if (materialsWindow.ShowDialog() == true)
                {
                    int index = AccountingByOrders.IndexOf(SelectedAccountByOrder);
                    AccountingByOrder accountingBy = new()
                    {
                        District = SelectedAccountByOrder.District,
                        MaterialCount = SelectedAccountByOrder.MaterialCount
                    };

                    AccountingByOrders.Remove(SelectedAccountByOrder);

                    Material material = materialsViewModel.SelectedMaterial;
                    accountingBy.MaterialId = material.MaterialID;
                    accountingBy.MaterialName = material;
                    if (material.MaterialUnit is not null)
                        accountingBy.Ediz = material.MaterialUnit.Name;
                    AccountingByOrders.Insert(index, accountingBy);
                }
            });

        public RelayCommand CopyInsertContent =>
            copyInsertContent ??= new RelayCommand((combo) => 
            {
                
                ComboBox box = (ComboBox)combo;
                
                MessageBox.Show($"{box.TemplatedParent.FindVisualAncestor<DataGrid>().CurrentCell.Item}");
                //box.KeyDown += (s, e) =>
                //{
                //    if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                //    {
                //        index = box.SelectedIndex;
                //        MessageBox.Show($"{index}");
                //    }
                //    if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V) 
                //    {
                //        box.SelectedIndex = index;
                //        MessageBox.Show($"{index}");
                //    }
                //};
            });

        private (bool, Material?) IsDistrictNull()
        {
            foreach (var item in AccountingByOrders)
            {
                if (item.District is null || item.District.Name is null)
                {
                    return (true, item.MaterialName);
                }
            }
            return (false, null);
        }


        private void InsertToAccess()
        {
            int? orderID = null;
            if (order is not null)
            {
                if (order.Name is not null)
                    orderID = AccessController.GetOrderID(connectionStringToAccess, order.Name, order.DeadLine);
            }

            foreach (AccountingByOrder accounting in AccountingByOrders)
            {
                int? materialID = accounting.MaterialId;
                double materialUF = accounting.MaterialUtolizationFactor;
                double ramainderRatio = accounting.RamainderRatio;
                int? districtID = null;
                if (accounting.District != null)
                    districtID = accounting.District.Id;

                double count = accounting.MaterialCount;

                AccessController.InsertСalculation(connectionStringToAccess, orderID, materialID, count, materialUF, ramainderRatio, districtID);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string pop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pop));
        }
    }
}
