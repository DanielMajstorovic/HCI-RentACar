using System;
using System.Windows;
using System.Linq;
using RentACar.Model;
using Microsoft.EntityFrameworkCore;

namespace RentACar.View
{
    public partial class RentalDetailsDialog : Window
    {
        private readonly Iznajmljivanje _rental;

        public RentalDetailsDialog(Iznajmljivanje rental)
        {
            InitializeComponent();
            _rental = rental;
            LoadRentalData();
        }

        private void LoadRentalData()
        {
            
            using (var context = new rentacarContext())
            {
                var korisnik = context.Korisniks
                    .FirstOrDefault(k => k.IdKorisnik == _rental.AgentKorisnikIdKorisnik);

                AgentTextBox.Text = korisnik?.KorisnickoIme ?? "";
            }

            ClientTextBox.Text = _rental.KlijentIdKlijentNavigation?.ToString() ?? "";
            RentalDateTextBox.Text = _rental.DatumIznajmljivanja.ToString("dd/MM/yyyy");

            if (_rental.DatumVracanja.HasValue)
            {
                ReturnDateTextBox.Text = _rental.DatumVracanja.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                ReturnDateLabel.Visibility = Visibility.Collapsed;
                ReturnDateTextBox.Visibility = Visibility.Collapsed;
            }

            
            VehicleTextBox.Text = $"{_rental.VoziloIdVoziloNavigation?.Marka} {_rental.VoziloIdVoziloNavigation?.Model} ({_rental.VoziloIdVoziloNavigation?.Godiste})";
            
            using (var context = new rentacarContext())
            {
                var kategoorija = context.KategorijaVozilas
                    .FirstOrDefault(k => k.IdKategorijaVozila == _rental.VoziloKategorijaVozilaIdKategorijaVozila);

                VehicleCategoryTextBox.Text = kategoorija?.Naziv ?? "";
            }
            
            PriceTextBox.Text = _rental.Cijena.ToString("0.00") + " $";
            StatusTextBox.Text = _rental.StatusIznajmljivanjaIdStatusIznajmljivanjaNavigation?.NazivStatusa ?? "";

            
            if (!string.IsNullOrEmpty(_rental.DodatniOpis))
            {
                AdditionalDescriptionTextBox.Text = _rental.DodatniOpis;
            }
            else
            {
                AdditionalDescriptionLabel.Visibility = Visibility.Collapsed;
                AdditionalDescriptionTextBox.Visibility = Visibility.Collapsed;
            }


            using (var context = new rentacarContext())
            {
                var opcijeNaIznajmljivanju = context.OpcijaNaIznajmljivanjus
        .Where(o => o.IznajmljivanjeIdIznajmljivanje == _rental.IdIznajmljivanje)
        .Include(o => o.OpcijaIdOpcijaNavigation) 
        .ToList();

                if (opcijeNaIznajmljivanju.Any())
                {
                    string options = string.Join(", ", opcijeNaIznajmljivanju.Select(o => o.OpcijaIdOpcijaNavigation.Naziv));
                    OptionsTextBox.Text = options;

                    decimal optionsTotal = opcijeNaIznajmljivanju.Sum(o => o.OpcijaIdOpcijaNavigation.Cijena);
                    OptionsPriceTextBox.Text = optionsTotal.ToString("0.00") + " $";
                }
                else
                {
                    OptionsLabel.Visibility = Visibility.Collapsed;
                    OptionsTextBox.Visibility = Visibility.Collapsed;
                    OptionsPriceLabel.Visibility = Visibility.Collapsed;
                    OptionsPriceTextBox.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}