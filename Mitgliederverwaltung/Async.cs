using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace My.Async
{
    #region Klassen AsyncResult
    public class AsyncResult<T>
    {
        public T Item = default(T);
        public Exception Exception = null;
    }
    #endregion

    #region Klasse AsyncTask
    public sealed class AsyncTask
    {
        #region Felder
        private Control mInvoking;
        private readonly object mSync = new object();
        private bool mIsBusy = false;
        #endregion

        #region Eigenschaften
        public Control Invoking
        {
            get { return mInvoking; }
            set { mInvoking = value; }
        }
        public bool IsBusy
        {
            get { return mIsBusy; }
        }
        #endregion

        #region Konstruktor
        public AsyncTask(Control invoking)
        {
            mInvoking = invoking;
        }
        #endregion

        #region Methoden
        public void Execute<T>(T item, Action<AsyncResult<T>> process, Action<AsyncResult<T>> callBack)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            result.Item = item;

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<T> res = (AsyncResult<T>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        process(result);
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void Execute<T>(Action<AsyncResult<T>> process, Action<AsyncResult<T>> callBack)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            result.Item = default(T);

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<T> res = (AsyncResult<T>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        process(result);
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void Execute<T>(T item, Action<AsyncResult<T>> process, Action<Exception> callBack)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            result.Item = item;

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<T> res = (AsyncResult<T>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res.Exception); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        process(result);
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void Execute(Action process, Action<Exception> callBack)
        {
            Exception exc = null;

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                async.EndInvoke(iar);

                Action act = () => { callBack(exc); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        process();
                    }
                    catch (Exception ex)
                    {
                        exc = ex;
                    }
                };

                async.BeginInvoke(cBack, null);
                mIsBusy = true;
            }
        }
        #endregion
    }
    #endregion

    #region Klasse AsyncSqlTask
    public sealed class AsyncSqlTask
    {
        #region Felder
        private readonly Control mInvoking;
        private readonly object mSync = new object();
        private bool mIsBusy = false;
        private string mConstr;
        #endregion

        #region Eigenschaften
        public Control Invoking
        {
            get { return mInvoking; }
        }
        public bool IsBusy
        {
            get { return mIsBusy; }
        }
        public string ConnectionString
        {
            get { return mConstr; }
            set { mConstr = value; }
        }
        #endregion

        #region Konstruktoren
        public AsyncSqlTask(Control invoking) 
        {
            mInvoking = invoking;
        }
        public AsyncSqlTask(Control invoking, string conStr)
        {
            mInvoking = invoking;
            mConstr = conStr;
        }
        #endregion

        #region Methoden
        //
        private static SqlCommand CreateCommand(string sql, params SqlParameter[] ps)
        {
            SqlCommand com = new SqlCommand(sql);
            com.CommandTimeout = 0;

            if(ps != null)
            {
                foreach (SqlParameter p in ps)
                    com.Parameters.Add(p);
            }

            return com;
        }
        //
        public void FillTable(string name, Action<AsyncResult<DataTable>> callBack)
        {
            AsyncResult<DataTable> result = new AsyncResult<DataTable>();

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<DataTable> res = (AsyncResult<DataTable>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();

                            SqlCommand com = con.CreateCommand();
                            com.CommandTimeout = 0;
                            com.CommandText = "SELECT * FROM " + name;

                            SqlDataAdapter adr = new SqlDataAdapter(com);

                            DataTable tbl = new DataTable();
                            adr.Fill(tbl);

                            result.Item = tbl;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        //
        public void ReadTable(Action<AsyncResult<DataTable>> callBack, SqlCommand com)
        {
            AsyncResult<DataTable> result = new AsyncResult<DataTable>();

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<DataTable> res = (AsyncResult<DataTable>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();
                            com.Connection = con;

                            SqlDataAdapter adr = new SqlDataAdapter(com);

                            DataTable tbl = new DataTable();
                            adr.Fill(tbl);

                            result.Item = tbl;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void ReadTable(Action<AsyncResult<DataTable>> callBack, string sql,
            params SqlParameter[] ps)
        {
            ReadTable(callBack, CreateCommand(sql, ps));
        }
        //
        public void ReadData<T>(T item, Action<SqlDataReader, AsyncResult<T>> process, 
            Action<AsyncResult<T>> callBack, SqlCommand com)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            result.Item = item;

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<T> res = (AsyncResult<T>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();
                            com.Connection = con;

                            using (SqlDataReader rdr = com.ExecuteReader())
                            {
                                process(rdr, result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void ReadData<T>(T item, Action<SqlDataReader,
            AsyncResult<T>> process, Action<AsyncResult<T>> callBack,
            string sql, params SqlParameter[] ps)
        {
            ReadData(item, process, callBack, CreateCommand(sql, ps));
        }
        //
        public void ReadScalar(Action<AsyncResult<object>> callBack,SqlCommand com)
        {
            AsyncResult<object> result = new AsyncResult<object>();

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<object> res = (AsyncResult<object>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();
                            com.Connection = con;
                            result.Item = com.ExecuteScalar();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void ReadScalar(Action<AsyncResult<object>> callBack, string sql,
            params SqlParameter[] ps)
        {
            ReadScalar(callBack, CreateCommand(sql, ps));
        }
        public void ReadGetDate(Action<AsyncResult<DateTime>> callBack)
        {
            
        }
        //
        public void Save(Action<AsyncResult<int>> callBack, SqlCommand com)
        {
            AsyncResult<int> result = new AsyncResult<int>();

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<int> res = (AsyncResult<int>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();
                            com.Connection = con;
                            result.Item = com.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        public void Save(Action<AsyncResult<int>> callBack, string sql,
            params SqlParameter[] ps)
        {
            Save(callBack, CreateCommand(sql, ps));
        }
        //
        public void Execute<T>(T item, Action<SqlConnection, AsyncResult<T>> process,
            Action<AsyncResult<T>> callBack)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            result.Item = item;

            AsyncCallback cBack = (iar) =>
            {
                Action async = (Action)((AsyncResult)iar).AsyncDelegate;
                AsyncResult<T> res = (AsyncResult<T>)iar.AsyncState;

                async.EndInvoke(iar);

                Action act = () => { callBack(res); };
                mInvoking.Invoke(act);

                lock (mSync) { mIsBusy = false; }
            };

            lock (mSync)
            {
                if (mIsBusy)
                    return;

                Action async = () =>
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(mConstr))
                        {
                            con.Open();
                            process(con, result);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Exception = ex;
                    }
                };

                async.BeginInvoke(cBack, result);
                mIsBusy = true;
            }
        }
        //
        #endregion
    }
    #endregion
}
