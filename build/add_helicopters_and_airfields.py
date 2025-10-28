#!/usr/bin/env python3
"""
Script pour ajouter des hélicoptères et des aérodromes au simulateur
"""

import sqlite3

def add_helicopters_and_airfields():
    """Ajoute des hélicoptères et des aérodromes à la base de données"""
    
    db_path = "../assets/data/simulator.db"
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    print("=" * 70)
    print("  Ajout d'hélicoptères et d'aérodromes")
    print("=" * 70)
    print()
    
    # Ajouter les hélicoptères
    print("Ajout des hélicoptères...")
    add_helicopters(cursor)
    
    # Ajouter les aérodromes
    print("\nAjout des aérodromes...")
    add_airfields(cursor)
    
    conn.commit()
    conn.close()
    
    print("\n✓ Base de données mise à jour avec succès!")

def add_helicopters(cursor):
    """Ajoute une liste d'hélicoptères"""
    
    # Type: 6=Helicopter
    
    helicopters = [
        # Hélicoptères légers
        ("r22", "R22 Beta II", "Robinson", 6, 102, 96, 14000, 200, 75, 30, 1, 880, 1370, 0, 0, 1000, 800, 5, 1, "Lycoming O-360", "", ""),
        ("r44", "R44 Raven II", "Robinson", 6, 130, 110, 14000, 300, 120, 40, 3, 1500, 2500, 0, 0, 1000, 800, 5, 1, "Lycoming IO-540", "", ""),
        ("r66", "R66 Turbine", "Robinson", 6, 140, 120, 14000, 350, 300, 80, 4, 2100, 3000, 0, 0, 1200, 900, 5, 1, "Rolls-Royce RR300", "", ""),
        
        # Hélicoptères utilitaires
        ("as350", "AS350 Écureuil", "Airbus", 6, 140, 122, 15000, 360, 640, 150, 5, 2250, 4960, 0, 0, 1400, 1000, 5, 1, "Turbomeca Arriel 2D", "", ""),
        ("bell206", "206 JetRanger", "Bell", 6, 130, 110, 13500, 370, 435, 120, 4, 1700, 3200, 0, 0, 1200, 900, 5, 1, "Allison 250-C20J", "", ""),
        ("bell407", "407", "Bell", 6, 140, 120, 20000, 350, 505, 130, 6, 2300, 5250, 0, 0, 1400, 1000, 5, 1, "Rolls-Royce 250-C47B", "", ""),
        ("ec135", "EC135", "Airbus", 6, 150, 130, 20000, 370, 710, 160, 6, 3350, 6350, 0, 0, 1500, 1100, 5, 2, "Pratt & Whitney PW206B2", "", ""),
        
        # Hélicoptères bimoteurs moyens
        ("as365", "AS365 Dauphin", "Airbus", 6, 165, 140, 15000, 500, 1400, 300, 12, 4750, 9350, 0, 0, 1600, 1200, 4, 2, "Turbomeca Arriel 2C", "", ""),
        ("bell412", "412", "Bell", 6, 140, 122, 20000, 420, 1200, 250, 13, 6100, 11900, 0, 0, 1400, 1000, 4, 1, "Pratt & Whitney PT6T-3D", "", ""),
        ("aw139", "AW139", "Leonardo", 6, 165, 145, 20000, 573, 2040, 400, 15, 8300, 15000, 0, 0, 1700, 1300, 4, 2, "Pratt & Whitney PT6C-67C", "", ""),
        ("s76", "S-76D", "Sikorsky", 6, 155, 135, 15000, 473, 2040, 380, 12, 7600, 11700, 0, 0, 1500, 1100, 4, 2, "Pratt & Whitney PW210S", "", ""),
        
        # Hélicoptères lourds
        ("as332", "AS332 Super Puma", "Airbus", 6, 165, 140, 15000, 515, 4200, 800, 24, 9350, 19350, 0, 0, 1600, 1200, 3, 2, "Turbomeca Makila 1A1", "", ""),
        ("s92", "S-92", "Sikorsky", 6, 165, 145, 15000, 594, 5000, 950, 19, 17000, 26500, 0, 0, 1700, 1300, 3, 2, "General Electric CT7-8A", "", ""),
        ("aw101", "AW101 Merlin", "Leonardo", 6, 167, 150, 15000, 863, 6000, 1100, 30, 14600, 32000, 0, 0, 1800, 1400, 3, 3, "Rolls-Royce Turbomeca RTM322", "", ""),
        
        # Hélicoptères militaires/utilitaires lourds
        ("uh60", "UH-60 Black Hawk", "Sikorsky", 6, 183, 150, 19000, 368, 2640, 600, 11, 11516, 22000, 0, 0, 1700, 1300, 3, 2, "General Electric T700-GE-701D", "", ""),
        ("ch47", "CH-47 Chinook", "Boeing", 6, 170, 140, 20000, 400, 10423, 2000, 55, 23400, 50000, 0, 0, 1600, 1200, 2.5, 2, "Lycoming T55-GA-714A", "", ""),
        ("mi8", "Mi-8", "Mil", 6, 155, 130, 14760, 295, 3700, 700, 24, 15700, 28000, 0, 0, 1500, 1100, 3, 2, "Klimov TV3-117", "", ""),
    ]
    
    for heli in helicopters:
        cursor.execute("""
            INSERT OR REPLACE INTO Aircraft VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)
        """, heli)
        print(f"  ✓ {heli[2]} {heli[1]}")

def add_airfields(cursor):
    """Ajoute une liste d'aérodromes (petits aéroports)"""
    
    # Size: 0=Small (aérodrome)
    
    airfields = [
        # Aérodromes France
        ("LFAT", "LBG", "Aérodrome du Bourget", "Le Bourget", "France", 48.9694, 2.4414, 218, 0, 1, 1, "118.85", "121.70", ""),
        ("LFPN", "TNF", "Aérodrome de Toussus-le-Noble", "Toussus-le-Noble", "France", 48.7519, 2.1062, 538, 0, 0, 1, "123.50", "", ""),
        ("LFPB", "LBP", "Aérodrome de Paris-Le Bourget", "Paris", "France", 48.9694, 2.4414, 218, 0, 1, 1, "118.85", "121.70", ""),
        ("LFPT", "POX", "Aérodrome de Pontoise-Cormeilles", "Pontoise", "France", 49.0967, 2.0408, 325, 0, 0, 1, "120.25", "", ""),
        ("LFPV", "VIY", "Aérodrome de Villacoublay", "Villacoublay", "France", 48.7744, 2.2014, 584, 0, 1, 1, "118.55", "121.85", ""),
        ("LFLP", "AUF", "Aérodrome d'Auxerre-Branches", "Auxerre", "France", 47.8502, 3.4971, 523, 0, 0, 1, "119.30", "", ""),
        ("LFLB", "CMF", "Aérodrome de Chambéry-Savoie", "Chambéry", "France", 45.6381, 5.8803, 779, 0, 1, 1, "118.30", "121.80", ""),
        ("LFMD", "CEQ", "Aérodrome de Cannes-Mandelieu", "Cannes", "France", 43.5420, 6.9535, 13, 0, 0, 1, "119.35", "", ""),
        ("LFMV", "AVN", "Aérodrome d'Avignon-Provence", "Avignon", "France", 43.9073, 4.9018, 124, 0, 1, 1, "118.55", "121.75", ""),
        ("LFBZ", "BIA", "Aérodrome de Biarritz-Pays Basque", "Biarritz", "France", 43.4683, -1.5231, 245, 0, 1, 1, "118.30", "121.70", ""),
        ("LFRB", "BES", "Aérodrome de Brest-Bretagne", "Brest", "France", 48.4479, -4.4185, 325, 1, 1, 1, "118.30", "121.70", "127.35"),
        ("LFOH", "LRH", "Aérodrome de La Rochelle-Île de Ré", "La Rochelle", "France", 46.1792, -1.1953, 74, 0, 1, 1, "118.20", "121.75", ""),
        ("LFBP", "PAU", "Aérodrome de Pau-Pyrénées", "Pau", "France", 43.3800, -0.4186, 616, 0, 1, 1, "118.50", "121.70", ""),
        ("LFBT", "TUF", "Aérodrome de Tours-Val de Loire", "Tours", "France", 47.4322, 0.7276, 357, 0, 1, 1, "118.15", "121.80", ""),
        ("LFRN", "RNS", "Aérodrome de Rennes-Saint-Jacques", "Rennes", "France", 48.0695, -1.7348, 124, 1, 1, 1, "118.30", "121.85", "127.45"),
        
        # Aérodromes Europe
        ("EGLF", "FAB", "Farnborough Airport", "Farnborough", "Royaume-Uni", 51.2758, -0.7763, 238, 0, 1, 1, "122.50", "121.70", ""),
        ("EGTK", "OXF", "Oxford Airport", "Oxford", "Royaume-Uni", 51.8369, -1.3200, 270, 0, 0, 1, "119.00", "", ""),
        ("LFPG", "LBG", "Aérodrome de Lognes-Émerainville", "Lognes", "France", 48.8217, 2.6267, 358, 0, 0, 0, "123.50", "", ""),
        ("EDNY", "FDH", "Flughafen Friedrichshafen", "Friedrichshafen", "Allemagne", 47.6713, 9.5115, 1367, 0, 1, 1, "119.90", "121.95", ""),
        ("EDDB", "SXF", "Berlin Schönefeld", "Berlin", "Allemagne", 52.3800, 13.5225, 157, 1, 1, 1, "118.50", "121.90", "128.95"),
        ("LSZG", "SGC", "St. Gallen-Altenrhein", "St. Gallen", "Suisse", 47.4850, 9.5608, 1306, 0, 1, 1, "120.05", "121.70", ""),
        ("LSZA", "LUG", "Lugano Airport", "Lugano", "Suisse", 46.0042, 8.9106, 915, 0, 1, 1, "118.10", "121.70", ""),
        ("LIPZ", "VCE", "Venezia-Lido Airport", "Venise", "Italie", 45.4286, 12.3886, 7, 0, 0, 1, "119.85", "", ""),
        ("LEPP", "PNA", "Pamplona Airport", "Pampelune", "Espagne", 42.7700, -1.6463, 1504, 0, 1, 1, "118.10", "121.70", ""),
        ("LFKJ", "CNG", "Aérodrome de Cognac-Châteaubernard", "Cognac", "France", 45.6583, -0.3175, 102, 0, 1, 1, "118.25", "121.75", ""),
        
        # Aérodromes touristiques/îles
        ("TFFR", "FDF", "Aéroport Martinique Aimé Césaire", "Fort-de-France", "Martinique", 14.5910, -61.0032, 16, 1, 1, 1, "118.10", "121.70", "127.60"),
        ("TFFG", "SFG", "Aéroport de Saint-Martin Grand Case", "Saint-Martin", "Saint-Martin", 18.0996, -63.0472, 10, 0, 1, 1, "118.30", "", ""),
        ("TFFF", "PTP", "Aéroport Guadeloupe Pôle Caraïbes", "Pointe-à-Pitre", "Guadeloupe", 16.2653, -61.5318, 36, 1, 1, 1, "118.10", "121.70", "127.60"),
        ("LFKC", "CCF", "Aéroport de Carcassonne-Salvaza", "Carcassonne", "France", 43.2160, 2.3063, 433, 0, 1, 1, "118.20", "121.75", ""),
        ("LFBH", "LDE", "Aéroport de Tarbes-Lourdes-Pyrénées", "Lourdes", "France", 43.1787, -0.0064, 1260, 1, 1, 1, "118.50", "121.70", "127.35"),
        ("LFMH", "ETZ", "Aérodrome de Metz-Nancy-Lorraine", "Metz", "France", 48.9821, 6.2513, 870, 1, 1, 1, "118.30", "121.75", "127.45"),
        ("LFOK", "CET", "Aérodrome de Cholet-Le Pontreau", "Cholet", "France", 47.0821, -0.8771, 443, 0, 0, 1, "123.50", "", ""),
        ("LFRI", "RNE", "Aérodrome de Roanne-Renaison", "Roanne", "France", 46.0583, 4.0014, 1106, 0, 0, 1, "123.50", "", ""),
        ("LFQQ", "LIL", "Aéroport de Lille-Lesquin", "Lille", "France", 50.5619, 3.0894, 157, 1, 1, 1, "118.30", "121.75", "127.45"),
        ("LFQB", "TLN", "Aéroport de Toulon-Hyères", "Toulon", "France", 43.0973, 6.1460, 7, 1, 1, 1, "118.60", "121.70", "127.35"),
        
        # Aérodromes montagne/spéciaux
        ("LFLJ", "CVF", "Aérodrome de Courchevel", "Courchevel", "France", 45.3967, 6.6347, 6588, 0, 0, 0, "130.00", "", ""),
        ("LFLG", "GNB", "Aérodrome de Grenoble-Le Versoud", "Grenoble", "France", 45.2129, 5.8294, 1302, 0, 0, 1, "123.50", "", ""),
        ("LFHU", "ANG", "Aérodrome d'Angers-Loire", "Angers", "France", 47.5603, -0.3122, 194, 0, 1, 1, "118.25", "121.75", ""),
        ("LFEY", "BVE", "Aérodrome de Brive-Souillac", "Brive", "France", 45.0397, 1.4856, 1016, 0, 1, 1, "118.20", "121.75", ""),
    ]
    
    for af in airfields:
        cursor.execute("""
            INSERT OR REPLACE INTO Airport VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?)
        """, af)
        print(f"  ✓ {af[1]} - {af[2]}, {af[3]}, {af[4]}")
    
    # Ajouter quelques pistes pour les aérodromes
    add_airfield_runways(cursor)

def add_airfield_runways(cursor):
    """Ajoute des pistes pour les aérodromes"""
    
    runways = [
        # Aérodromes français
        ("LFAT", "03/21", 9843, 148, "Béton", 1, 1),
        ("LFAT", "07/25", 6562, 98, "Béton", 1, 0),
        ("LFPN", "07/25", 3937, 98, "Asphalte", 1, 0),
        ("LFPT", "11/29", 4921, 98, "Asphalte", 1, 0),
        ("LFLB", "18/36", 7218, 148, "Asphalte", 1, 1),
        ("LFMD", "17/35", 4757, 148, "Asphalte", 1, 0),
        ("LFBZ", "08/26", 7382, 148, "Asphalte", 1, 1),
        ("LFRB", "07L/25R", 10499, 148, "Asphalte", 1, 1),
        ("LFRB", "07R/25L", 6890, 148, "Asphalte", 1, 1),
        ("LFOH", "09/27", 7874, 148, "Asphalte", 1, 1),
        ("LFBP", "13/31", 8202, 148, "Asphalte", 1, 1),
        ("LFBT", "02/20", 9843, 148, "Asphalte", 1, 1),
        ("LFRN", "07/25", 6890, 148, "Asphalte", 1, 1),
        
        # Courchevel (piste courte et en pente!)
        ("LFLJ", "04/22", 1722, 131, "Asphalte", 0, 0),
        
        # Aérodromes DOM-TOM
        ("TFFR", "10/28", 10827, 148, "Asphalte", 1, 1),
        ("TFFF", "11/29", 11483, 148, "Asphalte", 1, 1),
        
        # Autres aérodromes
        ("LFQQ", "08/26", 9514, 148, "Asphalte", 1, 1),
        ("LFQB", "04/22", 7874, 148, "Asphalte", 1, 1),
    ]
    
    for rw in runways:
        cursor.execute("""
            INSERT INTO Runway (AirportIcao, Designation, Length, Width, Surface, HasLights, HasILS)
            VALUES (?,?,?,?,?,?,?)
        """, rw)

if __name__ == "__main__":
    add_helicopters_and_airfields()
    
    # Afficher les statistiques
    conn = sqlite3.connect("../assets/data/simulator.db")
    c = conn.cursor()
    
    total_aircraft = c.execute("SELECT COUNT(*) FROM Aircraft").fetchone()[0]
    total_helicopters = c.execute("SELECT COUNT(*) FROM Aircraft WHERE Type = 6").fetchone()[0]
    total_airports = c.execute("SELECT COUNT(*) FROM Airport").fetchone()[0]
    total_airfields = c.execute("SELECT COUNT(*) FROM Airport WHERE Size = 0").fetchone()[0]
    total_runways = c.execute("SELECT COUNT(*) FROM Runway").fetchone()[0]
    
    print()
    print("=" * 70)
    print("  Résumé")
    print("=" * 70)
    print()
    print(f"  ✓ {total_helicopters} hélicoptères ajoutés")
    print(f"  ✓ {total_airfields} aérodromes ajoutés")
    print()
    print(f"  Total appareils: {total_aircraft} (dont {total_helicopters} hélicoptères)")
    print(f"  Total aéroports/aérodromes: {total_airports}")
    print(f"  Total pistes: {total_runways}")
    print()
    print("=" * 70)
    
    conn.close()

