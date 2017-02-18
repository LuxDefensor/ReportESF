using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ReportESF
{
    class DataModel
    {
        private string cs;

        #region Node types array
        public int[] NodeTypes =
        {
            13, // Все остальные типы                0
            0, // Объект учета                       1
            1, // Структурное подразделение          2
            13, // Все остальные типы                3
            13, // Все остальные типы                4
            2, // Подстанция                         5
            13, // Все остальные типы                6
            3, // Распредустройство                  7
            4, // Система шин (секция шин)           8
            5, // Обходной выключатель               9
            6, // Присоединение (фидер)              10
            13, // Все остальные типы                11
            7, // Присоединение с учетом ОВ          12
            13, // Все остальные типы                13
            13, // Все остальные типы                14
            13, // Все остальные типы                15
            13, // Все остальные типы                16
            13, // Все остальные типы                17
            13, // Все остальные типы                18
            13, // Все остальные типы                19
            13, // Все остальные типы                20
            8, // Счетчик                            21
            13, // Все остальные типы                22
            13, // Все остальные типы                23
            13, // Все остальные типы                24
            13, // Все остальные типы                25
            13, // Все остальные типы                26
            13, // Все остальные типы                27
            13, // Все остальные типы                28
            13, // Все остальные типы                29
            13, // Все остальные типы                30
            13, // Все остальные типы                31
            13, // Все остальные типы                32
            13, // Все остальные типы                33
            13, // Все остальные типы                34
            13, // Все остальные типы                35
            13, // Все остальные типы                36
            13, // Все остальные типы                37
            13, // Все остальные типы                38
            10, // Соседнее предприятие              39
        };

        #endregion

        public DataModel()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            try
            {
                csb.DataSource = Settings.GetSetting("server");
                csb.InitialCatalog = Settings.GetSetting("database");
                csb.UserID = Settings.GetSetting("user");
                csb.Password = Settings.GetSetting("password");
            }
            catch (Exception ex)
            {
                formError err = new formError("Невозможно прочитать настройки соединения с БД",
                    "Ошибка!", Settings.ErrorInfo(ex, "DataModel.Constructor"));
                err.ShowDialog();
                System.Windows.Forms.Application.Exit();
            }
            csb.IntegratedSecurity = false;
            csb.ConnectTimeout = 300;
            cs = csb.ConnectionString;
        }

        public List<int> GetRoots()
        {
            List<int> result = new List<int>();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = "select ID_Point from points where ID_Parent is null";
                SqlDataReader dr;
                try
                {
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add((int)dr[0]);
                        
                    }
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.GetRoots");
                    formError err = new formError("Ошибка при получении списка корневых элементов дерева",
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
            }
            return result;
        }

        public DataTable GetTree(int parentID)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                string sql = @"WITH AllPoints(PointID, PointName, PointType, ID_Parent) 
	                AS (
	                SELECT ID_Point, PointName, Point_Type, ID_Parent
	                FROM Points 
	                WHERE ID_Point = {0}
	                UNION ALL 
	                SELECT Points.ID_Point, Points.PointName, Points.Point_Type, Points.ID_Parent 
	                FROM Points INNER JOIN AllPoints ON Points.ID_Parent = AllPoints.PointID
	                )
	                SELECT ap.PointID,ap.ID_Parent,ap.PointName,
	                ap.PointType
	                FROM AllPoints ap 
	                where ap.PointType in (8,7,151,148,147,10,5,1,2)
                    and ap.PointID<>1
                    ";
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = sql.Replace("{0}", parentID.ToString());
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.GetTree");
                    formError err = new formError("Ошибка при получении списка подчинённых узлов для " + parentID,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
            }
            return result;
        }

        public DataTable PointInfo(int pointID)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                string sql = @"select p.ID_Point,p.ID_Parent,p.PointName,p.Point_Type,null,null
                    from points p 
                    where p.ID_Point=" + pointID;
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.PointInfo");
                    formError err = new formError("Ошибка при получении информации об узле " + pointID,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
            }
            return result;
        }

        public DataTable GetParams(string pointID)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                string sql = @"select pp.ID_PP,t.ParamName
	                from pointparams pp
	                inner join PointParamTypes t on pp.ID_Param=t.ID_Param
	                where ID_Point={0}
	                and pp.ID_Param in (2,4,6,8)";
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = sql.Replace("{0}", pointID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.GetParams");
                    formError err = new formError("Ошибка при получении параметров для " + pointID,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
            }
            return result;
        }
    }
}
