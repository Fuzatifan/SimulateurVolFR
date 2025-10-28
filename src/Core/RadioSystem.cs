using System;
using System.Collections.Generic;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de communications radio
    /// </summary>
    public class RadioSystem
    {
        private Airport? currentAirport;
        private FlightState? flightState;
        private string currentFrequency;
        private RadioType currentRadioType;

        public string CurrentFrequency => currentFrequency;
        public RadioType CurrentRadioType => currentRadioType;

        public event EventHandler<RadioMessageEventArgs>? MessageReceived;

        public RadioSystem()
        {
            currentFrequency = "118.00";
            currentRadioType = RadioType.Tower;
        }

        /// <summary>
        /// Initialise le système radio
        /// </summary>
        public void Initialize(Airport airport, FlightState flightState)
        {
            this.currentAirport = airport;
            this.flightState = flightState;
            
            // Syntoniser la fréquence de la tour par défaut
            if (!string.IsNullOrEmpty(airport.TowerFrequency))
            {
                TuneFrequency(airport.TowerFrequency, RadioType.Tower);
            }
        }

        /// <summary>
        /// Syntonise une fréquence radio
        /// </summary>
        public void TuneFrequency(string frequency, RadioType type)
        {
            currentFrequency = frequency;
            currentRadioType = type;
            
            string message = $"Fréquence {frequency} syntonisée. {GetRadioTypeName(type)}.";
            AccessibilityManager.Speak(message);
            
            // Envoyer un message de bienvenue selon le type
            SendWelcomeMessage(type);
        }

        /// <summary>
        /// Demande l'autorisation de démarrage des moteurs
        /// </summary>
        public void RequestStartupClearance()
        {
            if (currentAirport == null) return;

            string callsign = GenerateCallsign();
            string request = $"{callsign}, demande autorisation de démarrage des moteurs.";
            
            SendMessage(request, MessageDirection.Outgoing);
            
            // Simuler une réponse de la tour
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
            {
                string response = $"{callsign}, autorisation accordée pour démarrage des moteurs. " +
                                $"QNH 1013, température 15 degrés, vent 270 à 10 nœuds.";
                SendMessage(response, MessageDirection.Incoming);
            });
        }

        /// <summary>
        /// Demande l'autorisation de roulage
        /// </summary>
        public void RequestTaxiClearance()
        {
            if (currentAirport == null) return;

            string callsign = GenerateCallsign();
            string request = $"{callsign}, demande autorisation de roulage.";
            
            SendMessage(request, MessageDirection.Outgoing);
            
            // Simuler une réponse
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
            {
                string runway = GetActiveRunway();
                string response = $"{callsign}, autorisé à rouler vers la piste {runway} via taxiway Alpha.";
                SendMessage(response, MessageDirection.Incoming);
            });
        }

        /// <summary>
        /// Demande l'autorisation de décollage
        /// </summary>
        public void RequestTakeoffClearance()
        {
            if (currentAirport == null) return;

            string callsign = GenerateCallsign();
            string runway = GetActiveRunway();
            string request = $"{callsign}, prêt au décollage piste {runway}.";
            
            SendMessage(request, MessageDirection.Outgoing);
            
            // Simuler une réponse
            System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ =>
            {
                string response = $"{callsign}, autorisé au décollage piste {runway}. Vent 270 à 10 nœuds.";
                SendMessage(response, MessageDirection.Incoming);
            });
        }

        /// <summary>
        /// Demande l'autorisation d'atterrissage
        /// </summary>
        public void RequestLandingClearance()
        {
            if (currentAirport == null) return;

            string callsign = GenerateCallsign();
            string request = $"{callsign}, demande autorisation d'atterrissage.";
            
            SendMessage(request, MessageDirection.Outgoing);
            
            // Simuler une réponse
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
            {
                string runway = GetActiveRunway();
                string response = $"{callsign}, autorisé à l'atterrissage piste {runway}. Vent 270 à 10 nœuds.";
                SendMessage(response, MessageDirection.Incoming);
            });
        }

        /// <summary>
        /// Demande les informations ATIS
        /// </summary>
        public void RequestATIS()
        {
            if (currentAirport == null) return;

            string atis = GenerateATIS();
            SendMessage(atis, MessageDirection.Incoming);
        }

        /// <summary>
        /// Génère un message ATIS
        /// </summary>
        private string GenerateATIS()
        {
            if (currentAirport == null) return "";

            string runway = GetActiveRunway();
            string time = DateTime.Now.ToString("HH:mm");
            
            return $"Information {GetATISLetter()} de {currentAirport.Name}. " +
                   $"Heure {time} UTC. " +
                   $"Piste en service: {runway}. " +
                   $"Vent 270 degrés à 10 nœuds. " +
                   $"Visibilité 10 kilomètres. " +
                   $"Nuages épars à 3000 pieds. " +
                   $"Température 15 degrés. " +
                   $"Point de rosée 8 degrés. " +
                   $"QNH 1013 hectopascals. " +
                   $"Fin de l'information {GetATISLetter()}.";
        }

        /// <summary>
        /// Obtient la lettre ATIS actuelle
        /// </summary>
        private string GetATISLetter()
        {
            // Utiliser l'heure pour déterminer la lettre
            int hour = DateTime.Now.Hour;
            char letter = (char)('A' + (hour % 26));
            return letter.ToString();
        }

        /// <summary>
        /// Obtient la piste active
        /// </summary>
        private string GetActiveRunway()
        {
            if (currentAirport == null || currentAirport.Runways.Count == 0)
                return "09";

            // Retourner la première piste disponible
            return currentAirport.Runways[0].Designation.Split('/')[0];
        }

        /// <summary>
        /// Génère un indicatif d'appel
        /// </summary>
        private string GenerateCallsign()
        {
            // Générer un indicatif simple
            return "FBETA123";
        }

        /// <summary>
        /// Envoie un message radio
        /// </summary>
        private void SendMessage(string message, MessageDirection direction)
        {
            var args = new RadioMessageEventArgs(message, direction, currentFrequency, currentRadioType);
            MessageReceived?.Invoke(this, args);
            
            // Annoncer le message si c'est un message entrant
            if (direction == MessageDirection.Incoming)
            {
                AccessibilityManager.Speak(message);
            }
        }

        /// <summary>
        /// Envoie un message de bienvenue selon le type de radio
        /// </summary>
        private void SendWelcomeMessage(RadioType type)
        {
            string message = type switch
            {
                RadioType.Tower => "Tour de contrôle",
                RadioType.Ground => "Contrôle au sol",
                RadioType.ATIS => "Information ATIS automatique",
                RadioType.Approach => "Contrôle d'approche",
                RadioType.Departure => "Contrôle de départ",
                _ => "Radio"
            };
            
            SendMessage($"{message} sur fréquence {currentFrequency}", MessageDirection.System);
        }

        /// <summary>
        /// Obtient le nom du type de radio
        /// </summary>
        private string GetRadioTypeName(RadioType type)
        {
            return type switch
            {
                RadioType.Tower => "Tour de contrôle",
                RadioType.Ground => "Contrôle au sol",
                RadioType.ATIS => "ATIS",
                RadioType.Approach => "Approche",
                RadioType.Departure => "Départ",
                _ => "Radio"
            };
        }
    }

    public enum RadioType
    {
        Tower,      // Tour de contrôle
        Ground,     // Contrôle au sol
        ATIS,       // Information automatique
        Approach,   // Approche
        Departure   // Départ
    }

    public enum MessageDirection
    {
        Incoming,   // Message reçu
        Outgoing,   // Message envoyé
        System      // Message système
    }

    public class RadioMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public MessageDirection Direction { get; }
        public string Frequency { get; }
        public RadioType RadioType { get; }
        public DateTime Timestamp { get; }

        public RadioMessageEventArgs(string message, MessageDirection direction, string frequency, RadioType radioType)
        {
            Message = message;
            Direction = direction;
            Frequency = frequency;
            RadioType = radioType;
            Timestamp = DateTime.Now;
        }
    }
}

