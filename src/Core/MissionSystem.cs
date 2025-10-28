using System;
using System.Collections.Generic;
using System.Linq;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de missions et scénarios
    /// </summary>
    public class MissionSystem
    {
        private List<Mission> missions;
        private Mission currentMission;
        private MissionProgress currentProgress;
        private AccessibilityManager accessibilityManager;

        public Mission CurrentMission => currentMission;
        public MissionProgress CurrentProgress => currentProgress;
        public bool IsMissionActive => currentMission != null;

        public event EventHandler<MissionEventArgs> MissionStarted;
        public event EventHandler<MissionEventArgs> MissionCompleted;
        public event EventHandler<MissionEventArgs> MissionFailed;
        public event EventHandler<ObjectiveEventArgs> ObjectiveCompleted;

        public MissionSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.missions = new List<Mission>();
            
            InitializeMissions();
        }

        /// <summary>
        /// Initialise les missions
        /// </summary>
        private void InitializeMissions()
        {
            // Missions d'apprentissage
            missions.Add(CreateMission_FirstFlight());
            missions.Add(CreateMission_PatternWork());
            missions.Add(CreateMission_CrossCountry());
            
            // Missions de transport
            missions.Add(CreateMission_PassengerTransport());
            missions.Add(CreateMission_CargoDelivery());
            missions.Add(CreateMission_MedicalEvacuation());
            
            // Missions touristiques
            missions.Add(CreateMission_ParisOverflight());
            missions.Add(CreateMission_AlpineTour());
            missions.Add(CreateMission_CoastTour());
            
            // Missions de précision
            missions.Add(CreateMission_CourchevelChallenge());
            missions.Add(CreateMission_ShortFieldLanding());
            
            // Missions météo difficiles
            missions.Add(CreateMission_StrongWindLanding());
            missions.Add(CreateMission_LowVisibilityApproach());
        }

        #region Création des missions

        private Mission CreateMission_FirstFlight()
        {
            return new Mission
            {
                Id = "first_flight",
                Title = "Premier vol",
                Category = MissionCategory.Training,
                Difficulty = MissionDifficulty.Easy,
                Description = "Effectuez votre premier vol complet : démarrage, décollage, tour de piste et atterrissage.",
                Briefing = "Bienvenue pilote ! Cette mission vous apprendra les bases du vol. " +
                          "Vous décollerez de Paris-Orly en Cessna 172, effectuerez un tour de piste, " +
                          "et atterrirez en toute sécurité.",
                RequiredAircraft = "cessna172",
                DepartureAirport = "LFPO",
                ArrivalAirport = "LFPO",
                EstimatedDuration = TimeSpan.FromMinutes(15),
                Objectives = new List<MissionObjective>
                {
                    new MissionObjective
                    {
                        Id = "engine_start",
                        Description = "Démarrer le moteur",
                        Type = ObjectiveType.Action,
                        ValidationCondition = (state) => state.EngineRunning
                    },
                    new MissionObjective
                    {
                        Id = "takeoff",
                        Description = "Décoller",
                        Type = ObjectiveType.Action,
                        ValidationCondition = (state) => state.Altitude > 100 && !state.OnGround
                    },
                    new MissionObjective
                    {
                        Id = "reach_pattern_altitude",
                        Description = "Atteindre l'altitude du circuit (1000 pieds)",
                        Type = ObjectiveType.Altitude,
                        TargetValue = 1000,
                        Tolerance = 100,
                        ValidationCondition = (state) => Math.Abs(state.Altitude - 1000) < 100
                    },
                    new MissionObjective
                    {
                        Id = "complete_pattern",
                        Description = "Effectuer un tour de piste complet",
                        Type = ObjectiveType.Navigation,
                        ValidationCondition = (state) => state.PatternLegsCompleted >= 4
                    },
                    new MissionObjective
                    {
                        Id = "land",
                        Description = "Atterrir en douceur",
                        Type = ObjectiveType.Landing,
                        ValidationCondition = (state) => state.OnGround && state.TouchdownRate < 300
                    }
                },
                Scoring = new MissionScoring
                {
                    MaxScore = 100,
                    TimeBonus = 20,
                    PrecisionBonus = 30,
                    SmoothBonus = 20,
                    FuelBonus = 10
                }
            };
        }

        private Mission CreateMission_PassengerTransport()
        {
            return new Mission
            {
                Id = "passenger_transport",
                Title = "Transport de passagers",
                Category = MissionCategory.Transport,
                Difficulty = MissionDifficulty.Medium,
                Description = "Transportez 8 passagers de Paris à Lyon en toute sécurité et confort.",
                Briefing = "Vous pilotez un vol régional de Paris-Orly à Lyon-Saint Exupéry. " +
                          "8 passagers comptent sur vous pour un vol confortable et ponctuel. " +
                          "Évitez les turbulences excessives et respectez l'horaire.",
                RequiredAircraft = "atr72",
                DepartureAirport = "LFPO",
                ArrivalAirport = "LFLL",
                EstimatedDuration = TimeSpan.FromMinutes(45),
                Objectives = new List<MissionObjective>
                {
                    new MissionObjective
                    {
                        Id = "smooth_takeoff",
                        Description = "Décoller en douceur (taux de montée < 1500 ft/min)",
                        Type = ObjectiveType.Performance,
                        ValidationCondition = (state) => !state.OnGround && state.VerticalSpeed < 1500
                    },
                    new MissionObjective
                    {
                        Id = "cruise_altitude",
                        Description = "Atteindre l'altitude de croisière (15000 pieds)",
                        Type = ObjectiveType.Altitude,
                        TargetValue = 15000,
                        Tolerance = 500,
                        ValidationCondition = (state) => Math.Abs(state.Altitude - 15000) < 500
                    },
                    new MissionObjective
                    {
                        Id = "avoid_turbulence",
                        Description = "Minimiser les turbulences (< 5 événements)",
                        Type = ObjectiveType.Comfort,
                        ValidationCondition = (state) => state.TurbulenceEvents < 5
                    },
                    new MissionObjective
                    {
                        Id = "on_time",
                        Description = "Arriver à l'heure (± 5 minutes)",
                        Type = ObjectiveType.Time,
                        ValidationCondition = (state) => Math.Abs(state.TimeDeviation) < 300
                    },
                    new MissionObjective
                    {
                        Id = "smooth_landing",
                        Description = "Atterrir en douceur (< 200 ft/min)",
                        Type = ObjectiveType.Landing,
                        ValidationCondition = (state) => state.OnGround && state.TouchdownRate < 200
                    }
                },
                Scoring = new MissionScoring
                {
                    MaxScore = 100,
                    TimeBonus = 15,
                    PrecisionBonus = 25,
                    SmoothBonus = 30,
                    FuelBonus = 10
                }
            };
        }

        private Mission CreateMission_ParisOverflight()
        {
            return new Mission
            {
                Id = "paris_overflight",
                Title = "Survol de Paris",
                Category = MissionCategory.Scenic,
                Difficulty = MissionDifficulty.Easy,
                Description = "Survolez les monuments de Paris en hélicoptère.",
                Briefing = "Vol touristique au-dessus de Paris. Décollage du Bourget, " +
                          "survol de la Tour Eiffel, Arc de Triomphe, Sacré-Cœur et Notre-Dame, " +
                          "puis retour au Bourget. Altitude recommandée: 1500 pieds.",
                RequiredAircraft = "r44",
                DepartureAirport = "LFPB",
                ArrivalAirport = "LFPB",
                EstimatedDuration = TimeSpan.FromMinutes(30),
                Waypoints = new List<Waypoint>
                {
                    new Waypoint { Name = "Tour Eiffel", Latitude = 48.8584, Longitude = 2.2945 },
                    new Waypoint { Name = "Arc de Triomphe", Latitude = 48.8738, Longitude = 2.2950 },
                    new Waypoint { Name = "Sacré-Cœur", Latitude = 48.8867, Longitude = 2.3431 },
                    new Waypoint { Name = "Notre-Dame", Latitude = 48.8530, Longitude = 2.3499 }
                },
                Objectives = new List<MissionObjective>
                {
                    new MissionObjective
                    {
                        Id = "visit_eiffel",
                        Description = "Survoler la Tour Eiffel",
                        Type = ObjectiveType.Waypoint,
                        ValidationCondition = (state) => state.WaypointReached == "Tour Eiffel"
                    },
                    new MissionObjective
                    {
                        Id = "visit_arc",
                        Description = "Survoler l'Arc de Triomphe",
                        Type = ObjectiveType.Waypoint,
                        ValidationCondition = (state) => state.WaypointReached == "Arc de Triomphe"
                    },
                    new MissionObjective
                    {
                        Id = "visit_sacre_coeur",
                        Description = "Survoler le Sacré-Cœur",
                        Type = ObjectiveType.Waypoint,
                        ValidationCondition = (state) => state.WaypointReached == "Sacré-Cœur"
                    },
                    new MissionObjective
                    {
                        Id = "visit_notre_dame",
                        Description = "Survoler Notre-Dame",
                        Type = ObjectiveType.Waypoint,
                        ValidationCondition = (state) => state.WaypointReached == "Notre-Dame"
                    },
                    new MissionObjective
                    {
                        Id = "return_bourget",
                        Description = "Retourner au Bourget",
                        Type = ObjectiveType.Landing,
                        ValidationCondition = (state) => state.OnGround && state.AirportIcao == "LFPB"
                    }
                },
                Scoring = new MissionScoring
                {
                    MaxScore = 100,
                    TimeBonus = 10,
                    PrecisionBonus = 40,
                    SmoothBonus = 20,
                    FuelBonus = 10
                }
            };
        }

        private Mission CreateMission_CourchevelChallenge()
        {
            return new Mission
            {
                Id = "courchevel_challenge",
                Title = "Défi Courchevel",
                Category = MissionCategory.Challenge,
                Difficulty = MissionDifficulty.Expert,
                Description = "Atterrissez sur la piste la plus dangereuse d'Europe : Courchevel.",
                Briefing = "Courchevel est l'un des aérodromes les plus difficiles au monde. " +
                          "Piste courte (525m), pente de 18.5%, altitude élevée (2008m). " +
                          "Seuls les pilotes les plus expérimentés peuvent réussir cet atterrissage. " +
                          "Bonne chance !",
                RequiredAircraft = "cessna172",
                DepartureAirport = "LFLB",
                ArrivalAirport = "LFLJ",
                EstimatedDuration = TimeSpan.FromMinutes(20),
                Objectives = new List<MissionObjective>
                {
                    new MissionObjective
                    {
                        Id = "approach_altitude",
                        Description = "Approche à l'altitude correcte (8000 pieds)",
                        Type = ObjectiveType.Altitude,
                        TargetValue = 8000,
                        Tolerance = 200,
                        ValidationCondition = (state) => Math.Abs(state.Altitude - 8000) < 200
                    },
                    new MissionObjective
                    {
                        Id = "correct_speed",
                        Description = "Maintenir la vitesse d'approche (70 nœuds)",
                        Type = ObjectiveType.Speed,
                        TargetValue = 70,
                        Tolerance = 5,
                        ValidationCondition = (state) => Math.Abs(state.Speed - 70) < 5
                    },
                    new MissionObjective
                    {
                        Id = "touchdown_zone",
                        Description = "Toucher dans les 100 premiers mètres",
                        Type = ObjectiveType.Precision,
                        ValidationCondition = (state) => state.TouchdownDistance < 100
                    },
                    new MissionObjective
                    {
                        Id = "safe_landing",
                        Description = "Atterrir en toute sécurité",
                        Type = ObjectiveType.Landing,
                        ValidationCondition = (state) => state.OnGround && !state.Crashed
                    }
                },
                Scoring = new MissionScoring
                {
                    MaxScore = 100,
                    TimeBonus = 5,
                    PrecisionBonus = 50,
                    SmoothBonus = 30,
                    FuelBonus = 5
                }
            };
        }

        private Mission CreateMission_StrongWindLanding()
        {
            return new Mission
            {
                Id = "strong_wind_landing",
                Title = "Atterrissage par vent fort",
                Category = MissionCategory.Weather,
                Difficulty = MissionDifficulty.Hard,
                Description = "Atterrissez par vent de travers fort (25 nœuds).",
                Briefing = "Conditions météo difficiles aujourd'hui : vent de 25 nœuds avec rafales. " +
                          "Vous devez atterrir à Brest malgré le vent de travers important. " +
                          "Utilisez la technique du crabe et de l'aile basse.",
                RequiredAircraft = "cessna172",
                DepartureAirport = "LFRS",
                ArrivalAirport = "LFRB",
                EstimatedDuration = TimeSpan.FromMinutes(30),
                WeatherConditions = new WeatherRequirements
                {
                    MinWindSpeed = 20,
                    MaxWindSpeed = 30,
                    CrosswindComponent = 15
                },
                Objectives = new List<MissionObjective>
                {
                    new MissionObjective
                    {
                        Id = "stabilized_approach",
                        Description = "Approche stabilisée malgré le vent",
                        Type = ObjectiveType.Performance,
                        ValidationCondition = (state) => state.ApproachStabilized
                    },
                    new MissionObjective
                    {
                        Id = "centerline",
                        Description = "Rester sur l'axe de piste",
                        Type = ObjectiveType.Precision,
                        ValidationCondition = (state) => state.CenterlineDeviation < 10
                    },
                    new MissionObjective
                    {
                        Id = "safe_touchdown",
                        Description = "Toucher en toute sécurité",
                        Type = ObjectiveType.Landing,
                        ValidationCondition = (state) => state.OnGround && !state.Crashed
                    }
                },
                Scoring = new MissionScoring
                {
                    MaxScore = 100,
                    TimeBonus = 10,
                    PrecisionBonus = 40,
                    SmoothBonus = 30,
                    FuelBonus = 10
                }
            };
        }

        // Méthodes simplifiées pour les autres missions
        private Mission CreateMission_PatternWork() => new Mission { Id = "pattern_work", Title = "Tour de piste", Category = MissionCategory.Training, Difficulty = MissionDifficulty.Easy };
        private Mission CreateMission_CrossCountry() => new Mission { Id = "cross_country", Title = "Navigation", Category = MissionCategory.Training, Difficulty = MissionDifficulty.Medium };
        private Mission CreateMission_CargoDelivery() => new Mission { Id = "cargo_delivery", Title = "Livraison de fret", Category = MissionCategory.Transport, Difficulty = MissionDifficulty.Medium };
        private Mission CreateMission_MedicalEvacuation() => new Mission { Id = "medical_evac", Title = "Évacuation médicale", Category = MissionCategory.Emergency, Difficulty = MissionDifficulty.Hard };
        private Mission CreateMission_AlpineTour() => new Mission { Id = "alpine_tour", Title = "Tour des Alpes", Category = MissionCategory.Scenic, Difficulty = MissionDifficulty.Medium };
        private Mission CreateMission_CoastTour() => new Mission { Id = "coast_tour", Title = "Tour de la Côte d'Azur", Category = MissionCategory.Scenic, Difficulty = MissionDifficulty.Easy };
        private Mission CreateMission_ShortFieldLanding() => new Mission { Id = "short_field", Title = "Atterrissage court", Category = MissionCategory.Challenge, Difficulty = MissionDifficulty.Hard };
        private Mission CreateMission_LowVisibilityApproach() => new Mission { Id = "low_vis", Title = "Approche par faible visibilité", Category = MissionCategory.Weather, Difficulty = MissionDifficulty.Hard };

        #endregion

        /// <summary>
        /// Obtient toutes les missions
        /// </summary>
        public List<Mission> GetAllMissions()
        {
            return missions.ToList();
        }

        /// <summary>
        /// Obtient les missions par catégorie
        /// </summary>
        public List<Mission> GetMissionsByCategory(MissionCategory category)
        {
            return missions.Where(m => m.Category == category).ToList();
        }

        /// <summary>
        /// Démarre une mission
        /// </summary>
        public void StartMission(string missionId)
        {
            currentMission = missions.FirstOrDefault(m => m.Id == missionId);
            
            if (currentMission == null)
            {
                accessibilityManager.Speak("Mission non trouvée");
                return;
            }
            
            currentProgress = new MissionProgress
            {
                MissionId = missionId,
                StartTime = DateTime.Now,
                ObjectivesCompleted = new List<string>()
            };
            
            accessibilityManager.Speak($"Mission démarrée: {currentMission.Title}. {currentMission.Briefing}");
            MissionStarted?.Invoke(this, new MissionEventArgs(currentMission));
        }

        /// <summary>
        /// Met à jour la progression de la mission
        /// </summary>
        public void UpdateMission(MissionState state)
        {
            if (currentMission == null || currentProgress == null) return;
            
            // Vérifier chaque objectif
            foreach (var objective in currentMission.Objectives)
            {
                if (!currentProgress.ObjectivesCompleted.Contains(objective.Id))
                {
                    if (objective.ValidationCondition(state))
                    {
                        CompleteObjective(objective);
                    }
                }
            }
            
            // Vérifier si la mission est terminée
            if (currentProgress.ObjectivesCompleted.Count == currentMission.Objectives.Count)
            {
                CompleteMission(state);
            }
        }

        /// <summary>
        /// Complète un objectif
        /// </summary>
        private void CompleteObjective(MissionObjective objective)
        {
            currentProgress.ObjectivesCompleted.Add(objective.Id);
            accessibilityManager.Speak($"Objectif accompli: {objective.Description}");
            ObjectiveCompleted?.Invoke(this, new ObjectiveEventArgs(objective));
        }

        /// <summary>
        /// Complète la mission
        /// </summary>
        private void CompleteMission(MissionState state)
        {
            currentProgress.EndTime = DateTime.Now;
            currentProgress.Completed = true;
            
            // Calculer le score
            int score = CalculateScore(state);
            currentProgress.Score = score;
            
            accessibilityManager.Speak($"Mission accomplie ! Score: {score} sur {currentMission.Scoring.MaxScore}");
            MissionCompleted?.Invoke(this, new MissionEventArgs(currentMission, currentProgress));
            
            currentMission = null;
            currentProgress = null;
        }

        /// <summary>
        /// Calcule le score de la mission
        /// </summary>
        private int CalculateScore(MissionState state)
        {
            int score = 50; // Score de base
            
            // Bonus de temps
            if (state.CompletionTime <= currentMission.EstimatedDuration)
            {
                score += currentMission.Scoring.TimeBonus;
            }
            
            // Bonus de précision
            if (state.PrecisionErrors < 3)
            {
                score += currentMission.Scoring.PrecisionBonus;
            }
            
            // Bonus de douceur
            if (state.TouchdownRate < 200)
            {
                score += currentMission.Scoring.SmoothBonus;
            }
            
            // Bonus de carburant
            if (state.FuelRemaining > 0.2)
            {
                score += currentMission.Scoring.FuelBonus;
            }
            
            return Math.Min(score, currentMission.Scoring.MaxScore);
        }

        /// <summary>
        /// Annule la mission actuelle
        /// </summary>
        public void CancelMission()
        {
            if (currentMission != null)
            {
                accessibilityManager.Speak("Mission annulée");
                currentMission = null;
                currentProgress = null;
            }
        }
    }

    #region Modèles de données

    public class Mission
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public MissionCategory Category { get; set; }
        public MissionDifficulty Difficulty { get; set; }
        public string Description { get; set; }
        public string Briefing { get; set; }
        public string RequiredAircraft { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public List<Waypoint> Waypoints { get; set; }
        public List<MissionObjective> Objectives { get; set; }
        public WeatherRequirements WeatherConditions { get; set; }
        public MissionScoring Scoring { get; set; }

        public Mission()
        {
            Waypoints = new List<Waypoint>();
            Objectives = new List<MissionObjective>();
        }
    }

    public enum MissionCategory
    {
        Training,
        Transport,
        Emergency,
        Scenic,
        Challenge,
        Weather
    }

    public enum MissionDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    public class MissionObjective
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public ObjectiveType Type { get; set; }
        public double TargetValue { get; set; }
        public double Tolerance { get; set; }
        public Func<MissionState, bool> ValidationCondition { get; set; }
    }

    public enum ObjectiveType
    {
        Action,
        Altitude,
        Speed,
        Navigation,
        Waypoint,
        Landing,
        Performance,
        Comfort,
        Time,
        Precision
    }

    public class Waypoint
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class WeatherRequirements
    {
        public double MinWindSpeed { get; set; }
        public double MaxWindSpeed { get; set; }
        public double CrosswindComponent { get; set; }
    }

    public class MissionScoring
    {
        public int MaxScore { get; set; }
        public int TimeBonus { get; set; }
        public int PrecisionBonus { get; set; }
        public int SmoothBonus { get; set; }
        public int FuelBonus { get; set; }
    }

    public class MissionProgress
    {
        public string MissionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Completed { get; set; }
        public int Score { get; set; }
        public List<string> ObjectivesCompleted { get; set; }
    }

    public class MissionState
    {
        public bool EngineRunning { get; set; }
        public double Altitude { get; set; }
        public bool OnGround { get; set; }
        public int PatternLegsCompleted { get; set; }
        public double TouchdownRate { get; set; }
        public double VerticalSpeed { get; set; }
        public int TurbulenceEvents { get; set; }
        public double TimeDeviation { get; set; }
        public string WaypointReached { get; set; }
        public string AirportIcao { get; set; }
        public bool Crashed { get; set; }
        public double TouchdownDistance { get; set; }
        public bool ApproachStabilized { get; set; }
        public double CenterlineDeviation { get; set; }
        public TimeSpan CompletionTime { get; set; }
        public int PrecisionErrors { get; set; }
        public double FuelRemaining { get; set; }
        public double Speed { get; set; }
    }

    public class MissionEventArgs : EventArgs
    {
        public Mission Mission { get; }
        public MissionProgress Progress { get; }

        public MissionEventArgs(Mission mission, MissionProgress progress = null)
        {
            Mission = mission;
            Progress = progress;
        }
    }

    public class ObjectiveEventArgs : EventArgs
    {
        public MissionObjective Objective { get; }

        public ObjectiveEventArgs(MissionObjective objective)
        {
            Objective = objective;
        }
    }

    #endregion
}

