using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace UH.UserProfileTools
{
    public class ObservableProvider : BaseModelWrapper<Provider>
    {
        public ObservableProvider(Provider model) : base(model)
        {
        }

        #region Properties

        public String CareProviderGUID
        {
            get => GetValue<String>();
            internal set { SetValue(value); }
        }
        public String ProviderName
        {
            get => GetValue<String>();
            internal set { SetValue(value); }
        }
        public String ProviderNameOriginalValue => GetOriginalValue<String>(nameof(ProviderName));
        public bool ProviderNameIsChanged => GetIsChanged(nameof(ProviderName));

        public String DocHaloID
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String DocHaloIDOriginalValue => GetOriginalValue<String>(nameof(DocHaloID));
        public bool DocHaloIDIsChanged => GetIsChanged(nameof(DocHaloID));

        public String PagerNumber
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String PagerNumberOriginalValue => GetOriginalValue<String>(nameof(PagerNumber));
        public bool PagerNumberIsChanged => GetIsChanged(nameof(PagerNumber));

        // [EmailAddress(ErrorMessage = "Email is not a valid email address")]
        public String Email
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String EmailOriginalValue => GetOriginalValue<String>(nameof(Email));
        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public String FaxNumber
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String FaxNumberOriginalValue => GetOriginalValue<String>(nameof(FaxNumber));
        public bool FaxNumberIsChanged => GetIsChanged(nameof(FaxNumber));

        public String WrittenPreference
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String WrittenPreferenceOriginalValue => GetOriginalValue<String>(nameof(WrittenPreference));
        public bool WrittenPreferenceIsChanged => GetIsChanged(nameof(WrittenPreference));

        public String TelecomPreference
        {
            get => GetValue<String>();
            set { SetValue(value); }
        }
        public String TelecomPreferenceOriginalValue => GetOriginalValue<String>(nameof(TelecomPreference));
        public bool TelecomPreferenceIsChanged => GetIsChanged(nameof(TelecomPreference));

        public ChangeTrackingCollection<ObservableSpecialty> Specialties { get; private set; }
        #endregion

        protected override void InitializeCollectionProperties(Provider model)
        {
            if (model.Specialties == null)
            {
                throw new ArgumentException("Provider Specialties cannot be empty");
            }
            Specialties = new ChangeTrackingCollection<ObservableSpecialty>(model.Specialties.Select(e => new ObservableSpecialty(e)));
            RegisterCollection(Specialties, model.Specialties);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validateContext)
        {
            //Validate that email is a valid email with @ symbol and dot symbol
            if (Email != "" && WrittenPreference == "Email")
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                if (!regex.IsMatch(Email))
                {
                    yield return new ValidationResult("Email is not a valid email address", new[] { nameof(Email) });
                }
            }

            if (WrittenPreference == "Email" && Email == "")
            {
                yield return new ValidationResult("When Email is preferred communication, the email address cannot be blank",
                    new[] { nameof(Email) });
            }
            if (WrittenPreference == "Fax" && FaxNumber == "")
            {
                yield return new ValidationResult("When Email is preferred communication, the email address cannot be blank",
                    new[] { nameof(FaxNumber) });
            }
            if (TelecomPreference == "DocHalo" && DocHaloID == "")
            {
                yield return new ValidationResult("When DocHalo is preferred communication, the email address cannot be blank",
                    new[] { nameof(DocHaloID) });
            }

            if (TelecomPreference == "Pager" && PagerNumber == "")
            {
                yield return new ValidationResult(
                    "When Pager is preferred communication, the email address cannot be blank",
                    new[] { nameof(PagerNumber) });
            }

            //if (WrittenPreference == "Email" && Email == "")
            //{
            //    yield return new ValidationResult("Email is not a valid email address",
            //        new[] { nameof(Email) });
            //}
        }

        public void Save()
        {
            Model.Save();
        }
    }
}
