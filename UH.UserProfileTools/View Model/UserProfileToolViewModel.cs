using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace UH.UserProfileTools
{
    public class UserProfileToolViewModel : Observable
    {
        #region Constructors
        public UserProfileToolViewModel()
        {
            _Access = new DataAccessSQL();
            _Settings = new ProfileToolSettings();
            InitiateCommands();
            SetProvider();
            SetAvailableSpecialties();
            CreateCollectionViews();
        }

        #endregion

        #region Private members
        private ProfileToolSettings _Settings;
        private DataAccessSQL _Access;
        private ObservableProvider _Provider;
        private ICollectionView _AvailableSpecialtiesCollectionView;
        private ICollectionView _ProviderSpecialtiesCollectionView;


        #endregion

        #region Properties
        public ChangeTrackingCollection<ObservableSpecialty> AvailableSpecialties { get; private set; }

        public ObservableProvider CurrentProvider
        {
            get { return _Provider; }
            private set
            {
                _Provider = value;
                OnPropertyChanged();
            }
        }

        #region Collection Views
        public ICollectionView AvailableSpecialtiesCollectionView { get { return _AvailableSpecialtiesCollectionView; } }
        public ICollectionView ProviderSpecialtiesCollectionView { get { return _ProviderSpecialtiesCollectionView; } }

        #endregion
        #endregion

        #region commands
        public RelayCommand MoveOneCommand { get; set; }
        public RelayCommand MoveAllCommand { get; set; }
        public RelayCommand RemoveOneCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }

        public RelayCommand SearchForPatientCommand { get; set; }
        public RelayCommand SaveChangesCommand { get; set; }
        public RelayCommand CancelAndCloseCommand { get; set; }
        #endregion

        #region Private Methods
        private void SetProvider()
        {
            _Provider = new ObservableProvider(new Provider(_Access.CustContext.UserGUID));
            _Provider.PropertyChanged += (s, e_) =>
            {
                if (e_.PropertyName == nameof(_Provider.IsChanged) || e_.PropertyName == nameof(_Provider.IsValid))
                {
                    InvalidateCommands();
                }
            };
            InvalidateCommands();
        }

        //private void SetProvider()
        //{
        //    CurrentProvider = new ObservableProvider(new Provider(_Access.CustContext.UserGUID));
        //    CurrentProvider.PropertyChanged += (s, e) =>
        //    {
        //        if (e.PropertyName == nameof(CurrentProvider.IsChanged) || e.PropertyName == nameof(CurrentProvider.IsValid))
        //        {
        //            InvalidateCommands();
        //        }
        //    };
        //    InvalidateCommands();
        //}

        private void SetAvailableSpecialties()
        {
            //get available specialties.
            var _aSpecialties = TableToObjectConverter.ConvertDataTable<Specialty>(_Access.GetAvailSpecialties());
            AvailableSpecialties = new ChangeTrackingCollection<ObservableSpecialty>(_aSpecialties.Select(e => new ObservableSpecialty(e)));
            foreach (object item in AvailableSpecialties)
            {
                if (item is INotifyPropertyChanged)
                {
                    INotifyPropertyChanged observ = (INotifyPropertyChanged)item;
                    observ.PropertyChanged += new PropertyChangedEventHandler(ItemPropertyChanged);
                }
            }

        }
        private void CreateCollectionViews()
        {
            _AvailableSpecialtiesCollectionView = (CollectionView)new CollectionViewSource { Source = AvailableSpecialties }.View;
            _AvailableSpecialtiesCollectionView.Filter = AvailSpecialtyFilter;
        }
        private bool AvailSpecialtyFilter(Object item)
        {
            try
            {
                ObservableSpecialty Special = item as ObservableSpecialty;
                return (Special.Code != null);
            }
            catch (Exception Ex)
            {
                ErrorLog.LogAndRaiseError(Ex, "error encountered in creating Available specialty list", "AvailSpecialtyFilter", "UH.UserProfileTools");
                throw;
            }
        }
        private void RefreshLists()
        {
            //AvailableSpecialtiesCollectionView.Refresh();
            //ProviderSpecialtiescCollectionView.Refresh();
        }

        private void InitiateCommands()
        {
            MoveOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
            SearchForPatientCommand = new RelayCommand(OnSearch, CanSearch);
            SaveChangesCommand = new RelayCommand(OnSave, CanSave);
            CancelAndCloseCommand = new RelayCommand(OnCancel, CanCancel);

            InvalidateCommands();
        }
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateCommands();
        }
        private void InvalidateCommands()
        {
            ((RelayCommand)MoveOneCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RemoveOneCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RemoveAllCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SearchForPatientCommand).RaiseCanExecuteChanged();

            ((RelayCommand)SaveChangesCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CancelAndCloseCommand).RaiseCanExecuteChanged();
        }

        #region implimenting commands
        private void OnAddOne(object obj)
        {
            //read the Available Specialties and find the selected one
            foreach (ObservableSpecialty item in AvailableSpecialties)
            {
                List<ObservableSpecialty> newList = new List<ObservableSpecialty>();
                if (item.IsSelected)
                {
                    //make sure the specialty isn't in the profile already
                    if (!_Provider.Specialties.Any(p => item.SpecialtyGUID == p.SpecialtyGUID))
                    {
                        var newspecialty = new ObservableSpecialty(item.Model);
                        _Provider.Specialties.Add(newspecialty);
                    }
                }
                item.IsSelected = false;
            }
            RefreshLists();
        }
        private bool CanAddOne(object obj)
        {
            return AvailableSpecialties.Any(p => (p.IsSelected));
        }

        private void OnRemoveOne(object obj)
        {
            List<ObservableSpecialty> RemoveList = new List<ObservableSpecialty>();
            //read the patient Specialties and find the selected one to remove
            foreach (ObservableSpecialty item in _Provider.Specialties)
            {
                if (item.IsSelected)
                {
                    RemoveList.Add(item);
                    item.IsSelected = false;
                }
            }

            foreach (ObservableSpecialty item in RemoveList)
            {
                _Provider.Specialties.Remove(item);
            }
            RefreshLists();
        }
        private bool CanRemoveOne(object obj)
        {
            return _Provider.Specialties.Any(p => (p.IsSelected));
        }

        private void OnRemoveAll(object obj)
        {
            _Provider.Specialties.Clear();
        }
        private bool CanRemoveAll(object obj)
        {
            return true;
        }

        private void OnSearch(object obj)
        {
            var a = obj as Window;
            ProviderSearchView findprovider = new ProviderSearchView();
            var findvModel = new ProviderSearchViewModel();
            findprovider.DataContext = findvModel;
            findprovider.Owner = a;
            findprovider.ShowDialog();
            var sele = findvModel.Users.FirstOrDefault(p => p.IsSelected == true);
            if (sele == null)
                return;

            var n_Provider = new Provider(sele.ProviderGUID.ToString());
            CurrentProvider.ProviderName = n_Provider.ProviderName;
            CurrentProvider.CareProviderGUID = n_Provider.CareProviderGUID;
            CurrentProvider.DocHaloID = n_Provider.DocHaloID;
            CurrentProvider.Email = n_Provider.Email;
            CurrentProvider.FaxNumber = n_Provider.FaxNumber;
            CurrentProvider.PagerNumber = n_Provider.PagerNumber;
            CurrentProvider.WrittenPreference = n_Provider.WrittenPreference;
            CurrentProvider.TelecomPreference = n_Provider.TelecomPreference;
            CurrentProvider.Specialties.Clear();
            //CurrentProvider.Specialties - n_Provider.Specialties;
            foreach (var item in n_Provider.Specialties)
            {
                CurrentProvider.Specialties.Add(new ObservableSpecialty(item));
            }

            //_Provider = new ObservableProvider(new Provider(sele.ProviderGUID.ToString()));
            //this.ItemPropertyChanged(this, new PropertyChangedEventArgs("CurrentPovider"));

        }

        private bool CanSearch(object obj)
        {
            return _Settings.CanChangeProviders;
        }

        private void OnSave(object obj)
        {
            CurrentProvider.Save();
            Window wi = obj as Window;
            wi.Close();
            //(Window) obj;
        }
        private bool CanSave(object obj)
        {
            return CurrentProvider.IsChanged && CurrentProvider.IsValid;
        }

        private void OnCancel(object obj)
        {
            Window wi = obj as Window;
            wi.Close();
        }
        private bool CanCancel(object obj)
        {
            return true;
        }
        #endregion

        #endregion
    }
}
