using System;

namespace UH.UserProfileTools
{
    public class ObservableSpecialty : BaseModelWrapper<Specialty>
    {
        public ObservableSpecialty(Specialty model) : base(model)
        {
        }

        public decimal SpecialtyGUID
        {
            get => GetValue<decimal>(); set { SetValue(value); }
        }
        public string SpecialtyGUIDOriginalValue => GetOriginalValue<string>(nameof(SpecialtyGUID));
        public bool SpecialtyGUIDIsChanged => GetIsChanged(nameof(SpecialtyGUID));

        public string Code
        {
            get => GetValue<String>(); set { SetValue(value); }
        }
        public string CodeOriginalValue => GetOriginalValue<string>(nameof(Code));
        public bool CodeIsChanged => GetIsChanged(nameof(Code));

        public bool IsSelected
        {
            get => GetValue<bool>();
            set { SetValue(value); }

        }
        public bool IsSelectedOriginalValue => GetOriginalValue<bool>(nameof(IsSelected));
        public bool IsSelectedIsChanged => GetIsChanged(nameof(IsSelected));

    }
}
