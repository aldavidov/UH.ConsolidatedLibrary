using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using SCMLib.Context;
using SCMLib.HVCLogon;

namespace UH.FaxTab
{
    internal class DataAccess
    {
        internal CustomContextObj CustContext;

        internal DataAccess()
        {
            CustContext = CustomContextObj.GetInstance();
        }

        internal string GetUserInstructions()
        {
            string retString = "";
            var ds = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetFaxTabInstructions_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(ds);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetUserInstructions()", "DocumentFaxing");
                return retString;
            }
            if (ds.Rows.Count > 0)
            {
                retString = ds.Rows[0]["Instructions"].ToString();
            }
            return retString;
        }

        internal string GetUserInstructions(String tooltip)
        {
            string retString = "";
            var ds = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetFaxTabInstructions_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@InstructionType", SqlDbType.VarChar).Value = tooltip;
                        da.Fill(ds);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetUserInstructions(String tooltip)", "DocumentFaxing");
                return retString;
            }
            if (ds.Rows.Count > 0)
            {
                retString = ds.Rows[0]["Instructions"].ToString();
            }
            return retString;
        }

        internal DataTable GetColumns(String columnGroup)
        {
            var ds = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetColumns_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@ColumnGroup", SqlDbType.VarChar).Value = columnGroup;
                        da.Fill(ds);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetColumns(String columnGroup)", "DocumentFaxing");
            }

            return ds;
        }

        internal string GetProviderType()
        {
            string retString = "Primary Oncologist";
            using (var results = new DataTable())
            {
                string sqlstmt = "Select Top 1 ProviderType = Value from HVCEnvprofile (Nolock) where code = 'DefaultAddProviderType'";

                using (var conn = HVCLogonObj.GetSqlConnection())
                using (var command = new SqlCommand(sqlstmt, conn))
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(results);
                    if (results.Rows.Count > 0)
                    {
                        retString = results.Rows[0]["ProviderType"].ToString();
                    }
                    return retString;
                }
            }

        }

        internal DataTable GetProviders()
        {
            var ds = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetProviders_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(ds);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetProviders()", "DocumentFaxing");
            }
            return ds;
        }

        internal DataTable GetPatientProviders(string scope)
        {
            var ds = new DataTable();
            if (scope != null)
            {
                if (scope == "General")
                {
                    try
                    {
                        using (var sqlConn = HVCLogonObj.GetSqlConnection())
                        {
                            using (var da = new SqlDataAdapter("UH_FaxTab_GetProviders_Sel_Pr", sqlConn))
                            {
                                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                                da.Fill(ds);
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx, "GetPatientProviders(string scope)", "DocumentFaxing");
                    }
                    return ds;
                }

                try
                {
                    using (var sqlConn = HVCLogonObj.GetSqlConnection())
                    {
                        using (var da = new SqlDataAdapter("UH_FaxTab_GetVisitProviders_Sel_Pr", sqlConn))
                        {
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.SelectCommand.Parameters.Add("@ClientVisitGUID", SqlDbType.VarChar).Value =
                                CustContext.VisitGUID;
                            da.SelectCommand.Parameters.Add("@ClientGUID", SqlDbType.VarChar).Value =
                                CustContext.ClientGUID;
                            da.SelectCommand.Parameters.Add("@ChartGUID", SqlDbType.VarChar).Value =
                                CustContext.ChartGUID;
                            da.SelectCommand.Parameters.Add("@Scope", SqlDbType.VarChar).Value = scope;
                            da.Fill(ds);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    ErrorLog.LogError(sqlEx, "GetPatientProviders(string scope)", "DocumentFaxing");
                }
                return ds;
            }
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetProviders_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(ds);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetPatientProviders(string scope)", "DocumentFaxing");
            }
            return ds;
        }

        internal DataTable CreateBlankProviderTable()
        {
            var dt = new DataTable();
            string sqlstmt =
                "Create Table #temp(GUID numeric(16,0),DisplayName Varchar(80),IDCode Varchar(30),Discipline Varchar(30),Type varchar(30),TypeCode varchar(30),FaxNumber varchar(40),Email varchar(100)) Select * from #temp drop table #temp";


            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            using (var command = new SqlCommand(sqlstmt, sqlConn))
            using (var dataAdapter = new SqlDataAdapter(command))
            {
                dataAdapter.Fill(dt);
            }

            return dt;
        }

        internal DataTable GetFaxableDocuments(String scope)
        {
            var ds = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_FaxTab_GetFaxableDocuments_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@ClientVisitGUID", SqlDbType.BigInt).Value =
                            CustContext.VisitGUID;
                        da.SelectCommand.Parameters.Add("@ClientGUID", SqlDbType.BigInt).Value =
                            CustContext.ClientGUID;
                        da.SelectCommand.Parameters.Add("@ChartGUID", SqlDbType.BigInt).Value =
                            CustContext.ChartGUID;
                        da.SelectCommand.Parameters.Add("@Scope", SqlDbType.VarChar).Value = scope;
                        da.Fill(ds);
                    }
                }

            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetFaxableDocuments(String scope)", "DocumentFaxing");
            }
            return ds;
        }

        internal int SetScheduledMLM(String documentGUID)
        {
            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn; // <== lacking
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UH_Scheduled_MLM_Delay_Ins_PR";
                    command.Parameters.AddWithValue("@clientguid", CustContext.ClientGUID);
                    command.Parameters.AddWithValue("@EvokeGUID", documentGUID);
                    command.Parameters.AddWithValue("@userguid", CustContext.UserGUID);
                    command.Parameters.AddWithValue("@EvokingTableName", "ClientDocument");
                    command.Parameters.AddWithValue("@MLMName", "UH_FaxEmail_Delayed");
                    command.Parameters.AddWithValue("@EventType", "ClientDocumentEnter");
                    command.Parameters.AddWithValue("@DelayAmount", 1);
                    command.Parameters.AddWithValue("@DelayUOM", "Minute");

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show(sqlEx.Message);
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return 0;
        }

        public string GetDocumentName(Int64 documentGUID)
        {
            using (var results = new DataTable())
            {
                string sqlstmt = "select Name from CV3PatientCareDocument where guid =" + documentGUID +
                                "and Active = 1 ";

                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                using (var command = new SqlCommand(sqlstmt, sqlConn))
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(results);
                    if (results.Rows.Count > 0)
                    {
                        return results.Rows[0]["Name"].ToString();
                    }
                    return "";
                }
            }
        }

        internal Boolean IsValidDoc(Int64 documentGUID)
        {
            using (var results = new DataTable())
            {
                string sqlstmt = "select IsValidDoc = Convert(Bit,1) from ACS_AF3_ValidDocuments where documentname = '" + GetDocumentName(documentGUID) + "'";

                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                using (var command = new SqlCommand(sqlstmt, sqlConn))
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(results);
                    if (results.Rows.Count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }

        }

        internal int AddCCRow(string clientDocumentGUID, string sendingUserGUID, string recipentGUID, string recipeintName, string actionToTake, string fax, string email)
        {
            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                //TODO: need to figure out if it is email or shm
                actionToTake = fax != "" ? "FAX" : "SHM";
                if (fax == "") fax = "NO FAX NUMBER CONFIGURED";
                if (email == "") email = "NO SHM ADDRESS";

                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn;            // <== lacking
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO ACS_AF3_AutoCCPhysicianList(ClientVisitGUID,ClientDocumentGUID,SendingUserGUID,UserGUID,Type,RecipientName,FaxNumber,Email,GUID)"
                                            + " VALUES (@ClientVisitGUID, @ClientDocumentGUID, @SendingUserGUID, @UserGUID, @Type, @RecipientName, @FaxNumber, @Email, NEWID())";
                    command.Parameters.AddWithValue("@ClientVisitGUID", CustContext.VisitGUID);
                    command.Parameters.AddWithValue("@ClientDocumentGUID", clientDocumentGUID);
                    command.Parameters.AddWithValue("@SendingUserGUID", sendingUserGUID);
                    command.Parameters.AddWithValue("@UserGUID", recipentGUID);
                    command.Parameters.AddWithValue("@Type", actionToTake);
                    command.Parameters.AddWithValue("@RecipientName", recipeintName);
                    command.Parameters.AddWithValue("@FaxNumber", fax);
                    command.Parameters.AddWithValue("@Email", email);

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx,
                            "AddCCRow(string clientDocumentGUID, string sendingUserGUID, string recipentGUID, string recipeintName, string actionToTake, string fax, string email)",
                            "DocumentFaxing");
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                }
            }
            return 0;
        }

        internal int UpdateLog(string sessionId, string appname, string documentguid, string classname, string message, string stacktrace, int iserror)
        {
            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn;            // <== lacking
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO [dbo].[ACS_AF3_ErrorLog] ([SessionID],[CreatedWhen],[AppName],[UserGUID],[DocumentGUID],[ClassName],[Message],[StackTrace],[IsError],[GUID])"
                        + " VALUES (@autofax_SessionID, GetDate(), @DocumentFaxing,@UserGUID, @ClientDocumentGUID, @ClassName,@Message, @StackTrace, @IsError, @GUID) ";

                    command.Parameters.AddWithValue("@autofax_SessionID", sessionId);
                    command.Parameters.AddWithValue("@DocumentFaxing", appname);
                    command.Parameters.AddWithValue("@UserGUID", CustContext.UserGUID);
                    command.Parameters.AddWithValue("@ClientDocumentGUID", documentguid);
                    command.Parameters.AddWithValue("@ClassName", classname);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@StackTrace", stacktrace);
                    command.Parameters.AddWithValue("@IsError", iserror);
                    command.Parameters.AddWithValue("@GUID", Guid.NewGuid().ToString());
                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx,
                            "UpdateLog(string sessionId, string appname, string documentguid, string classname, string message, string stacktrace, int iserror)",
                            "DocumentFaxing");
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return 0;
        }

        internal int ClearCurrentFaxDocument(string documentGUID)
        {
            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConn;            // <== lacking
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "Delete from ACS_AF3_AutoCCPhysicianList where ClientVisitGUID = @ClientVisitGUID AND ClientDocumentGUID = @ClientDocumentGUID ";
                    command.Parameters.AddWithValue("@ClientVisitGUID", CustContext.VisitGUID);
                    command.Parameters.AddWithValue("@ClientDocumentGUID", documentGUID);
                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx, "ClearCurrentFaxDocument(string documentGUID)", "DocumentFaxing");
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return 0;
        }

        internal String GetNewUserGUID()
        {
            var random = new Random();
            return random.Next(1, 1101190).ToString();
        }
    }
}
