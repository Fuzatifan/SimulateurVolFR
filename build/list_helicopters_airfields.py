#!/usr/bin/env python3
import sqlite3

conn = sqlite3.connect('../assets/data/simulator.db')
c = conn.cursor()

print("\n" + "=" * 70)
print("  HÉLICOPTÈRES DISPONIBLES")
print("=" * 70 + "\n")

print("Hélicoptères légers (3 modèles):")
for row in c.execute("SELECT Manufacturer, Name, PassengerCapacity FROM Aircraft WHERE Type = 6 AND PassengerCapacity <= 4 ORDER BY PassengerCapacity"):
    print(f"  • {row[0]} {row[1]} - {row[2]} passagers")

print("\nHélicoptères utilitaires (4 modèles):")
for row in c.execute("SELECT Manufacturer, Name, PassengerCapacity FROM Aircraft WHERE Type = 6 AND PassengerCapacity BETWEEN 5 AND 6 ORDER BY PassengerCapacity"):
    print(f"  • {row[0]} {row[1]} - {row[2]} passagers")

print("\nHélicoptères bimoteurs moyens (4 modèles):")
for row in c.execute("SELECT Manufacturer, Name, PassengerCapacity FROM Aircraft WHERE Type = 6 AND PassengerCapacity BETWEEN 12 AND 15 ORDER BY PassengerCapacity"):
    print(f"  • {row[0]} {row[1]} - {row[2]} passagers")

print("\nHélicoptères lourds (3 modèles):")
for row in c.execute("SELECT Manufacturer, Name, PassengerCapacity FROM Aircraft WHERE Type = 6 AND PassengerCapacity BETWEEN 19 AND 30 ORDER BY PassengerCapacity"):
    print(f"  • {row[0]} {row[1]} - {row[2]} passagers")

print("\nHélicoptères militaires/utilitaires lourds (3 modèles):")
for row in c.execute("SELECT Manufacturer, Name, PassengerCapacity FROM Aircraft WHERE Type = 6 AND PassengerCapacity > 30 ORDER BY PassengerCapacity"):
    print(f"  • {row[0]} {row[1]} - {row[2]} passagers")

print("\n" + "=" * 70)
print("  AÉRODROMES PAR RÉGION")
print("=" * 70 + "\n")

print("Aérodromes région parisienne:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size = 0 AND Country = 'France' AND (City LIKE '%Paris%' OR City LIKE '%Le Bourget%' OR City LIKE '%Toussus%' OR City LIKE '%Pontoise%' OR City LIKE '%Villacoublay%' OR City LIKE '%Lognes%') ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nAérodromes Sud de la France:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size = 0 AND Country = 'France' AND City IN ('Cannes', 'Avignon', 'Toulon', 'Carcassonne') ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nAérodromes Ouest de la France:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size = 0 AND Country = 'France' AND City IN ('Biarritz', 'Brest', 'La Rochelle', 'Rennes', 'Angers', 'Cholet') ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nAérodromes Centre et Est de la France:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size = 0 AND Country = 'France' AND City IN ('Auxerre', 'Tours', 'Metz', 'Roanne', 'Brive', 'Cognac') ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nAérodromes montagne:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size = 0 AND Country = 'France' AND City IN ('Chambéry', 'Courchevel', 'Grenoble', 'Pau', 'Lourdes') ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

print("\nAérodromes DOM-TOM:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Size IN (0,1) AND Country IN ('Martinique', 'Guadeloupe', 'Saint-Martin') ORDER BY Country, City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nAérodromes Europe:")
for row in c.execute("SELECT IataCode, Name, City, Country FROM Airport WHERE Size = 0 AND Country != 'France' AND Country NOT IN ('Martinique', 'Guadeloupe', 'Saint-Martin') ORDER BY Country, City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}, {row[3]}")

print("\nAérodromes Nord de la France:")
for row in c.execute("SELECT IataCode, Name, City FROM Airport WHERE Size IN (0,1) AND Country = 'France' AND City = 'Lille' ORDER BY City"):
    print(f"  • {row[0]} - {row[1]}, {row[2]}")

# Statistiques
total_helicopters = c.execute("SELECT COUNT(*) FROM Aircraft WHERE Type = 6").fetchone()[0]
total_airfields = c.execute("SELECT COUNT(*) FROM Airport WHERE Size = 0").fetchone()[0]
total_aircraft = c.execute("SELECT COUNT(*) FROM Aircraft").fetchone()[0]
total_airports = c.execute("SELECT COUNT(*) FROM Airport").fetchone()[0]

print("\n" + "=" * 70)
print("  STATISTIQUES GLOBALES")
print("=" * 70)
print(f"\nHélicoptères: {total_helicopters}")
print(f"Aérodromes: {total_airfields}")
print(f"\nTotal appareils (avions + hélicoptères): {total_aircraft}")
print(f"Total sites (aéroports + aérodromes): {total_airports}")
print("=" * 70 + "\n")

conn.close()

