using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace ImportInBD
{
    partial class DBFController
    {

        public static List<MaterialFromDBF> GetMaterialFromDBFs(string? pathToFolderWithDB, string nameDB)
        {
            List<MaterialFromDBF> list = new List<MaterialFromDBF>();
            //MaterialFromDBF materialFromDBF = new MaterialFromDBF();

            string connectionString = $"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {pathToFolderWithDB}; Extended Properties = dBASE IV; User ID = Admin";
            string sqlExpretion = $"SELECT CODE, NAME, EDIZ, KOLORDER FROM {nameDB}";

            using (OleDbConnection connection = new(connectionString))
            {
                connection.Open();

                OleDbCommand oleDbCommand = new OleDbCommand(sqlExpretion, connection);

                using (OleDbDataReader reader = oleDbCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MaterialFromDBF materialFromDBF = new MaterialFromDBF();
                            //MaterialFromDBF materialFromDBF = new MaterialFromDBF();
                            if (reader["CODE"] != DBNull.Value)
                            {
                                materialFromDBF.CODE = reader.GetString(0);
                            }
                            else
                                materialFromDBF.CODE = string.Empty;

                            materialFromDBF.NAME = reader.GetString(1);
                            materialFromDBF.EDIZ = reader.GetString(2);
                            materialFromDBF.KOLORDER = reader.GetDouble(3);
                            list.Add(materialFromDBF);
                        }
                    }
                }
            }

            return list;

        }
    }
}