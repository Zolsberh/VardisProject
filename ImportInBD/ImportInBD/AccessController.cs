using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ImportInBD
{
    partial class AccessController
    {
        static List<Manager> managers = new List<Manager>();
        static List<Material> materials = new List<Material>();
        static List<District> districts = new List<District>();
        static List<Status> statuses = new List<Status>();
        static List<Constructor> constructors = new List<Constructor>();
        static List<Contractor> contractors = new List<Contractor>();

        enum EntityFromAccess { Manager, Material, District, Status, Constructor, Contractor }

        static OleDbDataReader GetConnectionToAccess(string? connectionString, string selectQuery, Dictionary<string, object> args)
        {
            OleDbConnection connect = new OleDbConnection(connectionString);
            connect.Open();
            OleDbCommand cmd = new OleDbCommand(selectQuery, connect);

            foreach(KeyValuePair<string, object> parm in args)
            {
                if(parm.Value is DateTime)
                    cmd.Parameters.AddWithValue(parm.Key, OleDbType.DBDate).Value = parm.Value.ToString();
                if(parm.Value is string)
                    cmd.Parameters.AddWithValue(parm.Key, OleDbType.WChar).Value = parm.Value.ToString();
            }
            //MessageBox.Show($"{cmd.Parameters[0].Value} and {cmd.Parameters[1].Value}");
            OleDbDataReader reader = cmd.ExecuteReader(); 
            return reader; // возвращаем
        }
        static OleDbDataReader GetConnectionToAccess(string? connectionString, string selectQuery)
        {
            OleDbConnection connect = new OleDbConnection(connectionString);
            connect.Open();
            OleDbCommand cmd = new OleDbCommand(selectQuery, connect);
            OleDbDataReader reader = cmd.ExecuteReader();
            return reader; // возвращаем
        }


        static List<IEntityFromAccess> GetEntity(string? connectionString, EntityFromAccess entityFromAccess, string selectQuery)
        {
            List<IEntityFromAccess> entities = new List<IEntityFromAccess>();
            using(OleDbDataReader reader = GetConnectionToAccess(connectionString, selectQuery))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        switch (entityFromAccess)
                        {
                            case EntityFromAccess.Manager:
                                Manager man = new Manager();
                                man.ManagerID = reader.GetInt32(0);
                                man.ManagerName = reader.GetString(1);
                                if (reader["Телефон"] != DBNull.Value)
                                {
                                    man.Phone = reader.GetString(2);
                                }
                                else
                                    man.Phone = string.Empty;
                                entities.Add(man);
                                break;
                            case EntityFromAccess.Material:
                                Material mat = new Material();
                                mat.MaterialID = reader.GetInt32(0);
                                mat.MaterialName = reader.GetString(1);
                                mat.MaterialCount = Math.Round(reader.GetFloat(2), 3); 
                                mat.MaterialUnit = new Unit();
                                mat.MaterialUnit.Name = reader.GetString(3);
                                entities.Add(mat);
                                break;
                            case EntityFromAccess.District:
                                District district = new District();
                                district.Id = reader.GetInt32(0);
                                district.Name = reader.GetString(1);
                                entities.Add(district);
                                break;
                            case EntityFromAccess.Status:
                                Status status = new Status();
                                status.NameStatus = reader.GetString(1);
                                status.Id = reader.GetInt32(0);
                                entities.Add(status);
                                break;
                            case EntityFromAccess.Constructor:
                                Constructor constructor = new Constructor();
                                constructor.ConstructorName = reader.GetString(1);
                                constructor.ConstructorID = reader.GetInt32(0);
                                entities.Add(constructor);
                                break;
                            case EntityFromAccess.Contractor:
                                Contractor contractor = new Contractor();
                                contractor.ContractorName = reader.GetString(1);
                                contractor.ContractorID = reader.GetInt32(0);
                                entities.Add(contractor);
                                break;
                        }
                    }
                }
            }
            return entities;
        }

        //Получение списка менеджеров из базы данных
        public static List<Manager> GetManagers(string? connectionString)
        {
            // Manager manager = null;
            string selectManager = $"SELECT * FROM Менеджеры";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Manager, selectManager);
            foreach (IEntityFromAccess entity in entities)
            {
                managers.Add((Manager)entity);
            }
            return managers;   
        }

        //Получение списка конструкторов из базы данных
        public static List<Constructor> GetConstructors(string? connectionString)
        {
            // Manager manager = null;
            string selectConstructor = $"SELECT * FROM Конструкторы";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Constructor, selectConstructor);
            foreach (IEntityFromAccess entity in entities)
            {
                constructors.Add((Constructor)entity);
            }
            return constructors;
        }

        //Получение списка исполнителей из базы данных
        public static List<Contractor> GetContractor(string? connectionString)
        {
            // Manager manager = null;
            string selectContractor = $"SELECT * FROM Исполнители";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Contractor, selectContractor);
            foreach (IEntityFromAccess entity in entities)
            {
                contractors.Add((Contractor)entity);
            }
            return contractors;
        }

        //Получение списка материалов из базы данных на основании данных из DBF
        public static List<Material> GetMaterialsFromAccess(string? connectionString, List<MaterialFromDBF> materialFromDBFs)
        {
            List<Material> materials = new List<Material>();
            
            foreach(MaterialFromDBF materialDBF in materialFromDBFs)
            {
                int materialId = 0;
                if (Int32.TryParse(materialDBF.CODE, out materialId))
                {
                    materialId = Int32.Parse(materialDBF.CODE);
                }

                Material material = GetMaterialById(connectionString, materialId);
                materials.Add(material);
            }

            //string selectMaterial = $"SELECT Артикул, Наименование FROM Материалы";
            //List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Material, selectMaterial);
            //foreach(IEntityFromAccess entity in entities)
            //{
            //    materials.Add((Material)entity);
            //}
            return materials;
        }

        public static List<Material> GetMaterialsFromAccessAll(string? connectionString)
        {
            //List<Material> materials = new List<Material>();

            //foreach (MaterialFromDBF materialDBF in materialFromDBFs)
            //{
            //    int materialId = 0;
            //    if (Int32.TryParse(materialDBF.CODE, out materialId))
            //    {
            //        materialId = Int32.Parse(materialDBF.CODE);
            //    }

            //    Material material = GetMaterialById(connectionString, materialId);
            //    materials.Add(material);
            //}

            string selectMaterial = $"SELECT Материалы.Артикул, Материалы.Наименование, Материалы.Наличие, [Еденица Измерения].Наименование FROM Материалы " +
                                    $"INNER JOIN [Еденица Измерения] ON [Еденица Измерения].Ид_Ед = Материалы.Ед_Измерения";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Material, selectMaterial);
            foreach (IEntityFromAccess entity in entities)
            {
                materials.Add((Material)entity);
            }
            return materials;
        }

        //Получение материала из базы данных
        public static Material GetMaterialById(string? connectionString, int materialId)
        {
            Material material = new Material();
            string message = "";

            string selectMaterial = $"SELECT Материалы.Артикул, Материалы.Наименование, [Еденица Измерения].Наименование FROM Материалы INNER JOIN " +
                                    $"[Еденица Измерения] ON [Еденица Измерения].Ид_Ед = Материалы.Ед_Измерения  WHERE Артикул = @materialId";
            using (OleDbConnection connection = new(connectionString))
            {
                connection.Open();

                OleDbCommand oleDbCommand = new OleDbCommand(selectMaterial, connection);
                oleDbCommand.Parameters.AddWithValue("@materialId", materialId);

                using (OleDbDataReader reader = oleDbCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            
                            material.MaterialID = reader.GetInt32(0);
                            material.MaterialName = reader.GetString(1);
                            material.MaterialUnit = new Unit();
                            material.MaterialUnit.Name = reader.GetString(2);
                        }
                    }
                    else
                    {
                        message += $"Не найден артикул: {materialId}! ";
                        MessageBox.Show(message);
                    }
                }
            }
            return material;
        }

        //Получени ID заказа
        public static int GetOrderID(string? connectionString, string nameOrder, DateTime dataDeadLine)
        {
            int orderID = 0;
            string shortData = dataDeadLine.ToShortDateString();

            string selectOrderID = $"SELECT Ид_Заказа FROM Заказы WHERE Заказ = @nameOrder AND Срок = @data";

            using (OleDbConnection connection = new(connectionString))
            {
                connection.Open ();
                
                OleDbCommand selectOrderCommand = new OleDbCommand(selectOrderID, connection);
                selectOrderCommand.Parameters.AddWithValue("@nameOrder", nameOrder);
                selectOrderCommand.Parameters.AddWithValue("@data", shortData);
                selectOrderCommand.ExecuteNonQuery();

                using(OleDbDataReader reader = selectOrderCommand.ExecuteReader()) 
                {
                    if (reader.HasRows) 
                    {
                        while (reader.Read())
                        {

                            orderID = reader.GetInt32(0);
                        }
                    }
                }
            }
            return orderID;
        }


        //Получение списка участков из базы данных
        public static List<District> GetDistricts(string? connectionString)
        {
            // Manager manager = null;
            string selectDistricts = $"SELECT * FROM Участки";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.District, selectDistricts);
            foreach(IEntityFromAccess entity in entities)
            {
                districts.Add((District)entity);
            }
            return districts;
        }

        //Получение списка статусов заказа из базы данных
        public static List<Status> GetStatuses(string? connectionString)
        {
            string selectStatus = $"SELECT * FROM Статус_Заказа";
            List<IEntityFromAccess> entities = GetEntity(connectionString, EntityFromAccess.Status, selectStatus);
            foreach(IEntityFromAccess entity in entities)
            {
                statuses.Add((Status)entity);
            }
            return statuses;
        }

        public static bool HasSameOrder(string? connectionString, string nameOrder, DateTime deadLine)
        {
            string shortDate = deadLine.ToShortDateString();
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("@nameOrder", nameOrder);
            args.Add("@deadLine", deadLine);
            string selectOrderID = $"SELECT Ид_Заказа FROM Заказы WHERE Заказ = ? and Срок = ?";
            using(OleDbDataReader reader = GetConnectionToAccess(connectionString, selectOrderID, args))
            {
                if (reader.HasRows)
                {
                    return true;
                }
            }
            return false;
        }

        //Создание строки подключения
        public static string GetConnectionStringToAccess()
        {
            string connectionString = $"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = D://Работа/Учет материалов.accdb;";

            //Не работает с SplashScreen!

            //MessageBoxResult result = MessageBox.Show("Выберите базу данных для сохранения значений", "Не выбрана база!", MessageBoxButton.OKCancel);
            //if (result == MessageBoxResult.OK)
            //{
            //    OpenFileDialog openFileDialog = new OpenFileDialog();
            //    openFileDialog.Filter = "Data base files(*.accdb|*.accdb";
            //    if (openFileDialog.ShowDialog() == true)
            //    {
            //        string? directory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            //        if (directory != null)
            //        {
            //            string path = System.IO.Path.Combine(directory, openFileDialog.FileName);
            //            if(path != null)
            //                connectionString = $"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {path};";
            //        }
            //    }
            //}

            return connectionString;
        }

        public static void InsertOrderToAccess(string? connectionString, string orderName, DateTime deadLine, Manager manager, Contractor contractor,
                                               Constructor constructor, Status status)
        {
            string shortDate = deadLine.ToShortDateString();
            string insertOrders = $"INSERT INTO Заказы (Заказ, Срок, Имя_Менеджера, Исполнитель, Конструктор, Статус) " +
                                  $"VALUES(@orderName, @shortDate, @manager.ManagerID, @contractor, @constructor, @status.Id)";
                using (OleDbConnection connect = new OleDbConnection(connectionString))
                {
                    connect.Open();
                    OleDbCommand cmd = new OleDbCommand(insertOrders, connect);
                    cmd.Parameters.AddWithValue("@orderName", orderName);
                    cmd.Parameters.AddWithValue("@shortDate", shortDate);
                    cmd.Parameters.AddWithValue("@manager.ManagerID", manager.ManagerID);
                    cmd.Parameters.AddWithValue("@contractor", contractor.ContractorID);
                    cmd.Parameters.AddWithValue("@constructor", constructor.ConstructorID);
                    cmd.Parameters.AddWithValue("@status.Id", status.Id);
                    cmd.ExecuteNonQuery();
                }
        }

        //Вставка расхода 
        public static void InsertСalculationToAccess(string? connectionString, int orderID, int materialID, double count, int? districtID)
        {
            string insertCalculation = $"INSERT INTO [Расход по заказам] (Заказ, Материал, Количество, Участок) VALUES(@orderID, @materialID, @count, @districtId)";
            using (OleDbConnection connect = new OleDbConnection(connectionString))
            {
                connect.Open();
                OleDbCommand cmd = new OleDbCommand(insertCalculation, connect);
                cmd.Parameters.AddWithValue("@orderID", orderID);
                cmd.Parameters.AddWithValue("@materialID", materialID);
                cmd.Parameters.AddWithValue("@count", count);
                cmd.Parameters.AddWithValue("@districtId", districtID);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
