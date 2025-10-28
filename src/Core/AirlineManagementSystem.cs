using System;
using System.Collections.Generic;
using System.Linq;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de gestion de compagnie aérienne
    /// Permet de créer et gérer sa propre compagnie
    /// </summary>
    public class AirlineManagementSystem
    {
        private AccessibilityManager accessibilityManager;
        private Airline playerAirline;
        private List<Route> availableRoutes;
        private List<Flight> scheduledFlights;
        private EconomyEngine economyEngine;
        private ReputationSystem reputationSystem;

        public event EventHandler<AirlineEventArgs> AirlineEvent;
        public event EventHandler<FlightCompletedEventArgs> FlightCompleted;

        public Airline PlayerAirline => playerAirline;
        public decimal Balance => playerAirline?.Balance ?? 0;

        public AirlineManagementSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.availableRoutes = new List<Route>();
            this.scheduledFlights = new List<Flight>();
            this.economyEngine = new EconomyEngine();
            this.reputationSystem = new ReputationSystem();
            
            InitializeRoutes();
        }

        /// <summary>
        /// Crée une nouvelle compagnie aérienne
        /// </summary>
        public void CreateAirline(string name, string code)
        {
            playerAirline = new Airline
            {
                Name = name,
                IATACode = code,
                Balance = 500000m, // Capital de départ : 500k€
                Founded = DateTime.Now,
                Fleet = new List<OwnedAircraft>(),
                Reputation = 50, // Réputation neutre
                TotalFlights = 0,
                TotalPassengers = 0,
                TotalRevenue = 0
            };

            accessibilityManager.Speak($"Compagnie {name} créée avec succès. " +
                $"Code IATA : {code}. Capital de départ : 500 mille euros.");

            AirlineEvent?.Invoke(this, new AirlineEventArgs(AirlineEventType.Created, playerAirline));
        }

        /// <summary>
        /// Achète un avion
        /// </summary>
        public bool PurchaseAircraft(Aircraft aircraft)
        {
            if (playerAirline == null)
            {
                accessibilityManager.Speak("Vous devez d'abord créer une compagnie.");
                return false;
            }

            decimal price = GetAircraftPrice(aircraft);

            if (playerAirline.Balance < price)
            {
                accessibilityManager.Speak($"Fonds insuffisants. Prix : {price:N0} euros. " +
                    $"Solde : {playerAirline.Balance:N0} euros.");
                return false;
            }

            // Déduction du prix
            playerAirline.Balance -= price;

            // Ajout à la flotte
            var ownedAircraft = new OwnedAircraft
            {
                Aircraft = aircraft,
                Registration = GenerateRegistration(),
                PurchaseDate = DateTime.Now,
                PurchasePrice = price,
                TotalFlightHours = 0,
                MaintenanceStatus = MaintenanceStatus.Good,
                NextMaintenanceDue = DateTime.Now.AddDays(90)
            };

            playerAirline.Fleet.Add(ownedAircraft);

            accessibilityManager.Speak($"Avion {aircraft.Name} acheté pour {price:N0} euros. " +
                $"Immatriculation : {ownedAircraft.Registration}. " +
                $"Solde restant : {playerAirline.Balance:N0} euros.");

            AirlineEvent?.Invoke(this, new AirlineEventArgs(AirlineEventType.AircraftPurchased, playerAirline));

            return true;
        }

        /// <summary>
        /// Vend un avion
        /// </summary>
        public bool SellAircraft(OwnedAircraft ownedAircraft)
        {
            if (!playerAirline.Fleet.Contains(ownedAircraft))
            {
                accessibilityManager.Speak("Cet avion n'est pas dans votre flotte.");
                return false;
            }

            // Prix de revente (70% du prix d'achat si en bon état)
            decimal sellPrice = ownedAircraft.PurchasePrice * 0.7m;

            // Réduction selon l'état
            sellPrice *= ownedAircraft.MaintenanceStatus switch
            {
                MaintenanceStatus.Excellent => 1.0m,
                MaintenanceStatus.Good => 0.9m,
                MaintenanceStatus.Fair => 0.7m,
                MaintenanceStatus.Poor => 0.5m,
                MaintenanceStatus.Critical => 0.3m,
                _ => 0.7m
            };

            playerAirline.Balance += sellPrice;
            playerAirline.Fleet.Remove(ownedAircraft);

            accessibilityManager.Speak($"Avion {ownedAircraft.Aircraft.Name} vendu pour {sellPrice:N0} euros. " +
                $"Nouveau solde : {playerAirline.Balance:N0} euros.");

            AirlineEvent?.Invoke(this, new AirlineEventArgs(AirlineEventType.AircraftSold, playerAirline));

            return true;
        }

        /// <summary>
        /// Programme un vol
        /// </summary>
        public bool ScheduleFlight(OwnedAircraft aircraft, Route route, DateTime departureTime, int passengers)
        {
            if (!playerAirline.Fleet.Contains(aircraft))
            {
                accessibilityManager.Speak("Cet avion n'est pas dans votre flotte.");
                return false;
            }

            if (passengers > aircraft.Aircraft.PassengerCapacity)
            {
                accessibilityManager.Speak($"Trop de passagers. Capacité maximale : {aircraft.Aircraft.PassengerCapacity}");
                return false;
            }

            // Vérifier que l'avion peut faire la route
            if (route.Distance > aircraft.Aircraft.Range)
            {
                accessibilityManager.Speak($"L'avion n'a pas l'autonomie suffisante. " +
                    $"Distance : {route.Distance} nm, Autonomie : {aircraft.Aircraft.Range} nm");
                return false;
            }

            var flight = new Flight
            {
                FlightNumber = GenerateFlightNumber(),
                Aircraft = aircraft,
                Route = route,
                DepartureTime = departureTime,
                Passengers = passengers,
                Status = FlightStatus.Scheduled,
                TicketPrice = CalculateTicketPrice(route, aircraft.Aircraft)
            };

            scheduledFlights.Add(flight);

            decimal estimatedRevenue = flight.TicketPrice * passengers;

            accessibilityManager.Speak($"Vol {flight.FlightNumber} programmé. " +
                $"{route.Origin.City} vers {route.Destination.City}. " +
                $"Départ : {departureTime:HH:mm}. " +
                $"Passagers : {passengers}. " +
                $"Revenu estimé : {estimatedRevenue:N0} euros.");

            return true;
        }

        /// <summary>
        /// Termine un vol et calcule les résultats
        /// </summary>
        public FlightResult CompleteFlight(Flight flight, FlightPerformance performance)
        {
            if (!scheduledFlights.Contains(flight))
            {
                accessibilityManager.Speak("Ce vol n'est pas dans vos vols programmés.");
                return null;
            }

            // Calcul du revenu
            decimal revenue = flight.TicketPrice * flight.Passengers;

            // Calcul des coûts
            decimal fuelCost = CalculateFuelCost(flight.Route.Distance, flight.Aircraft.Aircraft);
            decimal crewCost = CalculateCrewCost(flight.Route.Duration);
            decimal maintenanceCost = CalculateMaintenanceCost(flight.Aircraft.Aircraft, flight.Route.Distance);
            decimal airportFees = CalculateAirportFees(flight.Route);

            decimal totalCosts = fuelCost + crewCost + maintenanceCost + airportFees;
            decimal profit = revenue - totalCosts;

            // Bonus/malus selon la performance
            decimal performanceBonus = CalculatePerformanceBonus(performance);
            profit += performanceBonus;

            // Mise à jour de la compagnie
            playerAirline.Balance += profit;
            playerAirline.TotalFlights++;
            playerAirline.TotalPassengers += flight.Passengers;
            playerAirline.TotalRevenue += revenue;

            // Mise à jour de l'avion
            flight.Aircraft.TotalFlightHours += flight.Route.Duration;
            UpdateMaintenanceStatus(flight.Aircraft, performance);

            // Mise à jour de la réputation
            int reputationChange = reputationSystem.CalculateReputationChange(performance);
            playerAirline.Reputation += reputationChange;
            playerAirline.Reputation = Math.Clamp(playerAirline.Reputation, 0, 100);

            // Retirer le vol des programmés
            scheduledFlights.Remove(flight);

            var result = new FlightResult
            {
                Flight = flight,
                Revenue = revenue,
                FuelCost = fuelCost,
                CrewCost = crewCost,
                MaintenanceCost = maintenanceCost,
                AirportFees = airportFees,
                TotalCosts = totalCosts,
                PerformanceBonus = performanceBonus,
                Profit = profit,
                ReputationChange = reputationChange,
                Performance = performance
            };

            // Annonce vocale
            string profitStatus = profit >= 0 ? "bénéfice" : "perte";
            accessibilityManager.Speak($"Vol {flight.FlightNumber} terminé. " +
                $"{profitStatus} : {Math.Abs(profit):N0} euros. " +
                $"Nouveau solde : {playerAirline.Balance:N0} euros. " +
                $"Réputation : {playerAirline.Reputation}/100.");

            FlightCompleted?.Invoke(this, new FlightCompletedEventArgs(result));

            return result;
        }

        /// <summary>
        /// Effectue la maintenance d'un avion
        /// </summary>
        public bool PerformMaintenance(OwnedAircraft aircraft, MaintenanceType type)
        {
            decimal cost = type switch
            {
                MaintenanceType.Routine => 5000m,
                MaintenanceType.Major => 25000m,
                MaintenanceType.Overhaul => 100000m,
                _ => 5000m
            };

            if (playerAirline.Balance < cost)
            {
                accessibilityManager.Speak($"Fonds insuffisants pour la maintenance. Coût : {cost:N0} euros.");
                return false;
            }

            playerAirline.Balance -= cost;

            // Amélioration de l'état
            aircraft.MaintenanceStatus = type switch
            {
                MaintenanceType.Routine => MaintenanceStatus.Good,
                MaintenanceType.Major => MaintenanceStatus.Excellent,
                MaintenanceType.Overhaul => MaintenanceStatus.Excellent,
                _ => MaintenanceStatus.Good
            };

            aircraft.NextMaintenanceDue = DateTime.Now.AddDays(type switch
            {
                MaintenanceType.Routine => 90,
                MaintenanceType.Major => 180,
                MaintenanceType.Overhaul => 365,
                _ => 90
            });

            accessibilityManager.Speak($"Maintenance {type} effectuée pour {cost:N0} euros. " +
                $"État de l'avion : {aircraft.MaintenanceStatus}. " +
                $"Prochaine maintenance : {aircraft.NextMaintenanceDue:dd/MM/yyyy}.");

            return true;
        }

        /// <summary>
        /// Obtient les statistiques de la compagnie
        /// </summary>
        public AirlineStatistics GetStatistics()
        {
            if (playerAirline == null)
                return null;

            return new AirlineStatistics
            {
                TotalFlights = playerAirline.TotalFlights,
                TotalPassengers = playerAirline.TotalPassengers,
                TotalRevenue = playerAirline.TotalRevenue,
                CurrentBalance = playerAirline.Balance,
                FleetSize = playerAirline.Fleet.Count,
                AverageRevenuePerFlight = playerAirline.TotalFlights > 0 ? 
                    playerAirline.TotalRevenue / playerAirline.TotalFlights : 0,
                Reputation = playerAirline.Reputation,
                DaysInOperation = (DateTime.Now - playerAirline.Founded).Days
            };
        }

        #region Méthodes privées

        private void InitializeRoutes()
        {
            // Routes européennes courtes
            availableRoutes.Add(new Route
            {
                Name = "Paris - Londres",
                Origin = GetAirport("LFPG"),
                Destination = GetAirport("EGLL"),
                Distance = 215,
                Duration = 1.2f,
                Demand = RouteDemand.High
            });

            availableRoutes.Add(new Route
            {
                Name = "Paris - Amsterdam",
                Origin = GetAirport("LFPG"),
                Destination = GetAirport("EHAM"),
                Distance = 240,
                Duration = 1.3f,
                Demand = RouteDemand.High
            });

            // Routes moyennes
            availableRoutes.Add(new Route
            {
                Name = "Paris - Madrid",
                Origin = GetAirport("LFPG"),
                Destination = GetAirport("LEMD"),
                Distance = 650,
                Duration = 2.0f,
                Demand = RouteDemand.Medium
            });

            // Routes longues
            availableRoutes.Add(new Route
            {
                Name = "Paris - New York",
                Origin = GetAirport("LFPG"),
                Destination = GetAirport("KJFK"),
                Distance = 3625,
                Duration = 8.0f,
                Demand = RouteDemand.VeryHigh
            });
        }

        private Airport GetAirport(string icao)
        {
            // Placeholder - dans une vraie implémentation,
            // on récupérerait l'aéroport de la base de données
            return new Airport { ICAOCode = icao };
        }

        private decimal GetAircraftPrice(Aircraft aircraft)
        {
            // Prix basés sur le type d'avion
            return aircraft.Type switch
            {
                "Cessna 172" => 300000m,
                "Citation CJ4" => 9000000m,
                "ATR 72" => 26000000m,
                "A320" => 100000000m,
                "A350" => 300000000m,
                "A380" => 450000000m,
                _ => 1000000m
            };
        }

        private string GenerateRegistration()
        {
            Random rnd = new Random();
            return $"F-{(char)('A' + rnd.Next(26))}{(char)('A' + rnd.Next(26))}{(char)('A' + rnd.Next(26))}{(char)('A' + rnd.Next(26))}";
        }

        private string GenerateFlightNumber()
        {
            Random rnd = new Random();
            return $"{playerAirline.IATACode}{rnd.Next(1000, 9999)}";
        }

        private decimal CalculateTicketPrice(Route route, Aircraft aircraft)
        {
            // Prix de base selon la distance
            decimal basePrice = route.Distance * 0.15m;

            // Multiplicateur selon la demande
            decimal demandMultiplier = route.Demand switch
            {
                RouteDemand.VeryLow => 0.6m,
                RouteDemand.Low => 0.8m,
                RouteDemand.Medium => 1.0m,
                RouteDemand.High => 1.3m,
                RouteDemand.VeryHigh => 1.6m,
                _ => 1.0m
            };

            // Multiplicateur selon le type d'avion
            decimal aircraftMultiplier = aircraft.PassengerCapacity < 50 ? 1.5m :
                                         aircraft.PassengerCapacity < 150 ? 1.0m : 0.8m;

            return basePrice * demandMultiplier * aircraftMultiplier;
        }

        private decimal CalculateFuelCost(int distance, Aircraft aircraft)
        {
            // Consommation en litres par nm
            float consumption = aircraft.FuelCapacity / aircraft.Range * 1.2f;
            decimal fuelPrice = 1.5m; // €/litre
            return (decimal)(consumption * distance) * fuelPrice;
        }

        private decimal CalculateCrewCost(float duration)
        {
            // Coût équipage par heure
            return (decimal)duration * 500m;
        }

        private decimal CalculateMaintenanceCost(Aircraft aircraft, int distance)
        {
            // Coût de maintenance par nm
            return distance * 2m;
        }

        private decimal CalculateAirportFees(Route route)
        {
            // Frais d'aéroport (atterrissage + parking)
            return 1000m;
        }

        private decimal CalculatePerformanceBonus(FlightPerformance performance)
        {
            decimal bonus = 0;

            // Bonus pour atterrissage doux
            if (performance.LandingRate < 200)
                bonus += 500m;

            // Bonus pour ponctualité
            if (Math.Abs(performance.DelayMinutes) < 5)
                bonus += 300m;

            // Malus pour retard
            if (performance.DelayMinutes > 15)
                bonus -= 1000m;

            // Bonus pour économie de carburant
            if (performance.FuelEfficiency > 0.9f)
                bonus += 200m;

            return bonus;
        }

        private void UpdateMaintenanceStatus(OwnedAircraft aircraft, FlightPerformance performance)
        {
            // Dégradation selon la performance
            if (performance.LandingRate > 600)
            {
                // Atterrissage très dur
                aircraft.MaintenanceStatus = (MaintenanceStatus)Math.Max(0, (int)aircraft.MaintenanceStatus - 2);
            }
            else if (performance.LandingRate > 400)
            {
                aircraft.MaintenanceStatus = (MaintenanceStatus)Math.Max(0, (int)aircraft.MaintenanceStatus - 1);
            }
        }

        #endregion
    }

    #region Modèles de données

    public class Airline
    {
        public string Name { get; set; }
        public string IATACode { get; set; }
        public decimal Balance { get; set; }
        public DateTime Founded { get; set; }
        public List<OwnedAircraft> Fleet { get; set; }
        public int Reputation { get; set; }
        public int TotalFlights { get; set; }
        public int TotalPassengers { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OwnedAircraft
    {
        public Aircraft Aircraft { get; set; }
        public string Registration { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public float TotalFlightHours { get; set; }
        public MaintenanceStatus MaintenanceStatus { get; set; }
        public DateTime NextMaintenanceDue { get; set; }
    }

    public enum MaintenanceStatus
    {
        Critical = 0,
        Poor = 1,
        Fair = 2,
        Good = 3,
        Excellent = 4
    }

    public enum MaintenanceType
    {
        Routine,
        Major,
        Overhaul
    }

    public class Route
    {
        public string Name { get; set; }
        public Airport Origin { get; set; }
        public Airport Destination { get; set; }
        public int Distance { get; set; }
        public float Duration { get; set; }
        public RouteDemand Demand { get; set; }
    }

    public enum RouteDemand
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    }

    public class Flight
    {
        public string FlightNumber { get; set; }
        public OwnedAircraft Aircraft { get; set; }
        public Route Route { get; set; }
        public DateTime DepartureTime { get; set; }
        public int Passengers { get; set; }
        public FlightStatus Status { get; set; }
        public decimal TicketPrice { get; set; }
    }

    public enum FlightStatus
    {
        Scheduled,
        Boarding,
        Departed,
        InFlight,
        Arrived,
        Cancelled
    }

    public class FlightPerformance
    {
        public float LandingRate { get; set; }
        public int DelayMinutes { get; set; }
        public float FuelEfficiency { get; set; }
        public bool IncidentOccurred { get; set; }
    }

    public class FlightResult
    {
        public Flight Flight { get; set; }
        public decimal Revenue { get; set; }
        public decimal FuelCost { get; set; }
        public decimal CrewCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal AirportFees { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal PerformanceBonus { get; set; }
        public decimal Profit { get; set; }
        public int ReputationChange { get; set; }
        public FlightPerformance Performance { get; set; }
    }

    public class AirlineStatistics
    {
        public int TotalFlights { get; set; }
        public int TotalPassengers { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CurrentBalance { get; set; }
        public int FleetSize { get; set; }
        public decimal AverageRevenuePerFlight { get; set; }
        public int Reputation { get; set; }
        public int DaysInOperation { get; set; }
    }

    public class EconomyEngine
    {
        // Moteur économique pour fluctuations de prix, demande, etc.
    }

    public class ReputationSystem
    {
        public int CalculateReputationChange(FlightPerformance performance)
        {
            int change = 0;

            // Atterrissage
            if (performance.LandingRate < 200)
                change += 2;
            else if (performance.LandingRate > 600)
                change -= 3;

            // Ponctualité
            if (Math.Abs(performance.DelayMinutes) < 5)
                change += 1;
            else if (performance.DelayMinutes > 30)
                change -= 2;

            // Incident
            if (performance.IncidentOccurred)
                change -= 5;

            return change;
        }
    }

    public class AirlineEventArgs : EventArgs
    {
        public AirlineEventType EventType { get; }
        public Airline Airline { get; }

        public AirlineEventArgs(AirlineEventType eventType, Airline airline)
        {
            EventType = eventType;
            Airline = airline;
        }
    }

    public enum AirlineEventType
    {
        Created,
        AircraftPurchased,
        AircraftSold,
        RouteAdded,
        Bankrupt
    }

    public class FlightCompletedEventArgs : EventArgs
    {
        public FlightResult Result { get; }

        public FlightCompletedEventArgs(FlightResult result)
        {
            Result = result;
        }
    }

    #endregion
}

