using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace UH.EpicCutoverTab
{
    public static class ErrorLog
    {
        public static void LogAndRaiseError(Exception ex, string userFriendlyMessage, String routineName, String appName)
        {
            LogError(ex, routineName, appName);
            MessageBox.Show(userFriendlyMessage, "Error");
        }

        public static void LogAndRaiseError(SqlException ex, string userFriendlyMessage, String routineName, String appName)
        {
            LogError(ex, routineName, appName);
            MessageBox.Show(userFriendlyMessage, "Error");
        }

        public static void LogAndRaiseError(String ex, string userFriendlyMessage, String routineName, String appName)
        {
            LogError(ex, routineName, appName);
            MessageBox.Show(userFriendlyMessage, "Error");
        }

        public static int LogError(Exception ex, String routineName, String appName)
        {
            CustomContextObj cc = CustomContextObj.GetInstance();
            if (!long.TryParse(cc.UserGUID, out var userGUID)) userGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var clientGUID)) clientGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var visitGUID)) visitGUID = 0;

            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {

                    command.Connection = sqlConn;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO [UH_ErrorLog] ([ErrorDtm],[UserGUID],[ClientGUID],[VisitGUID],[Hostname],[ErrorType],[ApplicationName],[RoutineName],[ErrorMsg],[NotifiedDt])"
                        +
                        " VALUES (GetDate(), @UserGUID, @ClientGUID,@VisitGUID, @Hostname,  @ErrorType, @ApplicationName, @RoutineName, @ErrorMsg, getdate()) ";

                    command.Parameters.AddWithValue("@UserGUID", userGUID);
                    command.Parameters.AddWithValue("@ClientGUID", clientGUID);
                    command.Parameters.AddWithValue("@VisitGUID", visitGUID);
                    command.Parameters.AddWithValue("@Hostname", Environment.MachineName);
                    command.Parameters.AddWithValue("@ErrorType", ex.GetType().ToString());
                    command.Parameters.AddWithValue("@ApplicationName", appName);
                    command.Parameters.AddWithValue("@RoutineName", routineName);
                    command.Parameters.AddWithValue("@ErrorMsg", ex.Message);

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show(sqlEx.Message, "Error Log Error");
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        public static int LogError(SqlException ex, String routineName, String appName)
        {
            CustomContextObj cc = CustomContextObj.GetInstance();
            if (!long.TryParse(cc.UserGUID, out var userGUID)) userGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var clientGUID)) clientGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var visitGUID)) visitGUID = 0;

            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO [UH_ErrorLog] ([ErrorDtm],[UserGUID],[ClientGUID],[VisitGUID],[Hostname],[ErrorType],[ApplicationName],[RoutineName],[ErrorMsg],[NotifiedDt])"
                        +
                        " VALUES (GetDate(), @UserGUID, @ClientGUID,@VisitGUID, @Hostname,  @ErrorType, @ApplicationName, @RoutineName, @ErrorMsg, getdate()) ";

                    command.Parameters.AddWithValue("@UserGUID", userGUID);
                    command.Parameters.AddWithValue("@ClientGUID", clientGUID);
                    command.Parameters.AddWithValue("@VisitGUID", visitGUID);
                    command.Parameters.AddWithValue("@Hostname", Environment.MachineName);
                    command.Parameters.AddWithValue("@ErrorType", ex.GetType().ToString());
                    command.Parameters.AddWithValue("@ApplicationName", appName);
                    command.Parameters.AddWithValue("@RoutineName", routineName);
                    command.Parameters.AddWithValue("@ErrorMsg", ex.Message);

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show(sqlEx.Message, "Error Log Error");
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        public static int LogError(String ex, String routineName, String appName)
        {
            CustomContextObj cc = CustomContextObj.GetInstance();
            if (!long.TryParse(cc.UserGUID, out var userGUID)) userGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var clientGUID)) clientGUID = 0;
            if (!long.TryParse(cc.UserGUID, out var visitGUID)) visitGUID = 0;

            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO [UH_ErrorLog] ([ErrorDtm],[UserGUID],[ClientGUID],[VisitGUID],[Hostname],[ErrorType],[ApplicationName],[RoutineName],[ErrorMsg],[NotifiedDt])"
                        +
                        " VALUES (GetDate(), @UserGUID, @ClientGUID,@VisitGUID, @Hostname,  @ErrorType, @ApplicationName, @RoutineName, @ErrorMsg, getdate()) ";

                    command.Parameters.AddWithValue("@UserGUID", userGUID);
                    command.Parameters.AddWithValue("@ClientGUID", clientGUID);
                    command.Parameters.AddWithValue("@VisitGUID", visitGUID);
                    command.Parameters.AddWithValue("@Hostname", Environment.MachineName);
                    command.Parameters.AddWithValue("@ErrorType", ex.GetType().ToString());
                    command.Parameters.AddWithValue("@ApplicationName", appName);
                    command.Parameters.AddWithValue("@RoutineName", routineName);
                    command.Parameters.AddWithValue("@ErrorMsg", ex);

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show(sqlEx.Message, "Error Log Error");
                        throw;
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
        }
    }
}
