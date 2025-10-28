# Guide du Système de Communication Radio

## 🎙️ Vue d'ensemble

Le système de communication radio réaliste simule les communications aéronautiques conformes aux standards OACI (Organisation de l'Aviation Civile Internationale). Toutes les communications utilisent la phraséologie standard française.

## 📻 Fréquences radio

### Types de fréquences

Chaque aéroport dispose de plusieurs fréquences selon les services :

#### ATIS (Automatic Terminal Information Service)
- **Fréquence** : 126-128 MHz
- **Usage** : Information météo et aéroport automatique
- **Écoute** : Avant le contact avec l'ATC
- **Contenu** : Météo, piste active, QNH, consignes

#### Ground (Sol)
- **Fréquence** : 121.6-121.9 MHz
- **Usage** : Roulage au sol
- **Contact** : Après démarrage moteur et avant atterrissage
- **Autorisations** : Roulage vers piste, roulage vers parking

#### Tower (Tour)
- **Fréquence** : 118-119 MHz
- **Usage** : Décollage et atterrissage
- **Contact** : Au point d'arrêt et en finale
- **Autorisations** : Décollage, atterrissage, tour de piste

#### Approach/Departure (Approche/Départ)
- **Fréquence** : 119-120 MHz
- **Usage** : Approche et départ de la zone aéroportuaire
- **Contact** : Après décollage et avant approche
- **Services** : Guidage radar, séparation du trafic

## 🗣️ Phraséologie standard

### Structure d'un message

Tous les messages radio suivent cette structure :
1. **Qui on appelle** (contrôleur ou indicatif)
2. **Qui on est** (notre indicatif)
3. **Où on est** (position)
4. **Ce qu'on veut** (demande)

### Indicatifs d'appel

#### Aviation générale
Format : **F-XXXX** prononcé phonétiquement
- Exemple : "Foxtrot Alpha Bravo Charlie Delta"
- Abrégé après premier contact : "Foxtrot Charlie Delta"

#### Compagnies aériennes
Format : **Compagnie + Numéro de vol**
- Exemples : 
  - "Air France 1234"
  - "Transavia 7890"
  - "EasyJet 456"

### Alphabet phonétique OACI

| Lettre | Phonétique | Lettre | Phonétique |
|--------|-----------|--------|-----------|
| A | Alpha | N | November |
| B | Bravo | O | Oscar |
| C | Charlie | P | Papa |
| D | Delta | Q | Quebec |
| E | Echo | R | Romeo |
| F | Foxtrot | S | Sierra |
| G | Golf | T | Tango |
| H | Hotel | U | Uniform |
| I | India | V | Victor |
| J | Juliett | W | Whiskey |
| K | Kilo | X | X-ray |
| L | Lima | Y | Yankee |
| M | Mike | Z | Zulu |

### Chiffres phonétiques

| Chiffre | Prononciation |
|---------|---------------|
| 0 | Zéro |
| 1 | Un |
| 2 | Deux |
| 3 | Trois |
| 4 | Quatre |
| 5 | Cinq |
| 6 | Six |
| 7 | Sept |
| 8 | Huit |
| 9 | Neuf |
| . | Décimal |

## 📋 Procédures complètes

### Phase 1 : Pré-vol (au parking)

#### 1.1 Écouter l'ATIS
**Touche** : `A`

**Contenu typique** :
```
"Paris-Orly information Alpha. Heure 1430 Zulu. 
Piste en service 08. Vent 270 degrés 12 nœuds, rafales 18.
Visibilité 10 kilomètres. Nuages épars à 3000 pieds.
Température 18 degrés. Point de rosée 12 degrés.
QNH 1013. Rappelez information Alpha au contact."
```

**Points à noter** :
- Lettre d'information (Alpha, Bravo, etc.)
- Piste active
- Vent (direction et vitesse)
- QNH (pression atmosphérique)

#### 1.2 Demander autorisation de démarrage
**Touche** : `Ctrl+R` puis `1`

**Vous** :
```
"Orly Tour, bonjour, Foxtrot Alpha Bravo Charlie,
au parking, information Alpha,
demande autorisation de démarrage."
```

**ATC** :
```
"Foxtrot Alpha Bravo Charlie, Orly Tour, bonjour,
autorisé démarrage moteur, QNH 1013,
rappelez prêt à rouler."
```

**Vous** :
```
"Autorisé démarrage, QNH 1013, rappellerai prêt,
Foxtrot Charlie."
```

### Phase 2 : Roulage vers la piste

#### 2.1 Demander autorisation de rouler
**Touche** : `Ctrl+R` puis `2`

**Changement de fréquence** : Sol (Ground)

**Vous** :
```
"Orly Sol, Foxtrot Charlie,
prêt à rouler, piste 08."
```

**ATC** :
```
"Foxtrot Charlie, autorisé rouler point d'arrêt piste 08,
via Alpha, QNH 1013."
```

**Vous** :
```
"Autorisé rouler point d'arrêt 08 via Alpha,
Foxtrot Charlie."
```

#### 2.2 Pendant le roulage
- Suivre les voies de circulation (taxiways)
- S'arrêter aux lignes d'attente
- Surveiller le trafic

### Phase 3 : Décollage

#### 3.1 Au point d'arrêt
**Touche** : `Ctrl+R` puis `3`

**Changement de fréquence** : Tour (Tower)

**Vous** :
```
"Orly Tour, Foxtrot Charlie,
au point d'arrêt piste 08, prêt au départ."
```

**ATC** :
```
"Foxtrot Charlie, piste 08, autorisé décollage,
vent 270 degrés 12 nœuds."
```

**Vous** :
```
"Autorisé décollage piste 08, Foxtrot Charlie."
```

#### 3.2 Après décollage
**Touche** : `Ctrl+R` puis `4` (automatique en vol)

**Vous** :
```
"Foxtrot Charlie, décollé."
```

**ATC** :
```
"Foxtrot Charlie, contactez départ 119.150, bonne journée."
```

**Vous** :
```
"Départ 119.150, Foxtrot Charlie, bonne journée."
```

### Phase 4 : Montée et croisière

#### 4.1 Contact avec le départ
**Touche** : `Ctrl+R` puis `5`

**Changement de fréquence** : Départ/Approche

**Vous** :
```
"Approche Orly, bonjour, Foxtrot Charlie,
passant 1500 pieds, montant niveau de vol 50."
```

**ATC** :
```
"Foxtrot Charlie, approche Orly, bonjour,
radar vous identifie, continuez montée niveau de vol 50."
```

**Vous** :
```
"Continuons montée niveau 50, Foxtrot Charlie."
```

### Phase 5 : Descente et approche

#### 5.1 Demander l'approche
**Touche** : `Ctrl+R` puis `6`

**Vous** :
```
"Approche Lyon, Foxtrot Charlie,
demande approche piste 18."
```

**ATC** :
```
"Foxtrot Charlie, approuvé approche ILS piste 18,
descendez et maintenez 3000 pieds, QNH 1015."
```

**Vous** :
```
"Approuvé ILS 18, descendons 3000, QNH 1015,
Foxtrot Charlie."
```

#### 5.2 En finale
**Touche** : `Ctrl+R` puis `7`

**Changement de fréquence** : Tour

**Vous** :
```
"Lyon Tour, Foxtrot Charlie,
finale piste 18."
```

**ATC** :
```
"Foxtrot Charlie, piste 18, autorisé atterrir,
vent 180 degrés 8 nœuds."
```

**Vous** :
```
"Autorisé atterrir 18, Foxtrot Charlie."
```

### Phase 6 : Atterrissage et roulage

#### 6.1 Après atterrissage
**Touche** : `Ctrl+R` puis `8` (automatique au sol)

**Vous** :
```
"Foxtrot Charlie, piste dégagée."
```

**ATC** :
```
"Foxtrot Charlie, contactez sol 121.750."
```

**Vous** :
```
"Sol 121.750, Foxtrot Charlie."
```

#### 6.2 Roulage vers le parking
**Touche** : `Ctrl+R` puis `9`

**Changement de fréquence** : Sol

**Vous** :
```
"Lyon Sol, Foxtrot Charlie,
demande roulage parking."
```

**ATC** :
```
"Foxtrot Charlie, autorisé rouler parking via Bravo."
```

**Vous** :
```
"Autorisé parking via Bravo, Foxtrot Charlie."
```

#### 6.3 Au parking
**Vous** :
```
"Foxtrot Charlie, au parking."
```

**ATC** :
```
"Foxtrot Charlie, bien reçu, bonne journée."
```

**Vous** :
```
"Bonne journée."
```

## 🎯 Raccourcis clavier

| Touche | Action |
|--------|--------|
| `A` | Écouter ATIS |
| `Ctrl+R` | Ouvrir menu radio |
| `Ctrl+R` + `1` | Demander démarrage |
| `Ctrl+R` + `2` | Demander roulage départ |
| `Ctrl+R` + `3` | Demander décollage |
| `Ctrl+R` + `4` | Annoncer décollé |
| `Ctrl+R` + `5` | Contacter départ |
| `Ctrl+R` + `6` | Demander approche |
| `Ctrl+R` + `7` | Demander atterrissage |
| `Ctrl+R` + `8` | Annoncer piste dégagée |
| `Ctrl+R` + `9` | Demander roulage parking |
| `F` | Changer fréquence |
| `R` | Répéter dernier message |

## 📖 Termes et abréviations

### Termes courants

- **QNH** : Pression atmosphérique au niveau de la mer (en hectopascals)
- **Zulu** : Heure UTC (temps universel)
- **Niveau de vol (FL)** : Altitude en centaines de pieds (FL50 = 5000 pieds)
- **Point d'arrêt** : Ligne d'attente avant la piste
- **Finale** : Dernière phase de l'approche, aligné avec la piste
- **ILS** : Instrument Landing System (système d'atterrissage aux instruments)
- **VFR** : Visual Flight Rules (vol à vue)
- **IFR** : Instrument Flight Rules (vol aux instruments)

### Autorisations

- **Autorisé** : Permission accordée
- **Approuvé** : Demande acceptée
- **Négatif** : Refusé
- **Standby** : Attendez
- **Immédiat** : Sans délai
- **Expéditif** : Rapide

### Instructions

- **Maintenez** : Gardez l'altitude/cap actuel
- **Montez** : Augmentez l'altitude
- **Descendez** : Réduisez l'altitude
- **Virez** : Changez de cap
- **Ralentissez** : Réduisez la vitesse
- **Rappelez** : Contactez-nous quand...

## 🎓 Conseils pour débutants

### 1. Écoutez d'abord
Avant de parler, écoutez toujours l'ATIS pour connaître les conditions.

### 2. Soyez concis
Les messages doivent être courts et précis. Pas de bavardage.

### 3. Répétez les instructions
Répétez toujours les autorisations importantes (piste, altitude, QNH).

### 4. Notez le QNH
Le QNH est crucial pour l'altimétrie. Notez-le mentalement.

### 5. Utilisez l'indicatif abrégé
Après le premier contact, utilisez seulement les 2-3 dernières lettres.

### 6. Parlez clairement
Articulez bien, ne parlez ni trop vite ni trop lentement.

### 7. Attendez votre tour
Si la fréquence est occupée, attendez un silence avant de parler.

## 🚨 Situations d'urgence

### Déclaration d'urgence

**Mayday** (urgence vitale) :
```
"Mayday, Mayday, Mayday,
Foxtrot Charlie, panne moteur,
position 10 miles nord Orly, 2000 pieds,
demande assistance immédiate."
```

**Pan-Pan** (urgence non vitale) :
```
"Pan-Pan, Pan-Pan, Pan-Pan,
Foxtrot Charlie, problème technique,
demande priorité atterrissage."
```

## 📚 Exemples de vols complets

### Vol local (tour de piste)

1. ATIS → Information Alpha
2. Sol → Autorisation démarrage
3. Sol → Autorisation roulage piste 08
4. Tour → Autorisation décollage
5. Tour → Annonce vent arrière
6. Tour → Annonce finale
7. Tour → Autorisation atterrissage
8. Sol → Roulage parking

### Vol de navigation

1. ATIS départ → Information Bravo
2. Sol → Autorisation démarrage
3. Sol → Autorisation roulage
4. Tour → Autorisation décollage
5. Départ → Contact et montée
6. **En route** → Vol en croisière
7. ATIS destination → Information Charlie
8. Approche → Demande approche
9. Tour → Autorisation atterrissage
10. Sol → Roulage parking

## 🎯 Mode d'entraînement

Le simulateur propose un **mode d'entraînement radio** où :
- Les messages sont ralentis
- Des indices sont donnés
- Les erreurs sont corrigées
- La phraséologie est enseignée

**Activation** : Menu Options → Radio → Mode entraînement

## ✅ Checklist radio

Avant chaque vol, vérifiez :
- [ ] Radio allumée
- [ ] Volume audible
- [ ] ATIS écouté
- [ ] Indicatif mémorisé
- [ ] QNH noté
- [ ] Piste active connue
- [ ] Fréquences programmées

---

**Bon vol et bonnes communications ! 📻✈️**

