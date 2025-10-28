# Guide du Copilote Virtuel

## 🤖 Vue d'ensemble

Le copilote virtuel est un assistant intelligent alimenté par l'IA (GPT-4) qui vous accompagne pendant vos vols. Il peut converser naturellement, vous aider, vous alerter des dangers, et même vous enseigner.

## 🎭 Personnalités disponibles

Le copilote peut adopter différentes personnalités selon vos préférences :

### 1. Professionnel (par défaut)
- Compétent et précis
- Utilise la terminologie aéronautique standard
- Concis et clair
- Calme en toute situation
- Idéal pour : Vols sérieux, missions, entraînement réaliste

**Exemple** :
```
Vous: "Comment va le vol ?"
Copilote: "Tout est nominal. Altitude 5000 pieds, vitesse 120 nœuds, 
          cap 270. Carburant à 75%. On est stable."
```

### 2. Sympathique
- Bienveillant et encourageant
- Patient avec les débutants
- Explique simplement
- Félicite les bonnes actions
- Idéal pour : Débutants, apprentissage, vols détendus

**Exemple** :
```
Vous: "J'ai peur de rater mon atterrissage"
Copilote: "Ne t'inquiète pas ! Tu t'en sors très bien jusqu'ici. 
          Je vais te guider pas à pas. On va y arriver ensemble !"
```

### 3. Instructeur
- Exigeant mais juste
- Enseigne et corrige
- Explique le "pourquoi"
- Strict sur les procédures
- Idéal pour : Formation sérieuse, perfectionnement, préparation licence

**Exemple** :
```
Vous: "Pourquoi je dois sortir le train maintenant ?"
Copilote: "Le train doit être sorti avant 1000 pieds en approche. 
          C'est une procédure de sécurité standard. Ça te donne le temps 
          de vérifier qu'il est bien verrouillé avant la finale."
```

### 4. Humoristique
- Sens de l'humour développé
- Détend l'atmosphère
- Blagues aéronautiques
- Sérieux quand nécessaire
- Idéal pour : Vols loisirs, longues croisières, détente

**Exemple** :
```
Vous: "On est un peu secoués là"
Copilote: "Oui, le ciel fait son shaker à cocktail aujourd'hui ! 
          Mais t'inquiète, l'avion en a vu d'autres. On tient le coup !"
```

## 🎮 Utilisation

### Parler au copilote

**Touche** : `C` (ouvre le dialogue vocal)

Vous pouvez lui parler naturellement comme à une vraie personne :

**Questions générales** :
- "Comment va le vol ?"
- "Tout est OK ?"
- "On est où ?"
- "Combien de carburant il reste ?"

**Demandes d'aide** :
- "Aide-moi pour l'atterrissage"
- "Qu'est-ce que je dois faire maintenant ?"
- "Comment on fait un virage ?"
- "Je suis perdu, aide-moi"

**Demandes d'explication** :
- "C'est quoi le QNH ?"
- "Explique-moi les volets"
- "Pourquoi on doit faire ça ?"
- "Comment fonctionne l'altimètre ?"

**Conversations** :
- "Raconte-moi une anecdote"
- "Tu as déjà volé sur un A380 ?"
- "C'est beau ici non ?"
- "Ça va être long encore ?"

### Raccourcis rapides

| Touche | Action |
|--------|--------|
| `C` | Parler au copilote |
| `Ctrl+C` | Menu copilote |
| `Ctrl+C` + `1` | Demander rapport de situation |
| `Ctrl+C` + `2` | Demander check-list |
| `Ctrl+C` + `3` | Demander conseil |
| `Ctrl+C` + `4` | Changer personnalité |
| `Ctrl+C` + `5` | Activer/Désactiver copilote |
| `Ctrl+C` + `R` | Réinitialiser conversation |

## 🔔 Alertes automatiques

Le copilote surveille constamment le vol et vous alerte si nécessaire :

### Alertes d'altitude
```
"Attention, altitude basse : 450 pieds"
```
Déclenchée si altitude < 500 pieds hors atterrissage

### Alertes de vitesse
```
"Attention à la vitesse, on approche du décrochage"
```
Déclenchée si vitesse < vitesse de décrochage + 10 nœuds

```
"Vitesse élevée, attention à ne pas dépasser la VNE"
```
Déclenchée si vitesse > 90% de la vitesse maximale

### Alertes de carburant
```
"Carburant à 20%, on devrait penser à atterrir bientôt"
```
Déclenchée à 20% de carburant

```
"Carburant critique à 12%, il faut atterrir maintenant"
```
Déclenchée en dessous de 15% de carburant

### Alertes de configuration
```
"Train d'atterrissage pas sorti, on est en approche"
```
Déclenchée si altitude < 1000 pieds en approche sans train

```
"On pourrait sortir les volets pour l'approche"
```
Suggestion si vitesse basse en approche sans volets

## 💬 Commentaires automatiques

Le copilote commente spontanément les événements importants :

### Au décollage
```
"Beau décollage ! On monte bien."
```

### En croisière
```
"Altitude de croisière atteinte : 5000 pieds. Tout est stable."
```

### En descente
```
"On commence la descente. Je surveille l'altitude."
```

### À l'atterrissage
```
"Excellent atterrissage ! Très doux."
```
ou
```
"Bon atterrissage. On est au sol en sécurité."
```
ou
```
"On est au sol. L'atterrissage était un peu ferme."
```

## 📋 Check-lists

Le copilote peut vous lire les check-lists :

**Commande** : `Ctrl+C` puis `2`

**Ou dites** : "Lis-moi la check-list de décollage"

**Check-lists disponibles** :
- Pré-vol
- Démarrage
- Avant décollage
- Après décollage
- Croisière
- Descente
- Approche
- Avant atterrissage
- Après atterrissage
- Arrêt moteur

**Exemple** :
```
Vous: "Check-list avant décollage"
Copilote: "Check-list avant décollage :
          - Volets : Position décollage
          - Trim : Réglé
          - Carburant : Vérifié, suffisant
          - Instruments : Vérifiés
          - Portes : Fermées et verrouillées
          - Autorisations : Obtenues
          Check-list complète."
```

## 🎓 Mode instructeur

En mode instructeur, le copilote devient un vrai formateur :

### Corrections en temps réel
```
"Attention, tu montes trop vite. Réduis un peu l'assiette."
```

### Explications détaillées
```
"Le trim permet de soulager la pression sur le manche. 
En croisière, tu dois trimer pour que l'avion vole 
tout seul sans que tu aies à pousser ou tirer."
```

### Anticipation des erreurs
```
"On approche de la piste, n'oublie pas de sortir le train 
dans les prochaines secondes."
```

### Débriefing
Après le vol, demandez :
```
Vous: "Comment j'ai volé ?"
Copilote: "Globalement bien. Le décollage était propre, 
          la croisière stable. Par contre, tu as oublié 
          de réduire les gaz en descente, et l'atterrissage 
          était un peu ferme. On travaillera l'arrondi 
          la prochaine fois."
```

## 🆘 Situations d'urgence

Le copilote vous assiste en cas d'urgence :

### Panne moteur
```
Vous: "Le moteur vient de s'arrêter !"
Copilote: "OK, reste calme. Garde la vitesse de plané, 
          70 nœuds. Cherche un terrain pour atterrir. 
          Je t'aide à trouver un champ."
```

### Météo dégradée
```
Vous: "Je ne vois plus rien avec ce brouillard"
Copilote: "Passe aux instruments. Je te guide. 
          Maintiens cap 180, altitude 2000 pieds. 
          On va demander une approche ILS."
```

### Désorientation
```
Vous: "Je suis complètement perdu"
Copilote: "Pas de panique. Regarde ton GPS. 
          On est à 15 miles au nord de l'aéroport. 
          Vire cap 180 et je te guide."
```

## 💡 Conseils d'utilisation

### 1. Parlez naturellement
Le copilote comprend le langage naturel. Pas besoin de phrases formelles.

✅ "J'ai un doute sur l'altitude"
❌ "Copilote, demande information altitude actuelle"

### 2. Posez des questions
N'hésitez pas à demander des explications :
- "Pourquoi on fait ça ?"
- "C'est quoi ce bruit ?"
- "Comment ça marche ?"

### 3. Demandez de l'aide
Le copilote est là pour vous aider :
- "Aide-moi"
- "Je ne sais pas quoi faire"
- "Comment on fait ?"

### 4. Conversez
Le copilote peut discuter pendant les phases calmes :
- "Raconte-moi quelque chose"
- "Tu as déjà volé ici ?"
- "C'est quoi ton avion préféré ?"

### 5. Adaptez la personnalité
Changez de personnalité selon vos besoins :
- Débutant → Sympathique
- Apprentissage → Instructeur
- Vol sérieux → Professionnel
- Détente → Humoristique

## 🔧 Configuration

### Changer de personnalité

**Menu** : Options → Copilote → Personnalité

**Ou** : `Ctrl+C` puis `4`

**Ou dites** : "Change de personnalité"

### Activer/Désactiver

**Touche** : `Ctrl+C` puis `5`

**Ou dites** : "Désactive-toi" / "Active-toi"

### Réinitialiser la conversation

Si le copilote semble confus ou si vous voulez repartir à zéro :

**Touche** : `Ctrl+C` puis `R`

**Ou dites** : "Réinitialise la conversation"

### Configurer l'API OpenAI

Le copilote utilise GPT-4. Vous devez configurer votre clé API :

1. Obtenez une clé API sur https://platform.openai.com
2. Ouvrez le fichier de configuration
3. Remplacez `YOUR_API_KEY_HERE` par votre clé
4. Redémarrez le simulateur

**Note** : L'utilisation de l'API OpenAI est payante (quelques centimes par vol)

### Mode hors ligne

Si vous n'avez pas de clé API ou pas d'internet, le copilote fonctionne en mode dégradé :
- Alertes automatiques : ✅ Fonctionnent
- Commentaires automatiques : ✅ Fonctionnent
- Conversations : ❌ Réponses basiques uniquement

## 📊 Exemples de conversations

### Vol débutant
```
Vous: "C'est mon premier vol, j'ai peur"
Copilote: "Pas de souci ! Je suis là pour t'aider. 
          On va y aller doucement, étape par étape. 
          Tu es prêt ? On commence par démarrer le moteur."

Vous: "Comment je fais ?"
Copilote: "Appuie sur Ctrl+E. Tu vas entendre le moteur 
          qui démarre. Attends qu'il tourne bien."

Vous: "OK c'est fait"
Copilote: "Super ! Maintenant on va demander l'autorisation 
          de rouler. Appuie sur Ctrl+R puis 2."
```

### Vol expérimenté
```
Vous: "Rapport de situation"
Copilote: "FL150, cap 270, vitesse 180 nœuds. 
          Météo stable, vent de face 15 nœuds. 
          Carburant 60%, autonomie 2h30. 
          ETA destination dans 45 minutes. 
          Tout est nominal."

Vous: "Prépare l'approche"
Copilote: "Bien reçu. ATIS écouté, piste 27 active, 
          QNH 1015. Je programme l'ILS. 
          On commence la descente dans 10 minutes."
```

### Vol avec problème
```
Vous: "Le moteur fait un bruit bizarre"
Copilote: "Je surveille les paramètres. 
          Température et pression normales. 
          Ça peut être juste des turbulences. 
          Je te préviens si je vois quelque chose d'anormal."

Vous: "Ça continue"
Copilote: "OK, par précaution, on va atterrir 
          au prochain aéroport. Il y a Bordeaux 
          à 20 miles. Je demande une priorité."
```

## 🎯 Astuces

### Pour les débutants
1. Utilisez la personnalité **Sympathique**
2. Demandez des explications pour tout
3. Laissez le copilote vous guider
4. N'ayez pas peur de poser des questions "bêtes"

### Pour l'apprentissage
1. Utilisez la personnalité **Instructeur**
2. Demandez le "pourquoi" de chaque action
3. Faites-vous corriger
4. Demandez un débriefing après chaque vol

### Pour le réalisme
1. Utilisez la personnalité **Professionnel**
2. Communiquez comme dans un vrai cockpit
3. Demandez des rapports de situation
4. Suivez les procédures standard

### Pour la détente
1. Utilisez la personnalité **Humoristique**
2. Discutez pendant la croisière
3. Demandez des anecdotes
4. Profitez du vol

## ⚠️ Limitations

Le copilote virtuel est très avancé mais a quelques limitations :

1. **Pas de contrôle direct** : Il ne peut pas piloter à votre place
2. **Dépend d'internet** : Les conversations nécessitent une connexion
3. **Coût API** : L'utilisation de GPT-4 a un coût (minime)
4. **Délai de réponse** : 1-3 secondes pour répondre
5. **Contexte limité** : Il ne se souvient que des 10 derniers échanges

## 🌟 Avantages

1. **Apprentissage accéléré** : Apprenez plus vite avec un guide
2. **Sécurité accrue** : Alertes proactives des dangers
3. **Immersion totale** : Vraie présence dans le cockpit
4. **Adaptable** : 4 personnalités pour tous les styles
5. **Accessible** : 100% vocal, parfait pour les aveugles
6. **Intelligent** : Comprend le contexte et s'adapte

---

**Bon vol avec votre copilote ! 🤖✈️**

