using System;

namespace UH.UserProfileTools
{
    public class Specialty
    {
        public Specialty()
        {
        }

        private Decimal _SpecialtyGUID;
        private String _Code;
        private bool _IsSelected = false;

        public Decimal SpecialtyGUID
        {
            get => _SpecialtyGUID;
            set => _SpecialtyGUID = value;
        }
        public String Code
        {
            get => _Code;
            set => _Code = value;
        }

        public bool IsSelected { get => _IsSelected; set => _IsSelected = value; }
    }
}
