using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace RentACar.Model
{
    public partial class rentacarContext : DbContext
    {
        public rentacarContext()
        {
        }

        public rentacarContext(DbContextOptions<rentacarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Agent> Agents { get; set; } = null!;
        public virtual DbSet<Iznajmljivanje> Iznajmljivanjes { get; set; } = null!;
        public virtual DbSet<KategorijaVozila> KategorijaVozilas { get; set; } = null!;
        public virtual DbSet<Klijent> Klijents { get; set; } = null!;
        public virtual DbSet<Korisnik> Korisniks { get; set; } = null!;
        public virtual DbSet<Mjesto> Mjestos { get; set; } = null!;
        public virtual DbSet<Opcija> Opcijas { get; set; } = null!;
        public virtual DbSet<OpcijaNaIznajmljivanju> OpcijaNaIznajmljivanjus { get; set; } = null!;
        public virtual DbSet<Problem> Problems { get; set; } = null!;
        public virtual DbSet<Statusiznajmljivanja> Statusiznajmljivanjas { get; set; } = null!;
        public virtual DbSet<Telefon> Telefons { get; set; } = null!;
        public virtual DbSet<Vozilo> Vozilos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var serverVersion = configuration.GetSection("DatabaseOptions")["ServerVersion"];
                optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse(serverVersion));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");


            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.KorisnikIdKorisnik)
                    .HasName("PRIMARY");

                entity.ToTable("administrator");

                entity.Property(e => e.KorisnikIdKorisnik)
                    .ValueGeneratedNever()
                    .HasColumnName("KORISNIK_idKORISNIK");

                entity.HasOne(d => d.KorisnikIdKorisnikNavigation)
                    .WithOne(p => p.Administrator)
                    .HasForeignKey<Administrator>(d => d.KorisnikIdKorisnik)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ADMINISTRATOR_KORISNIK1");
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(e => e.KorisnikIdKorisnik)
                    .HasName("PRIMARY");

                entity.ToTable("agent");

                entity.Property(e => e.KorisnikIdKorisnik)
                    .ValueGeneratedNever()
                    .HasColumnName("KORISNIK_idKORISNIK");

                entity.Property(e => e.Plata).HasPrecision(6, 2);

                entity.HasOne(d => d.KorisnikIdKorisnikNavigation)
                    .WithOne(p => p.Agent)
                    .HasForeignKey<Agent>(d => d.KorisnikIdKorisnik)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AGENT_KORISNIK1");
            });

            modelBuilder.Entity<Iznajmljivanje>(entity =>
            {
                entity.HasKey(e => e.IdIznajmljivanje)
                    .HasName("PRIMARY");

                entity.ToTable("iznajmljivanje");

                entity.HasIndex(e => e.AgentKorisnikIdKorisnik, "fk_IZNAJMLJIVANJE_AGENT1_idx");

                entity.HasIndex(e => e.KlijentIdKlijent, "fk_IZNAJMLJIVANJE_Klijent1_idx");

                entity.HasIndex(e => e.StatusIznajmljivanjaIdStatusIznajmljivanja, "fk_IZNAJMLJIVANJE_StatusIznajmljivanja1_idx");

                entity.HasIndex(e => e.VoziloIdVozilo, "fk_IZNAJMLJIVANJE_VOZILO1_idx");

                entity.Property(e => e.IdIznajmljivanje).HasColumnName("idIZNAJMLJIVANJE");

                entity.Property(e => e.AgentKorisnikIdKorisnik).HasColumnName("AGENT_KORISNIK_idKORISNIK");

                entity.Property(e => e.Cijena).HasPrecision(6, 2);

                entity.Property(e => e.DodatniOpis).HasMaxLength(512);

                entity.Property(e => e.KlijentIdKlijent).HasColumnName("Klijent_idKlijent");

                entity.Property(e => e.StatusIznajmljivanjaIdStatusIznajmljivanja).HasColumnName("StatusIznajmljivanja_idStatusIznajmljivanja");

                entity.Property(e => e.VoziloIdVozilo).HasColumnName("VOZILO_idVOZILO");

                entity.Property(e => e.VoziloKategorijaVozilaIdKategorijaVozila).HasColumnName("VOZILO_KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA");

                entity.HasOne(d => d.AgentKorisnikIdKorisnikNavigation)
                    .WithMany(p => p.Iznajmljivanjes)
                    .HasForeignKey(d => d.AgentKorisnikIdKorisnik)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IZNAJMLJIVANJE_AGENT1");

                entity.HasOne(d => d.KlijentIdKlijentNavigation)
                    .WithMany(p => p.Iznajmljivanjes)
                    .HasForeignKey(d => d.KlijentIdKlijent)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_IZNAJMLJIVANJE_Klijent1");

                entity.HasOne(d => d.StatusIznajmljivanjaIdStatusIznajmljivanjaNavigation)
                    .WithMany(p => p.Iznajmljivanjes)
                    .HasForeignKey(d => d.StatusIznajmljivanjaIdStatusIznajmljivanja)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IZNAJMLJIVANJE_StatusIznajmljivanja1");

                entity.HasOne(d => d.VoziloIdVoziloNavigation)
                    .WithMany(p => p.Iznajmljivanjes)
                    .HasForeignKey(d => d.VoziloIdVozilo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IZNAJMLJIVANJE_VOZILO1");
            });

            modelBuilder.Entity<KategorijaVozila>(entity =>
            {
                entity.HasKey(e => e.IdKategorijaVozila)
                    .HasName("PRIMARY");

                entity.ToTable("kategorija_vozila");

                entity.Property(e => e.IdKategorijaVozila).HasColumnName("idKATEGORIJA_VOZILA");

                entity.Property(e => e.Naziv).HasMaxLength(45);
            });

            modelBuilder.Entity<Klijent>(entity =>
            {
                entity.HasKey(e => e.IdKlijent)
                    .HasName("PRIMARY");

                entity.ToTable("klijent");

                entity.Property(e => e.IdKlijent).HasColumnName("idKlijent");

                entity.Property(e => e.BrojTelefona).HasMaxLength(45);

                entity.Property(e => e.Email).HasMaxLength(45);

                entity.Property(e => e.Ime).HasMaxLength(45);

                entity.Property(e => e.Prezime).HasMaxLength(45);
            });

            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.HasKey(e => e.IdKorisnik)
                    .HasName("PRIMARY");

                entity.ToTable("korisnik");

                entity.HasIndex(e => e.KorisnickoIme, "KorisnickoIme_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MjestoIdMjesto, "fk_KORISNIK_MJESTO1_idx");

                entity.Property(e => e.IdKorisnik).HasColumnName("idKORISNIK");

                entity.Property(e => e.Eposta)
                    .HasMaxLength(128)
                    .HasColumnName("EPosta");

                entity.Property(e => e.KorisnickoIme).HasMaxLength(64);

                entity.Property(e => e.Lozinka).HasMaxLength(64);

                entity.Property(e => e.MjestoIdMjesto).HasColumnName("MJESTO_idMJESTO");

                entity.Property(e => e.TipNaloga).HasMaxLength(64);

                entity.HasOne(d => d.MjestoIdMjestoNavigation)
                    .WithMany(p => p.Korisniks)
                    .HasForeignKey(d => d.MjestoIdMjesto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_KORISNIK_MJESTO1");
            });

            modelBuilder.Entity<Mjesto>(entity =>
            {
                entity.HasKey(e => e.IdMjesto)
                    .HasName("PRIMARY");

                entity.ToTable("mjesto");

                entity.Property(e => e.IdMjesto).HasColumnName("idMJESTO");

                entity.Property(e => e.Naziv).HasMaxLength(45);
            });

            modelBuilder.Entity<Opcija>(entity =>
            {
                entity.HasKey(e => e.IdOpcija)
                    .HasName("PRIMARY");

                entity.ToTable("opcija");

                entity.Property(e => e.IdOpcija).HasColumnName("idOPCIJA");

                entity.Property(e => e.Cijena).HasPrecision(5, 2);

                entity.Property(e => e.Naziv).HasMaxLength(45);

                entity.Property(e => e.Opis).HasMaxLength(45);
            });

            modelBuilder.Entity<OpcijaNaIznajmljivanju>(entity =>
            {
                entity.HasKey(e => e.IdOpcijaNaIznajmljivanju)
                    .HasName("PRIMARY");

                entity.ToTable("opcija_na_iznajmljivanju");

                entity.HasIndex(e => e.IznajmljivanjeIdIznajmljivanje, "fk_OPCIJA_NA_IZNAJMLJIVANJU_IZNAJMLJIVANJE1_idx");

                entity.HasIndex(e => e.OpcijaIdOpcija, "fk_OPCIJA_NA_IZNAJMLJIVANJU_OPCIJA1");

                entity.Property(e => e.IdOpcijaNaIznajmljivanju).HasColumnName("idOPCIJA_NA_IZNAJMLJIVANJU");

                entity.Property(e => e.IznajmljivanjeIdIznajmljivanje).HasColumnName("IZNAJMLJIVANJE_idIZNAJMLJIVANJE");

                entity.Property(e => e.OpcijaIdOpcija).HasColumnName("OPCIJA_idOPCIJA");

                entity.HasOne(d => d.IznajmljivanjeIdIznajmljivanjeNavigation)
                    .WithMany(p => p.OpcijaNaIznajmljivanjus)
                    .HasForeignKey(d => d.IznajmljivanjeIdIznajmljivanje)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPCIJA_NA_IZNAJMLJIVANJU_IZNAJMLJIVANJE1");

                entity.HasOne(d => d.OpcijaIdOpcijaNavigation)
                    .WithMany(p => p.OpcijaNaIznajmljivanjus)
                    .HasForeignKey(d => d.OpcijaIdOpcija)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPCIJA_NA_IZNAJMLJIVANJU_OPCIJA1");
            });

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.HasKey(e => e.IdProblem)
                    .HasName("PRIMARY");

                entity.ToTable("problem");

                entity.HasIndex(e => e.AdministratorKorisnikIdKorisnik, "fk_PROBLEM_ADMINISTRATOR1_idx");

                entity.HasIndex(e => e.AgentKorisnikIdKorisnik, "fk_PROBLEM_AGENT1_idx");

                entity.Property(e => e.IdProblem).HasColumnName("idPROBLEM");

                entity.Property(e => e.AdministratorKorisnikIdKorisnik).HasColumnName("ADMINISTRATOR_KORISNIK_idKORISNIK");

                entity.Property(e => e.AgentKorisnikIdKorisnik).HasColumnName("AGENT_KORISNIK_idKORISNIK");

                entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");

                entity.Property(e => e.DatumObrade).HasColumnType("datetime");

                entity.Property(e => e.OpisProblema).HasMaxLength(256);

                entity.Property(e => e.PovratneInformacije).HasMaxLength(256);

                entity.HasOne(d => d.AdministratorKorisnikIdKorisnikNavigation)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.AdministratorKorisnikIdKorisnik)
                    .HasConstraintName("fk_PROBLEM_ADMINISTRATOR1");

                entity.HasOne(d => d.AgentKorisnikIdKorisnikNavigation)
                    .WithMany(p => p.Problems)
                    .HasForeignKey(d => d.AgentKorisnikIdKorisnik)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PROBLEM_AGENT1");
            });

            modelBuilder.Entity<Statusiznajmljivanja>(entity =>
            {
                entity.HasKey(e => e.IdStatusIznajmljivanja)
                    .HasName("PRIMARY");

                entity.ToTable("statusiznajmljivanja");

                entity.Property(e => e.IdStatusIznajmljivanja).HasColumnName("idStatusIznajmljivanja");

                entity.Property(e => e.NazivStatusa).HasMaxLength(64);
            });

            modelBuilder.Entity<Telefon>(entity =>
            {
                entity.HasKey(e => new { e.KorisnikIdKorisnik, e.Telefon1 })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("telefon");

                entity.HasIndex(e => e.KorisnikIdKorisnik, "fk_TELEFON_KORISNIK1_idx");

                entity.Property(e => e.KorisnikIdKorisnik).HasColumnName("KORISNIK_idKORISNIK");

                entity.Property(e => e.Telefon1)
                    .HasMaxLength(20)
                    .HasColumnName("Telefon");

                entity.HasOne(d => d.KorisnikIdKorisnikNavigation)
                    .WithMany(p => p.Telefons)
                    .HasForeignKey(d => d.KorisnikIdKorisnik)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_TELEFON_KORISNIK1");
            });

            modelBuilder.Entity<Vozilo>(entity =>
            {
                entity.HasKey(e => e.IdVozilo)
                    .HasName("PRIMARY");

                entity.ToTable("vozilo");

                entity.HasIndex(e => e.KategorijaVozilaIdKategorijaVozila, "fk_VOZILO_KATEGORIJA_VOZILA1_idx");

                entity.Property(e => e.IdVozilo).HasColumnName("idVOZILO");

                entity.Property(e => e.DnevnaTarifa).HasPrecision(5, 2);

                entity.Property(e => e.Godiste).HasColumnType("year");

                entity.Property(e => e.KategorijaVozilaIdKategorijaVozila).HasColumnName("KATEGORIJA_VOZILA_idKATEGORIJA_VOZILA");

                entity.Property(e => e.Marka).HasMaxLength(20);

                entity.Property(e => e.Model).HasMaxLength(20);

                entity.Property(e => e.SedmicnaTarifa).HasPrecision(6, 2);

                entity.HasOne(d => d.KategorijaVozilaIdKategorijaVozilaNavigation)
                    .WithMany(p => p.Vozilos)
                    .HasForeignKey(d => d.KategorijaVozilaIdKategorijaVozila)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_VOZILO_KATEGORIJA_VOZILA1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
