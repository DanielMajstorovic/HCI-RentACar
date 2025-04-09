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
    public partial class UserManagement : UserControl
    {
        private ObservableCollection<Agent> _agents;
        private ICollectionView _agentsView;
        private readonly rentacarContext _dbContext;

        public UserManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            LoadAgents();
        }

        private void LoadAgents()
        {
            try
            {
                
                _agents = new ObservableCollection<Agent>(
                    _dbContext.Agents
                        .Include(a => a.KorisnikIdKorisnikNavigation)
                        .Include(a => a.KorisnikIdKorisnikNavigation.MjestoIdMjestoNavigation)
                        .Include(a => a.KorisnikIdKorisnikNavigation.Telefons)
                        .ToList()
                );

                AgentsDataGrid.ItemsSource = _agents;
                _agentsView = CollectionViewSource.GetDefaultView(_agents);
                _agentsView.Filter = AgentFilter;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private bool AgentFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                return true;

            var agent = (Agent)item;
            var searchText = SearchBox.Text.ToLower();

            return agent.KorisnikIdKorisnikNavigation.KorisnickoIme.ToLower().Contains(searchText) ||
                   agent.KorisnikIdKorisnikNavigation.Eposta.ToLower().Contains(searchText) ||
                   (agent.KorisnikIdKorisnikNavigation.MjestoIdMjestoNavigation?.Naziv?.ToLower().Contains(searchText) ?? false) ||
                   (agent.Plata?.ToString().Contains(searchText) ?? false);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _agentsView?.Refresh();
        }

        private void AddAgentButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgentDialog(null, _dbContext);
            if (dialog.ShowDialog() == true)
            {
                LoadAgents();
                ShowMessage(FindResource("Success").ToString(), FindResource("AgentAdded").ToString(), MessageType.Success);
            }
        }

        private void EditAgentButton_Click(object sender, RoutedEventArgs e)
        {
            var agent = ((Button)sender).DataContext as Agent;
            if (agent != null)
            {
                var dialog = new AgentDialog(agent, _dbContext);
                if (dialog.ShowDialog() == true)
                {
                    LoadAgents();
                    ShowMessage(FindResource("Success").ToString(), FindResource("AgentUpdated").ToString(), MessageType.Success);
                }
            }
        }

        private void DeleteAgentButton_Click(object sender, RoutedEventArgs e)
        {
            var agent = ((Button)sender).DataContext as Agent;
            if (agent != null)
            {
                var dialogTitle = FindResource("ConfirmDelete").ToString();
                var dialogMessage = string.Format(FindResource("DeleteAgentConfirmation").ToString(), agent.KorisnikIdKorisnikNavigation.KorisnickoIme);

                var confirmDialog = new CustomMessageBox(dialogTitle, dialogMessage, MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {
                        
                        var telephones = agent.KorisnikIdKorisnikNavigation.Telefons.ToList();
                        foreach (var phone in telephones)
                        {
                            _dbContext.Telefons.Remove(phone);
                        }

                        
                        _dbContext.Agents.Remove(agent);

                        
                        _dbContext.Korisniks.Remove(agent.KorisnikIdKorisnikNavigation);

                        _dbContext.SaveChanges();

                        LoadAgents();
                        ShowMessage(FindResource("Success").ToString(), FindResource("AgentDeleted").ToString(), MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void AgentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}