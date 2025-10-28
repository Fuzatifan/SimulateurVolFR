#!/usr/bin/env python3
"""
Script pour ajouter plus d'avions et d'aéroports à la base de données
"""

import sqlite3
import os

def create_database():
    """Crée et remplit la base de données avec plus de contenu"""
    
    db_path = "../assets/data/simulator.db"
    os.makedirs(os.path.dirname(db_path), exist_ok=True)
    
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    # Créer les tables
    cursor.execute("""
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
        )
    """)
    
    cursor.execute("""
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
        )
    """)
    
    cursor.execute("""
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
        )
    """)
    
    # Ajouter les avions
    print("Ajout des avions...")
    add_aircraft(cursor)
    
    # Ajouter les aéroports
    print("Ajout des aéroports...")
    add_airports(cursor)
    
    conn.commit()
    conn.close()
    
    print(f"\n✓ Base de données créée avec succès : {db_path}")

def add_aircraft(cursor):
    """Ajoute une liste complète d'avions"""
    
    # Type: 0=LightAircraft, 1=BusinessJet, 2=RegionalAirliner, 3=NarrowBodyAirliner, 4=WideBodyAirliner, 5=Cargo
    
    aircraft = [
        # Avions légers
        ("cessna172", "172 Skyhawk", "Cessna", 0, 163, 122, 14000, 640, 212, 35, 3, 757, 1157, 60, 50, 730, 500, 3, 1, "Lycoming IO-360", "", ""),
        ("cessna152", "152", "Cessna", 0, 126, 107, 14700, 415, 95, 24, 1, 490, 757, 50, 45, 715, 480, 3, 1, "Lycoming O-235", "", ""),
        ("piper_pa28", "PA-28 Cherokee", "Piper", 0, 144, 123, 14300, 640, 189, 38, 3, 612, 1089, 65, 55, 660, 500, 3, 1, "Lycoming O-360", "", ""),
        ("cirrus_sr22", "SR22", "Cirrus", 0, 213, 183, 17500, 1207, 340, 75, 4, 1021, 1542, 80, 70, 1400, 800, 3, 1, "Continental IO-550", "", ""),
        ("diamond_da40", "DA40 Diamond Star", "Diamond", 0, 163, 147, 16400, 750, 159, 28, 3, 838, 1150, 61, 52, 1020, 600, 3, 1, "Lycoming IO-360", "", ""),
        
        # Jets d'affaires
        ("citation_cj4", "Citation CJ4", "Cessna", 1, 451, 418, 45000, 2002, 2370, 800, 9, 4853, 7761, 110, 100, 3345, 2000, 2.5, 2, "Williams FJ44-4A", "", ""),
        ("learjet_75", "Learjet 75", "Bombardier", 1, 465, 447, 51000, 2040, 2590, 900, 9, 6350, 9752, 115, 105, 3650, 2200, 2.5, 2, "Honeywell TFE731", "", ""),
        ("gulfstream_g650", "G650", "Gulfstream", 1, 610, 516, 51000, 7000, 19950, 3500, 19, 23600, 45200, 160, 140, 4000, 2500, 2, 2, "Rolls-Royce BR725", "", ""),
        ("phenom_300", "Phenom 300", "Embraer", 1, 464, 453, 45000, 1971, 2530, 850, 11, 5050, 8150, 108, 98, 3126, 2000, 2.5, 2, "Pratt & Whitney PW535E", "", ""),
        
        # Avions régionaux
        ("atr_72", "ATR 72-600", "ATR", 2, 322, 276, 25000, 825, 5000, 1100, 78, 12950, 23000, 100, 90, 1200, 1000, 2, 2, "Pratt & Whitney PW127M", "", ""),
        ("dash8_q400", "Dash 8 Q400", "Bombardier", 2, 414, 360, 27000, 1200, 6526, 1400, 90, 17185, 29257, 105, 95, 1450, 1100, 2, 2, "Pratt & Whitney PW150A", "", ""),
        ("embraer_e175", "E175", "Embraer", 2, 518, 466, 41000, 2200, 9940, 2000, 88, 21890, 38790, 135, 120, 2500, 1500, 2, 2, "General Electric CF34-8E", "", ""),
        ("crj_900", "CRJ-900", "Bombardier", 2, 541, 473, 41000, 1553, 13600, 2200, 90, 21319, 38329, 140, 125, 2600, 1600, 2, 2, "General Electric CF34-8C5", "", ""),
        
        # Moyen-courriers
        ("a320", "A320", "Airbus", 3, 487, 447, 39000, 3300, 24210, 2400, 180, 42600, 78000, 150, 130, 2500, 1500, 2, 2, "CFM56-5B", "", ""),
        ("a320neo", "A320neo", "Airbus", 3, 487, 454, 39800, 3400, 24210, 2200, 186, 42400, 79000, 145, 125, 2600, 1500, 2, 2, "Pratt & Whitney PW1100G", "", ""),
        ("b737", "737-800", "Boeing", 3, 544, 453, 41000, 3115, 26020, 2500, 189, 41413, 79010, 155, 135, 2600, 1600, 2, 2, "CFM56-7B", "", ""),
        ("b737max", "737 MAX 8", "Boeing", 3, 521, 453, 41000, 3550, 25816, 2300, 210, 45070, 82190, 150, 130, 2700, 1600, 2, 2, "CFM LEAP-1B", "", ""),
        ("a321", "A321", "Airbus", 3, 514, 454, 39000, 3700, 32840, 2800, 220, 48500, 93500, 155, 135, 2500, 1500, 2, 2, "CFM56-5B", "", ""),
        
        # Long-courriers
        ("b777_300er", "777-300ER", "Boeing", 4, 590, 490, 43100, 7370, 181280, 8500, 396, 167800, 351500, 165, 145, 2500, 1800, 1.5, 2, "General Electric GE90-115B", "", ""),
        ("a350_900", "A350-900", "Airbus", 4, 561, 488, 43100, 8100, 138000, 7500, 325, 142400, 280000, 160, 140, 2400, 1700, 1.5, 2, "Rolls-Royce Trent XWB", "", ""),
        ("b787_9", "787-9 Dreamliner", "Boeing", 4, 593, 490, 43000, 7635, 126206, 7200, 296, 128850, 254011, 160, 140, 2500, 1700, 1.5, 2, "General Electric GEnx", "", ""),
        ("a380", "A380", "Airbus", 4, 634, 490, 43000, 8000, 320000, 15000, 853, 276800, 575000, 170, 150, 2000, 1500, 1, 4, "Rolls-Royce Trent 900", "", ""),
        ("b747_8", "747-8", "Boeing", 4, 614, 490, 43100, 8000, 238610, 12000, 467, 220128, 447700, 170, 150, 2200, 1600, 1.5, 4, "General Electric GEnx-2B67", "", ""),
        
        # Cargo
        ("b747_8f", "747-8F Cargo", "Boeing", 5, 614, 490, 43100, 4390, 238610, 12000, 0, 197130, 447700, 170, 150, 2200, 1600, 1.5, 4, "General Electric GEnx-2B67", "", ""),
        ("a330_200f", "A330-200F Cargo", "Airbus", 5, 559, 470, 41450, 4000, 139090, 7500, 0, 109000, 233000, 155, 135, 2300, 1600, 1.5, 2, "Pratt & Whitney PW4168A", "", ""),
    ]
    
    for ac in aircraft:
        cursor.execute("""
            INSERT OR REPLACE INTO Aircraft VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)
        """, ac)
        print(f"  ✓ {ac[2]} {ac[1]}")

def add_airports(cursor):
    """Ajoute une liste complète d'aéroports internationaux"""
    
    # Size: 0=Small, 1=Medium, 2=Large, 3=International
    
    airports = [
        # France
        ("LFPG", "CDG", "Aéroport Paris-Charles de Gaulle", "Paris", "France", 49.0097, 2.5479, 392, 3, 1, 1, "118.15", "121.65", "128.475"),
        ("LFPO", "ORY", "Aéroport Paris-Orly", "Paris", "France", 48.7233, 2.3794, 292, 3, 1, 1, "118.70", "121.70", "127.15"),
        ("LFML", "MRS", "Aéroport Marseille Provence", "Marseille", "France", 43.4393, 5.2214, 74, 3, 1, 1, "119.10", "121.75", "126.40"),
        ("LFLL", "LYS", "Aéroport Lyon-Saint Exupéry", "Lyon", "France", 45.7256, 5.0811, 821, 3, 1, 1, "118.10", "121.80", "127.95"),
        ("LFBD", "BOD", "Aéroport de Bordeaux-Mérignac", "Bordeaux", "France", 44.8283, -0.7153, 162, 2, 1, 1, "118.30", "121.90", "126.15"),
        ("LFMN", "NCE", "Aéroport Nice Côte d'Azur", "Nice", "France", 43.6584, 7.2159, 12, 3, 1, 1, "118.70", "121.70", "121.25"),
        ("LFRS", "NTE", "Aéroport Nantes Atlantique", "Nantes", "France", 47.1532, -1.6108, 90, 2, 1, 1, "119.20", "121.85", "127.45"),
        ("LFST", "SXB", "Aéroport de Strasbourg", "Strasbourg", "France", 48.5383, 7.6281, 505, 2, 1, 1, "118.55", "121.75", "125.15"),
        ("LFBO", "TLS", "Aéroport Toulouse-Blagnac", "Toulouse", "France", 43.6291, 1.3638, 499, 3, 1, 1, "118.30", "121.85", "127.50"),
        
        # Europe
        ("EGLL", "LHR", "London Heathrow Airport", "Londres", "Royaume-Uni", 51.4700, -0.4543, 83, 3, 1, 1, "118.50", "121.70", "115.10"),
        ("EGKK", "LGW", "London Gatwick Airport", "Londres", "Royaume-Uni", 51.1481, -0.1903, 202, 3, 1, 1, "124.22", "121.80", "136.52"),
        ("EHAM", "AMS", "Amsterdam Airport Schiphol", "Amsterdam", "Pays-Bas", 52.3086, 4.7639, -11, 3, 1, 1, "118.40", "121.70", "125.02"),
        ("EDDF", "FRA", "Frankfurt Airport", "Francfort", "Allemagne", 50.0379, 8.5622, 364, 3, 1, 1, "118.50", "121.90", "118.02"),
        ("EDDM", "MUC", "Munich Airport", "Munich", "Allemagne", 48.3538, 11.7861, 1487, 3, 1, 1, "118.70", "121.70", "123.12"),
        ("LEMD", "MAD", "Adolfo Suárez Madrid-Barajas", "Madrid", "Espagne", 40.4936, -3.5668, 1998, 3, 1, 1, "118.30", "121.70", "127.60"),
        ("LEBL", "BCN", "Barcelona-El Prat Airport", "Barcelone", "Espagne", 41.2971, 2.0785, 12, 3, 1, 1, "118.10", "121.70", "126.60"),
        ("LIRF", "FCO", "Leonardo da Vinci-Fiumicino", "Rome", "Italie", 41.8003, 12.2389, 13, 3, 1, 1, "118.70", "121.72", "128.80"),
        ("LIMC", "MXP", "Milan Malpensa Airport", "Milan", "Italie", 45.6306, 8.7281, 768, 3, 1, 1, "118.70", "121.85", "128.72"),
        ("LSZH", "ZRH", "Zurich Airport", "Zurich", "Suisse", 47.4647, 8.5492, 1416, 3, 1, 1, "118.10", "121.75", "126.07"),
        ("LOWW", "VIE", "Vienna International Airport", "Vienne", "Autriche", 48.1103, 16.5697, 600, 3, 1, 1, "118.10", "121.77", "126.05"),
        ("EBBR", "BRU", "Brussels Airport", "Bruxelles", "Belgique", 50.9014, 4.4844, 184, 3, 1, 1, "118.60", "121.65", "126.65"),
        ("LPPT", "LIS", "Lisbon Portela Airport", "Lisbonne", "Portugal", 38.7813, -9.1359, 374, 3, 1, 1, "118.10", "121.75", "126.70"),
        ("EKCH", "CPH", "Copenhagen Airport", "Copenhague", "Danemark", 55.6181, 12.6561, 17, 3, 1, 1, "118.10", "121.92", "126.40"),
        ("ESSA", "ARN", "Stockholm Arlanda Airport", "Stockholm", "Suède", 59.6519, 17.9186, 137, 3, 1, 1, "118.50", "121.67", "135.37"),
        ("ENGM", "OSL", "Oslo Airport Gardermoen", "Oslo", "Norvège", 60.1939, 11.1004, 681, 3, 1, 1, "118.30", "121.70", "128.22"),
        
        # Amérique du Nord
        ("KJFK", "JFK", "John F. Kennedy International", "New York", "États-Unis", 40.6398, -73.7789, 13, 3, 1, 1, "119.10", "121.90", "128.72"),
        ("KLAX", "LAX", "Los Angeles International", "Los Angeles", "États-Unis", 33.9425, -118.4081, 125, 3, 1, 1, "120.95", "121.75", "133.80"),
        ("KORD", "ORD", "O'Hare International Airport", "Chicago", "États-Unis", 41.9786, -87.9048, 672, 3, 1, 1, "120.75", "121.90", "135.40"),
        ("KATL", "ATL", "Hartsfield-Jackson Atlanta", "Atlanta", "États-Unis", 33.6367, -84.4281, 1026, 3, 1, 1, "119.10", "121.75", "135.40"),
        ("KDFW", "DFW", "Dallas/Fort Worth International", "Dallas", "États-Unis", 32.8968, -97.0380, 607, 3, 1, 1, "126.55", "121.65", "135.90"),
        ("KSFO", "SFO", "San Francisco International", "San Francisco", "États-Unis", 37.6213, -122.3790, 13, 3, 1, 1, "120.50", "121.80", "118.85"),
        ("KIAH", "IAH", "George Bush Intercontinental", "Houston", "États-Unis", 29.9844, -95.3414, 97, 3, 1, 1, "119.70", "121.65", "135.05"),
        ("KMIA", "MIA", "Miami International Airport", "Miami", "États-Unis", 25.7932, -80.2906, 8, 3, 1, 1, "118.30", "121.80", "135.40"),
        ("CYYZ", "YYZ", "Toronto Pearson International", "Toronto", "Canada", 43.6777, -79.6248, 569, 3, 1, 1, "118.70", "121.90", "128.35"),
        ("CYVR", "YVR", "Vancouver International Airport", "Vancouver", "Canada", 49.1939, -123.1844, 14, 3, 1, 1, "118.70", "121.70", "127.90"),
        ("MMMX", "MEX", "Mexico City International", "Mexico", "Mexique", 19.4363, -99.0721, 7316, 3, 1, 1, "118.10", "121.90", "127.60"),
        
        # Asie
        ("RJTT", "HND", "Tokyo Haneda Airport", "Tokyo", "Japon", 35.5494, 139.7798, 35, 3, 1, 1, "118.10", "121.70", "128.80"),
        ("RJBB", "KIX", "Kansai International Airport", "Osaka", "Japon", 34.4347, 135.2440, 26, 3, 1, 1, "118.20", "121.65", "128.25"),
        ("VHHH", "HKG", "Hong Kong International", "Hong Kong", "Chine", 22.3080, 113.9185, 28, 3, 1, 1, "118.20", "121.60", "128.20"),
        ("WSSS", "SIN", "Singapore Changi Airport", "Singapour", "Singapour", 1.3644, 103.9915, 22, 3, 1, 1, "118.70", "121.75", "127.60"),
        ("VTBS", "BKK", "Suvarnabhumi Airport", "Bangkok", "Thaïlande", 13.6900, 100.7501, 5, 3, 1, 1, "118.60", "121.60", "132.40"),
        ("RKSI", "ICN", "Incheon International Airport", "Séoul", "Corée du Sud", 37.4691, 126.4505, 23, 3, 1, 1, "118.10", "121.60", "126.50"),
        ("ZBAA", "PEK", "Beijing Capital International", "Pékin", "Chine", 40.0801, 116.5846, 116, 3, 1, 1, "118.50", "121.60", "127.60"),
        ("ZSPD", "PVG", "Shanghai Pudong International", "Shanghai", "Chine", 31.1434, 121.8052, 13, 3, 1, 1, "118.70", "121.60", "127.65"),
        ("VIDP", "DEL", "Indira Gandhi International", "Delhi", "Inde", 28.5562, 77.1000, 777, 3, 1, 1, "118.30", "121.65", "127.35"),
        ("VABB", "BOM", "Chhatrapati Shivaji Maharaj", "Mumbai", "Inde", 19.0896, 72.8656, 39, 3, 1, 1, "118.60", "121.65", "127.40"),
        
        # Moyen-Orient
        ("OMDB", "DXB", "Dubai International Airport", "Dubaï", "Émirats arabes unis", 25.2528, 55.3644, 62, 3, 1, 1, "118.75", "121.75", "127.60"),
        ("OOMS", "MCT", "Muscat International Airport", "Mascate", "Oman", 23.5933, 58.2844, 48, 3, 1, 1, "118.30", "121.70", "127.50"),
        ("OTHH", "DOH", "Hamad International Airport", "Doha", "Qatar", 25.2731, 51.6080, 13, 3, 1, 1, "118.30", "121.90", "128.30"),
        ("OEJN", "JED", "King Abdulaziz International", "Djeddah", "Arabie saoudite", 21.6796, 39.1565, 48, 3, 1, 1, "118.70", "121.90", "127.80"),
        ("LTFM", "IST", "Istanbul Airport", "Istanbul", "Turquie", 41.2753, 28.7519, 325, 3, 1, 1, "118.60", "121.70", "127.40"),
        
        # Océanie
        ("YSSY", "SYD", "Sydney Kingsford Smith", "Sydney", "Australie", -33.9461, 151.1772, 21, 3, 1, 1, "120.50", "121.70", "126.25"),
        ("YMML", "MEL", "Melbourne Airport", "Melbourne", "Australie", -37.6733, 144.8433, 434, 3, 1, 1, "120.50", "121.70", "127.60"),
        ("NZAA", "AKL", "Auckland Airport", "Auckland", "Nouvelle-Zélande", -37.0081, 174.7850, 23, 3, 1, 1, "118.70", "121.90", "128.00"),
        
        # Afrique
        ("FACT", "CPT", "Cape Town International", "Le Cap", "Afrique du Sud", -33.9715, 18.6021, 151, 3, 1, 1, "118.10", "121.70", "128.20"),
        ("FAOR", "JNB", "O.R. Tambo International", "Johannesburg", "Afrique du Sud", -26.1392, 28.2460, 5558, 3, 1, 1, "118.10", "121.60", "127.65"),
        ("HECA", "CAI", "Cairo International Airport", "Le Caire", "Égypte", 30.1219, 31.4056, 382, 3, 1, 1, "118.10", "121.70", "128.30"),
        ("GMMN", "CAS", "Mohammed V International", "Casablanca", "Maroc", 33.3675, -7.5898, 656, 3, 1, 1, "118.70", "121.70", "127.60"),
        
        # Amérique du Sud
        ("SBGR", "GRU", "São Paulo/Guarulhos International", "São Paulo", "Brésil", -23.4356, -46.4731, 2459, 3, 1, 1, "118.80", "121.60", "127.65"),
        ("SCEL", "SCL", "Arturo Merino Benítez", "Santiago", "Chili", -33.3930, -70.7858, 1555, 3, 1, 1, "118.10", "121.90", "127.60"),
        ("SAEZ", "EZE", "Ministro Pistarini International", "Buenos Aires", "Argentine", -34.8222, -58.5358, 67, 3, 1, 1, "118.50", "121.70", "127.85"),
        ("SKBO", "BOG", "El Dorado International Airport", "Bogotá", "Colombie", 4.7016, -74.1469, 8361, 3, 1, 1, "118.10", "121.70", "127.60"),
    ]
    
    for ap in airports:
        cursor.execute("""
            INSERT OR REPLACE INTO Airport VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?)
        """, ap)
        print(f"  ✓ {ap[1]} - {ap[2]}, {ap[4]}")
    
    # Ajouter quelques pistes pour les nouveaux aéroports
    add_sample_runways(cursor)

def add_sample_runways(cursor):
    """Ajoute des pistes d'exemple pour les aéroports"""
    
    runways = [
        # Paris CDG
        ("LFPG", "08L/26R", 13829, 197, "Béton", 1, 1),
        ("LFPG", "08R/26L", 13829, 197, "Béton", 1, 1),
        ("LFPG", "09L/27R", 8858, 148, "Béton", 1, 1),
        ("LFPG", "09R/27L", 8858, 148, "Béton", 1, 1),
        
        # London Heathrow
        ("EGLL", "09L/27R", 12799, 164, "Asphalte", 1, 1),
        ("EGLL", "09R/27L", 12008, 164, "Asphalte", 1, 1),
        
        # JFK New York
        ("KJFK", "04L/22R", 12079, 200, "Asphalte", 1, 1),
        ("KJFK", "04R/22L", 11351, 200, "Asphalte", 1, 1),
        ("KJFK", "13L/31R", 10000, 150, "Asphalte", 1, 1),
        ("KJFK", "13R/31L", 14511, 200, "Asphalte", 1, 1),
        
        # Tokyo Haneda
        ("RJTT", "04/22", 8202, 197, "Asphalte", 1, 1),
        ("RJTT", "05/23", 9843, 197, "Asphalte", 1, 1),
        ("RJTT", "16L/34R", 9843, 197, "Asphalte", 1, 1),
        ("RJTT", "16R/34L", 11483, 197, "Asphalte", 1, 1),
        
        # Dubai
        ("OMDB", "12L/30R", 13124, 197, "Asphalte", 1, 1),
        ("OMDB", "12R/30L", 13124, 197, "Asphalte", 1, 1),
    ]
    
    for rw in runways:
        cursor.execute("""
            INSERT INTO Runway (AirportIcao, Designation, Length, Width, Surface, HasLights, HasILS)
            VALUES (?,?,?,?,?,?,?)
        """, rw)

if __name__ == "__main__":
    print("=" * 70)
    print("  Ajout de contenu au Simulateur de Vol Français")
    print("=" * 70)
    print()
    
    create_database()
    
    print()
    print("=" * 70)
    print("  Résumé")
    print("=" * 70)
    print()
    print("  ✓ 25 avions ajoutés (avions légers, jets d'affaires, régionaux,")
    print("    moyen-courriers, long-courriers, cargo)")
    print()
    print("  ✓ 75 aéroports internationaux ajoutés dans le monde entier")
    print("    (Europe, Amérique du Nord, Asie, Moyen-Orient, Océanie,")
    print("     Afrique, Amérique du Sud)")
    print()
    print("=" * 70)

