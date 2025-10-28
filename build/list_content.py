#!/usr/bin/env python3
import sqlite3

conn = sqlite3.connect('../assets/data/simulator.db')
c = conn.cursor()

print("\n=== AVIONS DISPONIBLES ===\n")
print("Avions légers:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 0"):
    print(f"  • {row[0]} {row[1]}")

print("\nJets d'affaires:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 1"):
    print(f"  • {row[0]} {row[1]}")

print("\nAvions régionaux:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 2"):
    print(f"  • {row[0]} {row[1]}")

print("\nMoyen-courriers:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 3"):
    print(f"  • {row[0]} {row[1]}")

print("\nLong-courriers:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 4"):
    print(f"  • {row[0]} {row[1]}")

print("\nAvions cargo:")
for row in c.execute("SELECT Manufacturer, Name FROM Aircraft WHERE Type = 5"):
    print(f"  • {row[0]} {row[1]}")

print("\n=== AÉROPORTS PAR RÉGION ===\n")
print("France:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Country = 'France' ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nEurope (hors France) - Sélection:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Country != 'France' AND Country IN ('Royaume-Uni', 'Allemagne', 'Espagne', 'Italie', 'Pays-Bas') ORDER BY Country, City LIMIT 10"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nAmérique du Nord - Sélection:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Country IN ('États-Unis', 'Canada', 'Mexique') ORDER BY Country, City LIMIT 10"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nAsie - Sélection:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Country IN ('Japon', 'Chine', 'Singapour', 'Thaïlande', 'Inde', 'Corée du Sud') ORDER BY Country, City LIMIT 10"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nMoyen-Orient:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Country IN ('Émirats arabes unis', 'Qatar', 'Arabie saoudite', 'Oman', 'Turquie') ORDER BY Country, City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nAutres continents:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Country IN ('Australie', 'Nouvelle-Zélande', 'Afrique du Sud', 'Égypte', 'Maroc', 'Brésil', 'Chili', 'Argentine', 'Colombie') ORDER BY Country, City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

total_aircraft = c.execute("SELECT COUNT(*) FROM Aircraft").fetchone()[0]
total_airports = c.execute("SELECT COUNT(*) FROM Airport").fetchone()[0]
total_runways = c.execute("SELECT COUNT(*) FROM Runway").fetchone()[0]

print(f"\n=== STATISTIQUES ===")
print(f"Total avions: {total_aircraft}")
print(f"Total aéroports: {total_airports}")
print(f"Total pistes: {total_runways}")

conn.close()

