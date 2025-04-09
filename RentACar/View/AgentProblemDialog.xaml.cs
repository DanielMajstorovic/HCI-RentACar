using System;
using System.Windows;
using RentACar.Model;

namespace RentACar.View
{
    public partial class AgentProblemDialog : Window
    {
        private readonly Problem _problem;
        private readonly rentacarContext _dbContext;
        private readonly Agent _agent;
        private readonly bool _isViewMode;
        private readonly bool _isCreatingMode;

        public AgentProblemDialog(Problem problem, rentacarContext dbContext, Agent agent, bool isViewMode, bool isCreatingMode = false)
        {
            InitializeComponent();
            _problem = problem;
            _dbContext = dbContext;
            _agent = agent;
            _isViewMode = isViewMode;
            _isCreatingMode = isCreatingMode;

            SetupDialog();
        }

        private void SetupDialog()
        {
            if (_isCreatingMode)
            {
                DialogTitle.Text = FindResource("AddProblem").ToString();
                SaveButton.Content = FindResource("Save").ToString();
                CreationDateTextBox.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                DialogTitle.Text = FindResource("ProblemDetails").ToString();
                SaveButton.Content = FindResource("Close").ToString();
                LoadProblemData();
            }

            ProblemDescriptionTextBox.IsReadOnly = _isViewMode;

            if (_isViewMode)
            {
                ProblemDescriptionTextBox.Style = FindResource("ReadOnlyTextBox") as Style;
            }

            bool isProcessed = !string.IsNullOrEmpty(_problem.PovratneInformacije);
            StatusLabel.Visibility = _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            StatusTextBox.Visibility = _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            AdminLabel.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            AdminTextBox.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            ProcessingDateLabel.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            ProcessingDateTextBox.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            FeedbackLabel.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;
            FeedbackTextBox.Visibility = isProcessed && _isViewMode ? Visibility.Visible : Visibility.Collapsed;

            SaveButton.Visibility = _isCreatingMode || !_isViewMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadProblemData()
        {
            CreationDateTextBox.Text = _problem.DatumKreiranja.ToString("dd.MM.yyyy HH:mm");
            ProblemDescriptionTextBox.Text = _problem.OpisProblema;

            if (_isViewMode)
            {
                StatusTextBox.Text = string.IsNullOrEmpty(_problem.PovratneInformacije) ?
                    FindResource("Unprocessed").ToString() :
                    FindResource("Processed").ToString();

                if (!string.IsNullOrEmpty(_problem.PovratneInformacije))
                {
                    if (_problem.AdministratorKorisnikIdKorisnikNavigation?.KorisnikIdKorisnikNavigation != null)
                    {
                        AdminTextBox.Text = _problem.AdministratorKorisnikIdKorisnikNavigation.KorisnikIdKorisnikNavigation.KorisnickoIme;
                    }

                    ProcessingDateTextBox.Text = _problem.DatumObrade?.ToString("dd.MM.yyyy HH:mm") ?? "";
                    FeedbackTextBox.Text = _problem.PovratneInformacije;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isCreatingMode || !_isViewMode)
            {
                if (string.IsNullOrWhiteSpace(ProblemDescriptionTextBox.Text))
                {
                    ShowMessage(
                        FindResource("Error").ToString(),
                        FindResource("ProblemDescriptionRequired").ToString(),
                        MessageType.Error);
                    return;
                }

                try
                {
                    _problem.OpisProblema = ProblemDescriptionTextBox.Text;

                    if (_isCreatingMode)
                    {
                        _dbContext.Problems.Add(_problem);
                    }

                    _dbContext.SaveChanges();
                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    ShowMessage(
                        FindResource("Error").ToString(),
                        ex.Message,
                        MessageType.Error);
                }
            }
            else
            {
                DialogResult = false;
            }

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowMessage(string title, string message, MessageType type)
        {
            var messageBox = new CustomMessageBox(title, message, type);
            messageBox.ShowDialog();
        }
    }
}