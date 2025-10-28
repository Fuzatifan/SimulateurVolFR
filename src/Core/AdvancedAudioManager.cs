using System;
using System.Collections.Generic;
using NAudio.Wave;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Gestionnaire audio avancé avec support des sons réels et audio 3D
    /// </summary>
    public class AdvancedAudioManager : IDisposable
    {
        private Dictionary<string, CachedSound> soundCache;
        private List<AudioPlayback> activePlaybacks;
        private IWavePlayer outputDevice;
        private MixingSampleProvider mixer;
        private FlightState flightState;

        // Volumes par catégorie
        private float engineVolume = 0.7f;
        private float ambientVolume = 0.5f;
        private float alertVolume = 0.9f;
        private float radioVolume = 0.8f;
        private float uiVolume = 0.6f;

        public AdvancedAudioManager()
        {
            soundCache = new Dictionary<string, CachedSound>();
            activePlaybacks = new List<AudioPlayback>();
            
            InitializeAudioDevice();
        }

        /// <summary>
        /// Initialise le périphérique audio
        /// </summary>
        private void InitializeAudioDevice()
        {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        /// <summary>
        /// Charge un son dans le cache
        /// </summary>
        public void LoadSound(string name, string filePath)
        {
            if (!soundCache.ContainsKey(name))
            {
                try
                {
                    soundCache[name] = new CachedSound(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement du son {name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Joue un son de moteur avec variation selon les gaz
        /// </summary>
        public void PlayEngineSound(Aircraft aircraft, float throttle)
        {
            string soundName = GetEngineSoundName(aircraft, throttle);
            
            // Arrêter les anciens sons de moteur
            StopSoundsByCategory("engine");
            
            // Jouer le nouveau son en boucle
            PlaySound(soundName, true, engineVolume, "engine");
        }

        /// <summary>
        /// Obtient le nom du son de moteur approprié
        /// </summary>
        private string GetEngineSoundName(Aircraft aircraft, float throttle)
        {
            string prefix = aircraft.Id.ToLower();
            
            if (throttle < 0.3f)
                return $"{prefix}_idle";
            else if (throttle < 0.7f)
                return $"{prefix}_cruise";
            else
                return $"{prefix}_full_throttle";
        }

        /// <summary>
        /// Joue un son d'ambiance
        /// </summary>
        public void PlayAmbientSound(string soundName, bool loop = true)
        {
            PlaySound(soundName, loop, ambientVolume, "ambient");
        }

        /// <summary>
        /// Joue une alerte sonore
        /// </summary>
        public void PlayAlert(string alertType)
        {
            string soundName = $"{alertType}_warning";
            PlaySound(soundName, false, alertVolume, "alert");
        }

        /// <summary>
        /// Joue un son radio
        /// </summary>
        public void PlayRadioSound(string soundName)
        {
            PlaySound(soundName, false, radioVolume, "radio");
        }

        /// <summary>
        /// Joue un son d'interface
        /// </summary>
        public void PlayUISound(string soundName)
        {
            PlaySound(soundName, false, uiVolume, "ui");
        }

        /// <summary>
        /// Joue un son avec paramètres
        /// </summary>
        private void PlaySound(string soundName, bool loop, float volume, string category)
        {
            if (!soundCache.ContainsKey(soundName))
            {
                Console.WriteLine($"Son non trouvé: {soundName}");
                return;
            }

            var sound = soundCache[soundName];
            var playback = new AudioPlayback(sound, loop, volume, category);
            
            activePlaybacks.Add(playback);
            mixer.AddMixerInput(playback.SampleProvider);
        }

        /// <summary>
        /// Arrête tous les sons d'une catégorie
        /// </summary>
        private void StopSoundsByCategory(string category)
        {
            var toRemove = new List<AudioPlayback>();
            
            foreach (var playback in activePlaybacks)
            {
                if (playback.Category == category)
                {
                    playback.Stop();
                    toRemove.Add(playback);
                }
            }
            
            foreach (var playback in toRemove)
            {
                activePlaybacks.Remove(playback);
            }
        }

        /// <summary>
        /// Arrête tous les sons
        /// </summary>
        public void StopAllSounds()
        {
            foreach (var playback in activePlaybacks)
            {
                playback.Stop();
            }
            
            activePlaybacks.Clear();
        }

        /// <summary>
        /// Met à jour le système audio
        /// </summary>
        public void Update(FlightState flightState)
        {
            this.flightState = flightState;
            
            // Nettoyer les sons terminés
            activePlaybacks.RemoveAll(p => !p.IsPlaying);
            
            // Ajuster les sons selon l'état du vol
            UpdateEnvironmentSounds();
        }

        /// <summary>
        /// Met à jour les sons d'environnement
        /// </summary>
        private void UpdateEnvironmentSounds()
        {
            if (flightState == null) return;
            
            // Son de vent selon la vitesse
            float windIntensity = flightState.Speed / 200.0f; // Normaliser
            
            if (windIntensity < 0.3f)
                PlayAmbientSound("wind_light");
            else if (windIntensity < 0.6f)
                PlayAmbientSound("wind_moderate");
            else
                PlayAmbientSound("wind_strong");
        }

        /// <summary>
        /// Définit le volume d'une catégorie
        /// </summary>
        public void SetCategoryVolume(string category, float volume)
        {
            switch (category.ToLower())
            {
                case "engine":
                    engineVolume = volume;
                    break;
                case "ambient":
                    ambientVolume = volume;
                    break;
                case "alert":
                    alertVolume = volume;
                    break;
                case "radio":
                    radioVolume = volume;
                    break;
                case "ui":
                    uiVolume = volume;
                    break;
            }
            
            // Mettre à jour les sons actifs
            foreach (var playback in activePlaybacks)
            {
                if (playback.Category == category)
                {
                    playback.SetVolume(volume);
                }
            }
        }

        public void Dispose()
        {
            StopAllSounds();
            outputDevice?.Stop();
            outputDevice?.Dispose();
        }
    }

    /// <summary>
    /// Représente un son en cache
    /// </summary>
    public class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(string filePath)
        {
            using (var audioFileReader = new AudioFileReader(filePath))
            {
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer);
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }

    /// <summary>
    /// Représente une lecture audio active
    /// </summary>
    public class AudioPlayback
    {
        public ISampleProvider SampleProvider { get; private set; }
        public string Category { get; private set; }
        public bool IsPlaying { get; private set; }
        
        private VolumeSampleProvider volumeProvider;
        private bool loop;

        public AudioPlayback(CachedSound sound, bool loop, float volume, string category)
        {
            this.loop = loop;
            this.Category = category;
            this.IsPlaying = true;
            
            var provider = new CachedSoundSampleProvider(sound, loop);
            volumeProvider = new VolumeSampleProvider(provider);
            volumeProvider.Volume = volume;
            
            SampleProvider = volumeProvider;
        }

        public void SetVolume(float volume)
        {
            if (volumeProvider != null)
            {
                volumeProvider.Volume = volume;
            }
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }

    /// <summary>
    /// Fournisseur de samples pour un son en cache
    /// </summary>
    public class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private readonly bool loop;
        private long position;

        public WaveFormat WaveFormat => cachedSound.WaveFormat;

        public CachedSoundSampleProvider(CachedSound cachedSound, bool loop)
        {
            this.cachedSound = cachedSound;
            this.loop = loop;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            
            if (loop && position >= cachedSound.AudioData.Length)
            {
                position = 0;
            }
            
            return (int)samplesToCopy;
        }
    }
}

