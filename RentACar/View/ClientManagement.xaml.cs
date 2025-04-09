using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RentACar.Model;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace RentACar.View
{
    public partial class ClientManagement : UserControl
    {
        private ObservableCollection<Klijent> _clients;
        private ICollectionView _clientsView;
        private readonly rentacarContext _dbContext;

        public ClientManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                _clients = new ObservableCollection<Klijent>(
                    _dbContext.Klijents
                        .Include(c => c.Iznajmljivanjes)
                        .ToList()
                );

                ClientsDataGrid.ItemsSource = _clients;
                _clientsView = CollectionViewSource.GetDefaultView(_clients);
                _clientsView.Filter = ClientFilter;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                LoadClients();  // Ucitavanje klijenata (dodavanje iz izn)
            }

        }

        private bool ClientFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                return true;

            var client = (Klijent)item;
            var searchText = SearchBox.Text.ToLower();

            return client.Ime.ToLower().Contains(searchText) ||
                   client.Prezime.ToLower().Contains(searchText) ||
                   client.BrojTelefona.ToLower().Contains(searchText) ||
                   (client.Email?.ToLower().Contains(searchText) ?? false);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _clientsView?.Refresh();
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ClientDialog(null, _dbContext);
            if (dialog.ShowDialog() == true)
            {
                LoadClients();
                ShowMessage(FindResource("Success").ToString(), FindResource("ClientAdded").ToString(), MessageType.Success);
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            var client = ((Button)sender).DataContext as Klijent;
            if (client != null)
            {
                var dialog = new ClientDialog(client, _dbContext);
                if (dialog.ShowDialog() == true)
                {
                    LoadClients();
                    ShowMessage(FindResource("Success").ToString(), FindResource("ClientUpdated").ToString(), MessageType.Success);
                }
            }
        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            var client = ((Button)sender).DataContext as Klijent;
            if (client != null)
            {
                var dialogTitle = FindResource("ConfirmDelete").ToString();
                var dialogMessage = string.Format(FindResource("DeleteClientConfirmation").ToString(), client.Ime + " " + client.Prezime);

                var confirmDialog = new CustomMessageBox(dialogTitle, dialogMessage, MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {

                        _dbContext.Klijents.Remove(client);
                        _dbContext.SaveChanges();

                        LoadClients();
                        ShowMessage(FindResource("Success").ToString(), FindResource("ClientDeleted").ToString(), MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}