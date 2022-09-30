using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace UH.EpicCutoverTab
{
    public class DataAccessSQL
    {
        #region Contructors
        public DataAccessSQL()
        {
            CustContext = CustomContextObj.GetInstance();
        }
        #endregion

        #region Properties
        public CustomContextObj CustContext;
        #endregion

        #region Public Methods
        public DataTable GetSettings()
        {
            var dataTable = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_Tools_Settings_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@UserGUID", SqlDbType.BigInt).Value = CustContext.UserGUID;
                        da.Fill(dataTable);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogAndRaiseError(sqlEx, sqlEx.Message, "GetSettings()", "UH.UserProfileTools");
            }
            return dataTable;
        }
        public DataTable GetProvider()
        {
            return GetProvider(CustContext.UserGUID);
        }
        public DataTable GetProvider(string careProviderGUID)
        {
            var results = new DataTable();

            string sqlstmt = "select Top 1 CareProviderGUID = GUID, ProviderName = DisplayName from CV3CareProvider where GUID = " + careProviderGUID;
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var command = new SqlCommand(sqlstmt, sqlConn))
                    {
                        using (var dataAdapter = new SqlDataAdapter(command))
                        {
                            dataAdapter.Fill(results);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetUserSpecialties()", "UH.UserProfileTools");
                throw;
            }
            return results;
        }
        public DataTable GetProviderSpecialties(string careProviderGUID)
        {
            var results = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_Specialies_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@CareProviderGUID", SqlDbType.BigInt).Value = careProviderGUID;
                        da.Fill(results);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "UH_GetUserSpecialties_Sel_Pr(careProviderGUID)", "UH.UserProfileTools");
            }
            return results;
        }
        public DataTable GetCommunicationPref(string careProviderGUID)
        {
            var results = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_CommPref_Sel_PR", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@CareProviderGUID", SqlDbType.BigInt).Value = Convert.ToInt64(careProviderGUID);
                        da.Fill(results);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetCommunicationPref(careProviderGUID)", "UH.UserProfileTools");
                throw;
            }
            return results;
        }
        public DataTable GetAvailSpecialties()
        {
            var results = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_AvailableSpecialties_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(results);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetAvailSpecialties()", "UH.UserProfileTools");
                throw;
            }
            return results;
        }
        public int SaveSpecialty(DataTable selectedSpecialties, string careProviderGUID)
        {
            //Get a fresh datatable of the current provider specialties
            DataTable tempSpecialties = GetProviderSpecialties(careProviderGUID);

            //Create two tables from the one.
            //First get all of the GUIDs in the current database rows that need to be marked as deleted
            var rowsToDelete = new DataTable();
            if (tempSpecialties.Rows.Count > 0)
            {
                if (tempSpecialties.Rows
                    .OfType<DataRow>().Any(a => tempSpecialties.Rows.OfType<DataRow>()
                        .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                        .Except(
                            selectedSpecialties.Rows.OfType<DataRow>()
                                .Select(k => Convert.ToInt64(k["SpecialtyGUID"])).DefaultIfEmpty()
                                .ToList())
                        .Contains(Convert.ToInt64(a["SpecialtyGUID"]))))
                {
                    rowsToDelete = tempSpecialties.Rows.OfType<DataRow>()
                        .Where(
                            a =>
                                tempSpecialties.Rows.OfType<DataRow>()
                                    .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                                    .Except(
                                        selectedSpecialties.Rows.OfType<DataRow>()
                                            .Select(k => Convert.ToInt64(k["SpecialtyGUID"])).DefaultIfEmpty()
                                            .ToList())
                                    .Contains(Convert.ToInt64(a["SpecialtyGUID"])))
                        .CopyToDataTable();
                }
            }

            var rowsToAdd = new DataTable();
            if (selectedSpecialties.Rows.OfType<DataRow>()
                .Where(
                    a =>
                        selectedSpecialties.Rows.OfType<DataRow>()
                            .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                            .Except(
                                tempSpecialties.Rows.OfType<DataRow>()
                                    .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                                    .ToList())
                            .Contains(Convert.ToInt64(a["SpecialtyGUID"]))).Any())
            {
                rowsToAdd = selectedSpecialties.Rows.OfType<DataRow>()
                    .Where(a => selectedSpecialties.Rows.OfType<DataRow>()
                        .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                        .Except(
                            tempSpecialties.Rows.OfType<DataRow>()
                                .Select(k => Convert.ToInt64(k["SpecialtyGUID"]))
                                .ToList())
                        .Contains(Convert.ToInt64(a["SpecialtyGUID"])))
                    .CopyToDataTable();
            }

            if (rowsToDelete.Rows.Count > 0)
            {
                foreach (DataRow r in rowsToDelete.Rows)
                {
                    using (var sqlConn = HVCLogonObj.GetSqlConnection())
                    {
                        using (var command = new SqlCommand())
                        {
                            command.Connection = sqlConn; // <== lacking
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "UH_UserProfile_Specialties_Del_PR";
                            command.Parameters.AddWithValue("@CareProviderGUID", careProviderGUID);
                            command.Parameters.AddWithValue("@SpecialtyGUID", r["SpecialtyGUID"]);
                            command.Parameters.AddWithValue("@UserGUID", CustContext.UserGUID);
                            try
                            {
                                sqlConn.Open();
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException sqlEx)
                            {
                                ErrorLog.LogError(sqlEx, "SaveSpecialty()", "UH.UserProfileTools");
                            }
                            finally
                            {
                                sqlConn.Close();
                            }
                        }
                    }
                }
            }


            if (rowsToAdd.Rows.Count > 0)
            {
                foreach (DataRow r in rowsToAdd.Rows)
                {

                    try
                    {
                        using (var sqlConn = HVCLogonObj.GetSqlConnection())
                        {
                            using (var command = new SqlCommand())
                            {
                                command.Connection = sqlConn; // <== lacking
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "UH_UserProfile_Specialties_Ins_PR";
                                command.Parameters.AddWithValue("@CareProviderGUID", careProviderGUID);
                                command.Parameters.AddWithValue("@SpecialtyGUID", r["SpecialtyGUID"]);
                                //   command.Parameters.AddWithValue("@isPrimary", r["IsPrimary"]);
                                command.Parameters.AddWithValue("@UserGUID", CustContext.UserGUID);

                                sqlConn.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx, "SaveSpecialty()", "UH.UserProfileTools");
                    }
                }
            }
            return 0;
        }
        public int SaveCommunicationPref(string careProviderGUID, string prefWritten, string prefTele, string docHaloID, string pagerNumber, string email, string fax)
        {
            using (var sqlConn = HVCLogonObj.GetSqlConnection())
            {
                using (var command = new SqlCommand())
                {
                    var pagerAreaCode = pagerNumber.Length > 3 ? pagerNumber.Substring(0, 3) : "";

                    var faxAreaCode = fax.Length > 3 ? fax.Substring(0, 3) : "";


                    command.Connection = sqlConn; // <== lacking
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UH_UserProfile_CommPref_Ins_PR";
                    command.Parameters.AddWithValue("@CareProviderGUID", careProviderGUID);
                    command.Parameters.AddWithValue("@WrittenPref", prefWritten);
                    command.Parameters.AddWithValue("@TelePref", prefTele);
                    command.Parameters.AddWithValue("@PagerAreaCode", pagerAreaCode);
                    command.Parameters.AddWithValue("@PagerNumber", pagerNumber);
                    command.Parameters.AddWithValue("@DocHaloID", docHaloID);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@faxAreaCode", faxAreaCode);
                    command.Parameters.AddWithValue("@FaxNum", fax);
                    command.Parameters.AddWithValue("@UserGUID", CustContext.UserGUID);

                    try
                    {
                        sqlConn.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException sqlEx)
                    {
                        ErrorLog.LogError(sqlEx, "SaveCommunicationPref()", "UH.UserProfileTools");
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
            }
            return 0;
        }
        public DataTable GetProviderList()
        {
            var results = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_ProviderList_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(results);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetProviderTable", "UH.UserProfileTools");
            }
            return results;
        }
        internal DataTable GetProviderList(string filter)
        {
            var results = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_UserProfile_ProviderList_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.AddWithValue("@SearchString", filter);
                        da.Fill(results);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetProviderTable", "UH.UserProfileTools");
            }
            return results;
        }
        #endregion
    }
}
