using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de météo dynamique avec intégration API
    /// </summary>
    public class WeatherSystem
    {
        private const string OPENWEATHER_API_KEY = "YOUR_API_KEY_HERE"; // À remplacer
        private const string OPENWEATHER_BASE_URL = "https://api.openweathermap.org/data/2.5/weather";
        
        private HttpClient httpClient;
        private WeatherData currentWeather;
        private DateTime lastUpdate;
        private TimeSpan updateInterval = TimeSpan.FromMinutes(15);

        public WeatherData CurrentWeather => currentWeather;

        public WeatherSystem()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Récupère la météo pour un aéroport
        /// </summary>
        public async Task<WeatherData> GetWeatherForAirport(Airport airport)
        {
            // Vérifier si une mise à jour est nécessaire
            if (currentWeather != null && 
                currentWeather.AirportIcao == airport.IcaoCode &&
                DateTime.Now - lastUpdate < updateInterval)
            {
                return currentWeather;
            }

            try
            {
                // Construire l'URL de l'API
                string url = $"{OPENWEATHER_BASE_URL}?lat={airport.Latitude}&lon={airport.Longitude}&appid={OPENWEATHER_API_KEY}&units=metric&lang=fr";
                
                // Faire la requête
                var response = await httpClient.GetStringAsync(url);
                var apiData = JsonConvert.DeserializeObject<OpenWeatherResponse>(response);
                
                // Convertir en WeatherData
                currentWeather = ConvertToWeatherData(apiData, airport);
                lastUpdate = DateTime.Now;
                
                return currentWeather;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la météo: {ex.Message}");
                
                // Retourner une météo par défaut en cas d'erreur
                return GetDefaultWeather(airport);
            }
        }

        /// <summary>
        /// Convertit la réponse API en WeatherData
        /// </summary>
        private WeatherData ConvertToWeatherData(OpenWeatherResponse apiData, Airport airport)
        {
            var weather = new WeatherData
            {
                AirportIcao = airport.IcaoCode,
                AirportName = airport.Name,
                Timestamp = DateTime.Now,
                
                // Température
                Temperature = apiData.Main.Temp,
                FeelsLike = apiData.Main.FeelsLike,
                
                // Pression (convertir en inHg)
                Pressure = apiData.Main.Pressure * 0.02953,
                
                // Humidité
                Humidity = apiData.Main.Humidity,
                
                // Vent
                WindSpeed = apiData.Wind.Speed * 1.94384, // m/s vers nœuds
                WindDirection = apiData.Wind.Deg,
                WindGust = apiData.Wind.Gust * 1.94384,
                
                // Visibilité (convertir en miles nautiques)
                Visibility = apiData.Visibility / 1852.0,
                
                // Nuages
                CloudCoverage = apiData.Clouds.All,
                
                // Description
                Condition = apiData.Weather[0].Main,
                Description = apiData.Weather[0].Description
            };
            
            // Calculer les conditions dérivées
            CalculateDerivedConditions(weather);
            
            return weather;
        }

        /// <summary>
        /// Calcule les conditions météo dérivées
        /// </summary>
        private void CalculateDerivedConditions(WeatherData weather)
        {
            // Déterminer le plafond nuageux
            if (weather.CloudCoverage < 25)
            {
                weather.Ceiling = 10000; // Ciel dégagé
                weather.CeilingDescription = "Ciel dégagé";
            }
            else if (weather.CloudCoverage < 50)
            {
                weather.Ceiling = 5000;
                weather.CeilingDescription = "Nuages épars";
            }
            else if (weather.CloudCoverage < 75)
            {
                weather.Ceiling = 3000;
                weather.CeilingDescription = "Nuages fragmentés";
            }
            else
            {
                weather.Ceiling = 1500;
                weather.CeilingDescription = "Couvert";
            }
            
            // Déterminer les turbulences selon le vent
            if (weather.WindSpeed < 10)
            {
                weather.Turbulence = TurbulenceLevel.None;
                weather.TurbulenceDescription = "Aucune turbulence";
            }
            else if (weather.WindSpeed < 20)
            {
                weather.Turbulence = TurbulenceLevel.Light;
                weather.TurbulenceDescription = "Turbulences légères";
            }
            else if (weather.WindSpeed < 30)
            {
                weather.Turbulence = TurbulenceLevel.Moderate;
                weather.TurbulenceDescription = "Turbulences modérées";
            }
            else
            {
                weather.Turbulence = TurbulenceLevel.Severe;
                weather.TurbulenceDescription = "Turbulences sévères";
            }
            
            // Déterminer les conditions de vol
            if (weather.Visibility > 5 && weather.Ceiling > 3000 && weather.WindSpeed < 20)
            {
                weather.FlightConditions = FlightConditions.VFR;
                weather.FlightConditionsDescription = "Conditions VFR (vol à vue)";
            }
            else if (weather.Visibility > 3 && weather.Ceiling > 1000)
            {
                weather.FlightConditions = FlightConditions.MVFR;
                weather.FlightConditionsDescription = "Conditions VFR marginales";
            }
            else if (weather.Visibility > 1 && weather.Ceiling > 500)
            {
                weather.FlightConditions = FlightConditions.IFR;
                weather.FlightConditionsDescription = "Conditions IFR (vol aux instruments)";
            }
            else
            {
                weather.FlightConditions = FlightConditions.LIFR;
                weather.FlightConditionsDescription = "Conditions IFR basses";
            }
        }

        /// <summary>
        /// Retourne une météo par défaut
        /// </summary>
        private WeatherData GetDefaultWeather(Airport airport)
        {
            var weather = new WeatherData
            {
                AirportIcao = airport.IcaoCode,
                AirportName = airport.Name,
                Timestamp = DateTime.Now,
                Temperature = 15,
                Pressure = 29.92,
                Humidity = 50,
                WindSpeed = 5,
                WindDirection = 270,
                Visibility = 10,
                CloudCoverage = 25,
                Ceiling = 5000,
                Condition = "Clear",
                Description = "Ciel dégagé",
                FlightConditions = FlightConditions.VFR
            };
            
            CalculateDerivedConditions(weather);
            
            return weather;
        }

        /// <summary>
        /// Génère un METAR textuel
        /// </summary>
        public string GenerateMETAR(WeatherData weather)
        {
            string time = DateTime.UtcNow.ToString("ddHHmm");
            string wind = $"{weather.WindDirection:000}{(int)weather.WindSpeed:00}KT";
            string visibility = $"{(int)(weather.Visibility * 1.852)}KM"; // Convertir en km
            string clouds = GetCloudsMETAR(weather.CloudCoverage, weather.Ceiling);
            string temp = $"{(int)weather.Temperature:00}/{CalculateDewPoint(weather.Temperature, weather.Humidity):00}";
            string pressure = $"Q{(int)(weather.Pressure / 0.02953):0000}";
            
            return $"{weather.AirportIcao} {time}Z {wind} {visibility} {clouds} {temp} {pressure}";
        }

        /// <summary>
        /// Obtient la partie nuages du METAR
        /// </summary>
        private string GetCloudsMETAR(int coverage, int ceiling)
        {
            if (coverage < 25)
                return "SKC"; // Sky Clear
            else if (coverage < 50)
                return $"FEW{ceiling / 100:000}";
            else if (coverage < 75)
                return $"SCT{ceiling / 100:000}";
            else
                return $"OVC{ceiling / 100:000}";
        }

        /// <summary>
        /// Calcule le point de rosée
        /// </summary>
        private int CalculateDewPoint(double temp, int humidity)
        {
            double a = 17.27;
            double b = 237.7;
            double alpha = ((a * temp) / (b + temp)) + Math.Log(humidity / 100.0);
            double dewPoint = (b * alpha) / (a - alpha);
            return (int)Math.Round(dewPoint);
        }

        /// <summary>
        /// Applique les effets météo sur le vol
        /// </summary>
        public void ApplyWeatherEffects(FlightState flightState, WeatherData weather)
        {
            if (weather == null) return;
            
            // Effet du vent
            ApplyWindEffect(flightState, weather);
            
            // Effet des turbulences
            ApplyTurbulenceEffect(flightState, weather);
            
            // Effet de la densité de l'air (température et pression)
            ApplyDensityEffect(flightState, weather);
        }

        /// <summary>
        /// Applique l'effet du vent
        /// </summary>
        private void ApplyWindEffect(FlightState flightState, WeatherData weather)
        {
            // Calculer la composante du vent
            double headingRad = flightState.Heading * Math.PI / 180.0;
            double windRad = weather.WindDirection * Math.PI / 180.0;
            
            double windAngle = windRad - headingRad;
            
            // Vent de face/arrière
            double headwind = weather.WindSpeed * Math.Cos(windAngle);
            flightState.GroundSpeed = flightState.Speed - headwind;
            
            // Vent de travers
            double crosswind = weather.WindSpeed * Math.Sin(windAngle);
            flightState.Drift = crosswind * 0.1; // Dérive
        }

        /// <summary>
        /// Applique l'effet des turbulences
        /// </summary>
        private void ApplyTurbulenceEffect(FlightState flightState, WeatherData weather)
        {
            Random random = new Random();
            
            double turbulenceIntensity = weather.Turbulence switch
            {
                TurbulenceLevel.None => 0,
                TurbulenceLevel.Light => 0.5,
                TurbulenceLevel.Moderate => 1.0,
                TurbulenceLevel.Severe => 2.0,
                _ => 0
            };
            
            if (turbulenceIntensity > 0)
            {
                // Variations aléatoires d'altitude et d'assiette
                flightState.Altitude += (random.NextDouble() - 0.5) * turbulenceIntensity * 10;
                flightState.Pitch += (random.NextDouble() - 0.5) * turbulenceIntensity * 2;
                flightState.Roll += (random.NextDouble() - 0.5) * turbulenceIntensity * 5;
            }
        }

        /// <summary>
        /// Applique l'effet de la densité de l'air
        /// </summary>
        private void ApplyDensityEffect(FlightState flightState, WeatherData weather)
        {
            // Température et pression affectent la densité de l'air
            // et donc les performances de l'avion
            
            // Température standard au niveau de la mer: 15°C
            double tempDeviation = weather.Temperature - 15;
            
            // Pression standard: 29.92 inHg
            double pressureDeviation = weather.Pressure - 29.92;
            
            // Facteur de densité (simplifié)
            double densityFactor = 1.0 - (tempDeviation * 0.01) - (pressureDeviation * 0.02);
            
            // Appliquer aux performances
            flightState.PerformanceFactor = densityFactor;
        }
    }

    /// <summary>
    /// Données météo
    /// </summary>
    public class WeatherData
    {
        public string AirportIcao { get; set; }
        public string AirportName { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Température
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        
        // Pression
        public double Pressure { get; set; }
        
        // Humidité
        public int Humidity { get; set; }
        
        // Vent
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public double WindGust { get; set; }
        
        // Visibilité
        public double Visibility { get; set; }
        
        // Nuages
        public int CloudCoverage { get; set; }
        public int Ceiling { get; set; }
        public string CeilingDescription { get; set; }
        
        // Conditions
        public string Condition { get; set; }
        public string Description { get; set; }
        
        // Turbulences
        public TurbulenceLevel Turbulence { get; set; }
        public string TurbulenceDescription { get; set; }
        
        // Conditions de vol
        public FlightConditions FlightConditions { get; set; }
        public string FlightConditionsDescription { get; set; }
    }

    /// <summary>
    /// Niveau de turbulence
    /// </summary>
    public enum TurbulenceLevel
    {
        None,
        Light,
        Moderate,
        Severe
    }

    /// <summary>
    /// Conditions de vol
    /// </summary>
    public enum FlightConditions
    {
        VFR,   // Visual Flight Rules
        MVFR,  // Marginal VFR
        IFR,   // Instrument Flight Rules
        LIFR   // Low IFR
    }

    #region Modèles API OpenWeather
    
    public class OpenWeatherResponse
    {
        [JsonProperty("weather")]
        public WeatherInfo[] Weather { get; set; }
        
        [JsonProperty("main")]
        public MainInfo Main { get; set; }
        
        [JsonProperty("wind")]
        public WindInfo Wind { get; set; }
        
        [JsonProperty("clouds")]
        public CloudsInfo Clouds { get; set; }
        
        [JsonProperty("visibility")]
        public int Visibility { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("main")]
        public string Main { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class MainInfo
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }
        
        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }
        
        [JsonProperty("pressure")]
        public int Pressure { get; set; }
        
        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    public class WindInfo
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
        
        [JsonProperty("deg")]
        public int Deg { get; set; }
        
        [JsonProperty("gust")]
        public double Gust { get; set; }
    }

    public class CloudsInfo
    {
        [JsonProperty("all")]
        public int All { get; set; }
    }
    
    #endregion
}

