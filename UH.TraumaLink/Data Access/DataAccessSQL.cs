using System;
using System.Data;
using System.Data.SqlClient;
using SCMLib.Context;
using SCMLib.HVCLogon;

namespace UH.TraumaLink
{
    public class DataAccessSQL
    {
        public CustomContextObj CustContext { get; set; }

        #region Contructors
        public DataAccessSQL()
        {
            CustContext = CustomContextObj.GetInstance();
        }

        #endregion

        #region Public Methods

        public DataTable GetSettings(String TableColumnGroup)
        {
            var resultsdata = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_TPL_GetColumns_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@ColumnGroup", SqlDbType.VarChar).Value = TableColumnGroup; // "TraumaPatientLink";
                        da.Fill(resultsdata);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogAndRaiseError(sqlEx, sqlEx.Message, "GetSettings()", "UH.TraumaPatientLink");
            }
            return resultsdata;

        }

        public DataTable GetLinkedTraumaPatients()
        {
            var resultsdata = new DataTable();
            try
            {
                using (var sqlConn = HVCLogonObj.GetSqlConnection())
                {
                    using (var da = new SqlDataAdapter("UH_TPL_ClientDetails_Sel_Pr", sqlConn))
                    {
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand.Parameters.Add("@Client_GUID", SqlDbType.BigInt).Value = Convert.ToInt64(CustContext.ClientGUID);
                        da.SelectCommand.Parameters.Add("@Chart_GUID", SqlDbType.BigInt).Value = Convert.ToInt64(CustContext.ChartGUID);
                        da.SelectCommand.Parameters.Add("@Visit_GUID", SqlDbType.BigInt).Value = Convert.ToInt64(CustContext.VisitGUID);
                        da.Fill(resultsdata);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ErrorLog.LogError(sqlEx, "GetLinkedPatients(DuplicateMRN, LastName)", "UH.TraumaPatientLink");
            }
            return resultsdata;
        }

        #endregion

    }
}
