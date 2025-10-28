# Instructions de compilation

Ce document explique comment compiler le Simulateur de Vol Français depuis les sources.

## Prérequis

Pour compiler le projet, vous aurez besoin de :

### Sur Windows
- **.NET 8.0 SDK** - [Télécharger](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (optionnel mais recommandé) ou **Visual Studio Code**
- **Git** pour cloner le dépôt

### Sur Linux (pour le développement cross-platform)
- **.NET 8.0 SDK**
- **Mono** (pour certaines dépendances Windows)

## Cloner le dépôt

```bash
git clone https://github.com/votre-repo/SimulateurVolFR.git
cd SimulateurVolFR
```

## Compilation

### Méthode 1: Ligne de commande

#### Debug (développement)
```bash
cd src
dotnet build
```

L'exécutable sera dans : `src/bin/Debug/net8.0-windows/SimulateurVolFR.exe`

#### Release (production)
```bash
cd src
dotnet build -c Release
```

L'exécutable sera dans : `src/bin/Release/net8.0-windows/SimulateurVolFR.exe`

### Méthode 2: Visual Studio

1. Ouvrez `src/SimulateurVolFR.csproj` dans Visual Studio 2022
2. Sélectionnez la configuration (Debug ou Release)
3. Appuyez sur F6 ou allez dans Build > Build Solution
4. L'exécutable sera dans le dossier bin correspondant

### Méthode 3: Visual Studio Code

1. Ouvrez le dossier du projet dans VS Code
2. Installez l'extension C# si ce n'est pas déjà fait
3. Appuyez sur Ctrl+Shift+B pour compiler
4. Ou utilisez le terminal intégré avec les commandes dotnet ci-dessus

## Génération des sons

Avant la première compilation, générez les sons de base :

```bash
cd build
python3 generate_sounds.py
```

Cela créera les fichiers audio dans `assets/audio/`.

**Note**: Ces sons sont synthétiques. Pour un résultat optimal, remplacez-les par des enregistrements réels.

## Publication

Pour créer une version distribuable :

### Publication autonome (avec runtime inclus)

```bash
cd src
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Cela créera un exécutable unique dans :
`src/bin/Release/net8.0-windows/win-x64/publish/`

### Publication dépendante du framework (plus petit)

```bash
cd src
dotnet publish -c Release -r win-x64 --self-contained false
```

L'utilisateur devra avoir .NET 8.0 Runtime installé.

## Création d'un installateur

Pour créer un installateur Windows :

### Avec Inno Setup (recommandé)

1. Installez [Inno Setup](https://jrsoftware.org/isinfo.php)
2. Ouvrez `build/installer.iss`
3. Compilez le script
4. L'installateur sera créé dans `build/Output/`

### Avec WiX Toolset

1. Installez [WiX Toolset](https://wixtoolset.org/)
2. Utilisez le script `build/installer.wxs`
3. Compilez avec `candle` et `light`

## Tests

### Exécuter les tests unitaires

```bash
cd tests
dotnet test
```

### Tests d'accessibilité

1. Installez NVDA
2. Lancez NVDA
3. Lancez le simulateur
4. Vérifiez que toutes les annonces vocales fonctionnent

## Structure du projet

```
SimulateurVolFR/
├── src/                    # Code source
│   ├── Core/              # Logique principale
│   ├── Models/            # Modèles de données
│   ├── UI/                # Interface utilisateur
│   ├── Data/              # Accès aux données
│   └── SimulateurVolFR.csproj
├── assets/                # Ressources
│   ├── audio/            # Fichiers audio
│   └── data/             # Base de données
├── docs/                  # Documentation
├── build/                 # Scripts de build
└── tests/                 # Tests unitaires
```

## Dépendances

Le projet utilise les packages NuGet suivants :

- **NAudio** (2.2.1) - Gestion audio
- **System.Data.SQLite** (1.0.118) - Base de données
- **Newtonsoft.Json** (13.0.3) - Sérialisation JSON

Ces packages sont automatiquement téléchargés lors de la compilation.

## Dépendances natives

### nvdaControllerClient

Pour le support NVDA, vous aurez besoin de :
- `nvdaControllerClient64.dll` (pour Windows 64 bits)
- `nvdaControllerClient32.dll` (pour Windows 32 bits)

Ces DLL doivent être placées dans le dossier de sortie. Elles sont disponibles sur le [site de NVDA](https://www.nvaccess.org/files/nvda/documentation/developerGuide.html).

## Problèmes courants

### Erreur: SDK not found

**Solution**: Installez .NET 8.0 SDK depuis https://dotnet.microsoft.com/download

### Erreur: NAudio not found

**Solution**: Restaurez les packages NuGet :
```bash
dotnet restore
```

### Erreur: Cannot find nvdaControllerClient64.dll

**Solution**: Téléchargez les DLL NVDA et placez-les dans le dossier de sortie.

### Sons manquants

**Solution**: Exécutez le script de génération de sons :
```bash
cd build
python3 generate_sounds.py
```

## Contribution

Pour contribuer au projet :

1. Forkez le dépôt
2. Créez une branche pour votre fonctionnalité
3. Committez vos changements
4. Poussez vers votre fork
5. Créez une Pull Request

### Standards de code

- Utilisez les conventions C# standard
- Commentez le code en français
- Ajoutez des tests pour les nouvelles fonctionnalités
- Assurez-vous que l'accessibilité est maintenue

## Licence

Ce projet est sous licence MIT. Voir le fichier LICENSE pour plus de détails.

## Support

Pour toute question sur la compilation :
- Ouvrez une issue sur GitHub
- Consultez la documentation
- Contactez l'équipe de développement

---

Bon développement ! 🚀

