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
    public partial class ProblemsManagement : UserControl
    {
        private ObservableCollection<Problem> _problems;
        private ICollectionView _problemsView;
        private readonly rentacarContext _dbContext;
        private Korisnik _admin;

        public ProblemsManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            LoadProblems();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Korisnik currentUser)
            {
                _admin = currentUser;
            }
        }

        private void LoadProblems()
        {
            try
            {
                
                _problems = new ObservableCollection<Problem>(
                    _dbContext.Problems
                        .Include(p => p.AgentKorisnikIdKorisnikNavigation)
                        .ThenInclude(a => a.KorisnikIdKorisnikNavigation)
                        .Where(p => string.IsNullOrEmpty(p.PovratneInformacije))
                        .ToList()
                );

                ProblemsDataGrid.ItemsSource = _problems;
                _problemsView = CollectionViewSource.GetDefaultView(_problems);
                _problemsView.Filter = ProblemFilter;
            }
            catch (Exception ex)
            {
                ShowMessage(FindResource("ErrorLoadingData").ToString(), ex.Message, MessageType.Error);
            }
        }

        private bool ProblemFilter(object item)
        {
            if (!(item is Problem problem))
                return false;

            
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchText = SearchBox.Text.ToLower();
                bool matchesSearch = problem.OpisProblema.ToLower().Contains(searchText) ||
                                     problem.AgentKorisnikIdKorisnikNavigation.KorisnikIdKorisnikNavigation.KorisnickoIme.ToLower().Contains(searchText) ||
                                     problem.DatumKreiranja.ToString().Contains(searchText);

                return matchesSearch;
            }

            return true;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _problemsView?.Refresh();
        }

        private void ProcessProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var problem = ((Button)sender).DataContext as Problem;
            if (problem != null)
            {
                var dialog = new ProblemDialog(problem, _dbContext, _admin);
                if (dialog.ShowDialog() == true)
                {
                    LoadProblems();
                    ShowMessage(FindResource("Success").ToString(), FindResource("ProblemProcessed").ToString(), MessageType.Success);
                }
            }
        }

        private void ViewProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var problem = ((Button)sender).DataContext as Problem;
            if (problem != null)
            {
                var dialog = new ProblemDialog(problem, _dbContext, _admin, true);
                dialog.ShowDialog();
            }
        }

        private void ProblemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}