using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de copilote virtuel avec IA conversationnelle
    /// Utilise OpenAI GPT pour des conversations naturelles et une assistance contextuelle
    /// </summary>
    public class VirtualCopilotSystem
    {
        private AccessibilityManager accessibilityManager;
        private HttpClient httpClient;
        private List<ConversationMessage> conversationHistory;
        private FlightContext currentContext;
        private CopilotPersonality personality;
        private bool isActive;
        private DateTime lastInteraction;

        private const string OPENAI_API_KEY = "YOUR_API_KEY_HERE"; // À configurer
        private const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";

        public event EventHandler<CopilotMessageEventArgs> CopilotSpoke;
        public event EventHandler<AlertEventArgs> CopilotAlert;

        public bool IsActive => isActive;
        public CopilotPersonality Personality => personality;

        public VirtualCopilotSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.httpClient = new HttpClient();
            this.conversationHistory = new List<ConversationMessage>();
            this.personality = CopilotPersonality.Professional;
            this.isActive = true;
            
            ConfigureHttpClient();
            InitializeSystemPrompt();
        }

        /// <summary>
        /// Configure le client HTTP
        /// </summary>
        private void ConfigureHttpClient()
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {OPENAI_API_KEY}");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Initialise le prompt système pour le copilote
        /// </summary>
        private void InitializeSystemPrompt()
        {
            string systemPrompt = GetSystemPromptForPersonality(personality);
            
            conversationHistory.Add(new ConversationMessage
            {
                Role = "system",
                Content = systemPrompt
            });
        }

        /// <summary>
        /// Obtient le prompt système selon la personnalité
        /// </summary>
        private string GetSystemPromptForPersonality(CopilotPersonality personality)
        {
            switch (personality)
            {
                case CopilotPersonality.Professional:
                    return @"Tu es un copilote professionnel et expérimenté dans un simulateur de vol.
Tu assistes le pilote avec compétence et professionnalisme.
Tu utilises la terminologie aéronautique française standard.
Tu es attentif, précis et toujours prêt à aider.
Tu surveilles les paramètres de vol et alertes le pilote si nécessaire.
Tu réponds de manière concise et claire.
Tu ne fais jamais de blagues pendant les phases critiques (décollage, atterrissage).
Tu es calme même en situation d'urgence.";

                case CopilotPersonality.Friendly:
                    return @"Tu es un copilote sympathique et encourageant dans un simulateur de vol.
Tu aides le pilote avec bienveillance et patience.
Tu utilises un langage accessible tout en restant professionnel.
Tu encourages et félicites le pilote pour ses bonnes actions.
Tu expliques les choses simplement quand nécessaire.
Tu peux faire des remarques légères en croisière, mais restes sérieux en phases critiques.
Tu es patient avec les débutants.";

                case CopilotPersonality.Instructor:
                    return @"Tu es un instructeur de vol exigeant mais juste dans un simulateur de vol.
Tu enseignes et corriges le pilote avec rigueur.
Tu utilises la terminologie aéronautique précise.
Tu expliques le 'pourquoi' derrière chaque procédure.
Tu anticipes les erreurs et guides le pilote.
Tu donnes des conseils techniques détaillés.
Tu notes mentalement les erreurs pour un débriefing.
Tu es strict sur les procédures de sécurité.";

                case CopilotPersonality.Humorous:
                    return @"Tu es un copilote avec un bon sens de l'humour dans un simulateur de vol.
Tu détends l'atmosphère avec des remarques amusantes.
Tu utilises des blagues et jeux de mots aéronautiques.
Mais tu redeviens sérieux pendant les phases critiques.
Tu commentes les situations avec légèreté quand approprié.
Tu encourages avec humour.
Tu sais quand être drôle et quand être professionnel.";

                default:
                    return GetSystemPromptForPersonality(CopilotPersonality.Professional);
            }
        }

        /// <summary>
        /// Change la personnalité du copilote
        /// </summary>
        public void ChangePersonality(CopilotPersonality newPersonality)
        {
            personality = newPersonality;
            conversationHistory.Clear();
            InitializeSystemPrompt();
            
            accessibilityManager.Speak($"Personnalité du copilote changée : {GetPersonalityName(newPersonality)}");
        }

        /// <summary>
        /// Obtient le nom de la personnalité
        /// </summary>
        private string GetPersonalityName(CopilotPersonality personality)
        {
            return personality switch
            {
                CopilotPersonality.Professional => "Professionnel",
                CopilotPersonality.Friendly => "Sympathique",
                CopilotPersonality.Instructor => "Instructeur",
                CopilotPersonality.Humorous => "Humoristique",
                _ => "Professionnel"
            };
        }

        /// <summary>
        /// Met à jour le contexte de vol
        /// </summary>
        public void UpdateFlightContext(FlightContext context)
        {
            currentContext = context;
            
            // Vérifications proactives
            CheckForAlerts(context);
            
            // Commentaires automatiques selon les événements
            CheckForAutoComments(context);
        }

        /// <summary>
        /// Vérifie les alertes et avertit le pilote
        /// </summary>
        private void CheckForAlerts(FlightContext context)
        {
            // Altitude basse
            if (context.Altitude < 500 && !context.OnGround && context.Phase != FlightPhase.Landing)
            {
                SendAlert("Attention, altitude basse : " + (int)context.Altitude + " pieds", AlertLevel.Warning);
            }

            // Vitesse de décrochage
            if (context.Speed < context.StallSpeed + 10 && !context.OnGround)
            {
                SendAlert("Attention à la vitesse, on approche du décrochage", AlertLevel.Warning);
            }

            // Vitesse excessive
            if (context.Speed > context.MaxSpeed * 0.9)
            {
                SendAlert("Vitesse élevée, attention à ne pas dépasser la VNE", AlertLevel.Caution);
            }

            // Carburant bas
            if (context.FuelRemaining < 0.2 && context.FuelRemaining > 0.15)
            {
                SendAlert("Carburant à 20%, on devrait penser à atterrir bientôt", AlertLevel.Caution);
            }
            else if (context.FuelRemaining < 0.15)
            {
                SendAlert("Carburant critique à " + (int)(context.FuelRemaining * 100) + "%, il faut atterrir maintenant", AlertLevel.Warning);
            }

            // Train d'atterrissage
            if (context.Altitude < 1000 && !context.GearDown && context.Phase == FlightPhase.Approach)
            {
                SendAlert("Train d'atterrissage pas sorti, on est en approche", AlertLevel.Warning);
            }

            // Volets
            if (context.Speed < 80 && context.Flaps == 0 && context.Phase == FlightPhase.Approach)
            {
                SendAlert("On pourrait sortir les volets pour l'approche", AlertLevel.Info);
            }
        }

        /// <summary>
        /// Vérifie et fait des commentaires automatiques
        /// </summary>
        private void CheckForAutoComments(FlightContext context)
        {
            // Commentaires uniquement si pas d'interaction récente
            if ((DateTime.Now - lastInteraction).TotalSeconds < 30)
                return;

            // Décollage réussi
            if (context.Phase == FlightPhase.Departure && context.Altitude > 500 && !context.DepartureCommentMade)
            {
                SpeakAsync("Beau décollage ! On monte bien.");
                context.DepartureCommentMade = true;
            }

            // Altitude de croisière atteinte
            if (context.Phase == FlightPhase.Cruise && !context.CruiseCommentMade && 
                Math.Abs(context.Altitude - context.CruiseAltitude) < 100)
            {
                SpeakAsync($"Altitude de croisière atteinte : {(int)context.Altitude} pieds. Tout est stable.");
                context.CruiseCommentMade = true;
            }

            // Début de descente
            if (context.Phase == FlightPhase.Descent && !context.DescentCommentMade)
            {
                SpeakAsync("On commence la descente. Je surveille l'altitude.");
                context.DescentCommentMade = true;
            }

            // Atterrissage réussi
            if (context.OnGround && context.Phase == FlightPhase.Landing && !context.LandingCommentMade)
            {
                if (context.TouchdownRate < 200)
                {
                    SpeakAsync("Excellent atterrissage ! Très doux.");
                }
                else if (context.TouchdownRate < 400)
                {
                    SpeakAsync("Bon atterrissage. On est au sol en sécurité.");
                }
                else
                {
                    SpeakAsync("On est au sol. L'atterrissage était un peu ferme.");
                }
                context.LandingCommentMade = true;
            }
        }

        /// <summary>
        /// Envoie une alerte au pilote
        /// </summary>
        private void SendAlert(string message, AlertLevel level)
        {
            // Ne pas répéter la même alerte trop souvent
            if ((DateTime.Now - lastInteraction).TotalSeconds < 10)
                return;

            string prefix = level switch
            {
                AlertLevel.Warning => "Attention ! ",
                AlertLevel.Caution => "Prudence, ",
                AlertLevel.Info => "",
                _ => ""
            };

            SpeakAsync(prefix + message);
            CopilotAlert?.Invoke(this, new AlertEventArgs(message, level));
            lastInteraction = DateTime.Now;
        }

        /// <summary>
        /// Le pilote parle au copilote
        /// </summary>
        public async Task<string> TalkToCopilot(string message)
        {
            if (!isActive)
            {
                return "Le copilote est désactivé.";
            }

            // Ajouter le message du pilote à l'historique
            conversationHistory.Add(new ConversationMessage
            {
                Role = "user",
                Content = EnrichMessageWithContext(message)
            });

            // Obtenir la réponse de l'IA
            string response = await GetAIResponse();

            // Ajouter la réponse à l'historique
            conversationHistory.Add(new ConversationMessage
            {
                Role = "assistant",
                Content = response
            });

            // Limiter l'historique à 20 messages (10 échanges)
            if (conversationHistory.Count > 21) // +1 pour le system prompt
            {
                conversationHistory.RemoveRange(1, 2); // Garder le system prompt
            }

            lastInteraction = DateTime.Now;

            // Vocaliser la réponse
            SpeakAsync(response);

            CopilotSpoke?.Invoke(this, new CopilotMessageEventArgs(response));

            return response;
        }

        /// <summary>
        /// Enrichit le message avec le contexte de vol
        /// </summary>
        private string EnrichMessageWithContext(string message)
        {
            if (currentContext == null)
                return message;

            string contextInfo = $"\n[Contexte: Phase={currentContext.Phase}, " +
                                $"Alt={currentContext.Altitude:F0}ft, " +
                                $"Speed={currentContext.Speed:F0}kts, " +
                                $"Fuel={currentContext.FuelRemaining * 100:F0}%]";

            return message + contextInfo;
        }

        /// <summary>
        /// Obtient la réponse de l'IA
        /// </summary>
        private async Task<string> GetAIResponse()
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-4",
                    messages = conversationHistory.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                    max_tokens = 150,
                    temperature = 0.7
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(OPENAI_API_URL, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var aiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(jsonResponse);

                if (aiResponse?.Choices != null && aiResponse.Choices.Length > 0)
                {
                    return aiResponse.Choices[0].Message.Content.Trim();
                }

                return "Désolé, je n'ai pas compris.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur IA: {ex.Message}");
                return GetFallbackResponse();
            }
        }

        /// <summary>
        /// Réponse de secours si l'IA n'est pas disponible
        /// </summary>
        private string GetFallbackResponse()
        {
            string[] responses = 
            {
                "Compris.",
                "D'accord.",
                "Bien reçu.",
                "Je surveille.",
                "Tout va bien.",
                "On continue."
            };

            return responses[new Random().Next(responses.Length)];
        }

        /// <summary>
        /// Parle de manière asynchrone
        /// </summary>
        private void SpeakAsync(string message)
        {
            Task.Run(() => accessibilityManager.Speak(message));
        }

        /// <summary>
        /// Demande une vérification de check-list
        /// </summary>
        public async Task<string> RequestChecklistReview(string checklistName)
        {
            string message = $"Peux-tu me lire la check-list {checklistName} ?";
            return await TalkToCopilot(message);
        }

        /// <summary>
        /// Demande un conseil
        /// </summary>
        public async Task<string> AskForAdvice(string situation)
        {
            string message = $"Que me conseilles-tu pour {situation} ?";
            return await TalkToCopilot(message);
        }

        /// <summary>
        /// Demande une explication
        /// </summary>
        public async Task<string> AskForExplanation(string topic)
        {
            string message = $"Peux-tu m'expliquer {topic} ?";
            return await TalkToCopilot(message);
        }

        /// <summary>
        /// Demande l'état actuel
        /// </summary>
        public async Task<string> RequestStatusReport()
        {
            string message = "Donne-moi un rapport de situation.";
            return await TalkToCopilot(message);
        }

        /// <summary>
        /// Active ou désactive le copilote
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
            string status = active ? "activé" : "désactivé";
            accessibilityManager.Speak($"Copilote {status}");
        }

        /// <summary>
        /// Réinitialise la conversation
        /// </summary>
        public void ResetConversation()
        {
            conversationHistory.Clear();
            InitializeSystemPrompt();
            accessibilityManager.Speak("Conversation avec le copilote réinitialisée");
        }
    }

    #region Modèles de données

    /// <summary>
    /// Personnalité du copilote
    /// </summary>
    public enum CopilotPersonality
    {
        Professional,   // Professionnel et précis
        Friendly,       // Sympathique et encourageant
        Instructor,     // Instructeur exigeant
        Humorous        // Avec humour
    }

    /// <summary>
    /// Message de conversation
    /// </summary>
    public class ConversationMessage
    {
        public string Role { get; set; }      // system, user, assistant
        public string Content { get; set; }
    }

    /// <summary>
    /// Contexte de vol pour le copilote
    /// </summary>
    public class FlightContext
    {
        public FlightPhase Phase { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double StallSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double FuelRemaining { get; set; }
        public bool OnGround { get; set; }
        public bool GearDown { get; set; }
        public int Flaps { get; set; }
        public double CruiseAltitude { get; set; }
        public double TouchdownRate { get; set; }
        
        // Flags pour éviter les répétitions
        public bool DepartureCommentMade { get; set; }
        public bool CruiseCommentMade { get; set; }
        public bool DescentCommentMade { get; set; }
        public bool LandingCommentMade { get; set; }
    }

    /// <summary>
    /// Niveau d'alerte
    /// </summary>
    public enum AlertLevel
    {
        Info,
        Caution,
        Warning
    }

    /// <summary>
    /// Arguments d'événement pour les messages du copilote
    /// </summary>
    public class CopilotMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public DateTime Timestamp { get; }

        public CopilotMessageEventArgs(string message)
        {
            Message = message;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Arguments d'événement pour les alertes
    /// </summary>
    public class AlertEventArgs : EventArgs
    {
        public string Message { get; }
        public AlertLevel Level { get; }
        public DateTime Timestamp { get; }

        public AlertEventArgs(string message, AlertLevel level)
        {
            Message = message;
            Level = level;
            Timestamp = DateTime.Now;
        }
    }

    #endregion

    #region Modèles API OpenAI

    public class OpenAIResponse
    {
        [JsonProperty("choices")]
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        [JsonProperty("message")]
        public MessageContent Message { get; set; }
    }

    public class MessageContent
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion
}

