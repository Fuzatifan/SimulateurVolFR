using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Data
{
    /// <summary>
    /// Gestionnaire de base de données pour les aéroports et avions
    /// </summary>
    public class DatabaseManager
    {
        private static DatabaseManager? instance;
        public static DatabaseManager Instance => instance ??= new DatabaseManager();

        private string dbPath;
        private SQLiteConnection? connection;

        private DatabaseManager()
        {
            dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "data", "simulator.db");
            EnsureDatabaseExists();
        }

        /// <summary>
        /// S'assure que la base de données existe
        /// </summary>
        private void EnsureDatabaseExists()
        {
            string? directory = Path.GetDirectoryName(dbPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
                PopulateDefaultData();
            }
        }

        /// <summary>
        /// Ouvre une connexion à la base de données
        /// </summary>
        private SQLiteConnection GetConnection()
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                connection.Open();
            }
            return connection;
        }

        /// <summary>
        /// Crée les tables de la base de données
        /// </summary>
        private void CreateTables()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            // Table des avions
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Aircraft (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Manufacturer TEXT NOT NULL,
                    Type INTEGER NOT NULL,
                    MaxSpeed REAL NOT NULL,
                    CruiseSpeed REAL NOT NULL,
                    MaxAltitude REAL NOT NULL,
                    Range REAL NOT NULL,
                    FuelCapacity REAL NOT NULL,
                    FuelConsumption REAL NOT NULL,
                    PassengerCapacity INTEGER NOT NULL,
                    EmptyWeight REAL NOT NULL,
                    MaxTakeoffWeight REAL NOT NULL,
                    TakeoffSpeed REAL NOT NULL,
                    LandingSpeed REAL NOT NULL,
                    ClimbRate REAL NOT NULL,
                    DescentRate REAL NOT NULL,
                    TurnRate REAL NOT NULL,
                    EngineCount INTEGER NOT NULL,
                    EngineType TEXT NOT NULL,
                    EngineSound TEXT,
                    CabinSound TEXT
                )";
            cmd.ExecuteNonQuery();

            // Table des aéroports
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Airport (
                    IcaoCode TEXT PRIMARY KEY,
                    IataCode TEXT,
                    Name TEXT NOT NULL,
                    City TEXT NOT NULL,
                    Country TEXT NOT NULL,
                    Latitude REAL NOT NULL,
                    Longitude REAL NOT NULL,
                    Elevation REAL NOT NULL,
                    Size INTEGER NOT NULL,
                    HasILS INTEGER NOT NULL,
                    HasTower INTEGER NOT NULL,
                    TowerFrequency TEXT,
                    GroundFrequency TEXT,
                    AtisFrequency TEXT
                )";
            cmd.ExecuteNonQuery();

            // Table des pistes
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Runway (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AirportIcao TEXT NOT NULL,
                    Designation TEXT NOT NULL,
                    Length INTEGER NOT NULL,
                    Width INTEGER NOT NULL,
                    Surface TEXT NOT NULL,
                    HasLights INTEGER NOT NULL,
                    HasILS INTEGER NOT NULL,
                    FOREIGN KEY (AirportIcao) REFERENCES Airport(IcaoCode)
                )";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Remplit la base de données avec des données par défaut
        /// </summary>
        private void PopulateDefaultData()
        {
            // Ajouter des avions par défaut
            AddDefaultAircraft();
            
            // Ajouter des aéroports français par défaut
            AddDefaultAirports();
        }

        /// <summary>
        /// Ajoute des avions par défaut
        /// </summary>
        private void AddDefaultAircraft()
        {
            var aircraft = new List<Aircraft>
            {
                new Aircraft
                {
                    Id = "cessna172",
                    Name = "172 Skyhawk",
                    Manufacturer = "Cessna",
                    Type = AircraftType.LightAircraft,
                    MaxSpeed = 163,
                    CruiseSpeed = 122,
                    MaxAltitude = 14000,
                    Range = 640,
                    FuelCapacity = 212,
                    FuelConsumption = 35,
                    PassengerCapacity = 3,
                    EmptyWeight = 757,
                    MaxTakeoffWeight = 1157,
                    TakeoffSpeed = 60,
                    LandingSpeed = 50,
                    ClimbRate = 730,
                    DescentRate = 500,
                    TurnRate = 3,
                    EngineCount = 1,
                    EngineType = "Lycoming IO-360"
                },
                new Aircraft
                {
                    Id = "a320",
                    Name = "A320",
                    Manufacturer = "Airbus",
                    Type = AircraftType.NarrowBodyAirliner,
                    MaxSpeed = 487,
                    CruiseSpeed = 447,
                    MaxAltitude = 39000,
                    Range = 3300,
                    FuelCapacity = 24210,
                    FuelConsumption = 2400,
                    PassengerCapacity = 180,
                    EmptyWeight = 42600,
                    MaxTakeoffWeight = 78000,
                    TakeoffSpeed = 150,
                    LandingSpeed = 130,
                    ClimbRate = 2500,
                    DescentRate = 1500,
                    TurnRate = 2,
                    EngineCount = 2,
                    EngineType = "CFM56-5B"
                },
                new Aircraft
                {
                    Id = "b737",
                    Name = "737-800",
                    Manufacturer = "Boeing",
                    Type = AircraftType.NarrowBodyAirliner,
                    MaxSpeed = 544,
                    CruiseSpeed = 453,
                    MaxAltitude = 41000,
                    Range = 3115,
                    FuelCapacity = 26020,
                    FuelConsumption = 2500,
                    PassengerCapacity = 189,
                    EmptyWeight = 41413,
                    MaxTakeoffWeight = 79010,
                    TakeoffSpeed = 155,
                    LandingSpeed = 135,
                    ClimbRate = 2600,
                    DescentRate = 1600,
                    TurnRate = 2,
                    EngineCount = 2,
                    EngineType = "CFM56-7B"
                }
            };

            foreach (var ac in aircraft)
            {
                SaveAircraft(ac);
            }
        }

        /// <summary>
        /// Ajoute des aéroports français par défaut
        /// </summary>
        private void AddDefaultAirports()
        {
            var airports = new List<Airport>
            {
                new Airport
                {
                    IcaoCode = "LFPG",
                    IataCode = "CDG",
                    Name = "Aéroport Paris-Charles de Gaulle",
                    City = "Paris",
                    Country = "France",
                    Latitude = 49.0097,
                    Longitude = 2.5479,
                    Elevation = 392,
                    Size = AirportSize.International,
                    HasILS = true,
                    HasTower = true,
                    TowerFrequency = "118.15",
                    GroundFrequency = "121.65",
                    AtisFrequency = "128.475",
                    Runways = new List<Runway>
                    {
                        new Runway { Designation = "08L/26R", Length = 13829, Width = 197, Surface = "Béton", HasLights = true, HasILS = true },
                        new Runway { Designation = "08R/26L", Length = 13829, Width = 197, Surface = "Béton", HasLights = true, HasILS = true }
                    }
                },
                new Airport
                {
                    IcaoCode = "LFPO",
                    IataCode = "ORY",
                    Name = "Aéroport Paris-Orly",
                    City = "Paris",
                    Country = "France",
                    Latitude = 48.7233,
                    Longitude = 2.3794,
                    Elevation = 292,
                    Size = AirportSize.International,
                    HasILS = true,
                    HasTower = true,
                    TowerFrequency = "118.70",
                    GroundFrequency = "121.70",
                    AtisFrequency = "127.15",
                    Runways = new List<Runway>
                    {
                        new Runway { Designation = "06/24", Length = 11975, Width = 148, Surface = "Béton", HasLights = true, HasILS = true },
                        new Runway { Designation = "07/25", Length = 7874, Width = 148, Surface = "Béton", HasLights = true, HasILS = true }
                    }
                },
                new Airport
                {
                    IcaoCode = "LFML",
                    IataCode = "MRS",
                    Name = "Aéroport Marseille Provence",
                    City = "Marseille",
                    Country = "France",
                    Latitude = 43.4393,
                    Longitude = 5.2214,
                    Elevation = 74,
                    Size = AirportSize.International,
                    HasILS = true,
                    HasTower = true,
                    TowerFrequency = "119.10",
                    GroundFrequency = "121.75",
                    AtisFrequency = "126.40",
                    Runways = new List<Runway>
                    {
                        new Runway { Designation = "13L/31R", Length = 11483, Width = 148, Surface = "Asphalte", HasLights = true, HasILS = true },
                        new Runway { Designation = "13R/31L", Length = 8530, Width = 148, Surface = "Asphalte", HasLights = true, HasILS = true }
                    }
                },
                new Airport
                {
                    IcaoCode = "LFLL",
                    IataCode = "LYS",
                    Name = "Aéroport Lyon-Saint Exupéry",
                    City = "Lyon",
                    Country = "France",
                    Latitude = 45.7256,
                    Longitude = 5.0811,
                    Elevation = 821,
                    Size = AirportSize.International,
                    HasILS = true,
                    HasTower = true,
                    TowerFrequency = "118.10",
                    GroundFrequency = "121.80",
                    AtisFrequency = "127.95",
                    Runways = new List<Runway>
                    {
                        new Runway { Designation = "17L/35R", Length = 13123, Width = 148, Surface = "Béton", HasLights = true, HasILS = true },
                        new Runway { Designation = "17R/35L", Length = 9186, Width = 148, Surface = "Béton", HasLights = true, HasILS = true }
                    }
                },
                new Airport
                {
                    IcaoCode = "LFBD",
                    IataCode = "BOD",
                    Name = "Aéroport de Bordeaux-Mérignac",
                    City = "Bordeaux",
                    Country = "France",
                    Latitude = 44.8283,
                    Longitude = -0.7153,
                    Elevation = 162,
                    Size = AirportSize.Large,
                    HasILS = true,
                    HasTower = true,
                    TowerFrequency = "118.30",
                    GroundFrequency = "121.90",
                    AtisFrequency = "126.15",
                    Runways = new List<Runway>
                    {
                        new Runway { Designation = "05/23", Length = 10499, Width = 148, Surface = "Asphalte", HasLights = true, HasILS = true },
                        new Runway { Designation = "11/29", Length = 8366, Width = 148, Surface = "Asphalte", HasLights = true, HasILS = true }
                    }
                }
            };

            foreach (var airport in airports)
            {
                SaveAirport(airport);
            }
        }

        /// <summary>
        /// Sauvegarde un avion dans la base de données
        /// </summary>
        public void SaveAircraft(Aircraft aircraft)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT OR REPLACE INTO Aircraft 
                (Id, Name, Manufacturer, Type, MaxSpeed, CruiseSpeed, MaxAltitude, Range, 
                 FuelCapacity, FuelConsumption, PassengerCapacity, EmptyWeight, MaxTakeoffWeight,
                 TakeoffSpeed, LandingSpeed, ClimbRate, DescentRate, TurnRate, EngineCount, EngineType,
                 EngineSound, CabinSound)
                VALUES (@Id, @Name, @Manufacturer, @Type, @MaxSpeed, @CruiseSpeed, @MaxAltitude, @Range,
                        @FuelCapacity, @FuelConsumption, @PassengerCapacity, @EmptyWeight, @MaxTakeoffWeight,
                        @TakeoffSpeed, @LandingSpeed, @ClimbRate, @DescentRate, @TurnRate, @EngineCount, @EngineType,
                        @EngineSound, @CabinSound)";

            cmd.Parameters.AddWithValue("@Id", aircraft.Id);
            cmd.Parameters.AddWithValue("@Name", aircraft.Name);
            cmd.Parameters.AddWithValue("@Manufacturer", aircraft.Manufacturer);
            cmd.Parameters.AddWithValue("@Type", (int)aircraft.Type);
            cmd.Parameters.AddWithValue("@MaxSpeed", aircraft.MaxSpeed);
            cmd.Parameters.AddWithValue("@CruiseSpeed", aircraft.CruiseSpeed);
            cmd.Parameters.AddWithValue("@MaxAltitude", aircraft.MaxAltitude);
            cmd.Parameters.AddWithValue("@Range", aircraft.Range);
            cmd.Parameters.AddWithValue("@FuelCapacity", aircraft.FuelCapacity);
            cmd.Parameters.AddWithValue("@FuelConsumption", aircraft.FuelConsumption);
            cmd.Parameters.AddWithValue("@PassengerCapacity", aircraft.PassengerCapacity);
            cmd.Parameters.AddWithValue("@EmptyWeight", aircraft.EmptyWeight);
            cmd.Parameters.AddWithValue("@MaxTakeoffWeight", aircraft.MaxTakeoffWeight);
            cmd.Parameters.AddWithValue("@TakeoffSpeed", aircraft.TakeoffSpeed);
            cmd.Parameters.AddWithValue("@LandingSpeed", aircraft.LandingSpeed);
            cmd.Parameters.AddWithValue("@ClimbRate", aircraft.ClimbRate);
            cmd.Parameters.AddWithValue("@DescentRate", aircraft.DescentRate);
            cmd.Parameters.AddWithValue("@TurnRate", aircraft.TurnRate);
            cmd.Parameters.AddWithValue("@EngineCount", aircraft.EngineCount);
            cmd.Parameters.AddWithValue("@EngineType", aircraft.EngineType);
            cmd.Parameters.AddWithValue("@EngineSound", aircraft.EngineSound ?? "");
            cmd.Parameters.AddWithValue("@CabinSound", aircraft.CabinSound ?? "");

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Sauvegarde un aéroport dans la base de données
        /// </summary>
        public void SaveAirport(Airport airport)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT OR REPLACE INTO Airport 
                (IcaoCode, IataCode, Name, City, Country, Latitude, Longitude, Elevation, Size,
                 HasILS, HasTower, TowerFrequency, GroundFrequency, AtisFrequency)
                VALUES (@IcaoCode, @IataCode, @Name, @City, @Country, @Latitude, @Longitude, @Elevation, @Size,
                        @HasILS, @HasTower, @TowerFrequency, @GroundFrequency, @AtisFrequency)";

            cmd.Parameters.AddWithValue("@IcaoCode", airport.IcaoCode);
            cmd.Parameters.AddWithValue("@IataCode", airport.IataCode ?? "");
            cmd.Parameters.AddWithValue("@Name", airport.Name);
            cmd.Parameters.AddWithValue("@City", airport.City);
            cmd.Parameters.AddWithValue("@Country", airport.Country);
            cmd.Parameters.AddWithValue("@Latitude", airport.Latitude);
            cmd.Parameters.AddWithValue("@Longitude", airport.Longitude);
            cmd.Parameters.AddWithValue("@Elevation", airport.Elevation);
            cmd.Parameters.AddWithValue("@Size", (int)airport.Size);
            cmd.Parameters.AddWithValue("@HasILS", airport.HasILS ? 1 : 0);
            cmd.Parameters.AddWithValue("@HasTower", airport.HasTower ? 1 : 0);
            cmd.Parameters.AddWithValue("@TowerFrequency", airport.TowerFrequency ?? "");
            cmd.Parameters.AddWithValue("@GroundFrequency", airport.GroundFrequency ?? "");
            cmd.Parameters.AddWithValue("@AtisFrequency", airport.AtisFrequency ?? "");

            cmd.ExecuteNonQuery();

            // Sauvegarder les pistes
            foreach (var runway in airport.Runways)
            {
                SaveRunway(airport.IcaoCode, runway);
            }
        }

        /// <summary>
        /// Sauvegarde une piste dans la base de données
        /// </summary>
        private void SaveRunway(string airportIcao, Runway runway)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO Runway (AirportIcao, Designation, Length, Width, Surface, HasLights, HasILS)
                VALUES (@AirportIcao, @Designation, @Length, @Width, @Surface, @HasLights, @HasILS)";

            cmd.Parameters.AddWithValue("@AirportIcao", airportIcao);
            cmd.Parameters.AddWithValue("@Designation", runway.Designation);
            cmd.Parameters.AddWithValue("@Length", runway.Length);
            cmd.Parameters.AddWithValue("@Width", runway.Width);
            cmd.Parameters.AddWithValue("@Surface", runway.Surface);
            cmd.Parameters.AddWithValue("@HasLights", runway.HasLights ? 1 : 0);
            cmd.Parameters.AddWithValue("@HasILS", runway.HasILS ? 1 : 0);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Récupère tous les avions
        /// </summary>
        public List<Aircraft> GetAllAircraft()
        {
            var aircraft = new List<Aircraft>();

            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Aircraft ORDER BY Manufacturer, Name";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                aircraft.Add(ReadAircraft(reader));
            }

            return aircraft;
        }

        /// <summary>
        /// Récupère tous les aéroports
        /// </summary>
        public List<Airport> GetAllAirports()
        {
            var airports = new List<Airport>();

            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Airport ORDER BY Country, City, Name";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var airport = ReadAirport(reader);
                airport.Runways = GetRunwaysForAirport(airport.IcaoCode);
                airports.Add(airport);
            }

            return airports;
        }

        /// <summary>
        /// Récupère les pistes d'un aéroport
        /// </summary>
        private List<Runway> GetRunwaysForAirport(string icaoCode)
        {
            var runways = new List<Runway>();

            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Runway WHERE AirportIcao = @IcaoCode";
            cmd.Parameters.AddWithValue("@IcaoCode", icaoCode);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                runways.Add(ReadRunway(reader));
            }

            return runways;
        }

        private Aircraft ReadAircraft(SQLiteDataReader reader)
        {
            return new Aircraft
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                Manufacturer = reader.GetString(2),
                Type = (AircraftType)reader.GetInt32(3),
                MaxSpeed = reader.GetDouble(4),
                CruiseSpeed = reader.GetDouble(5),
                MaxAltitude = reader.GetDouble(6),
                Range = reader.GetDouble(7),
                FuelCapacity = reader.GetDouble(8),
                FuelConsumption = reader.GetDouble(9),
                PassengerCapacity = reader.GetInt32(10),
                EmptyWeight = reader.GetDouble(11),
                MaxTakeoffWeight = reader.GetDouble(12),
                TakeoffSpeed = reader.GetDouble(13),
                LandingSpeed = reader.GetDouble(14),
                ClimbRate = reader.GetDouble(15),
                DescentRate = reader.GetDouble(16),
                TurnRate = reader.GetDouble(17),
                EngineCount = reader.GetInt32(18),
                EngineType = reader.GetString(19),
                EngineSound = reader.IsDBNull(20) ? "" : reader.GetString(20),
                CabinSound = reader.IsDBNull(21) ? "" : reader.GetString(21)
            };
        }

        private Airport ReadAirport(SQLiteDataReader reader)
        {
            return new Airport
            {
                IcaoCode = reader.GetString(0),
                IataCode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                Name = reader.GetString(2),
                City = reader.GetString(3),
                Country = reader.GetString(4),
                Latitude = reader.GetDouble(5),
                Longitude = reader.GetDouble(6),
                Elevation = reader.GetDouble(7),
                Size = (AirportSize)reader.GetInt32(8),
                HasILS = reader.GetInt32(9) == 1,
                HasTower = reader.GetInt32(10) == 1,
                TowerFrequency = reader.IsDBNull(11) ? "" : reader.GetString(11),
                GroundFrequency = reader.IsDBNull(12) ? "" : reader.GetString(12),
                AtisFrequency = reader.IsDBNull(13) ? "" : reader.GetString(13)
            };
        }

        private Runway ReadRunway(SQLiteDataReader reader)
        {
            return new Runway
            {
                Designation = reader.GetString(2),
                Length = reader.GetInt32(3),
                Width = reader.GetInt32(4),
                Surface = reader.GetString(5),
                HasLights = reader.GetInt32(6) == 1,
                HasILS = reader.GetInt32(7) == 1
            };
        }
    }
}

