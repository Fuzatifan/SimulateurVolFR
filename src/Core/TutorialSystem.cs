using System;
using System.Collections.Generic;
using System.Linq;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de tutoriel vocal guidé interactif
    /// </summary>
    public class TutorialSystem
    {
        private List<TutorialStep> steps;
        private int currentStepIndex;
        private TutorialStep currentStep;
        private AccessibilityManager accessibilityManager;
        private bool isActive;
        private TutorialProgress progress;

        public event EventHandler<TutorialStepEventArgs> StepCompleted;
        public event EventHandler TutorialCompleted;

        public bool IsActive => isActive;
        public int CurrentStepNumber => currentStepIndex + 1;
        public int TotalSteps => steps.Count;

        public TutorialSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.steps = new List<TutorialStep>();
            this.progress = new TutorialProgress();
            
            InitializeTutorialSteps();
        }

        /// <summary>
        /// Initialise les étapes du tutoriel
        /// </summary>
        private void InitializeTutorialSteps()
        {
            // Étape 1: Découverte de l'interface
            steps.Add(new TutorialStep
            {
                Id = "interface_discovery",
                Title = "Découverte de l'interface",
                Description = "Bienvenue dans le Simulateur de Vol Français ! " +
                             "Ce tutoriel vous guidera pas à pas pour apprendre à utiliser le simulateur. " +
                             "Appuyez sur Alt pour accéder au menu principal.",
                Objective = "Ouvrir le menu principal",
                ValidationCondition = (state) => state.MenuOpen,
                Hints = new List<string>
                {
                    "Appuyez sur la touche Alt de votre clavier",
                    "Le menu principal contient toutes les fonctions du simulateur",
                    "Vous pouvez naviguer dans le menu avec les flèches"
                }
            });

            // Étape 2: Navigation dans les menus
            steps.Add(new TutorialStep
            {
                Id = "menu_navigation",
                Title = "Navigation dans les menus",
                Description = "Excellent ! Le menu est maintenant ouvert. " +
                             "Utilisez les flèches Haut et Bas pour naviguer entre les options. " +
                             "Appuyez sur Entrée pour sélectionner 'Nouveau Vol'.",
                Objective = "Sélectionner 'Nouveau Vol' dans le menu",
                ValidationCondition = (state) => state.NewFlightSelected,
                Hints = new List<string>
                {
                    "Utilisez Flèche Bas pour descendre dans le menu",
                    "Chaque option est annoncée vocalement",
                    "Appuyez sur Entrée pour valider votre choix"
                }
            });

            // Étape 3: Sélection de l'avion
            steps.Add(new TutorialStep
            {
                Id = "aircraft_selection",
                Title = "Sélection de l'avion",
                Description = "Parfait ! Vous êtes maintenant dans l'écran de sélection d'avion. " +
                             "Pour ce premier vol, nous utiliserons le Cessna 172 Skyhawk, " +
                             "l'avion d'école par excellence. Naviguez avec les flèches et sélectionnez-le.",
                Objective = "Sélectionner le Cessna 172 Skyhawk",
                ValidationCondition = (state) => state.SelectedAircraft == "cessna172",
                Hints = new List<string>
                {
                    "Le Cessna 172 est dans la catégorie 'Avions légers'",
                    "Écoutez attentivement les descriptions vocales",
                    "Appuyez sur Entrée pour confirmer votre choix"
                }
            });

            // Étape 4: Sélection de l'aéroport de départ
            steps.Add(new TutorialStep
            {
                Id = "departure_airport",
                Title = "Choix de l'aéroport de départ",
                Description = "Très bien ! Maintenant, choisissons un aéroport de départ. " +
                             "Pour ce tutoriel, nous décollerons de Paris-Orly. " +
                             "Naviguez dans la liste et sélectionnez ORY - Paris-Orly.",
                Objective = "Sélectionner Paris-Orly comme aéroport de départ",
                ValidationCondition = (state) => state.DepartureAirport == "LFPO",
                Hints = new List<string>
                {
                    "Paris-Orly a le code IATA ORY et le code ICAO LFPO",
                    "Vous pouvez taper les premières lettres pour rechercher",
                    "Les aéroports français sont regroupés ensemble"
                }
            });

            // Étape 5: Premier démarrage
            steps.Add(new TutorialStep
            {
                Id = "engine_start",
                Title = "Démarrage des moteurs",
                Description = "Excellent choix ! Vous êtes maintenant dans le cockpit du Cessna 172 à Paris-Orly. " +
                             "Avant de décoller, nous devons démarrer le moteur. " +
                             "Appuyez sur Ctrl+E pour démarrer le moteur.",
                Objective = "Démarrer le moteur",
                ValidationCondition = (state) => state.EngineRunning,
                Hints = new List<string>
                {
                    "Maintenez Ctrl et appuyez sur E",
                    "Vous entendrez le son du moteur qui démarre",
                    "Le démarrage prend quelques secondes"
                }
            });

            // Étape 6: Augmenter les gaz
            steps.Add(new TutorialStep
            {
                Id = "throttle_up",
                Title = "Augmentation des gaz",
                Description = "Parfait ! Le moteur tourne maintenant au ralenti. " +
                             "Pour décoller, nous devons augmenter les gaz. " +
                             "Appuyez plusieurs fois sur + (pavé numérique) pour augmenter les gaz à 100%.",
                Objective = "Augmenter les gaz à 100%",
                ValidationCondition = (state) => state.Throttle >= 0.95,
                Hints = new List<string>
                {
                    "Utilisez la touche + du pavé numérique",
                    "Le niveau des gaz est annoncé vocalement",
                    "100% de gaz est nécessaire pour le décollage"
                }
            });

            // Étape 7: Décollage
            steps.Add(new TutorialStep
            {
                Id = "takeoff",
                Title = "Décollage",
                Description = "Excellent ! Les gaz sont à fond. L'avion accélère sur la piste. " +
                             "Lorsque la vitesse atteint 60 nœuds, tirez doucement sur le manche " +
                             "en appuyant sur Flèche Bas pour décoller.",
                Objective = "Décoller et atteindre 100 pieds d'altitude",
                ValidationCondition = (state) => state.Altitude >= 100 && !state.OnGround,
                Hints = new List<string>
                {
                    "Écoutez l'annonce de la vitesse",
                    "À 60 nœuds, appuyez sur Flèche Bas",
                    "Maintenez une légère pression pour monter progressivement"
                }
            });

            // Étape 8: Vol en palier
            steps.Add(new TutorialStep
            {
                Id = "level_flight",
                Title = "Vol en palier",
                Description = "Bravo ! Vous volez ! Maintenant, stabilisons l'avion à 1000 pieds d'altitude. " +
                             "Utilisez les flèches Haut et Bas pour ajuster votre assiette " +
                             "et maintenir une altitude stable.",
                Objective = "Maintenir une altitude entre 950 et 1050 pieds pendant 10 secondes",
                ValidationCondition = (state) => state.AltitudeStable && state.Altitude >= 950 && state.Altitude <= 1050,
                Hints = new List<string>
                {
                    "Flèche Haut pour piquer (descendre)",
                    "Flèche Bas pour cabrer (monter)",
                    "De petits ajustements suffisent",
                    "Écoutez les annonces d'altitude"
                }
            });

            // Étape 9: Virages
            steps.Add(new TutorialStep
            {
                Id = "turns",
                Title = "Virages",
                Description = "Parfait ! Vous maîtrisez le vol en palier. " +
                             "Maintenant, apprenons à virer. Appuyez sur Flèche Gauche " +
                             "pour incliner l'avion et effectuer un virage à gauche de 90 degrés.",
                Objective = "Effectuer un virage à gauche de 90 degrés",
                ValidationCondition = (state) => state.HeadingChanged >= 90,
                Hints = new List<string>
                {
                    "Flèche Gauche pour incliner à gauche",
                    "Flèche Droite pour revenir en ligne droite",
                    "Le cap est annoncé vocalement",
                    "Maintenez l'altitude pendant le virage"
                }
            });

            // Étape 10: Préparation à l'atterrissage
            steps.Add(new TutorialStep
            {
                Id = "landing_prep",
                Title = "Préparation à l'atterrissage",
                Description = "Excellent virage ! Il est temps de préparer l'atterrissage. " +
                             "Réduisez les gaz à 50% avec la touche - (pavé numérique). " +
                             "Puis sortez le train d'atterrissage avec la touche G.",
                Objective = "Réduire les gaz et sortir le train",
                ValidationCondition = (state) => state.Throttle <= 0.55 && state.GearDown,
                Hints = new List<string>
                {
                    "Appuyez plusieurs fois sur - pour réduire les gaz",
                    "Appuyez sur G pour le train d'atterrissage",
                    "Le train met quelques secondes à sortir"
                }
            });

            // Étape 11: Approche finale
            steps.Add(new TutorialStep
            {
                Id = "final_approach",
                Title = "Approche finale",
                Description = "Bien ! L'avion est configuré pour l'atterrissage. " +
                             "Descendez progressivement vers la piste en maintenant une vitesse de 70 nœuds. " +
                             "Utilisez Flèche Haut pour piquer doucement.",
                Objective = "Descendre en dessous de 100 pieds",
                ValidationCondition = (state) => state.Altitude <= 100 && state.Speed >= 60 && state.Speed <= 80,
                Hints = new List<string>
                {
                    "Descendez progressivement, pas trop vite",
                    "Maintenez une vitesse stable autour de 70 nœuds",
                    "Ajustez les gaz si nécessaire"
                }
            });

            // Étape 12: Atterrissage
            steps.Add(new TutorialStep
            {
                Id = "landing",
                Title = "Atterrissage",
                Description = "Vous approchez de la piste ! Continuez à descendre doucement. " +
                             "Juste avant de toucher le sol, tirez légèrement sur le manche (Flèche Bas) " +
                             "pour arrondir l'atterrissage.",
                Objective = "Atterrir en douceur",
                ValidationCondition = (state) => state.OnGround && state.VerticalSpeed > -500,
                Hints = new List<string>
                {
                    "Gardez les ailes horizontales",
                    "Arrondir juste avant le contact",
                    "Un bon atterrissage est un atterrissage doux"
                }
            });

            // Étape 13: Félicitations
            steps.Add(new TutorialStep
            {
                Id = "congratulations",
                Title = "Félicitations !",
                Description = "Bravo ! Vous avez réussi votre premier vol ! " +
                             "Vous avez appris à démarrer le moteur, décoller, voler en palier, " +
                             "virer et atterrir. Vous êtes maintenant prêt à explorer le simulateur " +
                             "et à effectuer des vols plus complexes. Appuyez sur Entrée pour terminer le tutoriel.",
                Objective = "Terminer le tutoriel",
                ValidationCondition = (state) => state.TutorialCompleteAcknowledged,
                Hints = new List<string>
                {
                    "Vous pouvez refaire le tutoriel à tout moment",
                    "Explorez les différents avions et aéroports",
                    "Essayez les missions pour progresser"
                }
            });
        }

        /// <summary>
        /// Démarre le tutoriel
        /// </summary>
        public void StartTutorial()
        {
            isActive = true;
            currentStepIndex = 0;
            currentStep = steps[currentStepIndex];
            
            accessibilityManager.Speak("Démarrage du tutoriel guidé");
            PresentCurrentStep();
        }

        /// <summary>
        /// Présente l'étape actuelle
        /// </summary>
        private void PresentCurrentStep()
        {
            if (currentStep == null) return;
            
            string announcement = $"Étape {CurrentStepNumber} sur {TotalSteps}: {currentStep.Title}. " +
                                 currentStep.Description;
            
            accessibilityManager.Speak(announcement);
        }

        /// <summary>
        /// Vérifie si l'étape actuelle est complétée
        /// </summary>
        public void CheckStepCompletion(TutorialState state)
        {
            if (!isActive || currentStep == null) return;
            
            if (currentStep.ValidationCondition(state))
            {
                CompleteCurrentStep();
            }
        }

        /// <summary>
        /// Complète l'étape actuelle
        /// </summary>
        private void CompleteCurrentStep()
        {
            accessibilityManager.Speak($"Étape {currentStep.Title} terminée !");
            
            progress.CompletedSteps.Add(currentStep.Id);
            StepCompleted?.Invoke(this, new TutorialStepEventArgs(currentStep));
            
            // Passer à l'étape suivante
            currentStepIndex++;
            
            if (currentStepIndex < steps.Count)
            {
                currentStep = steps[currentStepIndex];
                System.Threading.Thread.Sleep(1000); // Pause courte
                PresentCurrentStep();
            }
            else
            {
                CompleteTutorial();
            }
        }

        /// <summary>
        /// Termine le tutoriel
        /// </summary>
        private void CompleteTutorial()
        {
            isActive = false;
            progress.Completed = true;
            progress.CompletionDate = DateTime.Now;
            
            accessibilityManager.Speak("Tutoriel terminé ! Félicitations, vous êtes maintenant un pilote !");
            TutorialCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Donne un indice pour l'étape actuelle
        /// </summary>
        public void GiveHint()
        {
            if (!isActive || currentStep == null) return;
            
            if (currentStep.Hints.Count > 0)
            {
                var hint = currentStep.Hints[new Random().Next(currentStep.Hints.Count)];
                accessibilityManager.Speak($"Indice: {hint}");
            }
        }

        /// <summary>
        /// Répète l'instruction de l'étape actuelle
        /// </summary>
        public void RepeatInstruction()
        {
            if (!isActive || currentStep == null) return;
            
            PresentCurrentStep();
        }

        /// <summary>
        /// Annule le tutoriel
        /// </summary>
        public void CancelTutorial()
        {
            isActive = false;
            accessibilityManager.Speak("Tutoriel annulé");
        }
    }

    /// <summary>
    /// Représente une étape du tutoriel
    /// </summary>
    public class TutorialStep
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public Func<TutorialState, bool> ValidationCondition { get; set; }
        public List<string> Hints { get; set; }

        public TutorialStep()
        {
            Hints = new List<string>();
        }
    }

    /// <summary>
    /// État du tutoriel pour validation
    /// </summary>
    public class TutorialState
    {
        public bool MenuOpen { get; set; }
        public bool NewFlightSelected { get; set; }
        public string SelectedAircraft { get; set; }
        public string DepartureAirport { get; set; }
        public bool EngineRunning { get; set; }
        public float Throttle { get; set; }
        public float Altitude { get; set; }
        public bool OnGround { get; set; }
        public bool AltitudeStable { get; set; }
        public float HeadingChanged { get; set; }
        public bool GearDown { get; set; }
        public float Speed { get; set; }
        public float VerticalSpeed { get; set; }
        public bool TutorialCompleteAcknowledged { get; set; }
    }

    /// <summary>
    /// Progression du tutoriel
    /// </summary>
    public class TutorialProgress
    {
        public List<string> CompletedSteps { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletionDate { get; set; }

        public TutorialProgress()
        {
            CompletedSteps = new List<string>();
        }
    }

    /// <summary>
    /// Arguments d'événement pour les étapes du tutoriel
    /// </summary>
    public class TutorialStepEventArgs : EventArgs
    {
        public TutorialStep Step { get; }

        public TutorialStepEventArgs(TutorialStep step)
        {
            Step = step;
        }
    }
}

