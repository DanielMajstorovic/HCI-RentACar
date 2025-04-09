using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace RentACar.View
{
    public partial class AgentProblemsManagement : UserControl
    {
        private ObservableCollection<ProblemViewModel> _problems;
        private ICollectionView _problemsView;
        private readonly rentacarContext _dbContext;
        private Agent _agent;

        public AgentProblemsManagement()
        {
            InitializeComponent();
            _dbContext = new rentacarContext();
            FilterComboBox.SelectedIndex = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Korisnik currentUser)
            {
                _agent = _dbContext.Agents
                    .Include(a => a.KorisnikIdKorisnikNavigation)
                    .FirstOrDefault(a => a.KorisnikIdKorisnik == currentUser.IdKorisnik);

                if (_agent != null)
                {
                    LoadProblems();
                }
            }
        }

        private void LoadProblems()
        {
            try
            {
                var problems = _dbContext.Problems
                    .Include(p => p.AdministratorKorisnikIdKorisnikNavigation)
                    .ThenInclude(a => a.KorisnikIdKorisnikNavigation)
                    .Where(p => p.AgentKorisnikIdKorisnik == _agent.KorisnikIdKorisnik)
                    .ToList();

                _problems = new ObservableCollection<ProblemViewModel>(
                    problems.Select(p => new ProblemViewModel(p))
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
            if (!(item is ProblemViewModel problem))
                return false;

            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchText = SearchBox.Text.ToLower();
                if (!problem.OpisProblema.ToLower().Contains(searchText) &&
                    !problem.AdminName.ToLower().Contains(searchText) &&
                    !problem.DatumKreiranja.ToString().Contains(searchText))
                {
                    return false;
                }
            }

            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var filterTag = selectedItem.Tag.ToString();

                if (filterTag == "Processed" && string.IsNullOrEmpty(problem.PovratneInformacije))
                    return false;

                if (filterTag == "Unprocessed" && !string.IsNullOrEmpty(problem.PovratneInformacije))
                    return false;
            }

            return true;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _problemsView?.Refresh();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _problemsView?.Refresh();
        }

        private void ViewProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var problem = ((Button)sender).DataContext as ProblemViewModel;
            if (problem != null)
            {
                var dialog = new AgentProblemDialog(problem.OriginalProblem, _dbContext, _agent, true);
                dialog.ShowDialog();
            }
        }

        private void DeleteProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var problem = ((Button)sender).DataContext as ProblemViewModel;
            if (problem != null && !string.IsNullOrEmpty(problem.PovratneInformacije))
            {

                var confirmDialog = new CustomMessageBox(FindResource("Confirmation").ToString(), FindResource("DeleteProblemConfirmation").ToString(), MessageType.Question, true);
                if (confirmDialog.ShowDialog() == true)
                {
                    try
                    {
                        _dbContext.Problems.Remove(problem.OriginalProblem);
                        _dbContext.SaveChanges();
                        LoadProblems();
                        ShowMessage(FindResource("Success").ToString(), FindResource("ProblemDeleted").ToString(), MessageType.Success);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(FindResource("Error").ToString(), ex.Message, MessageType.Error);
                    }
                }
            }
        }

        private void AddProblemButton_Click(object sender, RoutedEventArgs e)
        {
            var newProblem = new Problem
            {
                AgentKorisnikIdKorisnik = _agent.KorisnikIdKorisnik,
                DatumKreiranja = DateTime.Now
            };

            var dialog = new AgentProblemDialog(newProblem, _dbContext, _agent, false, true);
            if (dialog.ShowDialog() == true)
            {
                LoadProblems();
                ShowMessage(FindResource("Success").ToString(), FindResource("ProblemCreated").ToString(), MessageType.Success);
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

    public class ProblemViewModel : INotifyPropertyChanged
    {
        public Problem OriginalProblem { get; }

        public int IdProblem => OriginalProblem.IdProblem;
        public string OpisProblema => OriginalProblem.OpisProblema;
        public DateTime DatumKreiranja => OriginalProblem.DatumKreiranja;
        public DateTime? DatumObrade => OriginalProblem.DatumObrade;
        public string? PovratneInformacije => OriginalProblem.PovratneInformacije;
        public bool CanDelete => !string.IsNullOrEmpty(PovratneInformacije);

        public string AdminName => OriginalProblem.AdministratorKorisnikIdKorisnikNavigation?.KorisnikIdKorisnikNavigation?.KorisnickoIme ?? string.Empty;

        public ProblemViewModel(Problem problem)
        {
            OriginalProblem = problem;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}