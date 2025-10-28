using System;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Gestionnaire d'accessibilité pour les lecteurs d'écran et la synthèse vocale
    /// </summary>
    public static class AccessibilityManager
    {
        private static SpeechSynthesizer? synthesizer;
        private static bool isInitialized = false;
        private static bool useScreenReader = true;

        // Import des fonctions NVDA
        [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvdaController_speakText(string text);

        [DllImport("nvdaControllerClient64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvdaController_testIfRunning();

        public static void Initialize()
        {
            if (isInitialized) return;

            try
            {
                // Initialiser la synthèse vocale
                synthesizer = new SpeechSynthesizer();
                synthesizer.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.NotSet, 0, new System.Globalization.CultureInfo("fr-FR"));
                synthesizer.Rate = 0; // Vitesse normale
                synthesizer.Volume = 100; // Volume maximum

                // Tester si NVDA est actif
                try
                {
                    useScreenReader = nvdaController_testIfRunning() == 0;
                }
                catch
                {
                    useScreenReader = false;
                }

                isInitialized = true;
                Speak("Simulateur de vol français initialisé");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur d'initialisation de l'accessibilité: {ex.Message}");
            }
        }

        public static void Shutdown()
        {
            synthesizer?.Dispose();
            synthesizer = null;
            isInitialized = false;
        }

        /// <summary>
        /// Annonce un texte via le lecteur d'écran ou la synthèse vocale
        /// </summary>
        public static void Speak(string text, bool interrupt = false)
        {
            if (!isInitialized || string.IsNullOrEmpty(text)) return;

            try
            {
                // Essayer d'abord avec NVDA
                if (useScreenReader)
                {
                    try
                    {
                        nvdaController_speakText(text);
                        return;
                    }
                    catch
                    {
                        useScreenReader = false;
                    }
                }

                // Sinon utiliser la synthèse vocale intégrée
                if (synthesizer != null)
                {
                    if (interrupt)
                    {
                        synthesizer.SpeakAsyncCancelAll();
                    }
                    synthesizer.SpeakAsync(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de synthèse vocale: {ex.Message}");
            }
        }

        /// <summary>
        /// Annonce un texte de manière synchrone (bloque jusqu'à la fin)
        /// </summary>
        public static void SpeakSync(string text)
        {
            if (!isInitialized || string.IsNullOrEmpty(text)) return;

            try
            {
                if (useScreenReader)
                {
                    try
                    {
                        nvdaController_speakText(text);
                        return;
                    }
                    catch
                    {
                        useScreenReader = false;
                    }
                }

                synthesizer?.Speak(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de synthèse vocale: {ex.Message}");
            }
        }

        /// <summary>
        /// Arrête toute annonce en cours
        /// </summary>
        public static void StopSpeaking()
        {
            try
            {
                synthesizer?.SpeakAsyncCancelAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'arrêt de la synthèse: {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si un lecteur d'écran est actif
        /// </summary>
        public static bool IsScreenReaderActive()
        {
            return useScreenReader;
        }
    }
}

