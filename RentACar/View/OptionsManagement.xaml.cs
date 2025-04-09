using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace RentACar.View
{
    public partial class OptionsManagement : UserControl
    {
        private ObservableCollection<Opcija> _options;
        private ICollectionView _optionsView;
        private readonly rentacarContext _dbContext;

        public OptionsManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            LoadOptions();
        }

        private void LoadOptions()
        {
            try
            {
                _options = new ObservableCollection<Opcija>(
                    _dbContext.Opcijas.ToList()
                );

                OptionsDataGrid.ItemsSource = _options;
                _optionsView = CollectionViewSource.GetDefaultView(_options);
                _optionsView.Filter = OptionFilter;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private bool OptionFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                return true;

            var option = (Opcija)item;
            var searchText = SearchBox.Text.ToLower();

            return option.Naziv.ToLower().Contains(searchText) ||
                   (option.Opis?.ToLower().Contains(searchText) ?? false) ||
                   option.Cijena.ToString().Contains(searchText);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _optionsView?.Refresh();
        }

        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OptionDialog(null, _dbContext);
            if (dialog.ShowDialog() == true)
            {
                LoadOptions();
                ShowMessage(FindResource("Success").ToString(), FindResource("OptionAdded").ToString(), MessageType.Success);
            }
        }

        private void EditOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var option = ((Button)sender).DataContext as Opcija;
            if (option != null)
            {
                var dialog = new OptionDialog(option, _dbContext);
                if (dialog.ShowDialog() == true)
                {
                    LoadOptions();
                    ShowMessage(FindResource("Success").ToString(), FindResource("OptionUpdated").ToString(), MessageType.Success);
                }
            }
        }

        private void DeleteOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var option = ((Button)sender).DataContext as Opcija;
            if (option != null)
            {
                var dialogTitle = FindResource("ConfirmDelete").ToString();
                var dialogMessage = string.Format(FindResource("DeleteOptionConfirmation").ToString(), option.Naziv);

                var confirmDialog = new CustomMessageBox(dialogTitle, dialogMessage, MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {
                        
                        var relatedRentals = _dbContext.OpcijaNaIznajmljivanjus
                            .Where(o => o.OpcijaIdOpcija == option.IdOpcija) 
                            .ToList();

                        _dbContext.OpcijaNaIznajmljivanjus.RemoveRange(relatedRentals);
                        _dbContext.Opcijas.Remove(option);
                        _dbContext.SaveChanges();

                        LoadOptions();
                        ShowMessage(FindResource("Success").ToString(), FindResource("OptionDeleted").ToString(), MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void OptionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}