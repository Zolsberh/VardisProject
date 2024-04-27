using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Text;
using ImportInBD.ContolEntities;

namespace ImportInBD.Controllers
{
    partial class DBFController
    {

        public static List<MaterialFromDBF> GetMaterialFromDBFs(string? pathToFolderWithDB, string nameDB)
        {
            List<MaterialFromDBF> list = [];
            //MaterialFromDBF materialFromDBF = new MaterialFromDBF();

            string connectionString = $"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {pathToFolderWithDB}; Extended Properties = dBASE IV; User ID = Admin";
            string sqlExpretion = $"SELECT CODE, NAME, EDIZ, KOLORDER FROM {nameDB}";

            using (OleDbConnection connection = new(connectionString))
            {
                connection.Open();

                OleDbCommand oleDbCommand = new(sqlExpretion, connection);

                using OleDbDataReader reader = oleDbCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MaterialFromDBF materialFromDBF = new();
                        //MaterialFromDBF materialFromDBF = new MaterialFromDBF();
                        if (reader["CODE"] != DBNull.Value)
                            materialFromDBF.CODE = reader.GetString(0);
                        else
                            materialFromDBF.CODE = string.Empty;

                        materialFromDBF.NAME = reader.GetString(1);

                        if (reader["EDIZ"] != DBNull.Value)
                            materialFromDBF.EDIZ = reader.GetString(2);
                        else
                            materialFromDBF.EDIZ = string.Empty;

                        materialFromDBF.KOLORDER = reader.GetDouble(3);

                        list.Add(materialFromDBF);
                    }
                }
            }

            return list;

        }
    }
}