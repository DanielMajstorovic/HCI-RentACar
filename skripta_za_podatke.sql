INSERT INTO `RentACar`.`MJESTO` (`Naziv`) VALUES 
('Banja Luka'),
('Laktaši'),
('Prijedor'),
('Bijeljina'),
('Sarajevo'),
('Mostar');

INSERT INTO `RentACar`.`KORISNIK` (`KorisnickoIme`, `Lozinka`, `EPosta`, `TipNaloga`, `MJESTO_idMJESTO`, `Tema`, `Jezik`) VALUES 
('admin', '12345678', 'admin@mail.com', 'Administrator', 1, 1, 1),
('adminDule', '12345678', 'dule@mail.com', 'Administrator', 2, 2, 2),
('agent', '12345678', 'agent@mail.com', 'Agent', 4, 1, 1),
('petarPetrovic', '12345678', 'pero@mail.com', 'Agent', 3, 1, 1),
('markoMarkovic', '12345678', 'markan@mail.com', 'Agent', 5, 2, 2);

INSERT INTO `RentACar`.`AGENT` (`KORISNIK_idKORISNIK`, `Plata`) VALUES 
(3, 1900.50),
(4, 1600.75),
(5, 2500.50);

INSERT INTO `RentACar`.`ADMINISTRATOR` (`KORISNIK_idKORISNIK`) VALUES 
(1),
(2);

INSERT INTO `RentACar`.`TELEFON` (`Telefon`, `KORISNIK_idKORISNIK`) VALUES 
('065123456', 1),
('065789123', 1),
('065456789', 2),
('065987654', 2),
('065321987', 3),
('065654321', 3),
('065111222', 4),
('065222333', 4),
('065333444', 5);

INSERT INTO `RentACar`.`KATEGORIJA_VOZILA` (`Naziv`) VALUES 
('Ekonomska'),
('Sportska'),
('SUV'),
('Luksuzna'),
('Kombi');

INSERT INTO `RentACar`.`VOZILO` (`KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA`, `Marka`, `Model`, `Godiste`, `Snaga`, `DnevnaTarifa`, `SedmicnaTarifa`) VALUES 
(2, 'Lexus', 'LC 500', 2023, 471, 200.00, 1200.00),
(2, 'Porsche', '911 GT3-RS', 2025, 525, 300.00, 1800.00),
(5, 'Mercedes-Benz', 'Sprinter', 2022, 180, 120.00, 700.00),
(4, 'Mercedes-Benz', 'S-Class', 2023, 429, 220.00, 1300.00),
(4, 'Audi', 'RS6', 2023, 630, 200.00, 1200.00),
(3, 'Lexus', 'GX', 2023, 349, 190.00, 1100.00),
(1, 'Toyota', 'Yaris', 2021, 250, 60.00, 300.00);

INSERT INTO `RentACar`.`Klijent` (`Ime`, `Prezime`, `BrojTelefona`, `Email`) VALUES 
('Ivan', 'Ivić', '065010101', 'ivan@mail.com'),
('Marko', 'Mirković', '065555333', 'marko.mirkovic@mail.com'),
('David', 'Davidović', '065777669', 'david@mail.com'),
('John', 'Doe', '065393939', 'john.doe@mail.com'),
('Ivana', 'Ivanović', '065393989', 'ivana.ive@mail.com'),
('Stojan', 'Ochoa', '065278111', 'stojke@mail.com');

INSERT INTO `RentACar`.`StatusIznajmljivanja` (`NazivStatusa`) VALUES 
('Rezervisano'),
('Aktivno'),
('Završeno'),
('Otkazano');

INSERT INTO `RentACar`.`OPCIJA` (`Naziv`, `Opis`, `Cijena`) VALUES 
('GPS', 'Navigacijski sistem', 5.00),
('Dječija sjedalica', 'Sigurnosno sjedište za djecu', 10.00),
('Osiguranje', 'Dodatno osiguranje', 40.00),
('Wi-Fi', 'Mobilni internet', 15.00),
('Pun rezervoar', 'Gorivo uključeno', 100.00);

INSERT INTO `RentACar`.`IZNAJMLJIVANJE` (`AGENT_KORISNIK_idKORISNIK`, `VOZILO_KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA`, `VOZILO_idVOZILO`, `Cijena`, `DatumIznajmljivanja`, `DatumVracanja`, `Klijent_idKlijent`, `DodatniOpis`, `StatusIznajmljivanja_idStatusIznajmljivanja`) VALUES 
(3, 2, 1, 415.00, '2025-04-10', '2025-04-12', 1, 'Očistiti auto detaljno', 1),
(3, 2, 2, 1240.00, '2025-04-16', '2025-04-20', 2, 'Planira putovanje van države', 1),
(4, 5, 3, 255.00, '2025-04-17', '2025-04-18', 3, 'Prevozi namještaj', 1),
(5, 4, 4, 320.00, '2025-04-21', '2025-04-21', 4, 'VIP potrebe', 1),
(3, 4, 5, 400.00, '2025-04-18', '2025-04-19', 5, 'Ide na stazu', 1);

INSERT INTO `RentACar`.`OPCIJA_NA_IZNAJMLJIVANJU` (`OPCIJA_idOPCIJA`, `IZNAJMLJIVANJE_idIZNAJMLJIVANJE`) VALUES 
(1, 1),
(2, 1),
(3, 2),
(4, 3),
(5, 4);

INSERT INTO `RentACar`.`PROBLEM` (`OpisProblema`, `DatumKreiranja`, `ADMINISTRATOR_KORISNIK_idKORISNIK`, `AGENT_KORISNIK_idKORISNIK`, `PovratneInformacije`, `DatumObrade`) VALUES 
('Vozilo nije vraćeno na vrijeme', '2025-01-01 10:00:00', 1, 3, 'Kontaktirali klijenta i naplaćena kazna', '2025-01-02 15:00:00'),
('Klijent prijavio prljavo vozilo', '2025-01-10 08:45:00', 2, 3, 'Odobreno umanjenje cijene', '2025-01-10 16:20:00'),
('Problem s rezervacijom', '2025-01-15 11:30:00', 1, 3, 'Rezervacija uspješno promijenjena', '2025-01-15 14:45:00'),
('Zahtjev za produženjem najma', '2025-01-18 09:15:00', 1, 3, NULL, NULL),
('Problem s klima uređajem', '2025-01-20 13:40:00', 2, 3, NULL, NULL),

('Tehnički problem s vozilom', '2025-01-03 14:30:00', 2, 4, 'Vozilo zamijenjeno novim', '2025-01-04 10:15:00'),
('Izgubljeni ključevi vozila', '2025-01-12 18:45:00', 2, 4, 'Izdana kopija ključa uz naknadu', '2025-01-13 09:00:00'),
('Kvar na mjenjaču', '2025-01-17 10:30:00', 1, 4, NULL, NULL),
('Zahtjev za promjenom lokacije preuzimanja', '2025-01-21 15:10:00', 2, 4, NULL, NULL),

('Neodgovarajuća tarifa naplaćena', '2025-01-05 09:15:00', 1, 5, 'Refundacija izvršena', '2025-01-06 11:45:00'),
('Klijent tvrdi da nije oštećen branik', '2025-01-09 13:30:00', 2, 5, 'Pregledane snimke, šteta postojala prije najma', '2025-01-11 10:20:00'),
('Prigovor na čistoću vozila', '2025-01-19 12:15:00', 2, 5, NULL, NULL),
('Zahtjev za povratom depozita', '2025-01-22 09:45:00', 1, 5, NULL, NULL);


