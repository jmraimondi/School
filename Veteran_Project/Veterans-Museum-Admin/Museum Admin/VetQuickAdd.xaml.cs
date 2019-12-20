using System;
using System.Windows;
using System.Windows.Controls;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for VetQuickAdd.xaml
    /// </summary>
    public partial class VetQuickAdd : UserControl
    {
        public VeteranDBInfo Veteran { get; set; }
        public VeteranDBInfo OlderVeteran { get; set; }
        public VeteranDBInfo OldestVeteran { get; set; }

        private string cemeteryDetails;

        public VetQuickAdd(string cemDetails)
        {
            InitializeComponent();

            cemeteryDetails = cemDetails;

            LoadNew();

            Tools.hasDataChanged = false;
        }

        public void LoadNew()
        {
            DataContext = null;

            // Assign down the chain and build a new record to add to
            OldestVeteran = OlderVeteran;
            OlderVeteran = Veteran;
            Veteran = new VeteranDBInfo();

            Veteran.CemDetails = cemeteryDetails;

            DataContext = this;

            // Set the Listboxes to the Lists in Veteran
            ListBox_ServiceDetails.DataContext = Veteran.ServiceDetails;
            ListBox_ConflictDetails.DataContext = Veteran.ConflictDetails;
        }

        private void Btn_SaveRecord_Click(object sender, RoutedEventArgs e)
        {
            Veteran.WriteDataToDatabase();

            // Setup a new record for editing
            LoadNew();
        }

        private void Btn_AddService_Click(object sender, RoutedEventArgs e)
        {
            ServiceDetails serviceWin = new ServiceDetails();

            // If we know the veteran ID, set it
            if (Veteran.Id != 0)
            {
                serviceWin.SetId(Veteran.Id);
            }

            serviceWin.ShowDialog();

            if (serviceWin.IsOk)
            {
                Veteran.ServiceDetails.Add(serviceWin.ServiceInfo);
            }

            ListBox_ServiceDetails.Items.Refresh();
        }

        private void Btn_EditService_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;
            VetServiceDBInfo selectedItem = null;
            bool found = false;

            // Get the sNum from the UI
            selectedId = Convert.ToInt32(ListBox_ServiceDetails.SelectedValue);

            foreach (VetServiceDBInfo service in Veteran.ServiceDetails)
            {
                if (service.sNum == selectedId)
                {
                    selectedItem = service;
                    found = true;
                }
            }

            // Should always be found, but if for some reason the record is not there, do nothing
            if (found)
            {
                ServiceDetails serviceWin = new ServiceDetails(selectedItem);

                serviceWin.ShowDialog();

                if (serviceWin.IsOk)
                {
                    // Remove the old listing of the item
                    Veteran.ServiceDetails.Remove(selectedItem);

                    // Insert the updated listing
                    Veteran.ServiceDetails.Add(serviceWin.ServiceInfo);
                }

                ListBox_ServiceDetails.Items.Refresh();
            }
        }

        private void Btn_DeleteService_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;
            VetServiceDBInfo removeItem = null;
            bool found = false;

            // Get the sNum from the UI
            selectedId = Convert.ToInt32(ListBox_ServiceDetails.SelectedValue);

            MessageBoxResult result;
            result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Find the record matching the selected ID
                foreach (VetServiceDBInfo service in Veteran.ServiceDetails)
                {
                    if (service.sNum == selectedId)
                    {
                        // Remove from the database
                        service.DeleteFromDatabase();

                        // Flag item for removal from list
                        found = true;
                        removeItem = service;
                    }
                }

                if (found)
                {
                    // Remove from the list
                    Veteran.ServiceDetails.Remove(removeItem);
                }

                ListBox_ServiceDetails.Items.Refresh();
            }
        }

        private void Btn_AddConflict_Click(object sender, RoutedEventArgs e)
        {
            ConflictDetails conflictWin = new ConflictDetails();

            // If we know the veteran ID, set it
            if (Veteran.Id != 0)
            {
                conflictWin.SetId(Veteran.Id);
            }

            conflictWin.ShowDialog();

            if (conflictWin.IsOk)
            {
                Veteran.ConflictDetails.Add(conflictWin.ConflictInfo);
            }

            ListBox_ConflictDetails.Items.Refresh();
        }

        private void Btn_EditConflict_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;
            VetConflictDBInfo selectedItem = null;
            bool found = false;

            // Get the sNum from the UI
            selectedId = Convert.ToInt32(ListBox_ConflictDetails.SelectedValue);

            foreach (VetConflictDBInfo conflict in Veteran.ConflictDetails)
            {
                if (conflict.CNum == selectedId)
                {
                    selectedItem = conflict;
                    found = true;
                }
            }

            // Should always be found, but if for some reason the record is not there, do nothing
            if (found)
            {
                ConflictDetails conflictWin = new ConflictDetails(selectedItem);

                conflictWin.ShowDialog();

                if (conflictWin.IsOk)
                {
                    // Remove the old listing of the item
                    Veteran.ConflictDetails.Remove(selectedItem);

                    // Insert the updated listing
                    Veteran.ConflictDetails.Add(conflictWin.ConflictInfo);
                }

                ListBox_ConflictDetails.Items.Refresh();
            }
        }

        private void Btn_DeleteConflict_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;
            VetConflictDBInfo removeItem = null;
            bool found = false;

            selectedId = Convert.ToInt32(ListBox_ConflictDetails.SelectedValue);

            MessageBoxResult result;
            result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                foreach (VetConflictDBInfo conflict in Veteran.ConflictDetails)
                {
                    if (conflict.CNum == selectedId)
                    {
                        // Remove from the database
                        conflict.DeleteFromDatabase();

                        // Flag item for removal from list
                        found = true;
                        removeItem = conflict;
                    }
                }

                if (found)
                {
                    // Remove from the list
                    Veteran.ConflictDetails.Remove(removeItem);
                }

                ListBox_ConflictDetails.Items.Refresh();
            }
        }
    }
}
