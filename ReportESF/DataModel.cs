﻿using System;
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

        #region Tree operations
        public List<int> GetRoots_deprecated()
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

        public List<int> GetRoots()
        {
            List<int> result = new List<int>();
            try
            {
                string roots = Settings.GetSetting("roots");
                string[] ids = roots.Split(';');
                result.AddRange(ids.Select<string, int>(s => int.Parse(s.Trim())));
                return result;
            }
            catch (Exception ex)
            {
                string details = Settings.ErrorInfo(ex, "DataModel.GetRoots");
                formError err = new formError("Ошибка при получении списка корней",
                    "Ошибка!",
                    details);
                err.ShowDialog();
                return new List<int>(new int[] { 1 });
            }
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
	                where ap.PointType in (8,7,151,148,147,10,5,1,2,9)
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

        public DataTable ParamInfo(string id_pp)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                string sql = @"select dbo.zzz_getps(p.id_point) psname,pointname, 
                    case pp.id_param
                        when 2 then '(А-)'
                        when 4 then '(А+)'
                        when 6 then '(Р-)'
                        when 8 then '(Р+)'
                    end
                    from points p inner join PointParams pp on p.ID_Point=pp.ID_Point
                    inner join PointParamTypes t on pp.ID_Param=t.ID_Param
                    where pp.ID_PP="+id_pp;
                cmd.CommandText = sql;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить сведения о параметре",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.ParamInfo") +
                        Environment.NewLine + Environment.NewLine + sql);
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        /// <summary>
        /// Returns point name from database
        /// </summary>
        /// <param name="id_pp">ID_PP column in the table PointParams</param>
        /// <returns>A tuple where the first item is Substation name and the second is the feeder's name</returns>
        public Tuple<string, string> GetFeederName(string id_pp)
        {
            SqlDataReader dr = null;
            Tuple<string, string> result;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
select distinct dbo.zzz_GetPS(p.id_point) PS, PointName from points p
inner join PointPParams pp on pp.id_point=p.id_point
where id_pp={0}", id_pp);
                try
                {
                    dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
                }
                catch(Exception ex)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.GetFeederName" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                if (!dr.HasRows)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(new Exception("The query returned empty rowset"), "DataModel.GetFeederName" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                dr.Read();
                result = new Tuple<string, string>(dr[0].ToString(), dr[1].ToString());
                dr.Close();
            }
            return result;
        }
        #endregion

        #region Data retrieving

        public DataTable FixedValues(string id_pp, DateTime dtStart, DateTime dtEnd, bool withKtr, bool measuredOnly)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                StringBuilder sql = new StringBuilder();
                if (measuredOnly)
                {
                    sql.AppendFormat(@"declare @dates table (dt0 datetime)
declare @dt1 datetime, @dt2 datetime, @dtcurrent datetime
set @dt1='{0}'
set @dt2='{1}'
set @dtcurrent=@dt1
while @dtcurrent<=@dt2
begin
	insert into @dates(dt0) values(@dtcurrent)
	set @dtcurrent=DATEADD(day,1,@dtcurrent)
end

select d.dt0, n.DT,n.Val
from @dates d
outer apply 
(select ni.DT,ni.Val from PointNIs_On_Main_Stack ni 
 inner join SchemaContents sc on sc.ID_Ref=ni.ID_PP and sc.RefIsPoint=2
where sc.ID_PP={2} and d.dt0=ni.dt and ni.DT between sc.DT1 and sc.DT2) as n
", dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"), id_pp);
                }
                else
                {
                    if (withKtr)
                    {
                        sql.AppendFormat("select * from dbo.f_Get_PointNIs({0},'{1}','{2}',3,0,1,null,0,null,null,null)",
                                         id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"));
                    }
                    else
                    {
                        sql.Append("select n.* from schemacontents sc cross apply ");
                        sql.AppendFormat("dbo.f_Get_PointNIs(id_ref,'{0}','{1}',3,0,1,null,0,null,null,null) n ",
                                         dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"));
                        sql.AppendFormat("where id_pp={0} and n.DT between sc.DT1 and sc.DT2", id_pp);
                    }
                }
                cmd.CommandText = sql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.HourValues" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        public DataTable DailyValues(string id_pp, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText=string.Format(
                    "select * from dbo.f_Get_PointProfile({0},'{1}','{2}',3,null,null,null,null,null)",
                    id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.AddDays(1).ToString("yyyyMMdd"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.DailyValues" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        public DataTable HourValues(string id_pp, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(
                    "select * from dbo.f_Get_PointProfile({0},'{1}','{2}',2,null,null,null,null,null)",
                    id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.AddDays(1).ToString("yyyyMMdd"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.HourValues" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        public DataTable HalfhourValues(string id_pp, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(
                    "select * from dbo.f_Get_PointProfile({0},'{1}','{2}',1,null,null,null,null,null)",
                    id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.AddDays(1).ToString("yyyyMMdd"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.HourValues" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        public DataTable GetPercentMains(List<string> id_pps, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
declare @dt1 datetime, @dt2 datetime

set @dt1='{0}'
set @dt2=DATEADD(minute,-30,'{1}');

with src as(
select dbo.zzz_GetPS(p.ID_Point) PS, PointName, ni.DT, ni.ID_PP
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
left join SchemaContents sc on pp.ID_PP=sc.ID_PP and sc.RefIsPoint=2
left join PointMains ni on ni.ID_PP=sc.ID_Ref
where pp.ID_PP in ({2})
and ni.dt between @dt1 and @dt2 and sc.DT1<@dt1 and sc.DT2>@dt2)

select PS,PointName,
100 * COUNT(*) / (DATEDIFF(HOUR,@dt1,@dt2)*2) / count(distinct id_pp) PC,
MAX(dt)
from src
group by ps,PointName",
                    dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"),
                    string.Join(",",id_pps));
                da.SelectCommand = cmd;
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.GetPercentMains" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                if (result.Rows.Count==0)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(new Exception("The query returned empty rowset"), "DataModel.GetPercentMains" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        public DataTable GetPercentNIs(List<string> id_pps, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
declare @dt1 datetime, @dt2 datetime

set @dt1='{0}'
set @dt2='{1}';

with src as(
select dbo.zzz_GetPS(p.ID_Point) PS, PointName, ni.DT, ni.ID_PP
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
left join SchemaContents sc on pp.ID_PP=sc.ID_PP and sc.RefIsPoint=2
left join PointNIs_On_Main_Stack ni on ni.ID_PP=sc.ID_Ref
where pp.ID_PP in ({2})
and ni.dt between @dt1 and @dt2 and sc.DT1<@dt1 and sc.DT2>@dt2)

select PS,PointName,
100 * COUNT(*) / (DATEDIFF(DAY,@dt1,@dt2)+1) / count(distinct id_pp) PC,
MAX(dt)
from src
group by ps,PointName",
                    dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"),
                    string.Join(",", id_pps));
                da.SelectCommand = cmd;
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.GetPercentNIs" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                if (result.Rows.Count == 0)
                {
                    formError err = new formError("Невозможно получить значение",
                        "Ошибка!", Settings.ErrorInfo(new Exception("The query returned empty rowset"), "DataModel.GetPercentNIs" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
            }
            return result;
        }

        #endregion
    }
}
