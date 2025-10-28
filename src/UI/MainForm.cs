using System;
using System.Windows.Forms;
using SimulateurVolFR.Core;

namespace SimulateurVolFR.UI
{
    /// <summary>
    /// Formulaire principal du simulateur
    /// </summary>
    public partial class MainForm : Form
    {
        private MenuStrip mainMenu;
        private ToolStripMenuItem fileMenu, flightMenu, settingsMenu, helpMenu;
        private Panel mainPanel;
        private Label statusLabel;

        public MainForm()
        {
            InitializeComponent();
            SetupAccessibility();
        }

        private void InitializeComponent()
        {
            // Configuration de base du formulaire
            this.Text = "Simulateur de Vol Français";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            // Créer le menu principal
            mainMenu = new MenuStrip();
            
            // Menu Fichier
            fileMenu = new ToolStripMenuItem("&Fichier");
            fileMenu.DropDownItems.Add("&Nouveau vol", null, NewFlight_Click);
            fileMenu.DropDownItems.Add("&Charger vol", null, LoadFlight_Click);
            fileMenu.DropDownItems.Add("&Sauvegarder vol", null, SaveFlight_Click);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("&Quitter", null, Exit_Click);

            // Menu Vol
            flightMenu = new ToolStripMenuItem("&Vol");
            flightMenu.DropDownItems.Add("&Démarrer moteurs", null, StartEngines_Click);
            flightMenu.DropDownItems.Add("&Arrêter moteurs", null, StopEngines_Click);
            flightMenu.DropDownItems.Add(new ToolStripSeparator());
            flightMenu.DropDownItems.Add("&Décollage automatique", null, AutoTakeoff_Click);
            flightMenu.DropDownItems.Add("&Atterrissage automatique", null, AutoLanding_Click);

            // Menu Paramètres
            settingsMenu = new ToolStripMenuItem("&Paramètres");
            settingsMenu.DropDownItems.Add("&Audio", null, AudioSettings_Click);
            settingsMenu.DropDownItems.Add("&Contrôles", null, ControlSettings_Click);
            settingsMenu.DropDownItems.Add("&Accessibilité", null, AccessibilitySettings_Click);

            // Menu Aide
            helpMenu = new ToolStripMenuItem("&Aide");
            helpMenu.DropDownItems.Add("&Guide de démarrage", null, QuickStart_Click);
            helpMenu.DropDownItems.Add("&Raccourcis clavier", null, KeyboardShortcuts_Click);
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("À &propos", null, About_Click);

            mainMenu.Items.Add(fileMenu);
            mainMenu.Items.Add(flightMenu);
            mainMenu.Items.Add(settingsMenu);
            mainMenu.Items.Add(helpMenu);

            // Panel principal
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30)
            };

            // Label de statut
            statusLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                Text = "Prêt",
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.FromArgb(50, 50, 50),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Ajouter les contrôles
            this.Controls.Add(mainPanel);
            this.Controls.Add(statusLabel);
            this.Controls.Add(mainMenu);
            this.MainMenuStrip = mainMenu;

            // Événements
            this.Load += MainForm_Load;
            this.KeyDown += MainForm_KeyDown;
            this.FormClosing += MainForm_FormClosing;
        }

        private void SetupAccessibility()
        {
            // Configurer les noms accessibles pour les lecteurs d'écran
            this.AccessibleName = "Fenêtre principale du simulateur de vol";
            this.AccessibleDescription = "Utilisez Alt pour accéder au menu, ou F1 pour l'aide";
            
            mainMenu.AccessibleName = "Menu principal";
            mainPanel.AccessibleName = "Zone de simulation";
            statusLabel.AccessibleName = "Barre de statut";
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Bienvenue dans le simulateur de vol français. Appuyez sur Alt pour accéder au menu, ou F1 pour l'aide.");
            UpdateStatus("Simulateur prêt. Appuyez sur F1 pour l'aide.");
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    ShowHelp();
                    e.Handled = true;
                    break;
                case Keys.F2:
                    NewFlight_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    if (MessageBox.Show("Voulez-vous vraiment quitter ?", "Confirmation", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Close();
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            AccessibilityManager.Speak("Fermeture du simulateur");
        }

        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
            AccessibilityManager.Speak(message);
        }

        // Gestionnaires d'événements du menu
        private void NewFlight_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Nouveau vol");
            UpdateStatus("Préparation d'un nouveau vol...");
            // TODO: Implémenter la création d'un nouveau vol
        }

        private void LoadFlight_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Charger un vol");
            UpdateStatus("Chargement d'un vol...");
            // TODO: Implémenter le chargement d'un vol
        }

        private void SaveFlight_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Sauvegarder le vol");
            UpdateStatus("Sauvegarde du vol...");
            // TODO: Implémenter la sauvegarde d'un vol
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void StartEngines_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Démarrage des moteurs");
            UpdateStatus("Moteurs en cours de démarrage...");
            // TODO: Implémenter le démarrage des moteurs
        }

        private void StopEngines_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Arrêt des moteurs");
            UpdateStatus("Moteurs en cours d'arrêt...");
            // TODO: Implémenter l'arrêt des moteurs
        }

        private void AutoTakeoff_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Décollage automatique");
            UpdateStatus("Préparation au décollage...");
            // TODO: Implémenter le décollage automatique
        }

        private void AutoLanding_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Atterrissage automatique");
            UpdateStatus("Préparation à l'atterrissage...");
            // TODO: Implémenter l'atterrissage automatique
        }

        private void AudioSettings_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Paramètres audio");
            UpdateStatus("Ouverture des paramètres audio...");
            // TODO: Ouvrir les paramètres audio
        }

        private void ControlSettings_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Paramètres des contrôles");
            UpdateStatus("Ouverture des paramètres des contrôles...");
            // TODO: Ouvrir les paramètres des contrôles
        }

        private void AccessibilitySettings_Click(object? sender, EventArgs e)
        {
            AccessibilityManager.Speak("Paramètres d'accessibilité");
            UpdateStatus("Ouverture des paramètres d'accessibilité...");
            // TODO: Ouvrir les paramètres d'accessibilité
        }

        private void QuickStart_Click(object? sender, EventArgs e)
        {
            ShowHelp();
        }

        private void KeyboardShortcuts_Click(object? sender, EventArgs e)
        {
            string shortcuts = @"Raccourcis clavier principaux:

F1 - Aide
F2 - Nouveau vol
Alt - Accéder au menu
Échap - Quitter

Contrôles de vol (à venir):
Flèches - Contrôler l'avion
+ / - - Augmenter/diminuer la puissance
Espace - Freins";

            MessageBox.Show(shortcuts, "Raccourcis clavier", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AccessibilityManager.Speak("Raccourcis clavier affichés");
        }

        private void About_Click(object? sender, EventArgs e)
        {
            string about = @"Simulateur de Vol Français
Version 1.0.0

Un simulateur de vol accessible conçu pour les personnes aveugles et malvoyantes.

Développé avec passion pour offrir une expérience de vol réaliste et immersive.

© 2025 - Tous droits réservés";

            MessageBox.Show(about, "À propos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AccessibilityManager.Speak("À propos du simulateur");
        }

        private void ShowHelp()
        {
            string help = @"Guide de démarrage rapide:

1. Appuyez sur F2 ou utilisez le menu Fichier > Nouveau vol
2. Sélectionnez un avion et un aéroport de départ
3. Utilisez le menu Vol pour démarrer les moteurs
4. Utilisez les contrôles pour piloter l'avion

Pour une expérience optimale:
- Utilisez un casque audio pour l'immersion
- Activez votre lecteur d'écran (NVDA recommandé)
- Consultez les raccourcis clavier avec Ctrl+K

Appuyez sur F1 à tout moment pour obtenir de l'aide contextuelle.";

            MessageBox.Show(help, "Guide de démarrage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AccessibilityManager.Speak("Guide de démarrage affiché");
        }
    }
}

