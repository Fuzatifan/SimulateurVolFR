using System;

namespace SimulateurVolFR.Models
{
    /// <summary>
    /// Représente l'état actuel du vol
    /// </summary>
    public class FlightState
    {
        // Position
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; } // en pieds
        
        // Vitesse et direction
        public double Speed { get; set; } // en nœuds (vitesse sol)
        public double IndicatedAirspeed { get; set; } // en nœuds (vitesse air)
        public double Heading { get; set; } // cap magnétique en degrés (0-360)
        public double Track { get; set; } // route suivie en degrés
        
        // Attitude
        public double Pitch { get; set; } // tangage en degrés (-90 à +90)
        public double Roll { get; set; } // roulis en degrés (-180 à +180)
        public double VerticalSpeed { get; set; } // vitesse verticale en pieds/minute
        
        // Moteurs et systèmes
        public double ThrottlePosition { get; set; } // position des gaz (0-100%)
        public bool EnginesRunning { get; set; }
        public double FuelRemaining { get; set; } // carburant restant en litres
        public double FuelFlow { get; set; } // consommation en litres/heure
        
        // Contrôles de vol
        public double FlapPosition { get; set; } // position des volets (0-100%)
        public bool GearDown { get; set; } // train d'atterrissage sorti
        public bool BrakesEngaged { get; set; } // freins engagés
        public bool AutopilotEnabled { get; set; } // pilote automatique actif
        
        // Phase de vol
        public FlightPhase Phase { get; set; }
        
        // Météo
        public double WindSpeed { get; set; } // vitesse du vent en nœuds
        public double WindDirection { get; set; } // direction du vent en degrés
        public double Temperature { get; set; } // température en Celsius
        public double Visibility { get; set; } // visibilité en miles nautiques
        
        // Temps
        public DateTime FlightStartTime { get; set; }
        public TimeSpan FlightDuration { get; set; }

        public FlightState()
        {
            FlightStartTime = DateTime.Now;
            Phase = FlightPhase.PreFlight;
        }

        /// <summary>
        /// Obtient une description vocale de l'état actuel
        /// </summary>
        public string GetVoiceStatus()
        {
            return $"Altitude: {Math.Round(Altitude)} pieds. " +
                   $"Vitesse: {Math.Round(IndicatedAirspeed)} nœuds. " +
                   $"Cap: {Math.Round(Heading)} degrés. " +
                   $"Vitesse verticale: {Math.Round(VerticalSpeed)} pieds par minute.";
        }

        /// <summary>
        /// Obtient la phase de vol en français
        /// </summary>
        public string GetPhaseDescription()
        {
            return Phase switch
            {
                FlightPhase.PreFlight => "Pré-vol",
                FlightPhase.Taxi => "Roulage",
                FlightPhase.Takeoff => "Décollage",
                FlightPhase.Climb => "Montée",
                FlightPhase.Cruise => "Croisière",
                FlightPhase.Descent => "Descente",
                FlightPhase.Approach => "Approche",
                FlightPhase.Landing => "Atterrissage",
                FlightPhase.Landed => "Atterri",
                _ => "Inconnu"
            };
        }

        /// <summary>
        /// Calcule le temps de vol estimé vers une destination
        /// </summary>
        public TimeSpan EstimatedTimeToDestination(double distanceNM)
        {
            if (Speed <= 0) return TimeSpan.Zero;
            double hours = distanceNM / Speed;
            return TimeSpan.FromHours(hours);
        }

        /// <summary>
        /// Calcule l'autonomie restante en heures
        /// </summary>
        public double GetRemainingEndurance()
        {
            if (FuelFlow <= 0) return 0;
            return FuelRemaining / FuelFlow;
        }

        /// <summary>
        /// Vérifie si l'avion est en vol
        /// </summary>
        public bool IsAirborne()
        {
            return Altitude > 50 && !GearDown;
        }

        /// <summary>
        /// Vérifie si l'avion est au sol
        /// </summary>
        public bool IsOnGround()
        {
            return Altitude <= 50;
        }
    }

    public enum FlightPhase
    {
        PreFlight,      // Avant le vol
        Taxi,           // Roulage
        Takeoff,        // Décollage
        Climb,          // Montée
        Cruise,         // Croisière
        Descent,        // Descente
        Approach,       // Approche
        Landing,        // Atterrissage
        Landed          // Atterri
    }
}

