using System;
using System.Threading;
using System.Threading.Tasks;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Gestionnaire principal de la simulation
    /// </summary>
    public class SimulationManager
    {
        private static SimulationManager? instance;
        public static SimulationManager Instance => instance ??= new SimulationManager();

        private Aircraft? currentAircraft;
        private Airport? departureAirport;
        private Airport? destinationAirport;
        private FlightState? flightState;
        private FlightPhysics? flightPhysics;

        private CancellationTokenSource? simulationCts;
        private Task? simulationTask;
        private bool isRunning;

        public Aircraft? CurrentAircraft => currentAircraft;
        public Airport? DepartureAirport => departureAirport;
        public Airport? DestinationAirport => destinationAirport;
        public FlightState? FlightState => flightState;
        public bool IsRunning => isRunning;

        public event EventHandler<FlightStateChangedEventArgs>? FlightStateChanged;
        public event EventHandler<FlightPhaseChangedEventArgs>? FlightPhaseChanged;
        public event EventHandler<string>? StatusMessageReceived;

        private SimulationManager()
        {
        }

        /// <summary>
        /// Initialise un nouveau vol
        /// </summary>
        public void InitializeFlight(Aircraft aircraft, Airport departure, Airport? destination = null)
        {
            StopSimulation();

            currentAircraft = aircraft;
            departureAirport = departure;
            destinationAirport = destination;

            // Créer l'état initial du vol
            flightState = new FlightState
            {
                Latitude = departure.Latitude,
                Longitude = departure.Longitude,
                Altitude = departure.Elevation,
                Speed = 0,
                IndicatedAirspeed = 0,
                Heading = 0,
                Pitch = 0,
                Roll = 0,
                VerticalSpeed = 0,
                ThrottlePosition = 0,
                EnginesRunning = false,
                FuelRemaining = aircraft.FuelCapacity,
                FuelFlow = 0,
                FlapPosition = 0,
                GearDown = true,
                BrakesEngaged = true,
                AutopilotEnabled = false,
                Phase = FlightPhase.PreFlight,
                WindSpeed = 5,
                WindDirection = 270,
                Temperature = 15,
                Visibility = 10,
                FlightStartTime = DateTime.Now
            };

            flightPhysics = new FlightPhysics(aircraft, flightState);

            SendStatusMessage($"Vol initialisé. Départ: {departure.Name}");
            if (destination != null)
            {
                double distance = departure.DistanceTo(destination);
                double bearing = departure.BearingTo(destination);
                SendStatusMessage($"Destination: {destination.Name}. Distance: {Math.Round(distance)} miles nautiques. Cap: {Math.Round(bearing)} degrés.");
            }
        }

        /// <summary>
        /// Démarre la simulation
        /// </summary>
        public void StartSimulation()
        {
            if (isRunning || flightState == null || flightPhysics == null) return;

            isRunning = true;
            simulationCts = new CancellationTokenSource();
            simulationTask = Task.Run(() => SimulationLoop(simulationCts.Token));

            SendStatusMessage("Simulation démarrée");
        }

        /// <summary>
        /// Arrête la simulation
        /// </summary>
        public void StopSimulation()
        {
            if (!isRunning) return;

            isRunning = false;
            simulationCts?.Cancel();
            simulationTask?.Wait(1000);
            simulationCts?.Dispose();
            simulationCts = null;

            SendStatusMessage("Simulation arrêtée");
        }

        /// <summary>
        /// Boucle principale de simulation
        /// </summary>
        private void SimulationLoop(CancellationToken cancellationToken)
        {
            const int UpdateIntervalMs = 50; // 20 FPS
            FlightPhase previousPhase = flightState!.Phase;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Mettre à jour la physique
                    flightPhysics!.Update();

                    // Mettre à jour la durée du vol
                    flightState!.FlightDuration = DateTime.Now - flightState.FlightStartTime;

                    // Détecter les changements de phase
                    if (flightState.Phase != previousPhase)
                    {
                        OnFlightPhaseChanged(previousPhase, flightState.Phase);
                        previousPhase = flightState.Phase;
                    }

                    // Notifier les changements d'état
                    OnFlightStateChanged();

                    // Attendre avant la prochaine mise à jour
                    Thread.Sleep(UpdateIntervalMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur dans la boucle de simulation: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Démarre les moteurs
        /// </summary>
        public void StartEngines()
        {
            if (flightState == null) return;

            flightState.EnginesRunning = true;
            SendStatusMessage("Moteurs démarrés");
            AccessibilityManager.Speak("Moteurs démarrés");
        }

        /// <summary>
        /// Arrête les moteurs
        /// </summary>
        public void StopEngines()
        {
            if (flightState == null) return;

            flightState.EnginesRunning = false;
            flightState.ThrottlePosition = 0;
            SendStatusMessage("Moteurs arrêtés");
            AccessibilityManager.Speak("Moteurs arrêtés");
        }

        /// <summary>
        /// Définit la position des gaz
        /// </summary>
        public void SetThrottle(double position)
        {
            if (flightState == null) return;

            flightState.ThrottlePosition = Math.Clamp(position, 0, 100);
        }

        /// <summary>
        /// Augmente la position des gaz
        /// </summary>
        public void IncreaseThrottle(double amount = 5)
        {
            if (flightState == null) return;
            SetThrottle(flightState.ThrottlePosition + amount);
        }

        /// <summary>
        /// Diminue la position des gaz
        /// </summary>
        public void DecreaseThrottle(double amount = 5)
        {
            if (flightState == null) return;
            SetThrottle(flightState.ThrottlePosition - amount);
        }

        /// <summary>
        /// Définit la position des volets
        /// </summary>
        public void SetFlaps(double position)
        {
            if (flightState == null) return;

            flightState.FlapPosition = Math.Clamp(position, 0, 100);
            SendStatusMessage($"Volets à {Math.Round(position)}%");
        }

        /// <summary>
        /// Bascule le train d'atterrissage
        /// </summary>
        public void ToggleGear()
        {
            if (flightState == null) return;

            flightState.GearDown = !flightState.GearDown;
            string status = flightState.GearDown ? "Train sorti" : "Train rentré";
            SendStatusMessage(status);
            AccessibilityManager.Speak(status);
        }

        /// <summary>
        /// Bascule les freins
        /// </summary>
        public void ToggleBrakes()
        {
            if (flightState == null) return;

            flightState.BrakesEngaged = !flightState.BrakesEngaged;
            string status = flightState.BrakesEngaged ? "Freins engagés" : "Freins relâchés";
            SendStatusMessage(status);
        }

        /// <summary>
        /// Applique une entrée de contrôle de tangage
        /// </summary>
        public void ApplyPitchControl(double input)
        {
            flightPhysics?.ApplyPitchInput(input);
        }

        /// <summary>
        /// Applique une entrée de contrôle de roulis
        /// </summary>
        public void ApplyRollControl(double input)
        {
            flightPhysics?.ApplyRollInput(input);
        }

        /// <summary>
        /// Applique une entrée de contrôle de lacet
        /// </summary>
        public void ApplyYawControl(double input)
        {
            flightPhysics?.ApplyYawInput(input);
        }

        /// <summary>
        /// Annonce l'état actuel du vol
        /// </summary>
        public void AnnounceFlightState()
        {
            if (flightState == null) return;

            string message = flightState.GetVoiceStatus();
            AccessibilityManager.Speak(message);
        }

        private void OnFlightStateChanged()
        {
            FlightStateChanged?.Invoke(this, new FlightStateChangedEventArgs(flightState!));
        }

        private void OnFlightPhaseChanged(FlightPhase oldPhase, FlightPhase newPhase)
        {
            FlightPhaseChanged?.Invoke(this, new FlightPhaseChangedEventArgs(oldPhase, newPhase));
            
            string message = $"Phase de vol: {flightState!.GetPhaseDescription()}";
            SendStatusMessage(message);
            AccessibilityManager.Speak(message);
        }

        private void SendStatusMessage(string message)
        {
            StatusMessageReceived?.Invoke(this, message);
        }
    }

    public class FlightStateChangedEventArgs : EventArgs
    {
        public FlightState FlightState { get; }

        public FlightStateChangedEventArgs(FlightState flightState)
        {
            FlightState = flightState;
        }
    }

    public class FlightPhaseChangedEventArgs : EventArgs
    {
        public FlightPhase OldPhase { get; }
        public FlightPhase NewPhase { get; }

        public FlightPhaseChangedEventArgs(FlightPhase oldPhase, FlightPhase newPhase)
        {
            OldPhase = oldPhase;
            NewPhase = newPhase;
        }
    }
}

