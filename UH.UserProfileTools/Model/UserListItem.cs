using System;

namespace UH.UserProfileTools
{
    public class UserListItem
    {
        public UserListItem()
        {

        }

        private decimal _ProviderGUID;
        private String _DisplayName;
        private string _PrimarySpecialty;
        private string _TypeCode;
        private bool _IsSelected;

        public decimal ProviderGUID { get => _ProviderGUID; set => _ProviderGUID = value; }
        public String DisplayName { get => _DisplayName; set => _DisplayName = value; }
        public String PrimarySpecialty { get => _PrimarySpecialty; set => _PrimarySpecialty = value; }

        public String TypeCode { get => _TypeCode; set => _TypeCode = value; }
        public bool IsSelected { get => _IsSelected; set => _IsSelected = value; }

    }
}
