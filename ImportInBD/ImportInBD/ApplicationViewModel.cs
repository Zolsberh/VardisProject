using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImportInBD
{
    class ApplicationViewModel : INotifyPropertyChanged
    {   
        private List<Manager> managers;
        private List<Constructor> constructors;
        private List<Contractor> contractors;
        private List<District> districts;
        private List<Status> statuses;
        private List<MaterialFromDBF> materialFromDBFs;
        private List<Material> materialsFromAccess;
        private AccountingByOrder selectedAccountByOrder;
        private Order order;

        private RelayCommand importCommand;
        private RelayCommand cancelCommand;
        private RelayCommand loadInDBCommand;
        private RelayCommand deleteAcccountByOrderCommand;

        public List<Manager> Managers 
        {
            get { return managers; }
            set 
            {
                managers = value;
                OnPropertyChanged("Managers");

            } 
        }

        public List<Constructor> Constructors
        {
            get { return constructors; }
            set
            {
                constructors = value;
                OnPropertyChanged("Constructors");
            }
        }

        public List<Contractor> Contractors
        {
            get { return contractors; }
            set
            {
                contractors = value;
                OnPropertyChanged("Contractors");
            }
        }

        public List<District> Districts
        {
            get { return districts; }
            set
            {
                districts = value;
                OnPropertyChanged("Districts");
            }
        }

        public List<Status> Statuses
        { 
            get { return statuses; }
            set 
            {
                statuses = value;
                OnPropertyChanged("Statuses");
            } 
        }

        public AccountingByOrder SelectedAccountByOrder
        {
            get
            {
                return selectedAccountByOrder;
            }
            set
            {
                selectedAccountByOrder = value;
                OnPropertyChanged("SelectedAccountByOrder");
            }
        }

        public Order Order
        {
            get
            {
                return order;
            }
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
            connectionStringToAccess = AccessController.GetConnectionStringToAccess();
            Managers = AccessController.GetManagers(connectionStringToAccess);
            Constructors = AccessController.GetConstructors(connectionStringToAccess);
            Contractors = AccessController.GetContractor(connectionStringToAccess);
            Districts = AccessController.GetDistricts(connectionStringToAccess);
            Statuses = AccessController.GetStatuses(connectionStringToAccess);
            AccountingByOrders = new ObservableCollection<AccountingByOrder>();
            Order = new Order();
        }

        public RelayCommand ImportCommand => importCommand ??
                    (importCommand = new RelayCommand(obj =>
                    {
                        string? directory = "";
                        string fileName = "";
                        AccountingByOrders.Clear();

                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Data base files(*.dbf)|*.dbf";
                        if (openFileDialog.ShowDialog() == true)
                        {
                            directory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                            fileName = openFileDialog.SafeFileName;

                            materialFromDBFs = DBFController.GetMaterialFromDBFs(directory, fileName);
                            materialsFromAccess = AccessController.GetMaterialsFromAccess(connectionStringToAccess, materialFromDBFs);

                            ObservableCollection<ControlOrderMaterial> materialsOfControl = new ObservableCollection<ControlOrderMaterial>();
                            
                            foreach (MaterialFromDBF material in materialFromDBFs)
                            {
                                ControlOrderMaterial controlOrderMaterial = new ControlOrderMaterial();
                                controlOrderMaterial.FromDBFID = material.CODE;
                                controlOrderMaterial.NameFromDBF = material.NAME;
                                controlOrderMaterial.UnitFromDBF = material.EDIZ;
                                controlOrderMaterial.Count = material.KOLORDER;
                                materialsOfControl.Add(controlOrderMaterial);
                            }

                            for(int i = 0; i < materialsOfControl.Count; ++i)
                            {
                                materialsOfControl[i].FromAccessID = materialsFromAccess[i].MaterialID;
                                materialsOfControl[i].NameFromAccess = materialsFromAccess[i].MaterialName;
                                materialsOfControl[i].UnitFromAccess = materialsFromAccess[i].MaterialUnit;
                            }

                            ControlWindow controlWindow = new ControlWindow();
                            controlWindow.DataContext = new ControlViewModel()
                            {
                                MaterialsOfControl = materialsOfControl

                            };
                            
                            if(controlWindow.ShowDialog() == true ) 
                            {
                                foreach (ControlOrderMaterial material in materialsOfControl)
                                {
                                    AccountingByOrder accountingByOrder = new AccountingByOrder();
                                    accountingByOrder.MaterialId = material.FromAccessID;
                                    Material accountMaterial = new Material() { MaterialName = material.NameFromAccess };
                                    accountingByOrder.MaterialName = accountMaterial;
                                    accountingByOrder.MaterialCount = material.Count;
                                    accountingByOrder.Ediz = material.UnitFromAccess.Name;
                                    accountingByOrder.District = new District();
                                    AccountingByOrders.Add(accountingByOrder);
                                }
                            }
                        }
                    }));


        public RelayCommand CancelComand => cancelCommand ??
            (cancelCommand = new RelayCommand(obj =>
            {
                AccountingByOrders.Clear();
            }));

        public RelayCommand LoadInDBCommand => loadInDBCommand ??
            (loadInDBCommand = new RelayCommand(obj => 
            {
            Order order = new Order();
            order.Name = Order.Name;
            order.DeadLine = Order.DeadLine;
            order.Manager = Order.Manager;
            order.Contractor = Order.Contractor;
            order.Constructor = Order.Constructor;
            order.Status = Order.Status;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите сохранить данные?", "Сохранение в БД", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (AccessController.HasSameOrder(connectionStringToAccess, order.Name, order.DeadLine))
                    {
                        MessageBox.Show("Такой заказ уже существует!");
                    }
                    else
                    {
                        AccessController.InsertOrderToAccess(connectionStringToAccess, order.Name, order.DeadLine, order.Manager, order.Contractor, order.Constructor, order.Status);
                        InsertToAccess();

                        MessageBox.Show("Данные успешно сохранены!");
                    }
                }
            }));

        public RelayCommand DeleteAcccountByOrderCommand => deleteAcccountByOrderCommand ??
            (deleteAcccountByOrderCommand = new RelayCommand(obj => 
            {
                AccountingByOrders.Remove(SelectedAccountByOrder);
            }));

        private void InsertToAccess()
        {
            int orderID = 0;
            orderID = AccessController.GetOrderID(connectionStringToAccess, order.Name, order.DeadLine);

            foreach (AccountingByOrder accounting in AccountingByOrders)
            {
                int materialID = accounting.MaterialId;
                int? districtID = null;

                if (accounting.District != null)
                    districtID = accounting.District.Id;
                double count = accounting.MaterialCount;

                AccessController.InsertСalculationToAccess(connectionStringToAccess, orderID, materialID, count, districtID);
            }
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
