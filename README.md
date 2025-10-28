# Simulateur de Vol Français

Un simulateur de vol accessible conçu spécialement pour les personnes aveugles et malvoyantes, avec un maximum de réalisme et une interface entièrement en français.

## Caractéristiques principales

### Accessibilité totale
- **Support complet des lecteurs d'écran** (NVDA, JAWS, Narrateur Windows)
- **Synthèse vocale française** intégrée pour toutes les annonces
- **Navigation 100% au clavier** sans besoin de souris
- **Annonces vocales contextuelles** pour chaque action
- **Interface optimisée** pour une utilisation sans vision

### Réalisme de simulation
- **Physique de vol réaliste** avec aérodynamique complète
- **Modèles d'avions détaillés** (Cessna 172, Airbus A320, Boeing 737)
- **Base de données d'aéroports** français et internationaux
- **Système de navigation GPS** avec calcul automatique de routes
- **Communications radio** réalistes avec la tour de contrôle
- **Météo dynamique** affectant le vol
- **Consommation de carburant** et gestion des systèmes

### Sons immersifs
- **Sons de moteurs** spécifiques à chaque avion
- **Ambiance cockpit** réaliste
- **Alertes sonores** pour tous les systèmes
- **Communications radio** avec effets audio
- **Sons d'environnement** (vent, turbulences, etc.)

## Configuration requise

### Système
- **OS**: Windows 10 ou supérieur (64 bits)
- **Processeur**: Intel Core i3 ou équivalent
- **RAM**: 4 GB minimum, 8 GB recommandé
- **Espace disque**: 500 MB
- **Carte son**: Carte son compatible Windows

### Logiciels
- **.NET 8.0 Runtime** (inclus dans l'installateur)
- **Lecteur d'écran** (NVDA recommandé, gratuit)

## Installation

### Méthode 1: Installateur automatique (recommandé)
1. Téléchargez `SimulateurVolFR_Setup.exe`
2. Exécutez l'installateur
3. Suivez les instructions vocales
4. Le simulateur se lancera automatiquement après l'installation

### Méthode 2: Compilation depuis les sources
1. Installez .NET 8.0 SDK
2. Clonez le dépôt GitHub
3. Ouvrez un terminal dans le dossier `src`
4. Exécutez: `dotnet build -c Release`
5. L'exécutable sera dans `bin/Release/net8.0-windows/`

## Guide de démarrage rapide

### Premier lancement
1. Lancez l'application (elle annoncera "Bienvenue dans le simulateur de vol français")
2. Appuyez sur **Alt** pour accéder au menu
3. Sélectionnez **Fichier > Nouveau vol** (ou appuyez sur **F2**)

### Créer un vol
1. Sélectionnez un **avion** dans la liste
2. Choisissez un **aéroport de départ**
3. Choisissez un **aéroport de destination** (optionnel)
4. Confirmez pour initialiser le vol

### Démarrer les moteurs
1. Ouvrez le menu **Vol > Démarrer moteurs**
2. Ou utilisez le raccourci clavier **Ctrl+E**
3. Attendez l'annonce "Moteurs démarrés"

### Décoller
1. Augmentez les gaz avec la touche **+** (pavé numérique)
2. Attendez d'atteindre la vitesse de décollage
3. Tirez doucement sur le manche avec **Flèche Bas**
4. L'avion décollera automatiquement

### Piloter
- **Flèches Haut/Bas**: Contrôle du tangage (monter/descendre)
- **Flèches Gauche/Droite**: Contrôle du roulis (tourner)
- **+/-** (pavé numérique): Augmenter/diminuer les gaz
- **G**: Sortir/rentrer le train d'atterrissage
- **F**: Régler les volets
- **B**: Freins

### Obtenir des informations
- **I**: Informations de vol actuelles
- **N**: État de la navigation
- **R**: Ouvrir la radio
- **F1**: Aide contextuelle

## Raccourcis clavier complets

### Menu et navigation
- **Alt**: Accéder au menu principal
- **F1**: Aide
- **F2**: Nouveau vol
- **F3**: Charger un vol
- **F4**: Sauvegarder le vol
- **Échap**: Quitter (avec confirmation)

### Contrôles de vol
- **Flèche Haut**: Piquer (descendre le nez)
- **Flèche Bas**: Cabrer (lever le nez)
- **Flèche Gauche**: Incliner à gauche
- **Flèche Droite**: Incliner à droite
- **+** (pavé numérique): Augmenter les gaz
- **-** (pavé numérique): Diminuer les gaz
- **0** (pavé numérique): Gaz au ralenti
- **Entrée** (pavé numérique): Plein gaz

### Systèmes
- **Ctrl+E**: Démarrer/arrêter les moteurs
- **G**: Train d'atterrissage
- **F**: Volets (cycle entre les positions)
- **B**: Freins
- **A**: Pilote automatique
- **Ctrl+A**: Atterrissage automatique

### Informations
- **I**: Informations de vol
- **N**: Navigation
- **R**: Radio
- **M**: Météo
- **Ctrl+I**: Instruments détaillés

### Radio
- **Ctrl+R**: Ouvrir le panneau radio
- **1**: Demander autorisation de démarrage
- **2**: Demander autorisation de roulage
- **3**: Demander autorisation de décollage
- **4**: Demander autorisation d'atterrissage
- **5**: Écouter l'ATIS

## Aéroports disponibles

Le simulateur inclut les principaux aéroports français :

- **LFPG** - Paris Charles de Gaulle
- **LFPO** - Paris Orly
- **LFML** - Marseille Provence
- **LFLL** - Lyon Saint-Exupéry
- **LFBD** - Bordeaux Mérignac

D'autres aéroports peuvent être ajoutés via la base de données.

## Avions disponibles

### Cessna 172 Skyhawk
Avion léger idéal pour l'apprentissage. Vitesse de croisière: 122 nœuds.

### Airbus A320
Avion de ligne moyen-courrier. Vitesse de croisière: 447 nœuds. Capacité: 180 passagers.

### Boeing 737-800
Avion de ligne moyen-courrier. Vitesse de croisière: 453 nœuds. Capacité: 189 passagers.

## Conseils pour une meilleure expérience

### Audio
- Utilisez un **casque audio** pour une immersion maximale
- Réglez le **volume** à un niveau confortable
- Les **sons 3D** vous aideront à vous orienter

### Lecteur d'écran
- **NVDA** est recommandé (gratuit et open source)
- Configurez la **vitesse de parole** selon vos préférences
- Activez les **annonces de progression** dans NVDA

### Apprentissage
- Commencez avec le **Cessna 172** pour apprendre les bases
- Utilisez le **pilote automatique** (touche A) pour vous familiariser
- Écoutez attentivement les **annonces vocales**
- Consultez l'**aide contextuelle** (F1) en cas de doute

### Vol réaliste
- Respectez les **procédures radio** pour plus de réalisme
- Suivez le **plan de vol** suggéré par la navigation
- Gérez votre **carburant** sur les longs vols
- Adaptez votre vol à la **météo**

## Dépannage

### Le lecteur d'écran ne fonctionne pas
- Vérifiez que NVDA est bien lancé avant le simulateur
- Redémarrez le simulateur
- Vérifiez les paramètres d'accessibilité Windows

### Pas de son
- Vérifiez que votre carte son est bien configurée
- Vérifiez le volume dans les paramètres audio du simulateur
- Redémarrez l'application

### L'avion ne répond pas aux commandes
- Vérifiez que les moteurs sont démarrés
- Vérifiez que vous avez suffisamment de vitesse
- Vérifiez que le pilote automatique n'est pas activé

### Performances lentes
- Fermez les autres applications
- Réduisez la qualité audio dans les paramètres
- Vérifiez que votre système répond aux exigences minimales

## Support et communauté

### Obtenir de l'aide
- Consultez la documentation complète dans le dossier `docs`
- Visitez le forum de la communauté
- Contactez le support technique

### Contribuer
Ce projet est open source. Les contributions sont les bienvenues :
- Rapporter des bugs
- Suggérer des améliorations
- Ajouter des traductions
- Contribuer au code

## Licence

Ce logiciel est distribué sous licence MIT. Voir le fichier LICENSE pour plus de détails.

## Crédits

Développé avec passion pour offrir une expérience de vol accessible à tous.

**Remerciements spéciaux** :
- À la communauté NVDA pour leur excellent lecteur d'écran
- Aux testeurs aveugles et malvoyants pour leurs retours précieux
- À tous les contributeurs du projet

## Version

**Version actuelle**: 1.0.0  
**Date de sortie**: Octobre 2025  
**Dernière mise à jour**: Octobre 2025

---

**Bon vol et profitez de l'expérience !** ✈️

