namespace UH.UserProfileTools
{
    public class ObservableUserListItem : BaseModelWrapper<UserListItem>
    {

        public ObservableUserListItem(UserListItem model) : base(model)
        {
        }

        public decimal ProviderGUID
        {
            get => GetValue<decimal>(); set { SetValue(value); }
        }
        public string ProviderGUIDDOriginalValue => GetOriginalValue<string>(nameof(ProviderGUID));
        public bool ProviderGUIDIsChanged => GetIsChanged(nameof(ProviderGUID));

        public string DisplayName
        {
            get => GetValue<string>(); set { SetValue(value); }
        }
        public string DisplayNameOriginalValue => GetOriginalValue<string>(nameof(DisplayName));
        public bool DisplayNameIsChanged => GetIsChanged(nameof(DisplayName));


        public string PrimarySpecialty
        {
            get => GetValue<string>(); set { SetValue(value); }
        }
        public string PrimarySpecialtyOriginalValue => GetOriginalValue<string>(nameof(PrimarySpecialty));
        public bool PrimarySpecialtyIsChanged => GetIsChanged(nameof(PrimarySpecialty));

        public string TypeCode
        {
            get => GetValue<string>(); set { SetValue(value); }
        }
        public string TypeCodeOriginalValue => GetOriginalValue<string>(nameof(PrimarySpecialty));
        public bool TypeCodeIsChanged => GetIsChanged(nameof(PrimarySpecialty));

        public bool IsSelected
        {
            get => GetValue<bool>();
            set { SetValue(value); }

        }
        public bool IsSelectedOriginalValue => GetOriginalValue<bool>(nameof(IsSelected));
        public bool IsSelectedIsChanged => GetIsChanged(nameof(IsSelected));
    }
}
