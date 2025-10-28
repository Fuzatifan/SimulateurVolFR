using System;
using System.Collections.Generic;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de navigation pour le simulateur
    /// </summary>
    public class NavigationSystem
    {
        private Airport? origin;
        private Airport? destination;
        private FlightState? flightState;
        private List<Waypoint> flightPlan;
        private int currentWaypointIndex;

        public Airport? Origin => origin;
        public Airport? Destination => destination;
        public List<Waypoint> FlightPlan => flightPlan;
        public Waypoint? CurrentWaypoint => currentWaypointIndex < flightPlan.Count ? flightPlan[currentWaypointIndex] : null;
        public Waypoint? NextWaypoint => currentWaypointIndex + 1 < flightPlan.Count ? flightPlan[currentWaypointIndex + 1] : null;

        public event EventHandler<WaypointReachedEventArgs>? WaypointReached;

        public NavigationSystem()
        {
            flightPlan = new List<Waypoint>();
            currentWaypointIndex = 0;
        }

        /// <summary>
        /// Initialise le système de navigation avec un plan de vol
        /// </summary>
        public void Initialize(Airport origin, Airport destination, FlightState flightState)
        {
            this.origin = origin;
            this.destination = destination;
            this.flightState = flightState;
            
            GenerateDirectFlightPlan();
        }

        /// <summary>
        /// Génère un plan de vol direct entre l'origine et la destination
        /// </summary>
        private void GenerateDirectFlightPlan()
        {
            flightPlan.Clear();
            currentWaypointIndex = 0;

            if (origin == null || destination == null) return;

            // Waypoint de départ
            flightPlan.Add(new Waypoint
            {
                Name = $"{origin.IcaoCode} Départ",
                Latitude = origin.Latitude,
                Longitude = origin.Longitude,
                Altitude = origin.Elevation,
                Type = WaypointType.Airport
            });

            // Waypoint de montée
            double climbDistance = 50; // 50 NM pour la montée
            double bearing = origin.BearingTo(destination);
            var climbPoint = CalculatePointAtDistance(origin.Latitude, origin.Longitude, bearing, climbDistance);
            
            flightPlan.Add(new Waypoint
            {
                Name = "Point de montée",
                Latitude = climbPoint.lat,
                Longitude = climbPoint.lon,
                Altitude = 10000,
                Type = WaypointType.Climb
            });

            // Waypoint de croisière (point milieu)
            double totalDistance = origin.DistanceTo(destination);
            double cruiseDistance = totalDistance / 2;
            var cruisePoint = CalculatePointAtDistance(origin.Latitude, origin.Longitude, bearing, cruiseDistance);
            
            flightPlan.Add(new Waypoint
            {
                Name = "Point de croisière",
                Latitude = cruisePoint.lat,
                Longitude = cruisePoint.lon,
                Altitude = 35000,
                Type = WaypointType.Cruise
            });

            // Waypoint de descente
            double descentDistance = totalDistance - 50; // Commencer la descente 50 NM avant
            var descentPoint = CalculatePointAtDistance(origin.Latitude, origin.Longitude, bearing, descentDistance);
            
            flightPlan.Add(new Waypoint
            {
                Name = "Point de descente",
                Latitude = descentPoint.lat,
                Longitude = descentPoint.lon,
                Altitude = 10000,
                Type = WaypointType.Descent
            });

            // Waypoint d'arrivée
            flightPlan.Add(new Waypoint
            {
                Name = $"{destination.IcaoCode} Arrivée",
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                Altitude = destination.Elevation,
                Type = WaypointType.Airport
            });
        }

        /// <summary>
        /// Met à jour le système de navigation
        /// </summary>
        public void Update()
        {
            if (flightState == null || CurrentWaypoint == null) return;

            // Calculer la distance au waypoint actuel
            double distance = CalculateDistance(
                flightState.Latitude, flightState.Longitude,
                CurrentWaypoint.Latitude, CurrentWaypoint.Longitude
            );

            // Si on est proche du waypoint (moins de 1 NM), passer au suivant
            if (distance < 1.0)
            {
                OnWaypointReached(CurrentWaypoint);
                currentWaypointIndex++;
            }
        }

        /// <summary>
        /// Obtient la distance au waypoint actuel en miles nautiques
        /// </summary>
        public double GetDistanceToCurrentWaypoint()
        {
            if (flightState == null || CurrentWaypoint == null) return 0;

            return CalculateDistance(
                flightState.Latitude, flightState.Longitude,
                CurrentWaypoint.Latitude, CurrentWaypoint.Longitude
            );
        }

        /// <summary>
        /// Obtient le cap vers le waypoint actuel
        /// </summary>
        public double GetBearingToCurrentWaypoint()
        {
            if (flightState == null || CurrentWaypoint == null) return 0;

            return CalculateBearing(
                flightState.Latitude, flightState.Longitude,
                CurrentWaypoint.Latitude, CurrentWaypoint.Longitude
            );
        }

        /// <summary>
        /// Obtient la distance totale restante en miles nautiques
        /// </summary>
        public double GetRemainingDistance()
        {
            if (flightState == null || currentWaypointIndex >= flightPlan.Count) return 0;

            double totalDistance = 0;

            // Distance jusqu'au waypoint actuel
            totalDistance += GetDistanceToCurrentWaypoint();

            // Distance entre les waypoints restants
            for (int i = currentWaypointIndex; i < flightPlan.Count - 1; i++)
            {
                totalDistance += CalculateDistance(
                    flightPlan[i].Latitude, flightPlan[i].Longitude,
                    flightPlan[i + 1].Latitude, flightPlan[i + 1].Longitude
                );
            }

            return totalDistance;
        }

        /// <summary>
        /// Obtient le temps estimé d'arrivée
        /// </summary>
        public TimeSpan GetEstimatedTimeToDestination()
        {
            if (flightState == null || flightState.Speed <= 0) return TimeSpan.Zero;

            double remainingDistance = GetRemainingDistance();
            double hours = remainingDistance / flightState.Speed;
            return TimeSpan.FromHours(hours);
        }

        /// <summary>
        /// Obtient une description vocale de la navigation
        /// </summary>
        public string GetNavigationStatus()
        {
            if (CurrentWaypoint == null) return "Aucun plan de vol actif";

            double distance = GetDistanceToCurrentWaypoint();
            double bearing = GetBearingToCurrentWaypoint();
            double remainingDistance = GetRemainingDistance();
            TimeSpan eta = GetEstimatedTimeToDestination();

            return $"Cap vers {CurrentWaypoint.Name}. " +
                   $"Distance: {Math.Round(distance)} miles nautiques. " +
                   $"Cap: {Math.Round(bearing)} degrés. " +
                   $"Distance restante totale: {Math.Round(remainingDistance)} miles nautiques. " +
                   $"Temps estimé d'arrivée: {eta.Hours} heures {eta.Minutes} minutes.";
        }

        /// <summary>
        /// Calcule la distance entre deux points en miles nautiques
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusNM = 3440.065;
            
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);
            
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;
            
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return EarthRadiusNM * c;
        }

        /// <summary>
        /// Calcule le cap entre deux points
        /// </summary>
        private double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);
            
            double dLon = lon2Rad - lon1Rad;
            
            double y = Math.Sin(dLon) * Math.Cos(lat2Rad);
            double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                      Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);
            
            double bearing = Math.Atan2(y, x);
            bearing = ToDegrees(bearing);
            bearing = (bearing + 360) % 360;
            
            return bearing;
        }

        /// <summary>
        /// Calcule un point à une certaine distance et cap d'un point de départ
        /// </summary>
        private (double lat, double lon) CalculatePointAtDistance(double lat, double lon, double bearing, double distanceNM)
        {
            const double EarthRadiusNM = 3440.065;
            
            double latRad = ToRadians(lat);
            double lonRad = ToRadians(lon);
            double bearingRad = ToRadians(bearing);
            double angularDistance = distanceNM / EarthRadiusNM;
            
            double lat2 = Math.Asin(Math.Sin(latRad) * Math.Cos(angularDistance) +
                                   Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad));
            
            double lon2 = lonRad + Math.Atan2(Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(latRad),
                                             Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(lat2));
            
            return (ToDegrees(lat2), ToDegrees(lon2));
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        private double ToDegrees(double radians) => radians * 180.0 / Math.PI;

        private void OnWaypointReached(Waypoint waypoint)
        {
            WaypointReached?.Invoke(this, new WaypointReachedEventArgs(waypoint));
            AccessibilityManager.Speak($"Waypoint atteint: {waypoint.Name}");
        }
    }

    /// <summary>
    /// Représente un point de navigation
    /// </summary>
    public class Waypoint
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public WaypointType Type { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Latitude:F4}, {Longitude:F4}) - {Altitude} ft";
        }
    }

    public enum WaypointType
    {
        Airport,
        Climb,
        Cruise,
        Descent,
        Custom
    }

    public class WaypointReachedEventArgs : EventArgs
    {
        public Waypoint Waypoint { get; }

        public WaypointReachedEventArgs(Waypoint waypoint)
        {
            Waypoint = waypoint;
        }
    }
}

