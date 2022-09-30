using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace UH.UserProfileTools
{
    public class ProviderSearchViewModel : Observable
    {
        #region Constructors
        public ProviderSearchViewModel()
        {
            _Access = new DataAccessSQL();
            InitiateCommands();
            SetProviderList();
            CreateCollectionViews();
        }

        #endregion

        #region Private Fields
        private DataAccessSQL _Access;
        private ICollectionView _ProviderCollectionView;
        private string _Filter;

        #endregion

        #region Properties
        public ChangeTrackingCollection<ObservableUserListItem> Users;

        public ICollectionView ProviderCollectionView { get { return _ProviderCollectionView; } }

        public String Filter
        {
            get { return _Filter; }
            set
            {
                _Filter = value;
                InvalidateCommands();
            }
        }

        public RelayCommand SaveChangesCommand { get; set; }
        public RelayCommand CancelAndCloseCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        #endregion

        #region Public  Methods



        #endregion

        #region Private Methods

        private void SetProviderList()
        {
            //get available specialties.
            var userListItems = TableToObjectConverter.ConvertDataTable<UserListItem>(_Access.GetProviderList());
            Users = new ChangeTrackingCollection<ObservableUserListItem>(userListItems.Select(e => new ObservableUserListItem(e)));
        }
        private void InitiateCommands()
        {
            SaveChangesCommand = new RelayCommand(OnSave, CanSave);
            CancelAndCloseCommand = new RelayCommand(OnCancel, CanCancel);
            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            InvalidateCommands();
        }
        private void InvalidateCommands()
        {
            ((RelayCommand)CancelAndCloseCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SaveChangesCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SearchCommand).RaiseCanExecuteChanged();
        }
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateCommands();
        }
        private void CreateCollectionViews()
        {
            _ProviderCollectionView = (CollectionView)new CollectionViewSource { Source = Users }.View;
            _ProviderCollectionView.Filter = ProviderFilter;
        }
        private bool ProviderFilter(Object item)
        {
            try
            {
                ObservableUserListItem Special = item as ObservableUserListItem;
                if (!String.IsNullOrEmpty(_Filter))
                {
                    return (Special.DisplayName != null && (Special.DisplayName.ToUpper().Contains(_Filter.ToUpper()))
                        || (Special.PrimarySpecialty != null && Special.PrimarySpecialty.ToUpper().Contains(_Filter.ToUpper())));
                }
                return (Special.DisplayName != null);
            }
            catch (Exception Ex)
            {
                ErrorLog.LogAndRaiseError(Ex, "error encountered in creating Provider Selection list", "ProviderFilter", "UH.UserProfileTools");
                throw;
            }
        }
        #endregion

        #region Command Implimentation
        private void OnSave(object obj)
        {
            Window wi = obj as Window;
            wi.Close();
            //(Window) obj;
        }
        private bool CanSave(object obj)
        {
            return true;
        }

        private void OnCancel(object obj)
        {
            Users.RejectChanges();
            Window wi = obj as Window;
            wi.Close();
        }
        private bool CanCancel(object obj)
        {
            return true;
        }

        private void OnSearch(object obj)
        {
            //Clear current Observable User list.
            Users.Clear();
            //Get new User List using filter if more than 3 letters are present.
            if (!String.IsNullOrEmpty(_Filter))
            {
                //get available specialties.
                var userListItems = TableToObjectConverter.ConvertDataTable<UserListItem>(_Access.GetProviderList(_Filter));
                foreach (var item in userListItems)
                {
                    var li = new ObservableUserListItem(item);
                    Users.Add(li);
                }
                //Users = new ChangeTrackingCollection<ObservableUserListItem>(userListItems.Select(e => new ObservableUserListItem(e)));
            }
            ProviderCollectionView.Refresh();
        }
        private bool CanSearch(object obj)
        {
            return !String.IsNullOrEmpty(_Filter) && _Filter.Length > 1;
            //Check to see if the filter has anything in it.
        }
        #endregion



    }
}
