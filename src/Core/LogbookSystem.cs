using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SimulateurVolFR.Models;

namespace SimulateurVolFR.Core
{
    /// <summary>
    /// Système de carnet de vol électronique
    /// Enregistrement automatique et statistiques détaillées
    /// </summary>
    public class LogbookSystem
    {
        private AccessibilityManager accessibilityManager;
        private List<LogbookEntry> entries;
        private string pilotName;
        private string pilotLicense;

        public List<LogbookEntry> Entries => entries;

        public LogbookSystem(AccessibilityManager accessibilityManager)
        {
            this.accessibilityManager = accessibilityManager;
            this.entries = new List<LogbookEntry>();
        }

        /// <summary>
        /// Initialise le carnet de vol
        /// </summary>
        public void Initialize(string pilotName, string pilotLicense)
        {
            this.pilotName = pilotName;
            this.pilotLicense = pilotLicense;
            
            accessibilityManager.Speak($"Carnet de vol initialisé pour {pilotName}, licence {pilotLicense}.");
        }

        /// <summary>
        /// Enregistre un vol
        /// </summary>
        public void RecordFlight(FlightData flightData)
        {
            var entry = new LogbookEntry
            {
                Date = DateTime.Now,
                AircraftType = flightData.Aircraft.Type,
                AircraftRegistration = flightData.AircraftRegistration,
                DepartureAirport = flightData.DepartureAirport.ICAOCode,
                ArrivalAirport = flightData.ArrivalAirport.ICAOCode,
                DepartureTime = flightData.DepartureTime,
                ArrivalTime = flightData.ArrivalTime,
                FlightTime = (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours,
                DayTime = flightData.IsDayFlight ? (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours : 0,
                NightTime = !flightData.IsDayFlight ? (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours : 0,
                IFRTime = flightData.IsIFR ? (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours : 0,
                PICTime = flightData.IsPIC ? (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours : 0,
                DualTime = flightData.IsDual ? (float)(flightData.ArrivalTime - flightData.DepartureTime).TotalHours : 0,
                Landings = flightData.Landings,
                NightLandings = flightData.NightLandings,
                InstructorName = flightData.InstructorName,
                InstructorSignature = flightData.InstructorSignature,
                Remarks = flightData.Remarks,
                FlightType = flightData.FlightType
            };

            entries.Add(entry);

            accessibilityManager.Speak($"Vol enregistré : {entry.DepartureAirport} vers {entry.ArrivalAirport}, " +
                $"durée {entry.FlightTime:F1} heures.");
        }

        /// <summary>
        /// Obtient les statistiques totales
        /// </summary>
        public LogbookStatistics GetStatistics()
        {
            return new LogbookStatistics
            {
                TotalFlightTime = entries.Sum(e => e.FlightTime),
                TotalDayTime = entries.Sum(e => e.DayTime),
                TotalNightTime = entries.Sum(e => e.NightTime),
                TotalIFRTime = entries.Sum(e => e.IFRTime),
                TotalPICTime = entries.Sum(e => e.PICTime),
                TotalDualTime = entries.Sum(e => e.DualTime),
                TotalLandings = entries.Sum(e => e.Landings),
                TotalNightLandings = entries.Sum(e => e.NightLandings),
                TotalFlights = entries.Count,
                AircraftTypesFlown = entries.Select(e => e.AircraftType).Distinct().Count(),
                AirportsVisited = entries.SelectMany(e => new[] { e.DepartureAirport, e.ArrivalAirport }).Distinct().Count()
            };
        }

        /// <summary>
        /// Obtient les heures par type d'appareil
        /// </summary>
        public Dictionary<string, float> GetHoursByAircraftType()
        {
            return entries.GroupBy(e => e.AircraftType)
                         .ToDictionary(g => g.Key, g => g.Sum(e => e.FlightTime));
        }

        /// <summary>
        /// Exporte le carnet en PDF
        /// </summary>
        public string ExportToPDF(string filePath)
        {
            // Implémentation simplifiée - dans une vraie version, utiliser une bibliothèque PDF
            var content = GenerateLogbookContent();
            File.WriteAllText(filePath.Replace(".pdf", ".txt"), content);
            
            accessibilityManager.Speak($"Carnet de vol exporté vers {filePath}");
            return filePath;
        }

        private string GenerateLogbookContent()
        {
            var content = $"CARNET DE VOL\n\n";
            content += $"Pilote : {pilotName}\n";
            content += $"Licence : {pilotLicense}\n\n";
            
            var stats = GetStatistics();
            content += $"TOTAUX\n";
            content += $"Heures de vol : {stats.TotalFlightTime:F1}h\n";
            content += $"Jour : {stats.TotalDayTime:F1}h | Nuit : {stats.TotalNightTime:F1}h\n";
            content += $"IFR : {stats.TotalIFRTime:F1}h | PIC : {stats.TotalPICTime:F1}h\n";
            content += $"Atterrissages : {stats.TotalLandings} (dont {stats.TotalNightLandings} de nuit)\n\n";
            
            content += $"VOLS\n";
            foreach (var entry in entries.OrderByDescending(e => e.Date))
            {
                content += $"{entry.Date:dd/MM/yyyy} | {entry.AircraftType} {entry.AircraftRegistration} | " +
                          $"{entry.DepartureAirport}-{entry.ArrivalAirport} | {entry.FlightTime:F1}h\n";
            }
            
            return content;
        }
    }

    public class LogbookEntry
    {
        public DateTime Date { get; set; }
        public string AircraftType { get; set; }
        public string AircraftRegistration { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public float FlightTime { get; set; }
        public float DayTime { get; set; }
        public float NightTime { get; set; }
        public float IFRTime { get; set; }
        public float PICTime { get; set; }
        public float DualTime { get; set; }
        public int Landings { get; set; }
        public int NightLandings { get; set; }
        public string InstructorName { get; set; }
        public string InstructorSignature { get; set; }
        public string Remarks { get; set; }
        public FlightType FlightType { get; set; }
    }

    public class FlightData
    {
        public Aircraft Aircraft { get; set; }
        public string AircraftRegistration { get; set; }
        public Airport DepartureAirport { get; set; }
        public Airport ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public bool IsDayFlight { get; set; }
        public bool IsIFR { get; set; }
        public bool IsPIC { get; set; }
        public bool IsDual { get; set; }
        public int Landings { get; set; }
        public int NightLandings { get; set; }
        public string InstructorName { get; set; }
        public string InstructorSignature { get; set; }
        public string Remarks { get; set; }
        public FlightType FlightType { get; set; }
    }

    public enum FlightType
    {
        Training,
        Solo,
        Commercial,
        Private,
        Cargo,
        Emergency
    }

    public class LogbookStatistics
    {
        public float TotalFlightTime { get; set; }
        public float TotalDayTime { get; set; }
        public float TotalNightTime { get; set; }
        public float TotalIFRTime { get; set; }
        public float TotalPICTime { get; set; }
        public float TotalDualTime { get; set; }
        public int TotalLandings { get; set; }
        public int TotalNightLandings { get; set; }
        public int TotalFlights { get; set; }
        public int AircraftTypesFlown { get; set; }
        public int AirportsVisited { get; set; }
    }
}

