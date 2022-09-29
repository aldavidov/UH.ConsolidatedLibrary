using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SCMLib;
using SCMLib.Context;
using SCMLib.CustomApp;
using SCMLib.PObj;

namespace UH.TraumaLink
{
    /// <summary>
    /// Interaction logic for TraumaLinkView.xaml
    /// </summary>

    [CLRApp("Trauma Link", CustomAppType.CustomApp)]
    public partial class TraumaLinkView : Window, ICustomApp
    {
        private DataAccessSQL _access;

        public CustomAppEventSource _customAppEventSource { get; private set; }
        public CustomContextObj contextinstance;

        private readonly ColumnList _LinkedPatientColumnList;
        private GridView _LinkedPatientGrid;
        private DataTable _linkedPatientTable;

        public TraumaLinkView()
        {
            InitializeComponent();
            VersionLabel.Content = SetVersionString();
            _access = new DataAccessSQL();

            contextinstance = CustomContextObj.GetInstance();
            if (string.IsNullOrWhiteSpace(contextinstance.GetCurrentClientVisitGUID()))
            {
                return;
            }


            //Set Patient Name in dialog.
            ClientVisitPObj currentvisit = ClientVisitPObj.GetCurrent();
            this.Title = currentvisit.ClientDisplayName + " " + currentvisit.IDCode + "/" + currentvisit.VisitIDCode;
            CurrentPtTextBlock.Text = "";

            //Get settings from config
            _LinkedPatientColumnList = new ColumnList();
            GetColumns("TraumaLink", _LinkedPatientColumnList);

            //Get linked patients from stored procedure
            _linkedPatientTable = new DataTable();
            LoadData();
            if (_linkedPatientTable.Rows.Count > 0)
            {
                LoadLinkedPatientListView();
            }
            else
            {
                MessageBox.Show("This patient has no active orders linking them to another patient.");
                return;
            }
        }

        private void GetColumns(string traumalink, ColumnList linkedPatientColumnList)
        {
            DataTable h = _access.GetSettings(traumalink);
            //Populate column list.
            foreach (DataRow row in h.Rows)
            {
                String name = Convert.ToString(row["Columnname"]);
                String headertext = Convert.ToString(row["ColumnHeader"]);

                int columnWidth;
                if (!int.TryParse(row["Columnwidth"].ToString(), out columnWidth))
                {
                    columnWidth = 0;
                }

                FilterType filter;
                if (!Enum.TryParse(Convert.ToString(row["ColumnFilter"]), true, out filter))
                {
                    filter = FilterType.None;
                }

                string filterLabel = Convert.ToString(row["FilterLabel"]);

                int filterLabelWidth;
                if (!int.TryParse(row["FilterLableWidth"].ToString(), out filterLabelWidth))
                {
                    filterLabelWidth = 0;
                }

                int filterControlWidth;
                if (!int.TryParse(row["FilterControlWidth"].ToString(), out filterControlWidth))
                {
                    filterControlWidth = 0;
                }

                int marginLeft;
                if (!int.TryParse(row["MarginLeft"].ToString(), out marginLeft))
                {
                    marginLeft = 0;
                }
                linkedPatientColumnList.Add(name, headertext, columnWidth, filter, filterLabel, filterLabelWidth, filterControlWidth,
                    marginLeft);
            }

        }

        private void LoadData()
        {
            try
            {
                _linkedPatientTable = new DataTable();
                _linkedPatientTable = _access.GetLinkedTraumaPatients();
                if (_linkedPatientTable.Rows.Count > 0)
                {
                    _linkedPatientTable.DefaultView.Sort = _LinkedPatientColumnList[0].Name;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogAndRaiseError(ex, "Error setting up grid", "LoadData()", "UH.TraumaLink");
            }
        }

        private void LoadLinkedPatientListView()
        {
            try
            {
                _LinkedPatientGrid = new GridView();
                foreach (Column c in _LinkedPatientColumnList)
                {
                    var gvc = new GridViewColumn { DisplayMemberBinding = new Binding(c.Name), Header = c.Header };
                    if (c.ColumnWidth > 0)
                    {
                        gvc.Width = c.ColumnWidth;

                    }

                    _LinkedPatientGrid.Columns.Add(gvc);
                }

                LinkedPatientsListView.View = _LinkedPatientGrid;
                LinkedPatientsListView.ItemsSource = _linkedPatientTable.DefaultView;

            }
            catch (Exception ex)
            {
                ErrorLog.LogAndRaiseError(ex, "Error setting up grid", "LoadLinkedPatientListView()", "UH.TraumaLink");
            }

        }

        private void listView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private object SetVersionString()
        {
            return "Version " +
                   Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(
                       CultureInfo.InvariantCulture) + "."
                   +
                   Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(
                       CultureInfo.InvariantCulture) + "."
                   +
                   Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(
                       CultureInfo.InvariantCulture) + "."
                   +
                   Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString(
                       CultureInfo.InvariantCulture);
        }

        #region Icustomapp methods
        public void InitEvents(CustomAppEventSource eventSource)
        {
            eventSource.OnUserChange += eventSource_OnUserChange;

            eventSource.OnCanLogoff += eventSource_OnCanLogoff;

            eventSource.OnApplicationSuspend += eventSource_OnApplicationSuspend;

            eventSource.OnApplicationTimedOut += eventSource_OnApplicationTimedOut;

            _customAppEventSource = eventSource;
        }

        private void UnsubscribeToEvents()
        {
            _customAppEventSource.OnUserChange -= eventSource_OnUserChange;

            _customAppEventSource.OnCanLogoff -= eventSource_OnCanLogoff;

            _customAppEventSource.OnApplicationSuspend -= eventSource_OnApplicationSuspend;

            _customAppEventSource.OnApplicationTimedOut -= eventSource_OnApplicationTimedOut;
        }

        private bool eventSource_OnCanLogoff()
        {
            // Always return true unless there is a good reason not to.
            UnsubscribeToEvents();
            return true;
        }

        private void eventSource_OnApplicationSuspend()
        {
            UnsubscribeToEvents();

        }

        private void eventSource_OnApplicationTimedOut()
        {
            UnsubscribeToEvents();

        }

        private void eventSource_OnUserChange()
        {
            UnsubscribeToEvents();

        }
        #endregion

        public int Process()
        {
            contextinstance = CustomContextObj.GetInstance();
            if (string.IsNullOrWhiteSpace(contextinstance.GetCurrentClientVisitGUID()))
            {
                int num = (int)MessageBox.Show("Please select a patient.");
                return -1;
            }

            if (_linkedPatientTable.Rows.Count == 0)
            {
                return 0;
            }
            if (_linkedPatientTable.Rows.Count == 1)
            {
                var selectedrow = _linkedPatientTable.Rows[0];
                var sVisitGUID = selectedrow["ClientVisitGUID"].ToString();
                contextinstance.SetVisitGUID(sVisitGUID);
                return 0;
            }
            if (_linkedPatientTable.Rows.Count > 0)
            {
                ShowDialog();
            }

            return 0;

        }

        private void LinkedPatientsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LinkedPatientsListView.SelectedItem == null && LinkedPatientsListView.Items.Count > 0)
            {
                MessageBox.Show("Please select a patient from the List", "Error Changing to new Patient",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
                return;
            }

            if (LinkedPatientsListView.SelectedItems.Count == 1)
            {
                //Change Context now
                var selectedrow = LinkedPatientsListView.SelectedItem as DataRowView;
                var sVisitGUID = (String)selectedrow.Row["ClientVisitGUID"].ToString();

                contextinstance.SetVisitGUID(sVisitGUID);
                Close();
            }

        }

        private void SetNewContext(object sender, RoutedEventArgs e)
        {
            if (LinkedPatientsListView.SelectedItem == null && LinkedPatientsListView.Items.Count > 0)
            {
                MessageBox.Show("Please select a patient from the List", "Error Changing to new Patient",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (LinkedPatientsListView.SelectedItems.Count == 1)
            {
                //Change Context now
                var selectedrow = LinkedPatientsListView.SelectedItem as DataRowView;
                var sVisitGUID = (String)selectedrow.Row["ClientVisitGUID"].ToString();

                contextinstance.SetVisitGUID(sVisitGUID);
                Close();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
