using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de communication radio réaliste avec phraséologie OACI
    /// </summary>
    public class RadioCommunicationSystem
    {
        private AccessibilityManager accessibilityManager;
        private SpeechSynthesizer atcVoice;
        private Dictionary<string, ATCController> controllers;
        private string currentFrequency;
        private Airport currentAirport;
        private Aircraft currentAircraft;
        private string callSign;
        private FlightPhase currentPhase;
        private Queue<RadioMessage> messageQueue;
        private bool radioEnabled;

        public event EventHandler<RadioMessageEventArgs> MessageReceived;
        public event EventHandler<RadioMessageEventArgs> MessageSent;

        public bool RadioEnabled => radioEnabled;
        public string CurrentFrequency => currentFrequency;

        public RadioCommunicationSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.atcVoice = new SpeechSynthesizer();
            this.controllers = new Dictionary<string, ATCController>();
            this.messageQueue = new Queue<RadioMessage>();
            this.radioEnabled = true;
            
            ConfigureATCVoice();
            InitializeControllers();
        }

        /// <summary>
        /// Configure la voix du contrôleur aérien
        /// </summary>
        private void ConfigureATCVoice()
        {
            // Utiliser une voix différente pour l'ATC
            atcVoice.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
            atcVoice.Rate = -1; // Légèrement plus lent pour clarté
            atcVoice.Volume = 100;
        }

        /// <summary>
        /// Initialise les contrôleurs pour les aéroports
        /// </summary>
        private void InitializeControllers()
        {
            // Aéroports français majeurs
            controllers.Add("LFPO", new ATCController 
            { 
                AirportIcao = "LFPO",
                AirportName = "Paris-Orly",
                TowerFrequency = "118.725",
                GroundFrequency = "121.750",
                AtisFrequency = "128.125",
                ApproachFrequency = "119.150",
                ControllerName = "Tour Orly",
                ControllerCallsign = "Orly Tour"
            });

            controllers.Add("LFPG", new ATCController 
            { 
                AirportIcao = "LFPG",
                AirportName = "Paris Charles de Gaulle",
                TowerFrequency = "118.150",
                GroundFrequency = "121.650",
                AtisFrequency = "128.025",
                ApproachFrequency = "119.250",
                ControllerName = "Tour Roissy",
                ControllerCallsign = "Roissy Tour"
            });

            controllers.Add("LFLL", new ATCController 
            { 
                AirportIcao = "LFLL",
                AirportName = "Lyon Saint-Exupéry",
                TowerFrequency = "118.100",
                GroundFrequency = "121.750",
                AtisFrequency = "127.975",
                ApproachFrequency = "119.100",
                ControllerName = "Tour Lyon",
                ControllerCallsign = "Lyon Tour"
            });

            controllers.Add("LFML", new ATCController 
            { 
                AirportIcao = "LFML",
                AirportName = "Marseille Provence",
                TowerFrequency = "118.300",
                GroundFrequency = "121.700",
                AtisFrequency = "127.125",
                ApproachFrequency = "119.700",
                ControllerName = "Tour Marseille",
                ControllerCallsign = "Marseille Tour"
            });

            controllers.Add("LFBD", new ATCController 
            { 
                AirportIcao = "LFBD",
                AirportName = "Bordeaux Mérignac",
                TowerFrequency = "118.300",
                GroundFrequency = "121.750",
                AtisFrequency = "126.425",
                ApproachFrequency = "119.500",
                ControllerName = "Tour Bordeaux",
                ControllerCallsign = "Bordeaux Tour"
            });

            // Ajouter d'autres contrôleurs selon besoin
        }

        /// <summary>
        /// Initialise le vol avec l'aéroport et l'avion
        /// </summary>
        public void InitializeFlight(Airport airport, Aircraft aircraft)
        {
            currentAirport = airport;
            currentAircraft = aircraft;
            currentPhase = FlightPhase.PreFlight;
            
            // Générer l'indicatif d'appel
            GenerateCallSign();
            
            // Se syntoniser sur ATIS
            TuneFrequency(GetATISFrequency());
        }

        /// <summary>
        /// Génère un indicatif d'appel réaliste
        /// </summary>
        private void GenerateCallSign()
        {
            Random random = new Random();
            
            if (currentAircraft.Type == "airline")
            {
                // Compagnie aérienne
                string[] airlines = { "Air France", "Transavia", "EasyJet", "Volotea" };
                string airline = airlines[random.Next(airlines.Length)];
                int flightNumber = random.Next(100, 9999);
                callSign = $"{airline} {flightNumber}";
            }
            else
            {
                // Aviation générale
                string registration = $"Foxtrot {GetPhoneticLetter()} {GetPhoneticLetter()} {GetPhoneticLetter()} {GetPhoneticLetter()}";
                callSign = registration;
            }
        }

        /// <summary>
        /// Obtient une lettre phonétique aléatoire
        /// </summary>
        private string GetPhoneticLetter()
        {
            string[] phonetic = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", 
                                 "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", 
                                 "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", 
                                 "Sierra", "Tango", "Uniform", "Victor", "Whiskey", 
                                 "X-ray", "Yankee", "Zulu" };
            return phonetic[new Random().Next(phonetic.Length)];
        }

        /// <summary>
        /// Se syntonise sur une fréquence
        /// </summary>
        public void TuneFrequency(string frequency)
        {
            currentFrequency = frequency;
            accessibilityManager.Speak($"Fréquence {FormatFrequency(frequency)}");
            
            // Jouer le son de syntonisation
            PlayRadioTuneSound();
        }

        /// <summary>
        /// Formate la fréquence pour l'annonce vocale
        /// </summary>
        private string FormatFrequency(string frequency)
        {
            // Convertir 118.725 en "un un huit décimal sept deux cinq"
            return frequency.Replace(".", " décimal ");
        }

        /// <summary>
        /// Obtient la fréquence ATIS
        /// </summary>
        private string GetATISFrequency()
        {
            if (controllers.ContainsKey(currentAirport.IcaoCode))
            {
                return controllers[currentAirport.IcaoCode].AtisFrequency;
            }
            return "128.000";
        }

        /// <summary>
        /// Écoute l'ATIS
        /// </summary>
        public void ListenATIS()
        {
            if (!controllers.ContainsKey(currentAirport.IcaoCode))
            {
                accessibilityManager.Speak("ATIS non disponible pour cet aéroport");
                return;
            }

            var controller = controllers[currentAirport.IcaoCode];
            var weather = GetCurrentWeather(); // À obtenir du WeatherSystem
            
            string atis = GenerateATIS(controller, weather);
            
            // Annoncer l'ATIS avec la voix ATC
            PlayRadioStatic();
            atcVoice.SpeakAsync(atis);
            
            accessibilityManager.Speak($"ATIS {currentAirport.Name}");
        }

        /// <summary>
        /// Génère le message ATIS
        /// </summary>
        private string GenerateATIS(ATCController controller, WeatherData weather)
        {
            string letter = GetATISLetter();
            
            string atis = $"{controller.AirportName} information {letter}. " +
                         $"Heure {DateTime.UtcNow:HHmm} Zulu. " +
                         $"Piste en service {GetActiveRunway()}. " +
                         $"Vent {weather.WindDirection:000} degrés {(int)weather.WindSpeed} nœuds. ";
            
            if (weather.WindGust > weather.WindSpeed + 5)
            {
                atis += $"Rafales {(int)weather.WindGust} nœuds. ";
            }
            
            atis += $"Visibilité {(int)weather.Visibility} kilomètres. ";
            atis += $"{weather.Description}. ";
            atis += $"Température {(int)weather.Temperature} degrés. ";
            atis += $"Point de rosée {(int)CalculateDewPoint(weather)} degrés. ";
            atis += $"QNH {(int)(weather.Pressure / 0.02953):0000}. ";
            atis += $"Rappelez information {letter} au contact.";
            
            return atis;
        }

        /// <summary>
        /// Obtient la lettre ATIS actuelle
        /// </summary>
        private string GetATISLetter()
        {
            // Utiliser l'heure pour déterminer la lettre (change toutes les heures)
            string[] letters = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", 
                               "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima" };
            return letters[DateTime.UtcNow.Hour % letters.Length];
        }

        /// <summary>
        /// Demande l'autorisation de démarrage
        /// </summary>
        public void RequestStartupClearance()
        {
            if (currentPhase != FlightPhase.PreFlight)
            {
                accessibilityManager.Speak("Demande non appropriée pour cette phase de vol");
                return;
            }

            // Message du pilote
            string pilotMessage = $"{GetControllerCallsign()}, bonjour, {callSign}, " +
                                 $"au parking, information {GetATISLetter()}, " +
                                 $"demande autorisation de démarrage.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            // Réponse du contrôleur (après un délai)
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, {GetControllerCallsign()}, bonjour, " +
                                   $"autorisé démarrage moteur, QNH {GetQNH()}, " +
                                   $"rappelez prêt à rouler.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.EngineStart;
            });
        }

        /// <summary>
        /// Demande l'autorisation de rouler
        /// </summary>
        public void RequestTaxiClearance()
        {
            if (currentPhase != FlightPhase.EngineStart)
            {
                accessibilityManager.Speak("Demande non appropriée pour cette phase de vol");
                return;
            }

            // Changer de fréquence vers Ground
            TuneFrequency(GetGroundFrequency());
            
            string pilotMessage = $"{GetGroundCallsign()}, {callSign}, " +
                                 $"prêt à rouler, piste {GetActiveRunway()}.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string taxiway = GetTaxiway();
                string atcResponse = $"{callSign}, autorisé rouler point d'arrêt piste {GetActiveRunway()}, " +
                                   $"via {taxiway}, QNH {GetQNH()}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Taxi;
            });
        }

        /// <summary>
        /// Demande l'autorisation de décollage
        /// </summary>
        public void RequestTakeoffClearance()
        {
            if (currentPhase != FlightPhase.Taxi)
            {
                accessibilityManager.Speak("Demande non appropriée pour cette phase de vol");
                return;
            }

            // Changer de fréquence vers Tower
            TuneFrequency(GetTowerFrequency());
            
            string pilotMessage = $"{GetControllerCallsign()}, {callSign}, " +
                                 $"au point d'arrêt piste {GetActiveRunway()}, prêt au départ.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(3000).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, piste {GetActiveRunway()}, autorisé décollage, " +
                                   $"vent {GetWindInfo()}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Takeoff;
            });
        }

        /// <summary>
        /// Annonce le décollage effectué
        /// </summary>
        public void ReportAirborne()
        {
            if (currentPhase != FlightPhase.Takeoff)
            {
                return;
            }

            string pilotMessage = $"{callSign}, décollé.";
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(1500).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, contactez départ {GetApproachFrequency()}, bonne journée.";
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Departure;
            });
        }

        /// <summary>
        /// Contact avec l'approche
        /// </summary>
        public void ContactApproach()
        {
            TuneFrequency(GetApproachFrequency());
            
            string pilotMessage = $"Approche {currentAirport.Name}, bonjour, {callSign}, " +
                                 $"passant {GetCurrentAltitude()} pieds, montant niveau de vol {GetCruiseLevel()}.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, approche {currentAirport.Name}, bonjour, " +
                                   $"radar vous identifie, continuez montée niveau de vol {GetCruiseLevel()}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Cruise;
            });
        }

        /// <summary>
        /// Demande l'approche pour l'atterrissage
        /// </summary>
        public void RequestApproach(Airport destinationAirport)
        {
            currentAirport = destinationAirport;
            
            string pilotMessage = $"Approche {destinationAirport.Name}, {callSign}, " +
                                 $"demande approche piste {GetActiveRunway()}.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, approuvé approche ILS piste {GetActiveRunway()}, " +
                                   $"descendez et maintenez {GetApproachAltitude()} pieds, QNH {GetQNH()}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Approach;
            });
        }

        /// <summary>
        /// Demande l'autorisation d'atterrissage
        /// </summary>
        public void RequestLandingClearance()
        {
            if (currentPhase != FlightPhase.Approach)
            {
                accessibilityManager.Speak("Demande non appropriée pour cette phase de vol");
                return;
            }

            TuneFrequency(GetTowerFrequency());
            
            string pilotMessage = $"{GetControllerCallsign()}, {callSign}, " +
                                 $"finale piste {GetActiveRunway()}.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, piste {GetActiveRunway()}, autorisé atterrir, " +
                                   $"vent {GetWindInfo()}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.Landing;
            });
        }

        /// <summary>
        /// Annonce l'atterrissage effectué
        /// </summary>
        public void ReportLanded()
        {
            if (currentPhase != FlightPhase.Landing)
            {
                return;
            }

            string pilotMessage = $"{callSign}, piste dégagée.";
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(1500).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, contactez sol {GetGroundFrequency()}.";
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.TaxiIn;
            });
        }

        /// <summary>
        /// Demande le roulage vers le parking
        /// </summary>
        public void RequestTaxiToParking()
        {
            TuneFrequency(GetGroundFrequency());
            
            string pilotMessage = $"{GetGroundCallsign()}, {callSign}, " +
                                 $"demande roulage parking.";
            
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                string taxiway = GetTaxiway();
                string atcResponse = $"{callSign}, autorisé rouler parking via {taxiway}.";
                
                ReceiveMessage(atcResponse, MessageType.ATC);
            });
        }

        /// <summary>
        /// Annonce arrivé au parking
        /// </summary>
        public void ReportParking()
        {
            string pilotMessage = $"{callSign}, au parking.";
            SendMessage(pilotMessage, MessageType.Pilot);
            
            System.Threading.Tasks.Task.Delay(1500).ContinueWith(t =>
            {
                string atcResponse = $"{callSign}, bien reçu, bonne journée.";
                ReceiveMessage(atcResponse, MessageType.ATC);
                currentPhase = FlightPhase.PostFlight;
            });
        }

        /// <summary>
        /// Envoie un message radio
        /// </summary>
        private void SendMessage(string message, MessageType type)
        {
            PlayRadioStatic();
            accessibilityManager.Speak(message);
            
            var radioMessage = new RadioMessage
            {
                Content = message,
                Type = type,
                Timestamp = DateTime.Now,
                Frequency = currentFrequency
            };
            
            messageQueue.Enqueue(radioMessage);
            MessageSent?.Invoke(this, new RadioMessageEventArgs(radioMessage));
        }

        /// <summary>
        /// Reçoit un message radio
        /// </summary>
        private void ReceiveMessage(string message, MessageType type)
        {
            PlayRadioStatic();
            atcVoice.SpeakAsync(message);
            
            var radioMessage = new RadioMessage
            {
                Content = message,
                Type = type,
                Timestamp = DateTime.Now,
                Frequency = currentFrequency
            };
            
            messageQueue.Enqueue(radioMessage);
            MessageReceived?.Invoke(this, new RadioMessageEventArgs(radioMessage));
        }

        #region Méthodes utilitaires

        private string GetControllerCallsign()
        {
            return controllers.ContainsKey(currentAirport.IcaoCode) 
                ? controllers[currentAirport.IcaoCode].ControllerCallsign 
                : "Tour";
        }

        private string GetGroundCallsign()
        {
            return controllers.ContainsKey(currentAirport.IcaoCode) 
                ? controllers[currentAirport.IcaoCode].ControllerCallsign.Replace("Tour", "Sol") 
                : "Sol";
        }

        private string GetTowerFrequency()
        {
            return controllers.ContainsKey(currentAirport.IcaoCode) 
                ? controllers[currentAirport.IcaoCode].TowerFrequency 
                : "118.100";
        }

        private string GetGroundFrequency()
        {
            return controllers.ContainsKey(currentAirport.IcaoCode) 
                ? controllers[currentAirport.IcaoCode].GroundFrequency 
                : "121.700";
        }

        private string GetApproachFrequency()
        {
            return controllers.ContainsKey(currentAirport.IcaoCode) 
                ? controllers[currentAirport.IcaoCode].ApproachFrequency 
                : "119.100";
        }

        private string GetActiveRunway()
        {
            // Simplification : retourner la piste principale
            return "08"; // À améliorer avec calcul selon le vent
        }

        private string GetQNH()
        {
            // À obtenir du WeatherSystem
            return "1013";
        }

        private string GetWindInfo()
        {
            // À obtenir du WeatherSystem
            return "270 degrés 10 nœuds";
        }

        private string GetTaxiway()
        {
            string[] taxiways = { "Alpha", "Bravo", "Charlie", "Delta" };
            return taxiways[new Random().Next(taxiways.Length)];
        }

        private int GetCurrentAltitude()
        {
            // À obtenir du FlightState
            return 1000;
        }

        private int GetCruiseLevel()
        {
            // À calculer selon la distance
            return 150; // FL150
        }

        private int GetApproachAltitude()
        {
            return 3000;
        }

        private WeatherData GetCurrentWeather()
        {
            // À obtenir du WeatherSystem
            return new WeatherData
            {
                Temperature = 15,
                Pressure = 29.92,
                WindSpeed = 10,
                WindDirection = 270,
                WindGust = 15,
                Visibility = 10,
                Description = "Ciel dégagé"
            };
        }

        private int CalculateDewPoint(WeatherData weather)
        {
            // Formule simplifiée
            return (int)(weather.Temperature - 5);
        }

        private void PlayRadioStatic()
        {
            // Jouer le son de parasites radio
            // À implémenter avec le système audio
        }

        private void PlayRadioTuneSound()
        {
            // Jouer le son de syntonisation
            // À implémenter avec le système audio
        }

        #endregion
    }

    #region Modèles de données

    public class ATCController
    {
        public string AirportIcao { get; set; }
        public string AirportName { get; set; }
        public string TowerFrequency { get; set; }
        public string GroundFrequency { get; set; }
        public string AtisFrequency { get; set; }
        public string ApproachFrequency { get; set; }
        public string ControllerName { get; set; }
        public string ControllerCallsign { get; set; }
    }

    public enum FlightPhase
    {
        PreFlight,
        EngineStart,
        Taxi,
        Takeoff,
        Departure,
        Cruise,
        Descent,
        Approach,
        Landing,
        TaxiIn,
        PostFlight
    }

    public class RadioMessage
    {
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string Frequency { get; set; }
    }

    public enum MessageType
    {
        Pilot,
        ATC,
        ATIS,
        Traffic
    }

    public class RadioMessageEventArgs : EventArgs
    {
        public RadioMessage Message { get; }

        public RadioMessageEventArgs(RadioMessage message)
        {
            Message = message;
        }
    }

    #endregion
}

