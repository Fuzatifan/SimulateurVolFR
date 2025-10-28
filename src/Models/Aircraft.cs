using System;

namespace SimulateurVolFR.Models
{
    /// <summary>
    /// Représente un avion dans le simulateur
    /// </summary>
    public class Aircraft
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public AircraftType Type { get; set; }
        
        // Caractéristiques physiques
        public double MaxSpeed { get; set; } // en nœuds
        public double CruiseSpeed { get; set; } // en nœuds
        public double MaxAltitude { get; set; } // en pieds
        public double Range { get; set; } // en miles nautiques
        public double FuelCapacity { get; set; } // en litres
        public double FuelConsumption { get; set; } // litres par heure
        public int PassengerCapacity { get; set; }
        public double EmptyWeight { get; set; } // en kg
        public double MaxTakeoffWeight { get; set; } // en kg
        
        // Caractéristiques de vol
        public double TakeoffSpeed { get; set; } // en nœuds
        public double LandingSpeed { get; set; } // en nœuds
        public double ClimbRate { get; set; } // pieds par minute
        public double DescentRate { get; set; } // pieds par minute
        public double TurnRate { get; set; } // degrés par seconde
        
        // Moteurs
        public int EngineCount { get; set; }
        public string EngineType { get; set; } = string.Empty;
        
        // Audio
        public string EngineSound { get; set; } = string.Empty;
        public string CabinSound { get; set; } = string.Empty;
        
        public Aircraft()
        {
        }

        public override string ToString()
        {
            return $"{Manufacturer} {Name}";
        }

        /// <summary>
        /// Obtient une description vocale de l'avion
        /// </summary>
        public string GetVoiceDescription()
        {
            return $"{Manufacturer} {Name}. {GetTypeDescription()}. " +
                   $"Vitesse de croisière: {CruiseSpeed} nœuds. " +
                   $"Altitude maximale: {MaxAltitude} pieds. " +
                   $"Capacité: {PassengerCapacity} passagers.";
        }

        private string GetTypeDescription()
        {
            return Type switch
            {
                AircraftType.LightAircraft => "Avion léger",
                AircraftType.BusinessJet => "Jet d'affaires",
                AircraftType.RegionalAirliner => "Avion régional",
                AircraftType.NarrowBodyAirliner => "Avion moyen-courrier",
                AircraftType.WideBodyAirliner => "Avion long-courrier",
                AircraftType.Cargo => "Avion cargo",
                _ => "Avion"
            };
        }
    }

    public enum AircraftType
    {
        LightAircraft,
        BusinessJet,
        RegionalAirliner,
        NarrowBodyAirliner,
        WideBodyAirliner,
        Cargo
    }
}

