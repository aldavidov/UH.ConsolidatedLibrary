using System;

namespace UH.UserProfileTools
{
    public class ProfileToolSettings
    {
        #region Constructors

        public ProfileToolSettings()
        {
            var access = new DataAccessSQL();
            UserGUID = access.CustContext.UserGUID;
            var settingsTable = access.GetSettings();
            try
            {
                if (settingsTable.Rows.Count > 0)
                {
                    //Populate the setting for this tool
                    CanChangeProviders = Convert.ToBoolean((settingsTable.Rows[0]["CanChangeProvider"]));
                    CanExport = Convert.ToBoolean(settingsTable.Rows[0]["CanExport"]);
                }

            }
            catch (Exception Ex)
            {
                ErrorLog.LogAndRaiseError(Ex, "error encountered in ProfileToolSettings", "ProfileToolSettings",
                    "UH_ProblemManager.Core");
                Console.WriteLine(Ex);
                throw;
            }
        }

        #endregion

        #region Public Properties

        public Boolean CanExport { get; } = false;
        public Boolean CanChangeProviders { get; } = false;
        public String UserGUID { get; }

        #endregion
    }
}
