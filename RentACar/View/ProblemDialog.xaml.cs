using System;
using System.Windows;
using RentACar.Model;
namespace RentACar.View
{
    public partial class ProblemDialog : Window
    {
        private readonly Problem _problem;
        private readonly rentacarContext _dbContext;
        private readonly Korisnik _admin;
        private readonly bool _viewOnly;
        public ProblemDialog(Problem problem, rentacarContext dbContext, Korisnik admin, bool viewOnly = false)
        {
            InitializeComponent();
            _problem = problem;
            _dbContext = dbContext;
            _admin = admin;
            _viewOnly = viewOnly;
            LoadProblemData();
            ConfigureDialog();
        }
        private void LoadProblemData()
        {
            AgentTextBox.Text = _problem.AgentKorisnikIdKorisnikNavigation?.KorisnikIdKorisnikNavigation?.KorisnickoIme ?? "";
            CreationDateTextBox.Text = _problem.DatumKreiranja.ToString("dd.MM.yyyy HH:mm");
            ProblemDescriptionTextBox.Text = _problem.OpisProblema;
            if (!string.IsNullOrEmpty(_problem.PovratneInformacije))
            {
                FeedbackTextBox.Text = _problem.PovratneInformacije;
                if (_problem.DatumObrade.HasValue)
                {
                    ProcessingDateLabel.Visibility = Visibility.Visible;
                    ProcessingDateTextBox.Visibility = Visibility.Visible;
                    ProcessingDateTextBox.Text = _problem.DatumObrade.Value.ToString("dd.MM.yyyy HH:mm");
                }
            }
        }
        private void ConfigureDialog()
        {
            if (_viewOnly)
            {
                DialogTitle.Text = FindResource("ProblemDetails").ToString();
                FeedbackTextBox.IsReadOnly = true;
                FeedbackTextBox.Background = (System.Windows.Media.Brush)FindResource("TextSecondaryBrush");
                SaveButton.Visibility = Visibility.Collapsed;
                CloseButton.Content = FindResource("Close").ToString();
            }
            else
            {
                DialogTitle.Text = string.IsNullOrEmpty(_problem.PovratneInformacije)
                    ? FindResource("ProcessProblem").ToString()
                    : FindResource("EditProblemFeedback").ToString();
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FeedbackTextBox.Text))
            {
                var messageBox = new CustomMessageBox(
                    FindResource("Error").ToString(),
                    FindResource("FeedbackRequired").ToString(),
                    MessageType.Error);
                messageBox.ShowDialog();
                return;
            }
            try
            {
                _problem.PovratneInformacije = FeedbackTextBox.Text;
                _problem.DatumObrade = DateTime.Now;
                _problem.AdministratorKorisnikIdKorisnik = _admin.IdKorisnik;
                _dbContext.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var messageBox = new CustomMessageBox(
                    FindResource("Error").ToString(),
                    ex.Message,
                    MessageType.Error);
                messageBox.ShowDialog();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}