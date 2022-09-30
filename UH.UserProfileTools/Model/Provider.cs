using System;
using System.Collections.Generic;
using System.Data;

namespace UH.UserProfileTools
{
    public class Provider
    {
        #region Constructors
        public Provider(String providerGUID)
        {
            _CareProviderGUID = providerGUID;
            _accessSql = new DataAccessSQL();
            LoadProviderDetails();
            LoadCommunicationPref();
            LoadProviderSpecialtiesList();
        }
        #endregion

        #region Private Fields
        private String _CareProviderGUID;
        private String _ProviderName;
        private String _DocHaloID;
        private String _PagerNumber;
        private String _Email;
        private String _FaxNumber;
        private String _WrittenPreference;
        private String _TelecomPreference;
        private DataAccessSQL _accessSql;
        #endregion

        #region Properties
        public String CareProviderGUID
        {
            get => _CareProviderGUID;
            internal set { _CareProviderGUID = value; }
        }
        public String ProviderName
        {
            get => _ProviderName;
            internal set { _ProviderName = value; }
        }
        public String DocHaloID
        {
            get => _DocHaloID;
            set { _DocHaloID = value; }
        }
        public String PagerNumber
        {
            get => _PagerNumber;
            set { _PagerNumber = value; }
        }
        public String Email
        {
            get => _Email;
            set { _Email = value; }
        }
        public String FaxNumber
        {
            get => _FaxNumber;
            set { _FaxNumber = value; }
        }
        public String WrittenPreference
        {
            get => _WrittenPreference;
            set { _WrittenPreference = value; }
        }
        public String TelecomPreference
        {
            get => _TelecomPreference;
            set { _TelecomPreference = value; }
        }
        public List<Specialty> Specialties;
        #endregion

        #region Private Methods
        private void LoadProviderDetails()
        {
            DataTable providerDetailTable = _accessSql.GetProvider(CareProviderGUID);
            try
            {
                if (providerDetailTable.Rows.Count > 0)
                {
                    _CareProviderGUID = Convert.ToString(providerDetailTable.Rows[0]["CareProviderGUID"]);
                    _ProviderName = Convert.ToString(providerDetailTable.Rows[0]["ProviderName"]);
                }
            }
            catch (Exception Ex)
            {
                ErrorLog.LogAndRaiseError(Ex, "error encountered in SetPatient", "LoadCommunicationPref()", "UH.UserProfileTools");
            }
        }
        private void LoadCommunicationPref()
        {
            DataTable providerDetailTable = _accessSql.GetCommunicationPref(CareProviderGUID);
            try
            {
                if (providerDetailTable.Rows.Count > 0)
                {
                    _DocHaloID = Convert.ToString(providerDetailTable.Rows[0]["DocHaloID"]);
                    _PagerNumber = Convert.ToString(providerDetailTable.Rows[0]["PagerNumber"]);
                    _Email = Convert.ToString(providerDetailTable.Rows[0]["EmailAddress"]);
                    _FaxNumber = Convert.ToString(providerDetailTable.Rows[0]["FaxNumber"]);
                    _WrittenPreference = Convert.ToString(providerDetailTable.Rows[0]["WrittenPreference"]);
                    _TelecomPreference = Convert.ToString(providerDetailTable.Rows[0]["TelecomPreference"]);
                }
            }
            catch (Exception Ex)
            {
                ErrorLog.LogAndRaiseError(Ex, "error encountered in Loading Communications", "LoadCommunicationPref()", "UH.UserProfileTools");
            }
        }
        private void LoadProviderSpecialtiesList()
        {
            Specialties = TableToObjectConverter.ConvertDataTable<Specialty>(_accessSql.GetProviderSpecialties(CareProviderGUID));
        }
        #endregion

        #region Internal Methods
        internal bool Save()
        {
            int a;
            a = _accessSql.SaveCommunicationPref(CareProviderGUID, WrittenPreference, TelecomPreference, DocHaloID, PagerNumber, Email, FaxNumber);

            DataTable specialties = new DataTable();
            if (Specialties.Count > 0)
            {
                specialties.Columns.Add("SpecialtyGUID", typeof(Decimal));
                specialties.Columns.Add("Code", typeof(String));

                //Add each specialty to the datatable.
                foreach (Specialty s in Specialties)
                {
                    DataRow row = specialties.NewRow();
                    row["SpecialtyGUID"] = s.SpecialtyGUID;
                    row["Code"] = s.Code;
                    specialties.Rows.Add(row);
                }
            }
            a = _accessSql.SaveSpecialty(specialties, CareProviderGUID);

            return true;
        }

        #endregion

    }
}
