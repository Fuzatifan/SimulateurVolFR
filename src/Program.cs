using System;
using System.Windows.Forms;
using SimulateurVolFR.UI;
using SimulateurVolFR.Core;

namespace SimulateurVolFR
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Initialiser le système d'accessibilité
            AccessibilityManager.Initialize();
            
            // Initialiser le système audio
            AudioManager.Initialize();
            
            // Lancer l'application
            Application.Run(new MainForm());
            
            // Nettoyage
            AudioManager.Shutdown();
            AccessibilityManager.Shutdown();
        }
    }
}

