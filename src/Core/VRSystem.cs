using System;
using System.Collections.Generic;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de réalité virtuelle pour immersion totale
    /// Support des casques VR (Meta Quest, HTC Vive, Valve Index, etc.)
    /// </summary>
    public class VRSystem
    {
        private AccessibilityManager accessibilityManager;
        private AudioManager audioManager;
        private bool isVREnabled;
        private VRHeadset connectedHeadset;
        private VRCockpitEnvironment cockpitEnvironment;
        private VRHandTracking handTracking;
        private VRSpatialAudio spatialAudio;

        public event EventHandler<VREventArgs> VRStateChanged;
        public event EventHandler<VRInteractionEventArgs> VRInteraction;

        public bool IsVREnabled => isVREnabled;
        public VRHeadset ConnectedHeadset => connectedHeadset;

        public VRSystem(AccessibilityManager accessibilityManager, AudioManager audioManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.audioManager = audioManager;
            this.isVREnabled = false;
            this.cockpitEnvironment = new VRCockpitEnvironment();
            this.handTracking = new VRHandTracking();
            this.spatialAudio = new VRSpatialAudio(audioManager);
        }

        /// <summary>
        /// Initialise le mode VR
        /// </summary>
        public bool InitializeVR()
        {
            try
            {
                // Détection du casque VR
                connectedHeadset = DetectVRHeadset();

                if (connectedHeadset == null)
                {
                    accessibilityManager.Speak("Aucun casque VR détecté. Vérifiez la connexion.");
                    return false;
                }

                // Initialisation de l'environnement VR
                InitializeCockpitEnvironment();
                InitializeHandTracking();
                InitializeSpatialAudio();

                isVREnabled = true;

                accessibilityManager.Speak($"Mode VR activé avec {connectedHeadset.Name}. " +
                    "Vous êtes maintenant dans le cockpit virtuel.");

                VRStateChanged?.Invoke(this, new VREventArgs(true, connectedHeadset));

                return true;
            }
            catch (Exception ex)
            {
                accessibilityManager.Speak($"Erreur lors de l'initialisation VR : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Détecte le casque VR connecté
        /// </summary>
        private VRHeadset DetectVRHeadset()
        {
            // Simulation de détection - dans une vraie implémentation,
            // on utiliserait OpenVR/OpenXR pour détecter le casque

            // Pour l'instant, on retourne un casque simulé
            return new VRHeadset
            {
                Name = "Meta Quest 3",
                Type = VRHeadsetType.MetaQuest,
                Resolution = new VRResolution { Width = 2064, Height = 2208 },
                RefreshRate = 120,
                FieldOfView = 110,
                HasHandTracking = true,
                HasEyeTracking = false,
                HasHapticFeedback = true
            };
        }

        /// <summary>
        /// Initialise l'environnement du cockpit VR
        /// </summary>
        private void InitializeCockpitEnvironment()
        {
            cockpitEnvironment.LoadCockpit(CockpitType.Cessna172);

            // Éléments du cockpit
            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Manche",
                Position = new VRPosition(0, -0.3f, 0.2f),
                Type = VRElementType.Yoke,
                IsGrabbable = true,
                HasHapticFeedback = true
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Manette des gaz",
                Position = new VRPosition(0.2f, -0.2f, 0.1f),
                Type = VRElementType.Throttle,
                IsGrabbable = true,
                HasHapticFeedback = true
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Palonnier gauche",
                Position = new VRPosition(-0.3f, -0.5f, 0.3f),
                Type = VRElementType.Rudder,
                IsGrabbable = false,
                HasHapticFeedback = true
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Palonnier droit",
                Position = new VRPosition(0.3f, -0.5f, 0.3f),
                Type = VRElementType.Rudder,
                IsGrabbable = false,
                HasHapticFeedback = true
            });

            // Instruments
            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Altimètre",
                Position = new VRPosition(0, 0, 0.5f),
                Type = VRElementType.Instrument,
                IsGrabbable = false,
                HasHapticFeedback = false,
                VoiceDescription = "Altimètre indiquant l'altitude en pieds"
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Indicateur de vitesse",
                Position = new VRPosition(-0.15f, 0, 0.5f),
                Type = VRElementType.Instrument,
                IsGrabbable = false,
                HasHapticFeedback = false,
                VoiceDescription = "Indicateur de vitesse en nœuds"
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Horizon artificiel",
                Position = new VRPosition(0.15f, 0, 0.5f),
                Type = VRElementType.Instrument,
                IsGrabbable = false,
                HasHapticFeedback = false,
                VoiceDescription = "Horizon artificiel montrant l'assiette de l'avion"
            });

            // Interrupteurs et boutons
            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Contact magnétos",
                Position = new VRPosition(-0.3f, -0.1f, 0.3f),
                Type = VRElementType.Switch,
                IsGrabbable = false,
                HasHapticFeedback = true,
                VoiceDescription = "Interrupteur de contact des magnétos"
            });

            cockpitEnvironment.AddInteractiveElement(new VRInteractiveElement
            {
                Name = "Pompe à carburant",
                Position = new VRPosition(-0.25f, -0.15f, 0.3f),
                Type = VRElementType.Switch,
                IsGrabbable = false,
                HasHapticFeedback = true,
                VoiceDescription = "Interrupteur de la pompe à carburant"
            });
        }

        /// <summary>
        /// Initialise le suivi des mains
        /// </summary>
        private void InitializeHandTracking()
        {
            if (!connectedHeadset.HasHandTracking)
                return;

            handTracking.Enable();
            handTracking.OnHandGesture += OnHandGesture;
            handTracking.OnGrab += OnGrab;
            handTracking.OnRelease += OnRelease;
        }

        /// <summary>
        /// Initialise l'audio spatial 3D
        /// </summary>
        private void InitializeSpatialAudio()
        {
            // Positionnement des sources audio dans l'espace 3D

            // Moteur (devant)
            spatialAudio.AddSpatialSource("engine", new VRPosition(0, 0, 2f), 1.0f);

            // Radio (panneau gauche)
            spatialAudio.AddSpatialSource("radio", new VRPosition(-0.4f, 0, 0.4f), 0.5f);

            // Alertes (panneau central)
            spatialAudio.AddSpatialSource("alerts", new VRPosition(0, 0, 0.5f), 0.7f);

            // Vent extérieur (tout autour)
            spatialAudio.AddSpatialSource("wind", new VRPosition(0, 0, 0), 0.3f);

            // Copilote (siège droit)
            spatialAudio.AddSpatialSource("copilot", new VRPosition(0.5f, 0, 0), 0.8f);
        }

        /// <summary>
        /// Gestion des gestes de la main
        /// </summary>
        private void OnHandGesture(object sender, HandGestureEventArgs e)
        {
            switch (e.Gesture)
            {
                case HandGesture.ThumbsUp:
                    accessibilityManager.Speak("Geste reconnu : Pouce levé");
                    break;

                case HandGesture.Point:
                    // Pointer vers un instrument pour obtenir sa valeur
                    VRInteractiveElement pointedElement = GetPointedElement(e.Direction);
                    if (pointedElement != null)
                    {
                        accessibilityManager.Speak(pointedElement.VoiceDescription);
                    }
                    break;

                case HandGesture.OpenPalm:
                    accessibilityManager.Speak("Menu VR ouvert");
                    break;
            }
        }

        /// <summary>
        /// Gestion de la saisie d'un élément
        /// </summary>
        private void OnGrab(object sender, GrabEventArgs e)
        {
            VRInteractiveElement element = GetElementAtPosition(e.Position);

            if (element != null && element.IsGrabbable)
            {
                element.IsGrabbed = true;
                accessibilityManager.Speak($"{element.Name} saisi");

                if (element.HasHapticFeedback)
                {
                    SendHapticFeedback(e.Hand, 0.3f, 0.1f);
                }

                VRInteraction?.Invoke(this, new VRInteractionEventArgs(element, VRInteractionType.Grab));
            }
        }

        /// <summary>
        /// Gestion du relâchement d'un élément
        /// </summary>
        private void OnRelease(object sender, ReleaseEventArgs e)
        {
            VRInteractiveElement element = GetGrabbedElement(e.Hand);

            if (element != null)
            {
                element.IsGrabbed = false;
                accessibilityManager.Speak($"{element.Name} relâché");

                if (element.HasHapticFeedback)
                {
                    SendHapticFeedback(e.Hand, 0.2f, 0.05f);
                }

                VRInteraction?.Invoke(this, new VRInteractionEventArgs(element, VRInteractionType.Release));
            }
        }

        /// <summary>
        /// Obtient l'élément pointé
        /// </summary>
        private VRInteractiveElement GetPointedElement(VRDirection direction)
        {
            // Raycast pour trouver l'élément pointé
            foreach (var element in cockpitEnvironment.InteractiveElements)
            {
                if (IsPointingAt(direction, element.Position))
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Obtient l'élément à une position
        /// </summary>
        private VRInteractiveElement GetElementAtPosition(VRPosition position)
        {
            foreach (var element in cockpitEnvironment.InteractiveElements)
            {
                if (Vector3Distance(position, element.Position) < 0.1f)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Obtient l'élément saisi par une main
        /// </summary>
        private VRInteractiveElement GetGrabbedElement(VRHand hand)
        {
            foreach (var element in cockpitEnvironment.InteractiveElements)
            {
                if (element.IsGrabbed && element.GrabbedByHand == hand)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Vérifie si on pointe vers un élément
        /// </summary>
        private bool IsPointingAt(VRDirection direction, VRPosition position)
        {
            // Calcul simplifié - dans une vraie implémentation,
            // on ferait un vrai raycast 3D
            return true; // Placeholder
        }

        /// <summary>
        /// Calcule la distance entre deux positions
        /// </summary>
        private float Vector3Distance(VRPosition a, VRPosition b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Envoie un retour haptique
        /// </summary>
        private void SendHapticFeedback(VRHand hand, float intensity, float duration)
        {
            if (!connectedHeadset.HasHapticFeedback)
                return;

            // Dans une vraie implémentation, on enverrait le feedback au contrôleur
            Console.WriteLine($"Haptic feedback: {hand}, intensity={intensity}, duration={duration}");
        }

        /// <summary>
        /// Met à jour le système VR
        /// </summary>
        public void Update(FlightState flightState)
        {
            if (!isVREnabled)
                return;

            // Mise à jour de l'audio spatial selon la position de la tête
            UpdateSpatialAudio(flightState);

            // Mise à jour des instruments visuels
            UpdateInstruments(flightState);

            // Mise à jour de l'environnement extérieur
            UpdateExteriorView(flightState);
        }

        /// <summary>
        /// Met à jour l'audio spatial
        /// </summary>
        private void UpdateSpatialAudio(FlightState flightState)
        {
            // Ajuster le volume du moteur selon les RPM
            float engineVolume = flightState.ThrottlePosition;
            spatialAudio.SetVolume("engine", engineVolume);

            // Ajuster le vent selon la vitesse
            float windVolume = Math.Min(flightState.Speed / 200f, 1.0f);
            spatialAudio.SetVolume("wind", windVolume);
        }

        /// <summary>
        /// Met à jour les instruments
        /// </summary>
        private void UpdateInstruments(FlightState flightState)
        {
            // Dans une vraie implémentation, on mettrait à jour
            // les modèles 3D des instruments
        }

        /// <summary>
        /// Met à jour la vue extérieure
        /// </summary>
        private void UpdateExteriorView(FlightState flightState)
        {
            // Mise à jour du paysage, des nuages, etc.
            // selon la position et l'altitude
        }

        /// <summary>
        /// Désactive le mode VR
        /// </summary>
        public void DisableVR()
        {
            if (!isVREnabled)
                return;

            handTracking.Disable();
            spatialAudio.Disable();

            isVREnabled = false;

            accessibilityManager.Speak("Mode VR désactivé. Retour au mode standard.");

            VRStateChanged?.Invoke(this, new VREventArgs(false, null));
        }

        /// <summary>
        /// Recalibre la position VR
        /// </summary>
        public void RecenterView()
        {
            if (!isVREnabled)
                return;

            accessibilityManager.Speak("Vue recentrée");
        }
    }

    #region Modèles de données VR

    /// <summary>
    /// Types de casques VR
    /// </summary>
    public enum VRHeadsetType
    {
        MetaQuest,
        HTCVive,
        ValveIndex,
        PicoNeo,
        Other
    }

    /// <summary>
    /// Casque VR
    /// </summary>
    public class VRHeadset
    {
        public string Name { get; set; }
        public VRHeadsetType Type { get; set; }
        public VRResolution Resolution { get; set; }
        public int RefreshRate { get; set; }
        public float FieldOfView { get; set; }
        public bool HasHandTracking { get; set; }
        public bool HasEyeTracking { get; set; }
        public bool HasHapticFeedback { get; set; }
    }

    /// <summary>
    /// Résolution VR
    /// </summary>
    public class VRResolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>
    /// Position 3D en VR
    /// </summary>
    public class VRPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public VRPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// Direction 3D en VR
    /// </summary>
    public class VRDirection
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    /// <summary>
    /// Environnement du cockpit VR
    /// </summary>
    public class VRCockpitEnvironment
    {
        public CockpitType CurrentCockpit { get; private set; }
        public List<VRInteractiveElement> InteractiveElements { get; private set; }

        public VRCockpitEnvironment()
        {
            InteractiveElements = new List<VRInteractiveElement>();
        }

        public void LoadCockpit(CockpitType type)
        {
            CurrentCockpit = type;
            InteractiveElements.Clear();
        }

        public void AddInteractiveElement(VRInteractiveElement element)
        {
            InteractiveElements.Add(element);
        }
    }

    /// <summary>
    /// Types de cockpit
    /// </summary>
    public enum CockpitType
    {
        Cessna172,
        A320,
        Boeing737,
        Helicopter
    }

    /// <summary>
    /// Élément interactif en VR
    /// </summary>
    public class VRInteractiveElement
    {
        public string Name { get; set; }
        public VRPosition Position { get; set; }
        public VRElementType Type { get; set; }
        public bool IsGrabbable { get; set; }
        public bool HasHapticFeedback { get; set; }
        public string VoiceDescription { get; set; }
        public bool IsGrabbed { get; set; }
        public VRHand GrabbedByHand { get; set; }
    }

    /// <summary>
    /// Types d'éléments VR
    /// </summary>
    public enum VRElementType
    {
        Yoke,
        Throttle,
        Rudder,
        Switch,
        Button,
        Knob,
        Lever,
        Instrument
    }

    /// <summary>
    /// Main VR
    /// </summary>
    public enum VRHand
    {
        Left,
        Right
    }

    /// <summary>
    /// Suivi des mains
    /// </summary>
    public class VRHandTracking
    {
        public event EventHandler<HandGestureEventArgs> OnHandGesture;
        public event EventHandler<GrabEventArgs> OnGrab;
        public event EventHandler<ReleaseEventArgs> OnRelease;

        public void Enable() { }
        public void Disable() { }
    }

    /// <summary>
    /// Gestes de la main
    /// </summary>
    public enum HandGesture
    {
        ThumbsUp,
        Point,
        OpenPalm,
        Fist,
        Peace
    }

    /// <summary>
    /// Audio spatial 3D
    /// </summary>
    public class VRSpatialAudio
    {
        private AudioManager audioManager;
        private Dictionary<string, SpatialAudioSource> sources;

        public VRSpatialAudio(AudioManager audioManager)
        {
            this.audioManager = audioManager;
            this.sources = new Dictionary<string, SpatialAudioSource>();
        }

        public void AddSpatialSource(string name, VRPosition position, float volume)
        {
            sources[name] = new SpatialAudioSource
            {
                Position = position,
                Volume = volume
            };
        }

        public void SetVolume(string name, float volume)
        {
            if (sources.ContainsKey(name))
            {
                sources[name].Volume = volume;
            }
        }

        public void Disable() { }
    }

    /// <summary>
    /// Source audio spatiale
    /// </summary>
    public class SpatialAudioSource
    {
        public VRPosition Position { get; set; }
        public float Volume { get; set; }
    }

    /// <summary>
    /// Arguments d'événement VR
    /// </summary>
    public class VREventArgs : EventArgs
    {
        public bool IsEnabled { get; }
        public VRHeadset Headset { get; }

        public VREventArgs(bool isEnabled, VRHeadset headset)
        {
            IsEnabled = isEnabled;
            Headset = headset;
        }
    }

    /// <summary>
    /// Arguments d'événement d'interaction VR
    /// </summary>
    public class VRInteractionEventArgs : EventArgs
    {
        public VRInteractiveElement Element { get; }
        public VRInteractionType Type { get; }

        public VRInteractionEventArgs(VRInteractiveElement element, VRInteractionType type)
        {
            Element = element;
            Type = type;
        }
    }

    /// <summary>
    /// Types d'interaction VR
    /// </summary>
    public enum VRInteractionType
    {
        Grab,
        Release,
        Touch,
        Point
    }

    public class HandGestureEventArgs : EventArgs
    {
        public HandGesture Gesture { get; set; }
        public VRDirection Direction { get; set; }
    }

    public class GrabEventArgs : EventArgs
    {
        public VRHand Hand { get; set; }
        public VRPosition Position { get; set; }
    }

    public class ReleaseEventArgs : EventArgs
    {
        public VRHand Hand { get; set; }
    }

    #endregion
}

