using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//*using ATD_ID4P.Model; // unnecessary
using System.Reflection;
using ATD_ID4P.Class;

// need to add Nuget System.Data.SqlClient

// Check 
namespace ATD_ID4P.Class
{
    public class SqlCls
    {
        //public string connString = "Server =.\\SQLExpress;Database=ID4P;Trusted_Connection = yes;";
        //public string connString = "Server =.\\SQLExpress;Database=PalletizerDB;Trusted_Connection = yes;";
        //public string connString = Properties.Settings.Default.MasterDB_ConString;
        public string connString = "Server=L0740_NAPAPOSA2\\SQLEXPRESS;Database=PalletizerDB;TrustServerCertificate=true;user id = SA; password=PKG#2021;MultipleActiveResultSets=True";
        private LogCls Log = new LogCls();

        public string GetPM2Connection()
        {
            string connPM2 = string.Format("Server={0};Database=PM2;User Id={1};Password={2};", Properties.Settings.Default.ERP_IP, Properties.Settings.Default.ERP_User, Properties.Settings.Default.ERP_Password);
            return connPM2;
        }

        public string GetPM3Connection()
        {
            string connPM3 = string.Format("Server={0};Database={3};User Id={1};Password={2};", Properties.Settings.Default.PM3_IP, Properties.Settings.Default.PM3_User, Properties.Settings.Default.PM3_Password, Properties.Settings.Default.PM3_Database);
            return connPM3;
        }

        public DataTable GetDataTableFromSql(string Query, string _connString = "")
        {
            DataTable dt = new DataTable();
            string cons = (!string.IsNullOrEmpty(_connString) ? _connString : connString);
            SqlConnection conn = new SqlConnection(cons);

            try
            {
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return dt;
        }

        public DataTable GetDataTableFromSqlwithCUD(ref SqlDataAdapter Da, string Query, SqlConnection SCon = null, SqlTransaction STrans = null, string _connString = "")
        {
            DataTable dt = new DataTable();
            string cons = (!string.IsNullOrEmpty(_connString) ? _connString : connString);
            SqlConnection conn = new SqlConnection(cons);
            if (SCon != null)
                conn = SCon;

            try
            {
                SqlCommand cmd = new SqlCommand(Query, conn);
                if (STrans != null)
                    cmd.Transaction = STrans;
                SqlCommandBuilder builder = new SqlCommandBuilder(Da);
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                builder.SetAllValues = false;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                Da.SelectCommand = cmd;
                Da.Fill(dt);
                //Create Command
                Da.InsertCommand = builder.GetInsertCommand();
                Da.UpdateCommand = builder.GetUpdateCommand();
                Da.DeleteCommand = builder.GetDeleteCommand();
                //Binding Connection
                Da.InsertCommand.Connection = conn;
                Da.UpdateCommand.Connection = conn;
                Da.DeleteCommand.Connection = conn;
                //Add Transaction If Extits
                if (STrans != null)
                {
                    Da.InsertCommand.Transaction = STrans;
                    Da.UpdateCommand.Transaction = STrans;
                    Da.DeleteCommand.Transaction = STrans;
                }

                //conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return dt;
        }

        public bool ExcSQL(string Query, string _connString = "")
        {
            bool IsComplete = false;
            string cons = (!string.IsNullOrEmpty(_connString) ? _connString : connString);

            SqlConnection conn = new SqlConnection(cons);

            try
            {
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                cmd.CommandText = Query;
                var Resp = cmd.ExecuteNonQuery();
                conn.Close();
                IsComplete = (Resp == -1 ? false : true);
            }
            catch (Exception e)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                IsComplete = false;
                Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return IsComplete;
        }

        public int ExcSQLwithReturn(string Query, string _connString = "")
        {
            int Result = -1;
            string cons = (!string.IsNullOrEmpty(_connString) ? _connString : connString);

            SqlConnection conn = new SqlConnection(cons);

            try
            {
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                cmd.CommandText = Query;
                var Resp = cmd.ExecuteNonQuery();
                conn.Close();
                Result = Resp;
            }
            catch (Exception e)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                Result = -1;
               Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return Result;
        }

        public object GetScalarFromSql(string Query, string _connString = "")
        {
            object result;

            string cons = (!string.IsNullOrEmpty(_connString) ? _connString : connString);

            SqlConnection conn = new SqlConnection(cons);

            try
            {
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                cmd.CommandText = Query;
                result = cmd.ExecuteScalar();
                conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                result = null;
                Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return result;
        }

        public IEnumerable<T> GetObjectFromSql<T>(string Query)
        {
            List<T> items = new List<T>();
            try
            {
                var dt = GetDataTableFromSql(Query);
                var fields = typeof(T).GetFields();
                foreach (DataRow dr in dt.Rows)
                {
                    var ob = Activator.CreateInstance<T>();

                    foreach (var fieldInfo in fields)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            // Matching the columns with fields
                            if (fieldInfo.Name == dc.ColumnName)
                            {
                                // Get the value from the datatable cell
                                object value = dr[dc.ColumnName];

                                // Set the value into the object
                                fieldInfo.SetValue(ob, value);
                                break;
                            }
                        }
                    }

                    items.Add(ob);
                    //T item = (T)Activator.CreateInstance(typeof(T), row.ItemArray);
                    //items.Add(item);
                }
            }
            catch (Exception e)
            {
                Log.WriteLog("Error! - While trying to 'Query:'" + Query + "' : " + e.Message, LogType.Fail);
            }

            return items;
        }
    }
}
