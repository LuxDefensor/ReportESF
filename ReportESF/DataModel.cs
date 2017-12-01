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

        public DataTable GetChildren(int parentID)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                string sql = @"
select id_point, pointname, point_type, id_parent from points
where id_parent=" + parentID;
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

        public string PointName(int idPoint)
        {
            object result = "Ошибка!";
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = "select PointName from points where ID_Point=" + idPoint;
                try
                {
                    result = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.PointName");
                    formError err = new formError("Ошибка при получении информации об узле " + idPoint,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
                if (result == null || Convert.IsDBNull(result))
                {
                    string details = Settings.ErrorInfo(new Exception("Запрос вернул пустое значение"), "DataModel.PointInfo");
                    formError err = new formError("Ошибка при получении информации об узле " + idPoint,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                    result = "Ошибка!";
                }
                return result.ToString();
            }
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

        public string PointPath(int pointID, string separator)
        {
            object result = "Ошибка!";
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
select dbo.PointPath({0},dbo.TreeRootID({0}),'{1}')",
pointID, separator);
                try
                {
                    result = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "DataModel.PointPath");
                    formError err = new formError("Ошибка при получении списка подчинённых узлов для " + pointID,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                }
                if (result == null || Convert.IsDBNull(result))
                {
                    string details = Settings.ErrorInfo(new Exception("Запрос вернул пустое значение"), "DataModel.PointPath");
                    formError err = new formError("Ошибка при получении информации об узле " + pointID,
                        "Ошибка!",
                        details + Environment.NewLine + cmd.CommandText);
                    err.ShowDialog();
                    result = "Ошибка!";
                }
                return result.ToString();
            }
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

        /// <summary>
        /// Checks Ktr on the both ends of the time interval.
        /// If two Ktr values are the same then returns this value
        /// otherwise returns NULL
        /// </summary>
        /// <param name="id_pp"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public Nullable<double> GetKtr(string id_pp, DateTime dtStart, DateTime dtEnd)
        {
            object result;
            object ktr1, ktr2;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
select dbo.zzz_getcoef(dbo.pp_id_point({0}),{1}", id_pp, dtStart);
                try
                {
                    ktr1 = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
    "Ошибка!", Settings.ErrorInfo(ex, "DataModel.GetKtr" +
    Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    return null;
                }
                cmd.CommandText = string.Format(@"
select dbo.zzz_getcoef(dbo.pp_id_point({0}),{1}", id_pp, dtEnd);
                try
                {
                    ktr2 = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
"Ошибка!", Settings.ErrorInfo(ex, "DataModel.GetKtr" +
Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    return null;
                }
                if (ktr1?.ToString() == ktr2?.ToString())
                    return (double)ktr1;
                else
                    return null;
            }
        }

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

select n.DT,n.Val, n.State
from @dates d
outer apply 
(select ni.DT,ni.Val,ni.State from PointNIs_On_Main_Stack ni 
 inner join SchemaContents sc on sc.ID_Ref=ni.ID_PP and sc.RefIsPoint=2
where sc.ID_PP={2} and d.dt0=ni.dt and ni.DT between sc.DT1 and sc.DT2) as n
", dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"), id_pp);
                }
                else
                {
                    if (withKtr)
                    {
                        sql.AppendFormat("select dt, value, state from dbo.f_Get_PointNIs({0},'{1}','{2}',3,0,1,null,0,null,null,null)",
                                         id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"));
                    }
                    else
                    {
                        sql.Append("select n.dt, n.value, n.state from schemacontents sc cross apply ");
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

        public DataTable PairOfFixedValues(string id_pp, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
select dt,value,state 
from dbo.f_Get_PointNIs({0},'{1}','{2}',0,default,default,default,default,default,default,default) n1",
                    id_pp, dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    formError err = new formError("Невозможно получить значения",
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.PairOfFixedValues" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                if (result.Rows.Count != 2)
                {
                    result.Rows.Clear();
                    result.Rows.Add(dtStart, null, 1);
                    result.Rows.Add(dtEnd, null, 1);
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
                    "select dt,value,state from dbo.f_Get_PointProfile({0},'{1}','{2}',3,null,null,null,null,null)",
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
                    "select dt,value,state from dbo.f_Get_PointProfile({0},'{1}','{2}',2,null,null,null,null,null)",
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
                    "select dt,value,state from dbo.f_Get_PointProfile({0},'{1}','{2}',1,null,null,null,null,null)",
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
declare @halfhours int

set @dt1='{0}'
set @dt2=DATEADD(minute,-30,'{1}')
set @halfhours=DATEDIFF(HOUR,@dt1,@dt2)*2

if @halfhours=0 set @halfhours=1;

with src as(
select p.id_point, dbo.zzz_GetPS(p.ID_Point) PS, PointName, ni.DT, ni.ID_PP meter, pp.ID_PP feeder
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
left join SchemaContents sc on pp.ID_PP=sc.ID_PP and sc.RefIsPoint=2
left join PointMains ni on ni.ID_PP=sc.ID_Ref
where pp.ID_PP in ({2})
and ni.dt between @dt1 and @dt2 and sc.DT1<@dt1 and sc.DT2>@dt2)

select dbo.zzz_GetPS(p.ID_Point) PS, PointName,
100 * (select count(*) from src where src.ID_Point=p.ID_Point)/@halfhours/count(id_pp) PC,
(select max(dt) from pointmains m
 right join pointparams pp1 on pp1.ID_PP=m.ID_PP
 where pp1.ID_Point=p.ID_point) LastDate
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
where pp.ID_PP in ({2})
group by p.ID_Point,PointName",
                    dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"),
                    string.Join(",",id_pps));
                cmd.CommandTimeout = 300;
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
declare @days int

set @dt1='{0}'
set @dt2='{1}'
set @days=DATEDIFF(DAY,@dt1,@dt2)

if @days=0 set @days=1;

with src as(
select p.id_point, dbo.zzz_GetPS(p.ID_Point) PS, PointName, ni.DT, ni.ID_PP meter, pp.ID_PP feeder
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
left join SchemaContents sc on pp.ID_PP=sc.ID_PP and sc.RefIsPoint=2
left join PointNIs_On_Main_Stack ni on ni.ID_PP=sc.ID_Ref
where pp.ID_PP in ({2})
and ni.dt between @dt1 and @dt2 and sc.DT1<@dt1 and sc.DT2>@dt2)

select dbo.zzz_GetPS(p.ID_Point) PS, PointName,
100 * (select count(*) from src where src.ID_Point=p.ID_Point)/@days/count(id_pp) PC,
(select max(dt) from src
 right join pointparams pp1 on pp1.id_pp=src.feeder
 where pp1.ID_Point=p.ID_Point) LastDate
from points p left join pointparams pp on p.ID_Point=pp.ID_Point
where pp.ID_PP in ({2})
group by p.ID_Point,PointName",
                    dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"),
                    string.Join(",", id_pps));
                cmd.CommandTimeout = 300;
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

        public DataTable GetPercentLogs(List<string> id_pps, DateTime dtStart, DateTime dtEnd)
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

select dbo.zzz_getps(pp.id_point) ps,
(select pointname from points where id_point=pp.ID_Point) feeder,
case count(*)
when 0 then 0
else 100
end pc,
max(d.dt)
from pointparams pp
inner join schemacontents sc_high on pp.ID_PP=sc_high.ID_PP and
sc_high.RefIsPoint=2
inner join schemacontents sc_low on sc_high.ID_Ref=sc_low.ID_PP and
sc_low.refispoint=1
inner join channels_main c1 on c1.ID_Channel=sc_low.ID_Ref and
sc_low.RefIsPoint=1
inner join channels_main c2 on c2.ID_USPD=c1.ID_USPD and c2.TypeChan='J'
inner join vwDiscretsWithDesc d on d.id_channel=c2.ID_Channel
where d.dt between @dt1 and @dt2
and sc_high.id_pp in ({2})
group by pp.id_point
order by 1,2",
                    dtStart.ToString("yyyyMMdd"), dtEnd.ToString("yyyyMMdd"),
                    string.Join(",", id_pps));
                cmd.CommandTimeout = 300;
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
                    result.Rows.Add("Журнал пуст", "", 0, null);
                }
            }
            return result;
        }

        public DataTable MeterLogs(List<string> id_pps, DateTime dtStart, DateTime dtEnd)
        {
            DataTable result = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = string.Format(@"
select dbo.zzz_getps(pp.id_point) Подстанция,
(select pointname from points where id_point=pp.ID_Point) Счетчик,
d.dt Дата, d.description Описание,d.comment Дополнительно
from pointparams pp
inner join schemacontents sc_high on pp.ID_PP=sc_high.ID_PP and
sc_high.RefIsPoint=2
inner join schemacontents sc_low on sc_high.ID_Ref=sc_low.ID_PP and
sc_low.refispoint=1
inner join channels_main c1 on c1.ID_Channel=sc_low.ID_Ref and
sc_low.RefIsPoint=1
inner join channels_main c2 on c2.ID_USPD=c1.ID_USPD and c2.TypeChan='J'
inner join vwDiscretsWithDesc d on d.id_channel=c2.ID_Channel
where d.dt between '{0}' and '{1}'
and sc_high.id_pp in ({2})
order by 1,2,3 desc",
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
                        "Ошибка!", Settings.ErrorInfo(ex, "DataModel.MeterLogs" +
                        Environment.NewLine + Environment.NewLine + cmd.CommandText));
                    err.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                if (result.Rows.Count == 0)
                {
                    result.Rows.Add("Нет событий", "", null, "", "");
                }
            }
            return result;
        }

        #endregion
    }
}