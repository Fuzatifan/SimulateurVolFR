using System;
using System.Collections.Generic;
using System.Linq;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système d'école de pilotage avec instructeur virtuel exigeant
    /// Formation structurée pour obtenir différentes licences
    /// </summary>
    public class FlightSchoolSystem
    {
        private AccessibilityManager accessibilityManager;
        private VirtualCopilotSystem copilot;
        private StudentProfile studentProfile;
        private List<Lesson> curriculum;
        private Lesson currentLesson;
        private InstructorPersonality instructorPersonality;
        private LessonEvaluation currentEvaluation;

        public event EventHandler<LessonEventArgs> LessonStarted;
        public event EventHandler<LessonCompletedEventArgs> LessonCompleted;
        public event EventHandler<LicenseEarnedEventArgs> LicenseEarned;

        public StudentProfile StudentProfile => studentProfile;
        public Lesson CurrentLesson => currentLesson;

        public FlightSchoolSystem(AccessibilityManager accessibilityManager, VirtualCopilotSystem copilot)
        {
            this.accessibilityManager = accessibilityManager;
            this.copilot = copilot;
            this.instructorPersonality = InstructorPersonality.Strict;
            this.curriculum = new List<Lesson>();
            
            InitializeCurriculum();
        }

        /// <summary>
        /// Crée un nouveau profil d'étudiant
        /// </summary>
        public void CreateStudentProfile(string name)
        {
            studentProfile = new StudentProfile
            {
                Name = name,
                EnrollmentDate = DateTime.Now,
                TotalFlightHours = 0,
                LessonsCompleted = new List<Lesson>(),
                LicensesEarned = new List<License>(),
                OverallGrade = 0,
                CurrentLicenseGoal = LicenseType.PPL
            };

            accessibilityManager.Speak($"Bienvenue à l'école de pilotage, {name}. " +
                "Je suis votre instructeur. Nous allons travailler ensemble pour obtenir votre licence de pilote privé.");

            // Configurer le copilote en mode instructeur
            copilot.ChangePersonality(CopilotPersonality.Instructor);
        }

        /// <summary>
        /// Initialise le programme de formation
        /// </summary>
        private void InitializeCurriculum()
        {
            // ===== LICENCE PPL (Pilote Privé) =====
            
            // Module 1 : Bases du pilotage
            curriculum.Add(new Lesson
            {
                Id = 1,
                Title = "Introduction et familiarisation",
                Description = "Découverte du cockpit et des instruments de base",
                LicenseType = LicenseType.PPL,
                Module = 1,
                Duration = 1.0f,
                MinimumGrade = 60,
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Identifier tous les instruments", Weight = 20 },
                    new LessonObjective { Description = "Comprendre les contrôles de vol", Weight = 30 },
                    new LessonObjective { Description = "Effectuer les vérifications pré-vol", Weight = 30 },
                    new LessonObjective { Description = "Démarrer le moteur correctement", Weight = 20 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 2,
                Title = "Roulage au sol",
                Description = "Maîtriser le roulage et les virages au sol",
                LicenseType = LicenseType.PPL,
                Module = 1,
                Duration = 1.0f,
                MinimumGrade = 70,
                Prerequisites = new List<int> { 1 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Rouler en ligne droite", Weight = 25 },
                    new LessonObjective { Description = "Effectuer des virages au sol", Weight = 25 },
                    new LessonObjective { Description = "Respecter les voies de circulation", Weight = 25 },
                    new LessonObjective { Description = "S'arrêter précisément", Weight = 25 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 3,
                Title = "Premier décollage",
                Description = "Apprendre la procédure de décollage",
                LicenseType = LicenseType.PPL,
                Module = 1,
                Duration = 1.5f,
                MinimumGrade = 70,
                Prerequisites = new List<int> { 2 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Aligner correctement sur la piste", Weight = 15 },
                    new LessonObjective { Description = "Appliquer la puissance progressivement", Weight = 20 },
                    new LessonObjective { Description = "Maintenir l'axe de piste", Weight = 25 },
                    new LessonObjective { Description = "Décoller à la bonne vitesse", Weight = 20 },
                    new LessonObjective { Description = "Adopter l'assiette de montée", Weight = 20 }
                }
            });

            // Module 2 : Vol en palier
            curriculum.Add(new Lesson
            {
                Id = 4,
                Title = "Vol en palier",
                Description = "Maintenir l'altitude et le cap",
                LicenseType = LicenseType.PPL,
                Module = 2,
                Duration = 1.5f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 3 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Maintenir l'altitude (±100 ft)", Weight = 30 },
                    new LessonObjective { Description = "Maintenir le cap (±5°)", Weight = 30 },
                    new LessonObjective { Description = "Maintenir la vitesse (±5 kts)", Weight = 30 },
                    new LessonObjective { Description = "Utiliser le trim correctement", Weight = 10 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 5,
                Title = "Virages de base",
                Description = "Effectuer des virages à taux standard",
                LicenseType = LicenseType.PPL,
                Module = 2,
                Duration = 1.5f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 4 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Virage à 15° d'inclinaison", Weight = 25 },
                    new LessonObjective { Description = "Virage à 30° d'inclinaison", Weight = 25 },
                    new LessonObjective { Description = "Maintenir l'altitude en virage", Weight = 30 },
                    new LessonObjective { Description = "Sortir du virage au cap voulu", Weight = 20 }
                }
            });

            // Module 3 : Montées et descentes
            curriculum.Add(new Lesson
            {
                Id = 6,
                Title = "Montées",
                Description = "Techniques de montée",
                LicenseType = LicenseType.PPL,
                Module = 3,
                Duration = 1.5f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 5 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Établir une montée normale", Weight = 30 },
                    new LessonObjective { Description = "Maintenir la vitesse de montée", Weight = 30 },
                    new LessonObjective { Description = "Niveler à l'altitude assignée", Weight = 30 },
                    new LessonObjective { Description = "Gérer la puissance", Weight = 10 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 7,
                Title = "Descentes",
                Description = "Techniques de descente",
                LicenseType = LicenseType.PPL,
                Module = 3,
                Duration = 1.5f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 6 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Établir une descente contrôlée", Weight = 30 },
                    new LessonObjective { Description = "Maintenir la vitesse de descente", Weight = 30 },
                    new LessonObjective { Description = "Niveler à l'altitude assignée", Weight = 30 },
                    new LessonObjective { Description = "Gérer la puissance", Weight = 10 }
                }
            });

            // Module 4 : Atterrissages
            curriculum.Add(new Lesson
            {
                Id = 8,
                Title = "Approche et atterrissage",
                Description = "Première approche et atterrissage",
                LicenseType = LicenseType.PPL,
                Module = 4,
                Duration = 2.0f,
                MinimumGrade = 70,
                Prerequisites = new List<int> { 7 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Établir l'approche finale", Weight = 20 },
                    new LessonObjective { Description = "Maintenir l'axe de piste", Weight = 25 },
                    new LessonObjective { Description = "Contrôler la vitesse d'approche", Weight = 20 },
                    new LessonObjective { Description = "Effectuer l'arrondi", Weight = 20 },
                    new LessonObjective { Description = "Atterrir en douceur (< 300 ft/min)", Weight = 15 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 9,
                Title = "Tour de piste",
                Description = "Circuit complet autour de l'aéroport",
                LicenseType = LicenseType.PPL,
                Module = 4,
                Duration = 1.5f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 8 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Respecter le circuit", Weight = 25 },
                    new LessonObjective { Description = "Gérer l'altitude et la vitesse", Weight = 25 },
                    new LessonObjective { Description = "Communications radio correctes", Weight = 25 },
                    new LessonObjective { Description = "Atterrissage réussi", Weight = 25 }
                }
            });

            // Module 5 : Navigation
            curriculum.Add(new Lesson
            {
                Id = 10,
                Title = "Navigation de base",
                Description = "Vol vers un point de navigation",
                LicenseType = LicenseType.PPL,
                Module = 5,
                Duration = 2.0f,
                MinimumGrade = 75,
                Prerequisites = new List<int> { 9 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Planifier la route", Weight = 20 },
                    new LessonObjective { Description = "Maintenir le cap", Weight = 30 },
                    new LessonObjective { Description = "Gérer le carburant", Weight = 20 },
                    new LessonObjective { Description = "Arriver au point prévu", Weight = 30 }
                }
            });

            // Module 6 : Situations spéciales
            curriculum.Add(new Lesson
            {
                Id = 11,
                Title = "Décrochages",
                Description = "Reconnaissance et sortie de décrochage",
                LicenseType = LicenseType.PPL,
                Module = 6,
                Duration = 1.5f,
                MinimumGrade = 80,
                Prerequisites = new List<int> { 10 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Reconnaître les signes de décrochage", Weight = 30 },
                    new LessonObjective { Description = "Sortir du décrochage correctement", Weight = 40 },
                    new LessonObjective { Description = "Minimiser la perte d'altitude", Weight = 30 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 12,
                Title = "Pannes moteur",
                Description = "Gestion d'une panne moteur",
                LicenseType = LicenseType.PPL,
                Module = 6,
                Duration = 1.5f,
                MinimumGrade = 80,
                Prerequisites = new List<int> { 11 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Adopter la vitesse de plané", Weight = 30 },
                    new LessonObjective { Description = "Choisir un terrain d'atterrissage", Weight = 30 },
                    new LessonObjective { Description = "Effectuer les vérifications", Weight = 20 },
                    new LessonObjective { Description = "Atterrir en sécurité", Weight = 20 }
                }
            });

            // Module 7 : Examen final PPL
            curriculum.Add(new Lesson
            {
                Id = 13,
                Title = "Examen pratique PPL",
                Description = "Test final pour la licence PPL",
                LicenseType = LicenseType.PPL,
                Module = 7,
                Duration = 2.0f,
                MinimumGrade = 85,
                Prerequisites = new List<int> { 12 },
                IsExam = true,
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Vol complet sans erreur majeure", Weight = 100 }
                }
            });

            // ===== LICENCE CPL (Pilote Commercial) =====
            // (Nécessite PPL)

            curriculum.Add(new Lesson
            {
                Id = 14,
                Title = "Voltige de base",
                Description = "Manœuvres acrobatiques",
                LicenseType = LicenseType.CPL,
                Module = 1,
                Duration = 2.0f,
                MinimumGrade = 80,
                Prerequisites = new List<int> { 13 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Tonneau", Weight = 33 },
                    new LessonObjective { Description = "Looping", Weight = 33 },
                    new LessonObjective { Description = "Virage engagé", Weight = 34 }
                }
            });

            curriculum.Add(new Lesson
            {
                Id = 15,
                Title = "Vol aux instruments de base",
                Description = "Introduction au vol IFR",
                LicenseType = LicenseType.CPL,
                Module = 2,
                Duration = 2.0f,
                MinimumGrade = 85,
                Prerequisites = new List<int> { 14 },
                Objectives = new List<LessonObjective>
                {
                    new LessonObjective { Description = "Vol en palier aux instruments", Weight = 40 },
                    new LessonObjective { Description = "Virages aux instruments", Weight = 30 },
                    new LessonObjective { Description = "Interprétation des instruments", Weight = 30 }
                }
            });

            // Plus de leçons CPL et ATPL peuvent être ajoutées...
        }

        /// <summary>
        /// Commence une leçon
        /// </summary>
        public bool StartLesson(int lessonId)
        {
            if (studentProfile == null)
            {
                accessibilityManager.Speak("Vous devez d'abord créer un profil d'étudiant.");
                return false;
            }

            currentLesson = curriculum.FirstOrDefault(l => l.Id == lessonId);

            if (currentLesson == null)
            {
                accessibilityManager.Speak("Leçon introuvable.");
                return false;
            }

            // Vérifier les prérequis
            if (!CheckPrerequisites(currentLesson))
            {
                accessibilityManager.Speak("Vous n'avez pas complété les leçons prérequises.");
                return false;
            }

            // Initialiser l'évaluation
            currentEvaluation = new LessonEvaluation
            {
                Lesson = currentLesson,
                StartTime = DateTime.Now,
                ObjectiveScores = new Dictionary<LessonObjective, float>(),
                Errors = new List<string>(),
                Comments = new List<string>()
            };

            // Briefing de l'instructeur
            string briefing = GenerateBriefing(currentLesson);
            accessibilityManager.Speak(briefing);

            LessonStarted?.Invoke(this, new LessonEventArgs(currentLesson));

            return true;
        }

        /// <summary>
        /// Vérifie les prérequis d'une leçon
        /// </summary>
        private bool CheckPrerequisites(Lesson lesson)
        {
            if (lesson.Prerequisites == null || lesson.Prerequisites.Count == 0)
                return true;

            foreach (int prereqId in lesson.Prerequisites)
            {
                if (!studentProfile.LessonsCompleted.Any(l => l.Id == prereqId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Génère le briefing de la leçon
        /// </summary>
        private string GenerateBriefing(Lesson lesson)
        {
            string briefing = $"Leçon {lesson.Id} : {lesson.Title}. ";
            briefing += $"{lesson.Description}. ";
            briefing += $"Durée estimée : {lesson.Duration:F1} heures. ";
            
            if (lesson.IsExam)
            {
                briefing += "Ceci est un examen. Vous devez démontrer toutes vos compétences. ";
                briefing += $"Note minimale requise : {lesson.MinimumGrade} sur 100. ";
            }
            else
            {
                briefing += "Objectifs de la leçon : ";
                foreach (var obj in lesson.Objectives)
                {
                    briefing += $"{obj.Description}. ";
                }
            }

            briefing += "Êtes-vous prêt ? Allons-y.";

            return briefing;
        }

        /// <summary>
        /// Enregistre une erreur pendant la leçon
        /// </summary>
        public void RecordError(string error, ErrorSeverity severity)
        {
            if (currentEvaluation == null)
                return;

            currentEvaluation.Errors.Add(error);

            // Feedback immédiat selon la sévérité
            string feedback = severity switch
            {
                ErrorSeverity.Minor => $"Attention : {error}",
                ErrorSeverity.Moderate => $"Erreur : {error}. Corrigez immédiatement.",
                ErrorSeverity.Major => $"Erreur grave : {error} ! C'est inacceptable.",
                ErrorSeverity.Critical => $"Erreur critique : {error} ! En situation réelle, ce serait un accident.",
                _ => error
            };

            accessibilityManager.Speak(feedback);

            // Pénalité selon la sévérité
            float penalty = severity switch
            {
                ErrorSeverity.Minor => 2f,
                ErrorSeverity.Moderate => 5f,
                ErrorSeverity.Major => 10f,
                ErrorSeverity.Critical => 20f,
                _ => 5f
            };

            currentEvaluation.TotalPenalty += penalty;
        }

        /// <summary>
        /// Enregistre un commentaire positif
        /// </summary>
        public void RecordPositiveComment(string comment)
        {
            if (currentEvaluation == null)
                return;

            currentEvaluation.Comments.Add(comment);
            currentEvaluation.PositivePoints += 2f;
        }

        /// <summary>
        /// Évalue un objectif
        /// </summary>
        public void EvaluateObjective(LessonObjective objective, float score)
        {
            if (currentEvaluation == null)
                return;

            currentEvaluation.ObjectiveScores[objective] = score;
        }

        /// <summary>
        /// Termine la leçon et calcule la note
        /// </summary>
        public LessonResult CompleteLesson(FlightPerformance performance)
        {
            if (currentLesson == null || currentEvaluation == null)
            {
                accessibilityManager.Speak("Aucune leçon en cours.");
                return null;
            }

            currentEvaluation.EndTime = DateTime.Now;
            currentEvaluation.Performance = performance;

            // Calcul de la note
            float finalGrade = CalculateFinalGrade(currentEvaluation);

            bool passed = finalGrade >= currentLesson.MinimumGrade;

            var result = new LessonResult
            {
                Lesson = currentLesson,
                Evaluation = currentEvaluation,
                FinalGrade = finalGrade,
                Passed = passed,
                CompletionDate = DateTime.Now
            };

            // Débriefing de l'instructeur
            string debriefing = GenerateDebriefing(result);
            accessibilityManager.Speak(debriefing);

            // Mise à jour du profil étudiant
            if (passed)
            {
                studentProfile.LessonsCompleted.Add(currentLesson);
                studentProfile.TotalFlightHours += currentLesson.Duration;
                UpdateOverallGrade(finalGrade);

                // Vérifier si une licence est obtenue
                CheckLicenseEarned();
            }

            LessonCompleted?.Invoke(this, new LessonCompletedEventArgs(result));

            currentLesson = null;
            currentEvaluation = null;

            return result;
        }

        /// <summary>
        /// Calcule la note finale
        /// </summary>
        private float CalculateFinalGrade(LessonEvaluation evaluation)
        {
            float grade = 100f;

            // Évaluation des objectifs
            float objectivesScore = 0f;
            float totalWeight = 0f;

            foreach (var objective in evaluation.Lesson.Objectives)
            {
                if (evaluation.ObjectiveScores.ContainsKey(objective))
                {
                    objectivesScore += evaluation.ObjectiveScores[objective] * objective.Weight;
                }
                totalWeight += objective.Weight;
            }

            if (totalWeight > 0)
            {
                grade = objectivesScore / totalWeight;
            }

            // Pénalités
            grade -= evaluation.TotalPenalty;

            // Points positifs
            grade += evaluation.PositivePoints;

            // Bonus de performance
            if (evaluation.Performance != null)
            {
                if (evaluation.Performance.LandingRate < 200)
                    grade += 5f;
                if (Math.Abs(evaluation.Performance.DelayMinutes) < 2)
                    grade += 3f;
                if (evaluation.Performance.FuelEfficiency > 0.95f)
                    grade += 2f;
            }

            return Math.Clamp(grade, 0, 100);
        }

        /// <summary>
        /// Génère le débriefing
        /// </summary>
        private string GenerateDebriefing(LessonResult result)
        {
            string debriefing = $"Débriefing de la leçon {result.Lesson.Id}. ";
            debriefing += $"Note finale : {result.FinalGrade:F1} sur 100. ";

            if (result.Passed)
            {
                if (result.FinalGrade >= 95)
                {
                    debriefing += "Excellent travail ! Performance exceptionnelle. ";
                }
                else if (result.FinalGrade >= 85)
                {
                    debriefing += "Très bien. Vous maîtrisez les compétences. ";
                }
                else
                {
                    debriefing += "Leçon réussie, mais il y a encore des points à améliorer. ";
                }
            }
            else
            {
                debriefing += $"Leçon échouée. Note minimale requise : {result.Lesson.MinimumGrade}. ";
                debriefing += "Vous devez refaire cette leçon. ";
            }

            // Points forts et faibles
            if (result.Evaluation.Comments.Count > 0)
            {
                debriefing += "Points positifs : ";
                debriefing += string.Join(". ", result.Evaluation.Comments.Take(3)) + ". ";
            }

            if (result.Evaluation.Errors.Count > 0)
            {
                debriefing += "Erreurs à corriger : ";
                debriefing += string.Join(". ", result.Evaluation.Errors.Take(3)) + ". ";
            }

            return debriefing;
        }

        /// <summary>
        /// Met à jour la note globale
        /// </summary>
        private void UpdateOverallGrade(float lessonGrade)
        {
            int completedCount = studentProfile.LessonsCompleted.Count;
            studentProfile.OverallGrade = 
                (studentProfile.OverallGrade * (completedCount - 1) + lessonGrade) / completedCount;
        }

        /// <summary>
        /// Vérifie si une licence est obtenue
        /// </summary>
        private void CheckLicenseEarned()
        {
            // Vérifier PPL
            if (!studentProfile.LicensesEarned.Any(l => l.Type == LicenseType.PPL))
            {
                var pplLessons = curriculum.Where(l => l.LicenseType == LicenseType.PPL).ToList();
                bool allCompleted = pplLessons.All(l => studentProfile.LessonsCompleted.Contains(l));

                if (allCompleted)
                {
                    var license = new License
                    {
                        Type = LicenseType.PPL,
                        Name = "Licence de Pilote Privé (PPL)",
                        IssueDate = DateTime.Now,
                        Number = GenerateLicenseNumber()
                    };

                    studentProfile.LicensesEarned.Add(license);

                    accessibilityManager.Speak($"Félicitations ! Vous avez obtenu votre {license.Name} ! " +
                        $"Numéro de licence : {license.Number}. " +
                        "Vous êtes maintenant pilote privé qualifié.");

                    LicenseEarned?.Invoke(this, new LicenseEarnedEventArgs(license));
                }
            }

            // Vérifier CPL (similaire)
            // Vérifier ATPL (similaire)
        }

        /// <summary>
        /// Génère un numéro de licence
        /// </summary>
        private string GenerateLicenseNumber()
        {
            Random rnd = new Random();
            return $"FR-{DateTime.Now.Year}-{rnd.Next(10000, 99999)}";
        }

        /// <summary>
        /// Obtient les leçons disponibles
        /// </summary>
        public List<Lesson> GetAvailableLessons()
        {
            if (studentProfile == null)
                return new List<Lesson>();

            return curriculum.Where(l => CheckPrerequisites(l) && 
                                        !studentProfile.LessonsCompleted.Contains(l)).ToList();
        }

        /// <summary>
        /// Obtient les statistiques de l'étudiant
        /// </summary>
        public StudentStatistics GetStatistics()
        {
            if (studentProfile == null)
                return null;

            return new StudentStatistics
            {
                Name = studentProfile.Name,
                TotalFlightHours = studentProfile.TotalFlightHours,
                LessonsCompleted = studentProfile.LessonsCompleted.Count,
                LicensesEarned = studentProfile.LicensesEarned.Count,
                OverallGrade = studentProfile.OverallGrade,
                DaysEnrolled = (DateTime.Now - studentProfile.EnrollmentDate).Days,
                NextLicenseGoal = studentProfile.CurrentLicenseGoal
            };
        }
    }

    #region Modèles de données

    public class StudentProfile
    {
        public string Name { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public float TotalFlightHours { get; set; }
        public List<Lesson> LessonsCompleted { get; set; }
        public List<License> LicensesEarned { get; set; }
        public float OverallGrade { get; set; }
        public LicenseType CurrentLicenseGoal { get; set; }
    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public LicenseType LicenseType { get; set; }
        public int Module { get; set; }
        public float Duration { get; set; }
        public int MinimumGrade { get; set; }
        public List<int> Prerequisites { get; set; }
        public bool IsExam { get; set; }
        public List<LessonObjective> Objectives { get; set; }
    }

    public class LessonObjective
    {
        public string Description { get; set; }
        public float Weight { get; set; }
    }

    public enum LicenseType
    {
        PPL,    // Private Pilot License
        CPL,    // Commercial Pilot License
        ATPL    // Airline Transport Pilot License
    }

    public class License
    {
        public LicenseType Type { get; set; }
        public string Name { get; set; }
        public DateTime IssueDate { get; set; }
        public string Number { get; set; }
    }

    public enum InstructorPersonality
    {
        Strict,
        Encouraging,
        Balanced
    }

    public class LessonEvaluation
    {
        public Lesson Lesson { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Dictionary<LessonObjective, float> ObjectiveScores { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Comments { get; set; }
        public float TotalPenalty { get; set; }
        public float PositivePoints { get; set; }
        public FlightPerformance Performance { get; set; }
    }

    public enum ErrorSeverity
    {
        Minor,
        Moderate,
        Major,
        Critical
    }

    public class LessonResult
    {
        public Lesson Lesson { get; set; }
        public LessonEvaluation Evaluation { get; set; }
        public float FinalGrade { get; set; }
        public bool Passed { get; set; }
        public DateTime CompletionDate { get; set; }
    }

    public class StudentStatistics
    {
        public string Name { get; set; }
        public float TotalFlightHours { get; set; }
        public int LessonsCompleted { get; set; }
        public int LicensesEarned { get; set; }
        public float OverallGrade { get; set; }
        public int DaysEnrolled { get; set; }
        public LicenseType NextLicenseGoal { get; set; }
    }

    public class LessonEventArgs : EventArgs
    {
        public Lesson Lesson { get; }

        public LessonEventArgs(Lesson lesson)
        {
            Lesson = lesson;
        }
    }

    public class LessonCompletedEventArgs : EventArgs
    {
        public LessonResult Result { get; }

        public LessonCompletedEventArgs(LessonResult result)
        {
            Result = result;
        }
    }

    public class LicenseEarnedEventArgs : EventArgs
    {
        public License License { get; }

        public LicenseEarnedEventArgs(License license)
        {
            License = license;
        }
    }

    #endregion
}

