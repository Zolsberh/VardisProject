using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Windows;
using System.Data;
using ImportInBD.Entities;
using ImportInBD.ContolEntities;

namespace ImportInBD.Controllers
{
    partial class AccessController
    {
        enum EntityFromAccess { Manager, Material, District, Status, Constructor, Contractor, Order }

        static List<IEntityFromAccess> GetEntities(string? connectionString, EntityFromAccess entityFromAccess, string selectQuery)
        {
            IEntityFromAccess entity = null;
            List<IEntityFromAccess> entities = new();

            using (OleDbConnection connect = new OleDbConnection(connectionString))
            {
                connect.Open();
                OleDbCommand cmd = new OleDbCommand(selectQuery, connect);
                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        switch (entityFromAccess)
                        {
                            case EntityFromAccess.Manager:
                                entity = GetManager(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.Material:
                                entity = GetMaterial(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.District:
                                entity = GetDistrict(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.Status:
                                entity = GetStatus(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.Constructor:
                                entity = GetConstructor(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.Contractor:
                                entity = GetContractor(reader);
                                entities.Add(entity);
                                break;
                            case EntityFromAccess.Order:
                                entity = GetOrder(reader);
                                entities.Add(entity);
                                break;
                        }
                    }
                }
            }
            return entities;
        }

        static Manager GetManager(OleDbDataReader reader)
        {
            Manager manager = new();
            manager.ManagerID = reader.GetInt32(0);
            manager.ManagerName = reader.GetString(1);
            if (reader["Телефон"] != DBNull.Value)
            {
                manager.Phone = reader.GetString(2);
            }
            else
                manager.Phone = string.Empty;
            return manager;
        }

        static Material GetMaterial(OleDbDataReader reader)
        {
            Material material = new();
            material.MaterialID = reader.GetInt32(0);
            material.MaterialName = reader.GetString(1);
            material.MaterialCount = Math.Round(reader.GetFloat(2), 3);
            material.MaterialUnit = new Unit();
            material.MaterialUnit.Name = reader.GetString(3);
            return material;
        }

        static District GetDistrict(OleDbDataReader reader)
        {
            District district = new();
            district.Id = reader.GetInt32(0);
            district.Name = reader.GetString(1);
            return district;
        }

        static Status GetStatus(OleDbDataReader reader)
        {
            Status status = new();
            status.NameStatus = reader.GetString(1);
            status.Id = reader.GetInt32(0);
            return status;
        }

        static Constructor GetConstructor(OleDbDataReader reader)
        {
            Constructor constructor = new();
            constructor.ConstructorName = reader.GetString(1);
            constructor.ConstructorID = reader.GetInt32(0);
            return constructor;
        }

        static Contractor GetContractor(OleDbDataReader reader)
        {
            Contractor contractor = new();
            contractor.ContractorName = reader.GetString(1);
            contractor.ContractorID = reader.GetInt32(0);
            return contractor;
        }

        static Order GetOrder(OleDbDataReader reader)
        {
            Order order = new();
            order.Id = reader.GetInt32(0);
            order.Name = reader.GetString(1);
            order.DeadLine = reader.GetDateTime(2);
            order.Manager = new();
            order.Manager.ManagerID = reader.GetInt32(3);
            order.Contractor = new();
            if (!reader.IsDBNull(4))
                order.Contractor.ContractorID = reader.GetInt32(4);
            order.Constructor = new();
            if (!reader.IsDBNull(5))
                order.Constructor.ConstructorID = reader.GetInt32(5);
            order.Status = new();
            order.Status.Id = reader.GetInt32(6);
            if (!reader.IsDBNull(7))
                order.CloseOrder = reader.GetDateTime(7);
            return order;
        }

        //Получение списка менеджеров из базы данных
        public static List<Manager> GetManagers(string? connectionString)
        {
            // Manager manager = null;
            string selectManager = $"SELECT * FROM Менеджеры Order by Имя";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Manager, selectManager);
            List<Manager> managers = new(entities.Select(m => (Manager)m));

            return managers;
        }

        //Получение списка конструкторов из базы данных
        public static List<Constructor> GetConstructors(string? connectionString)
        {
            // Manager manager = null;
            string selectConstructor = $"SELECT * FROM Конструкторы Order by Имя";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Constructor, selectConstructor);
            List<Constructor> constructors = new(entities.Select(c => (Constructor)c));

            return constructors;
        }

        //Получение списка исполнителей из базы данных
        public static List<Contractor> GetContractors(string? connectionString)
        {
            // Manager manager = null;
            string selectContractor = $"SELECT * FROM Исполнители Order by Исполнитель";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Contractor, selectContractor);
            List<Contractor> contractors = new(entities.Select(c => (Contractor)c));

            return contractors;
        }

        //Получение списка материалов из базы данных на основании данных из DBF
        public static List<Material> GetMaterialsByDBF(string? connectionString, List<MaterialFromDBF> materialFromDBFs)
        {
            List<Material> materials = new();

            foreach (MaterialFromDBF materialDBF in materialFromDBFs)
            {
                int? materialId = null;
                if (int.TryParse(materialDBF.CODE, out int id))
                {
                    materialId = id;
                }

                Material material = GetMaterialById(connectionString, materialId);
                materials.Add(material);
            }
            return materials;
        }

        public static List<Material> GetMaterialsAll(string? connectionString)
        {

            string selectMaterial = $"SELECT Материалы.Артикул, Материалы.Наименование, Материалы.Наличие, [Еденица Измерения].Наименование FROM Материалы " +
                                    $"INNER JOIN [Еденица Измерения] ON [Еденица Измерения].Ид_Ед = Материалы.Ед_Измерения Order by Материалы.Наименование";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Material, selectMaterial);
            List<Material> materials = new(entities.Select(m => (Material)m));

            return materials;
        }

        //Получение материала из базы данных
        public static Material GetMaterialById(string? connectionString, int? materialId)
        {
            List<Material> materials = GetMaterialsAll(connectionString);
            Material? material = materials.FirstOrDefault(m => m.MaterialID == materialId);
            material ??= new Material();
            return material;
        }

        //Доделать!!!!
        public static List<Order> GetOrdersAll(string? connectionString)
        {
            string selectOrderID = $"SELECT * FROM Заказы";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Order, selectOrderID);
            List<Order> orders = new(entities.Select(m => (Order)m));
            return orders;
        }

        //Получени ID заказа
        public static int? GetOrderID(string? connectionString, string nameOrder, DateTime dataDeadLine)
        {
            int? orderID = null;
            var orders = GetOrdersAll(connectionString);
            Order? order = orders.FirstOrDefault(o => o.Name == nameOrder && o.DeadLine.Year == dataDeadLine.Year);
            if (order != null)
                orderID = order.Id;

            return orderID;
        }


        //Получение списка участков из базы данных
        public static List<District> GetDistricts(string? connectionString)
        {
            // Manager manager = null;
            string selectDistricts = $"SELECT * FROM Участки Order by Наименование";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.District, selectDistricts);
            List<District> districts = new(entities.Select(d => (District)d));

            return districts;
        }

        //Получение списка статусов заказа из базы данных
        public static List<Status> GetStatuses(string? connectionString)
        {
            string selectStatus = $"SELECT * FROM Статус_Заказа Order by Статус";
            List<IEntityFromAccess> entities = GetEntities(connectionString, EntityFromAccess.Status, selectStatus);
            List<Status> statuses = new(entities.Select(s => (Status)s));

            return statuses;
        }

        public static bool HasSameOrder(string? connectionString, string nameOrder, DateTime deadLine)
        {
            int? orderID = GetOrderID(connectionString, nameOrder, deadLine);
            if (orderID == null) return false;
            return true;
        }

        //Создание строки подключения
        public static string GetConnectionString()
        {
            string connectionString = $"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = D://Работа/Учет материалов.accdb;";
            return connectionString;
        }

        public static void InsertOrder(string? connectionString, string orderName, DateTime deadLine, Manager manager, Contractor contractor,
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

        public static bool IsDistrictNotNull(int? districtID)
        {
            if (districtID == null) { return false; }
            return true;
        }

        //Вставка расхода 
        public static void InsertСalculation(string? connectionString, int? orderID, int? materialID, double count, double materialUF, double ramainderRatio, int? districtID)
        {
            string insertCalculation = $"INSERT INTO [Расход по заказам] (Заказ, Материал, Количество, КИМ, К_Отходов, Участок) " +
                                       $"VALUES(@orderID, @materialID, @count, @materialUF, @ramainderRatio, @districtId)";
            using (OleDbConnection connect = new OleDbConnection(connectionString))
            {
                connect.Open();
                OleDbCommand cmd = new OleDbCommand(insertCalculation, connect);
                cmd.Parameters.AddWithValue("@orderID", orderID);
                cmd.Parameters.AddWithValue("@materialID", materialID);
                cmd.Parameters.AddWithValue("@count", count);
                cmd.Parameters.AddWithValue("@materialUF", materialUF);
                cmd.Parameters.AddWithValue("@ramainderRatio", ramainderRatio);
                cmd.Parameters.AddWithValue("@districtId", districtID);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
