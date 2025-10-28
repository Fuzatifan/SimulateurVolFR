using System;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Moteur de physique pour la simulation de vol
    /// </summary>
    public class FlightPhysics
    {
        private const double GRAVITY = 9.81; // m/s²
        private const double AIR_DENSITY_SEA_LEVEL = 1.225; // kg/m³
        private const double FEET_TO_METERS = 0.3048;
        private const double KNOTS_TO_MS = 0.514444;
        private const double MS_TO_KNOTS = 1.94384;

        private Aircraft aircraft;
        private FlightState state;
        private DateTime lastUpdate;

        public FlightPhysics(Aircraft aircraft, FlightState state)
        {
            this.aircraft = aircraft;
            this.state = state;
            this.lastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Met à jour la physique du vol
        /// </summary>
        public void Update()
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            if (deltaTime <= 0 || deltaTime > 1.0) return; // Éviter les sauts de temps

            // Mettre à jour les systèmes
            UpdateEngines(deltaTime);
            UpdateFuel(deltaTime);
            UpdateAerodynamics(deltaTime);
            UpdatePosition(deltaTime);
            UpdateFlightPhase();
        }

        /// <summary>
        /// Met à jour les moteurs
        /// </summary>
        private void UpdateEngines(double deltaTime)
        {
            if (!state.EnginesRunning)
            {
                state.ThrottlePosition = 0;
                return;
            }

            // La poussée dépend de la position des gaz et de l'altitude
            double altitudeFactor = GetAltitudeFactor();
            double thrust = state.ThrottlePosition * altitudeFactor;

            // Calculer l'accélération
            double mass = aircraft.EmptyWeight + state.FuelRemaining;
            double acceleration = (thrust * 100) / mass; // Simplification

            // Mettre à jour la vitesse
            state.IndicatedAirspeed += acceleration * deltaTime * MS_TO_KNOTS;
            state.IndicatedAirspeed = Math.Max(0, state.IndicatedAirspeed);
            state.IndicatedAirspeed = Math.Min(state.IndicatedAirspeed, aircraft.MaxSpeed);
        }

        /// <summary>
        /// Met à jour la consommation de carburant
        /// </summary>
        private void UpdateFuel(double deltaTime)
        {
            if (!state.EnginesRunning) return;

            // Consommation basée sur la position des gaz
            double consumptionRate = aircraft.FuelConsumption * (state.ThrottlePosition / 100.0);
            state.FuelFlow = consumptionRate;

            // Consommer le carburant
            double consumed = consumptionRate * (deltaTime / 3600.0); // Convertir en heures
            state.FuelRemaining = Math.Max(0, state.FuelRemaining - consumed);

            // Arrêter les moteurs si plus de carburant
            if (state.FuelRemaining <= 0)
            {
                state.EnginesRunning = false;
            }
        }

        /// <summary>
        /// Met à jour l'aérodynamique
        /// </summary>
        private void UpdateAerodynamics(double deltaTime)
        {
            // Calculer la portance
            double lift = CalculateLift();
            double weight = (aircraft.EmptyWeight + state.FuelRemaining) * GRAVITY;

            // Calculer la vitesse verticale
            if (state.IsAirborne())
            {
                double netForce = lift - weight;
                double verticalAcceleration = netForce / (aircraft.EmptyWeight + state.FuelRemaining);
                state.VerticalSpeed += verticalAcceleration * deltaTime * 196.85; // Convertir en pieds/min
            }
            else
            {
                state.VerticalSpeed = 0;
            }

            // Limiter la vitesse verticale
            state.VerticalSpeed = Math.Clamp(state.VerticalSpeed, -3000, aircraft.ClimbRate);

            // Calculer le tangage basé sur la vitesse verticale
            if (state.IndicatedAirspeed > 0)
            {
                state.Pitch = Math.Atan(state.VerticalSpeed / (state.IndicatedAirspeed * 101.269)) * (180.0 / Math.PI);
                state.Pitch = Math.Clamp(state.Pitch, -15, 15);
            }

            // Appliquer la traînée
            ApplyDrag(deltaTime);
        }

        /// <summary>
        /// Calcule la portance
        /// </summary>
        private double CalculateLift()
        {
            double airDensity = GetAirDensity();
            double velocity = state.IndicatedAirspeed * KNOTS_TO_MS;
            double wingArea = 50.0; // Simplification - surface alaire en m²
            double liftCoefficient = 0.5 + (state.FlapPosition / 100.0) * 0.5;

            return 0.5 * airDensity * velocity * velocity * wingArea * liftCoefficient;
        }

        /// <summary>
        /// Applique la traînée
        /// </summary>
        private void ApplyDrag(double deltaTime)
        {
            double dragCoefficient = 0.02 + (state.GearDown ? 0.01 : 0) + (state.FlapPosition / 100.0 * 0.01);
            double airDensity = GetAirDensity();
            double velocity = state.IndicatedAirspeed * KNOTS_TO_MS;
            double wingArea = 50.0;

            double drag = 0.5 * airDensity * velocity * velocity * wingArea * dragCoefficient;
            double mass = aircraft.EmptyWeight + state.FuelRemaining;
            double deceleration = drag / mass;

            state.IndicatedAirspeed -= deceleration * deltaTime * MS_TO_KNOTS;
            state.IndicatedAirspeed = Math.Max(0, state.IndicatedAirspeed);
        }

        /// <summary>
        /// Met à jour la position de l'avion
        /// </summary>
        private void UpdatePosition(double deltaTime)
        {
            // Calculer la vitesse sol en tenant compte du vent
            double windEffect = CalculateWindEffect();
            state.Speed = state.IndicatedAirspeed + windEffect;

            // Convertir la vitesse en degrés de latitude/longitude par seconde
            double speedMS = state.Speed * KNOTS_TO_MS;
            double distanceM = speedMS * deltaTime;

            // Calculer le déplacement en latitude et longitude
            double latChange = (distanceM / 111320.0) * Math.Cos(state.Heading * Math.PI / 180.0);
            double lonChange = (distanceM / (111320.0 * Math.Cos(state.Latitude * Math.PI / 180.0))) * 
                              Math.Sin(state.Heading * Math.PI / 180.0);

            state.Latitude += latChange;
            state.Longitude += lonChange;

            // Mettre à jour l'altitude
            state.Altitude += state.VerticalSpeed * (deltaTime / 60.0);
            state.Altitude = Math.Max(0, state.Altitude);

            // Mettre à jour la route suivie
            state.Track = state.Heading; // Simplification
        }

        /// <summary>
        /// Met à jour la phase de vol
        /// </summary>
        private void UpdateFlightPhase()
        {
            if (!state.EnginesRunning)
            {
                state.Phase = FlightPhase.PreFlight;
            }
            else if (state.IsOnGround())
            {
                if (state.IndicatedAirspeed > 10)
                {
                    state.Phase = FlightPhase.Taxi;
                }
                else if (state.IndicatedAirspeed > aircraft.TakeoffSpeed * 0.8)
                {
                    state.Phase = FlightPhase.Takeoff;
                }
            }
            else // En vol
            {
                if (state.Altitude < 1000 && state.VerticalSpeed > 0)
                {
                    state.Phase = FlightPhase.Climb;
                }
                else if (state.VerticalSpeed > 500)
                {
                    state.Phase = FlightPhase.Climb;
                }
                else if (state.VerticalSpeed < -500)
                {
                    state.Phase = FlightPhase.Descent;
                }
                else if (state.Altitude < 3000 && state.VerticalSpeed < 0)
                {
                    state.Phase = FlightPhase.Approach;
                }
                else if (state.Altitude < 500)
                {
                    state.Phase = FlightPhase.Landing;
                }
                else
                {
                    state.Phase = FlightPhase.Cruise;
                }
            }
        }

        /// <summary>
        /// Calcule l'effet du vent sur la vitesse sol
        /// </summary>
        private double CalculateWindEffect()
        {
            double headingRad = state.Heading * Math.PI / 180.0;
            double windDirRad = state.WindDirection * Math.PI / 180.0;
            double angleDiff = windDirRad - headingRad;

            return state.WindSpeed * Math.Cos(angleDiff);
        }

        /// <summary>
        /// Obtient la densité de l'air en fonction de l'altitude
        /// </summary>
        private double GetAirDensity()
        {
            double altitudeM = state.Altitude * FEET_TO_METERS;
            return AIR_DENSITY_SEA_LEVEL * Math.Exp(-altitudeM / 8500.0);
        }

        /// <summary>
        /// Obtient le facteur d'altitude pour la poussée des moteurs
        /// </summary>
        private double GetAltitudeFactor()
        {
            return Math.Exp(-state.Altitude / 30000.0);
        }

        /// <summary>
        /// Applique une entrée de contrôle de tangage
        /// </summary>
        public void ApplyPitchInput(double input)
        {
            // input: -1 (piquer) à +1 (cabrer)
            double targetPitch = input * 15.0; // Maximum 15 degrés
            state.Pitch += (targetPitch - state.Pitch) * 0.1;
        }

        /// <summary>
        /// Applique une entrée de contrôle de roulis
        /// </summary>
        public void ApplyRollInput(double input)
        {
            // input: -1 (gauche) à +1 (droite)
            double targetRoll = input * 30.0; // Maximum 30 degrés
            state.Roll += (targetRoll - state.Roll) * 0.1;

            // Le roulis affecte le cap
            state.Heading += state.Roll * 0.1;
            state.Heading = (state.Heading + 360) % 360;
        }

        /// <summary>
        /// Applique une entrée de lacet
        /// </summary>
        public void ApplyYawInput(double input)
        {
            // input: -1 (gauche) à +1 (droite)
            state.Heading += input * 2.0;
            state.Heading = (state.Heading + 360) % 360;
        }
    }
}

