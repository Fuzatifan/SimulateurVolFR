# 🛩️ Simulateur de Vol Français

**Simulateur de vol accessible en français avec intelligence artificielle, réalité virtuelle, école de pilotage et gestion de compagnie aérienne**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-11.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 🌟 Caractéristiques principales

### ♿ 100% Accessible
- **Support complet des lecteurs d'écran** (NVDA, JAWS)
- **Synthèse vocale française** intégrée
- **Navigation 100% au clavier**
- **Audio spatial 3D** pour l'orientation
- **Descriptions vocales** de tous les éléments

### 🤖 Intelligence Artificielle
- **Copilote virtuel** alimenté par GPT-4
- **Conversations naturelles** en français
- **4 personnalités** (Professionnel, Sympathique, Instructeur, Humoristique)
- **Assistance contextuelle** en temps réel
- **Check-lists vocales** automatiques

### 🥽 Réalité Virtuelle
- **Support VR complet** (Meta Quest, HTC Vive, Valve Index)
- **Cockpit 3D immersif** à échelle 1:1
- **Suivi des mains** (Hand Tracking)
- **Retour haptique** réaliste
- **Audio spatial 3D** positionné

### 🎓 École de Pilotage
- **15 leçons structurées** pour la licence PPL
- **Instructeur virtuel exigeant** avec feedback en temps réel
- **Système d'évaluation rigoureux** (notes, briefings, débriefings)
- **3 niveaux de licence** (PPL, CPL, ATPL)
- **Certificats officiels** avec numéros de licence

### ✈️ Gestion de Compagnie Aérienne
- **Créez votre compagnie** avec nom et code IATA
- **Achetez et vendez des avions** (300k€ à 450M€)
- **Programmez des vols** avec passagers
- **Économie réaliste** (revenus, coûts, profits)
- **Système de maintenance** et réputation
- **Statistiques complètes** de gestion

### 📻 Communications Radio Réalistes
- **Phraséologie OACI standard** complète
- **4 fréquences** (ATIS, Ground, Tower, Approach)
- **Génération automatique d'ATIS** avec météo
- **Voix ATC distincte** professionnelle
- **9 sons radio** (bip, squelch, interférences)

### 🌦️ Météo Dynamique
- **Intégration API OpenWeatherMap** en temps réel
- **Génération de METAR** standard
- **Impact réel sur le vol** (vent, turbulences, densité)
- **Conditions VFR/IFR** calculées automatiquement

### 🎯 Système de Missions
- **13 missions complètes** avec objectifs
- **5 types** (Apprentissage, Transport, Tourisme, Précision, Météo)
- **Notation sur 100 points** avec bonus
- **Validation automatique** des objectifs

## 📊 Contenu

### Appareils (42 au total)
- **25 avions** (légers, jets d'affaires, régionaux, moyen-courriers, long-courriers, cargo)
- **17 hélicoptères** (légers, utilitaires, moyens, lourds, militaires)

### Sites (100 au total)
- **62 aéroports internationaux** (6 continents, 35 pays)
- **38 aérodromes** (France, Europe, DOM-TOM)

### Audio
- **32 sons réalistes** (23 moteurs + 9 radio)
- **Synthèse vocale** française de haute qualité
- **Audio spatial 3D** immersif

## 🚀 Installation

### Prérequis
- **Windows 10/11** (64-bit)
- **.NET 8.0 SDK** ou supérieur
- **4 GB RAM** minimum (8 GB recommandé)
- **1 GB espace disque**
- **Carte son** compatible

### Optionnel
- **Casque VR** (Meta Quest, HTC Vive, Valve Index)
- **Clé API OpenAI** (pour copilote IA)
- **Clé API OpenWeatherMap** (pour météo temps réel)

### Compilation

```bash
# Cloner le dépôt
git clone https://github.com/Fuzatifan/SimulateurVolFR.git
cd SimulateurVolFR

# Restaurer les dépendances
dotnet restore src/SimulateurVolFR.csproj

# Compiler
dotnet build src/SimulateurVolFR.csproj -c Release

# Exécuter
dotnet run --project src/SimulateurVolFR.csproj
```

## 📖 Documentation

- **[README.md](README.md)** - Vue d'ensemble et guide utilisateur
- **[INSTALLATION.txt](docs/INSTALLATION.txt)** - Guide d'installation détaillé
- **[GUIDE_RADIO.md](docs/GUIDE_RADIO.md)** - Guide des communications radio
- **[GUIDE_COPILOTE.md](docs/GUIDE_COPILOTE.md)** - Guide du copilote virtuel
- **[AMELIORATIONS_RECOMMANDEES.md](docs/AMELIORATIONS_RECOMMANDEES.md)** - Feuille de route

### Livraisons par version
- **[v3.0](docs/LIVRAISON_SimulateurVolFR_v3.0.md)** - Sons, tutoriel, météo, missions
- **[v3.1](docs/LIVRAISON_SimulateurVolFR_v3.1.md)** - Communications radio
- **[v3.2](docs/LIVRAISON_SimulateurVolFR_v3.2.md)** - Copilote virtuel IA
- **[v3.3](docs/LIVRAISON_SimulateurVolFR_v3.3.md)** - Mode VR + Compagnie aérienne
- **[v3.4](docs/LIVRAISON_SimulateurVolFR_v3.4.md)** - École de pilotage

## 🎮 Utilisation

### Commandes de base
- **Flèches** : Contrôles de vol (tangage, roulis)
- **A/Z** : Manette des gaz
- **Q/D** : Palonnier (lacet)
- **G** : Train d'atterrissage
- **F** : Volets
- **Espace** : Freins

### Commandes avancées
- **C** : Parler au copilote
- **Ctrl+R** : Menu radio
- **A** : Écouter ATIS
- **M** : Menu missions
- **E** : Menu école de pilotage
- **L** : Menu compagnie aérienne
- **V** : Activer/désactiver VR

### Navigation
- **Tab** : Élément suivant
- **Shift+Tab** : Élément précédent
- **Entrée** : Activer
- **Échap** : Retour/Annuler

## 🏆 Fonctionnalités uniques

### Ce qui rend ce simulateur unique

1. **Premier simulateur de vol 100% accessible aux personnes aveugles**
   - Tout est vocalisé et navigable au clavier
   - Audio spatial 3D pour l'orientation
   - Descriptions détaillées de chaque élément

2. **Copilote IA conversationnel**
   - Comprend le langage naturel
   - Adapte sa personnalité
   - Surveille et assiste en temps réel

3. **VR accessible**
   - Descriptions vocales en VR
   - Interactions gestuelles
   - Utilisable sans voir

4. **École de pilotage complète**
   - Programme structuré professionnel
   - Instructeur exigeant
   - Licences officielles

5. **Mode compagnie aérienne**
   - Gestion économique réaliste
   - Progression à long terme
   - Stratégie et tactique

## 🛠️ Technologies utilisées

- **C# 11.0** - Langage principal
- **.NET 8.0** - Framework
- **Windows Forms** - Interface utilisateur
- **System.Speech** - Synthèse vocale
- **OpenXR** - Support VR
- **SQLite** - Base de données
- **OpenAI API** - Copilote IA
- **OpenWeatherMap API** - Météo temps réel

## 📈 Statistiques du projet

- **21 fichiers C#** (~8900 lignes)
- **6 guides complets** (~2000 lignes de documentation)
- **32 sons** (moteurs + radio)
- **100 sites** (aéroports + aérodromes)
- **42 appareils** (avions + hélicoptères)
- **15 leçons** (école de pilotage)
- **13 missions** (scénarios)

## 🗺️ Roadmap

### Version 3.5 (Prochaine)
- [ ] Leçons CPL complètes
- [ ] Leçons ATPL
- [ ] Certificats imprimables
- [ ] Carnet de vol électronique

### Version 4.0 (Future)
- [ ] Multijoueur
- [ ] Examens théoriques (QCM)
- [ ] Plus de routes (100+)
- [ ] Événements économiques

Voir [AMELIORATIONS_RECOMMANDEES.md](docs/AMELIORATIONS_RECOMMANDEES.md) pour la liste complète.

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à :
- Signaler des bugs
- Proposer des améliorations
- Soumettre des pull requests
- Améliorer la documentation

## 📝 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 👥 Auteur

**Développé par Manus AI** pour Fuzatifan

## 🙏 Remerciements

- **Eurofly 3** pour l'inspiration
- **Communauté de l'aviation accessible**
- **OpenAI** pour l'API GPT-4
- **OpenWeatherMap** pour l'API météo

## 📞 Contact

Pour toute question ou suggestion :
- **GitHub Issues** : [https://github.com/Fuzatifan/SimulateurVolFR/issues](https://github.com/Fuzatifan/SimulateurVolFR/issues)
- **Discussions** : [https://github.com/Fuzatifan/SimulateurVolFR/discussions](https://github.com/Fuzatifan/SimulateurVolFR/discussions)

---

**Bon vol ! 🛩️✈️🚁**

