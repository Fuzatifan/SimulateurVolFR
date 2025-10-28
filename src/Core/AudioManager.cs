using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Gestionnaire audio pour les sons du simulateur
    /// </summary>
    public static class AudioManager
    {
        private static WaveOutEvent? outputDevice;
        private static MixingSampleProvider? mixer;
        private static Dictionary<string, CachedSound> soundCache = new Dictionary<string, CachedSound>();
        private static bool isInitialized = false;
        private static float masterVolume = 1.0f;

        public static void Initialize()
        {
            if (isInitialized) return;

            try
            {
                outputDevice = new WaveOutEvent();
                mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
                {
                    ReadFully = true
                };
                outputDevice.Init(mixer);
                outputDevice.Play();

                isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur d'initialisation audio: {ex.Message}");
            }
        }

        public static void Shutdown()
        {
            try
            {
                outputDevice?.Stop();
                outputDevice?.Dispose();
                outputDevice = null;
                mixer = null;
                soundCache.Clear();
                isInitialized = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'arrêt audio: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge un son en mémoire
        /// </summary>
        public static void LoadSound(string name, string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                soundCache[name] = new CachedSound(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de chargement du son {name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Joue un son
        /// </summary>
        public static void PlaySound(string name, float volume = 1.0f, bool loop = false)
        {
            if (!isInitialized || mixer == null) return;

            try
            {
                if (soundCache.TryGetValue(name, out var sound))
                {
                    var sampleProvider = new CachedSoundSampleProvider(sound)
                    {
                        Volume = volume * masterVolume
                    };

                    if (loop)
                    {
                        var loopProvider = new LoopingSampleProvider(sampleProvider);
                        mixer.AddMixerInput(loopProvider);
                    }
                    else
                    {
                        mixer.AddMixerInput(sampleProvider);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du son {name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Définit le volume principal
        /// </summary>
        public static void SetMasterVolume(float volume)
        {
            masterVolume = Math.Clamp(volume, 0.0f, 1.0f);
        }

        /// <summary>
        /// Obtient le volume principal
        /// </summary>
        public static float GetMasterVolume()
        {
            return masterVolume;
        }
    }

    /// <summary>
    /// Représente un son mis en cache
    /// </summary>
    internal class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(string audioFileName)
        {
            using var audioFileReader = new AudioFileReader(audioFileName);
            WaveFormat = audioFileReader.WaveFormat;
            var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
            var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer[..samplesRead]);
            }
            AudioData = wholeFile.ToArray();
        }
    }

    /// <summary>
    /// Fournisseur d'échantillons pour les sons mis en cache
    /// </summary>
    internal class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;
        public float Volume { get; set; } = 1.0f;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            
            // Appliquer le volume
            for (int i = 0; i < samplesToCopy; i++)
            {
                buffer[offset + i] *= Volume;
            }
            
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat => cachedSound.WaveFormat;
    }

    /// <summary>
    /// Fournisseur d'échantillons en boucle
    /// </summary>
    internal class LoopingSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private bool hasEnded;

        public LoopingSampleProvider(ISampleProvider source)
        {
            this.source = source;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = source.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    if (hasEnded)
                    {
                        break;
                    }
                    // Redémarrer la lecture
                    if (source is CachedSoundSampleProvider cachedProvider)
                    {
                        // Créer une nouvelle instance pour recommencer
                        break;
                    }
                }
                totalRead += read;
            }
            return totalRead;
        }

        public WaveFormat WaveFormat => source.WaveFormat;
    }
}

