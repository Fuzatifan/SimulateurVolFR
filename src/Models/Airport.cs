using System;
using System.Collections.Generic;

namespace SimulateurVolFR.Models
{
    /// <summary>
    /// Représente un aéroport dans le simulateur
    /// </summary>
    public class Airport
    {
        public string IcaoCode { get; set; } = string.Empty;
        public string IataCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        
        // Position géographique
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; } // en pieds
        
        // Caractéristiques
        public AirportSize Size { get; set; }
        public List<Runway> Runways { get; set; } = new List<Runway>();
        public bool HasILS { get; set; } // Système d'atterrissage aux instruments
        public bool HasTower { get; set; } // Tour de contrôle
        
        // Fréquences radio
        public string TowerFrequency { get; set; } = string.Empty;
        public string GroundFrequency { get; set; } = string.Empty;
        public string AtisFrequency { get; set; } = string.Empty;
        
        public Airport()
        {
        }

        public override string ToString()
        {
            return $"{IcaoCode} - {Name}, {City}";
        }

        /// <summary>
        /// Obtient une description vocale de l'aéroport
        /// </summary>
        public string GetVoiceDescription()
        {
            string sizeDesc = Size switch
            {
                AirportSize.Small => "petit aéroport",
                AirportSize.Medium => "aéroport moyen",
                AirportSize.Large => "grand aéroport",
                AirportSize.International => "aéroport international",
                _ => "aéroport"
            };

            string runwayInfo = Runways.Count > 0 
                ? $"{Runways.Count} piste{(Runways.Count > 1 ? "s" : "")}" 
                : "aucune piste";

            return $"{Name}, {City}, {Country}. Code OACI: {IcaoCode}. " +
                   $"{sizeDesc} avec {runwayInfo}. " +
                   $"Altitude: {Elevation} pieds.";
        }

        /// <summary>
        /// Calcule la distance vers un autre aéroport en miles nautiques
        /// </summary>
        public double DistanceTo(Airport other)
        {
            const double EarthRadiusNM = 3440.065; // Rayon de la Terre en miles nautiques
            
            double lat1 = ToRadians(Latitude);
            double lon1 = ToRadians(Longitude);
            double lat2 = ToRadians(other.Latitude);
            double lon2 = ToRadians(other.Longitude);
            
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(lat1) * Math.Cos(lat2) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return EarthRadiusNM * c;
        }

        /// <summary>
        /// Calcule le cap vers un autre aéroport en degrés
        /// </summary>
        public double BearingTo(Airport other)
        {
            double lat1 = ToRadians(Latitude);
            double lon1 = ToRadians(Longitude);
            double lat2 = ToRadians(other.Latitude);
            double lon2 = ToRadians(other.Longitude);
            
            double dLon = lon2 - lon1;
            
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                      Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            
            double bearing = Math.Atan2(y, x);
            bearing = ToDegrees(bearing);
            bearing = (bearing + 360) % 360;
            
            return bearing;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }

    /// <summary>
    /// Représente une piste d'aéroport
    /// </summary>
    public class Runway
    {
        public string Designation { get; set; } = string.Empty; // ex: "09L/27R"
        public int Length { get; set; } // en pieds
        public int Width { get; set; } // en pieds
        public string Surface { get; set; } = string.Empty; // ex: "Asphalte", "Béton"
        public bool HasLights { get; set; }
        public bool HasILS { get; set; }

        public override string ToString()
        {
            return $"Piste {Designation}, {Length} pieds";
        }
    }

    public enum AirportSize
    {
        Small,
        Medium,
        Large,
        International
    }
}

