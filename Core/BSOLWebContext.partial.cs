using BSOL.Core.Models.Common;
using BSOL.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core
{
    public partial class BSOLWebContext
    {
        /// <summary>
        /// Execute stored procedure and return result as generic list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<T>> ExecuteSpAsync<T>(string spName, object param = null) where T : class
        {
            try
            {
                return await Set<T>().FromSqlRaw(ConstructWithObjectParam(spName, param), GetSQLParams(param)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw DBError(ex, spName, param);
            }
        }

        public async Task<List<T>> ExecuteSqlSpAsync<T>(string spName, IEnumerable<SqlParameter> param) where T : class
        {
            try
            {
                return await Set<T>().FromSqlRaw(ConstructWithSqlParam(spName, param), FormatSqlParams(param)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw DBSqlError(ex, spName, param);
            }
        }

        /// <summary>
        /// Execute stored procedure and return result as Single Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> ExecuteSingleAsync<T>(string spName, object param = null) where T : class
        {
            try
            {
                return (await Set<T>().FromSqlRaw(ConstructWithObjectParam(spName, param), GetSQLParams(param)).ToListAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw DBError(ex, spName, param);
            }
        }

        /// <summary>
        /// Execute stored procedure for CRUD operations
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(string spName, object param = null)
        {
            try
            {
                return await Database.ExecuteSqlRawAsync(ConstructWithObjectParam(spName, param), GetSQLParams(param));
            }
            catch (Exception ex)
            {
                throw DBError(ex, spName, param);
            }
        }

        /// <summary>
        /// Execute stored procedure for CRUD operations with SQL Params
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlNonQueryAsync(string spName, IEnumerable<SqlParameter> param)
        {
            try
            {
                return await Database.ExecuteSqlRawAsync(ConstructWithSqlParam(spName, param), FormatSqlParams(param));
            }
            catch (Exception ex)
            {
                throw DBSqlError(ex, spName, param);
            }
        }

        /// <summary>
        /// Execute stored procedure for CRUD operations and return the Identity value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<T> ExecuteNonQueryWithReturnAsync<T>(string spName, object param = null)
        {
            try
            {
                SqlParameter returnCode = new SqlParameter
                {
                    ParameterName = "@ReturnCode",
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Output
                };

                List<SqlParameter> par = new List<SqlParameter>
                {
                    returnCode
                };
                if (param != null)
                    par.AddRange(GetSQLParams(param));

                await Database.ExecuteSqlRawAsync("Exec @ReturnCode =  " + ConstructWithObjectParam(spName, param), par);
                return (T)Convert.ChangeType(returnCode.Value, typeof(T));
            }
            catch (Exception ex)
            {
                throw DBError(ex, spName, param);
            }
        }

        public async Task<T> ExecuteSqlNonQueryWithReturnAsync<T>(string spName, IEnumerable<SqlParameter> param)
        {
            try
            {
                SqlParameter returnCode = new SqlParameter
                {
                    ParameterName = "@ReturnCode",
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Output
                };

                List<SqlParameter> par = new List<SqlParameter>
                {
                    returnCode
                };
                if (param != null)
                    par.AddRange(param);

                await Database.ExecuteSqlRawAsync("Exec @ReturnCode =  " + ConstructWithSqlParam(spName, param), FormatSqlParams(par));
                return (T)Convert.ChangeType(returnCode.Value, typeof(T));
            }
            catch (Exception ex)
            {
                throw DBSqlError(ex, spName, param);
            }
        }

        /// <summary>
        /// Execute stored procedure and return result as DataTable
        /// </summary>
        /// <param name="SpName"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(string SpName, object Params = null)
        {
            try
            {
                DataTable dtResult = new DataTable();
                using (var command = this.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = SpName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(GetSQLParams(Params));
                    await this.Database.OpenConnectionAsync();
                    using (var result = await command.ExecuteReaderAsync())
                        dtResult.Load(result);
                    await this.Database.CloseConnectionAsync();
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                throw DBError(ex, SpName, Params);
            }
        }

        /// <summary>
        /// Construct Paramerised stored procedure query
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public string ConstructWithObjectParam(string SPName, object Params)
        {
            return Params == null ? SPName : (SPName + " " + string.Join(",", Params.GetType().GetProperties().Select(x => string.Format("@{0}=@{0}", x.Name))));
        }

        /// <summary>
        /// Construct Paramerised stored procedure query
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public string ConstructWithSqlParam(string SPName, IEnumerable<SqlParameter> Params)
        {
            return SPName + " " + string.Join(",", Params.Select(x => string.Format("@{0}=@{0}", x.ParameterName.Replace("@", ""))));
        }

        /// <summary>
        /// Convert object into sql parameters
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public SqlParameter[] GetSQLParams(object Params)
        {
            SqlParameter[] arrParams = Params == null ? new List<SqlParameter>().ToArray()
                                                        : Params.GetType().GetProperties().Select(x => new SqlParameter(x.Name, x.GetValue(Params) == null ? DBNull.Value : x.GetValue(Params))).ToArray();
            return arrParams;
        }

        /// <summary>
        /// Convert null into sql db null
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public SqlParameter[] FormatSqlParams(IEnumerable<SqlParameter> Params)
        {
            Params.ToList().ForEach(x => x.Value = x.Value == null ? DBNull.Value : x.Value);
            return Params.ToArray();
        }

        /// <summary>
        /// Get params from object
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public string GetParam(object Params)
        {
            List<string> lstParams = Params == null ? new List<string>() : Params.GetType().GetProperties().Select(y =>
            {
                var value = y.GetValue(Params);

                if (y.PropertyType == typeof(string))
                    value = value == null ? "NULL" : string.Format("'{0}'", value.ToString().Replace("'", "''"));

                else if (y.PropertyType == typeof(DateTime) || y.PropertyType == typeof(DateTime?))
                    value = value == null ? "NULL" : string.Format("'{0}'", (Convert.ToDateTime(value).TimeOfDay == new TimeSpan() ? Convert.ToDateTime(value).ToString("yyyy-MM-dd") : Convert.ToDateTime(value).ToString("yyyy-MM-dd hh:mm tt")));

                else if (y.PropertyType == typeof(Enum))
                    value = string.Format("'{0}'", ((Enum)value).Description());

                if (value != null && (y.PropertyType == typeof(TimeSpan) || y.PropertyType == typeof(TimeSpan?)))
                    value = string.Format("'{0}'", value);

                if (value == null || value == DBNull.Value)
                    value = "NULL";

                return string.Format("@{0}={1}", y.Name, value);
            }).ToList();

            return string.Join(",", lstParams);
        }

        /// <summary>
        /// Format Exception
        /// </summary>
        /// <param name="Ex"></param>
        /// <param name="Query"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        private Exception DBError(Exception Ex, string Query, object Params = null)
        {
            string query = string.Format("Exec {0} {1}", Query, GetParam(Params));
            return new Exception(query, Ex);
        }

        /// <summary>
        /// Format Exception
        /// </summary>
        /// <param name="Ex"></param>
        /// <param name="Query"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        private Exception DBSqlError(Exception Ex, string Query, IEnumerable<SqlParameter> Params)
        {
            string query = string.Format("Exec {0} {1}", Query, string.Join(",", Params.Select(x => string.Format("@{0}='{1}'", x.ParameterName.Replace("@", ""), x.Value))));
            return new Exception(query, Ex);
        }

        public async Task<List<string>> ValidateAsync(string spName, object param = null)
        {
            try
            {
                var result = await ExecuteSpAsync<ValidationModel>(spName, param);
                return result.Select(x => x.Msg).ToList();
            }
            catch (Exception ex)
            {
                throw DBError(ex, spName, param);
            }
        }

        public async Task<List<string>> ValidateSqlAsync(string spName, IEnumerable<SqlParameter> Params)
        {
            try
            {
                var result = await ExecuteSqlSpAsync<ValidationModel>(spName, Params);
                return result.Select(x => x.Msg).ToList();
            }
            catch (Exception ex)
            {
                throw DBSqlError(ex, spName, Params);
            }
        }
    }
}
