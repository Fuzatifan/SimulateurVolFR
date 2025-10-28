#!/usr/bin/env python3
"""
Script pour générer des sons réalistes pour le simulateur
Utilise des techniques de synthèse audio avancées
"""

import numpy as np
from scipy.io import wavfile
from scipy import signal
import os

# Paramètres audio
SAMPLE_RATE = 44100
DURATION = 5  # secondes

def create_directory():
    """Crée le répertoire des sons s'il n'existe pas"""
    sounds_dir = "../assets/sounds/realistic"
    os.makedirs(sounds_dir, exist_ok=True)
    return sounds_dir

def generate_engine_sound(base_freq, harmonics, noise_level, filename):
    """
    Génère un son de moteur réaliste
    """
    t = np.linspace(0, DURATION, int(SAMPLE_RATE * DURATION))
    sound = np.zeros_like(t)
    
    # Ajouter les harmoniques
    for i, (freq, amp) in enumerate(harmonics):
        # Variation légère de fréquence pour le réalisme
        freq_variation = np.sin(2 * np.pi * 0.5 * t) * freq * 0.02
        phase = 2 * np.pi * (freq + freq_variation) * t
        sound += amp * np.sin(phase)
    
    # Ajouter du bruit pour le réalisme
    noise = np.random.normal(0, noise_level, len(t))
    sound += noise
    
    # Filtrer pour simuler la résonance
    sos = signal.butter(4, [base_freq * 0.5, base_freq * 8], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, sound)
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    # Sauvegarder
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_propeller_sound(rpm, num_blades, filename):
    """
    Génère un son d'hélice
    """
    t = np.linspace(0, DURATION, int(SAMPLE_RATE * DURATION))
    
    # Fréquence de base (passage des pales)
    blade_freq = (rpm / 60) * num_blades
    
    sound = np.zeros_like(t)
    
    # Harmoniques de l'hélice
    for i in range(1, 6):
        amp = 1.0 / i
        sound += amp * np.sin(2 * np.pi * blade_freq * i * t)
    
    # Modulation pour le réalisme
    modulation = 1 + 0.1 * np.sin(2 * np.pi * 3 * t)
    sound *= modulation
    
    # Bruit aérodynamique
    noise = np.random.normal(0, 0.1, len(t))
    sos = signal.butter(2, [200, 4000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    noise = signal.sosfilt(sos, noise)
    sound += noise
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_jet_sound(thrust_level, filename):
    """
    Génère un son de réacteur
    """
    t = np.linspace(0, DURATION, int(SAMPLE_RATE * DURATION))
    
    # Bruit blanc filtré pour simuler le jet
    noise = np.random.normal(0, 1, len(t))
    
    # Filtrage selon le niveau de poussée
    if thrust_level == "idle":
        sos = signal.butter(4, [200, 2000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        base_amp = 0.3
    elif thrust_level == "cruise":
        sos = signal.butter(4, [300, 5000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        base_amp = 0.6
    else:  # full_throttle
        sos = signal.butter(4, [400, 8000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        base_amp = 0.9
    
    sound = signal.sosfilt(sos, noise) * base_amp
    
    # Ajouter une composante tonale (compresseur)
    compressor_freq = 150 + thrust_level_to_number(thrust_level) * 100
    sound += 0.2 * np.sin(2 * np.pi * compressor_freq * t)
    
    # Modulation pour le réalisme
    modulation = 1 + 0.05 * np.sin(2 * np.pi * 5 * t)
    sound *= modulation
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def thrust_level_to_number(level):
    """Convertit le niveau de poussée en nombre"""
    levels = {"idle": 0, "cruise": 1, "full_throttle": 2}
    return levels.get(level, 1)

def generate_helicopter_rotor(rpm, filename):
    """
    Génère un son de rotor d'hélicoptère
    """
    t = np.linspace(0, DURATION, int(SAMPLE_RATE * DURATION))
    
    # Fréquence du rotor
    rotor_freq = rpm / 60
    
    sound = np.zeros_like(t)
    
    # Son caractéristique "whop whop"
    for i in range(1, 8):
        amp = 1.0 / (i ** 0.7)
        sound += amp * np.sin(2 * np.pi * rotor_freq * i * t)
    
    # Modulation d'amplitude pour le "whop whop"
    modulation = 0.5 + 0.5 * np.sin(2 * np.pi * rotor_freq * t)
    sound *= modulation
    
    # Bruit aérodynamique
    noise = np.random.normal(0, 0.15, len(t))
    sos = signal.butter(2, [100, 3000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    noise = signal.sosfilt(sos, noise)
    sound += noise
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_wind_sound(intensity, filename):
    """
    Génère un son de vent
    """
    t = np.linspace(0, DURATION, int(SAMPLE_RATE * DURATION))
    
    # Bruit filtré pour le vent
    noise = np.random.normal(0, 1, len(t))
    
    if intensity == "light":
        sos = signal.butter(2, [100, 1000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        amp = 0.2
    elif intensity == "moderate":
        sos = signal.butter(2, [150, 2000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        amp = 0.4
    else:  # strong
        sos = signal.butter(2, [200, 3000], 'bandpass', fs=SAMPLE_RATE, output='sos')
        amp = 0.6
    
    sound = signal.sosfilt(sos, noise) * amp
    
    # Modulation lente pour le réalisme
    modulation = 1 + 0.3 * np.sin(2 * np.pi * 0.3 * t)
    sound *= modulation
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_alert_sound(alert_type, filename):
    """
    Génère un son d'alerte
    """
    t = np.linspace(0, 2, int(SAMPLE_RATE * 2))  # Alertes plus courtes
    
    if alert_type == "altitude":
        # Bip-bip rapide
        freq = 1000
        beep_duration = 0.1
        beep_interval = 0.2
        sound = np.zeros_like(t)
        for i in range(int(2 / beep_interval)):
            start = int(i * beep_interval * SAMPLE_RATE)
            end = int((i * beep_interval + beep_duration) * SAMPLE_RATE)
            if end < len(sound):
                sound[start:end] = np.sin(2 * np.pi * freq * t[start:end])
    
    elif alert_type == "stall":
        # Son grave pulsé
        freq = 400
        sound = np.sin(2 * np.pi * freq * t)
        modulation = 0.5 + 0.5 * np.sin(2 * np.pi * 4 * t)
        sound *= modulation
    
    elif alert_type == "terrain":
        # "Pull up" simulé avec variation de fréquence
        freq_start = 600
        freq_end = 900
        freq = np.linspace(freq_start, freq_end, len(t))
        sound = np.sin(2 * np.pi * freq * t)
    
    else:  # generic
        freq = 800
        sound = np.sin(2 * np.pi * freq * t)
    
    # Envelope pour éviter les clics
    envelope = np.ones_like(sound)
    fade_samples = int(0.01 * SAMPLE_RATE)
    envelope[:fade_samples] = np.linspace(0, 1, fade_samples)
    envelope[-fade_samples:] = np.linspace(1, 0, fade_samples)
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.9
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_radio_static(filename):
    """
    Génère un son de parasites radio
    """
    t = np.linspace(0, 1, int(SAMPLE_RATE * 1))
    
    # Bruit blanc filtré
    noise = np.random.normal(0, 1, len(t))
    sos = signal.butter(2, [300, 3000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, noise) * 0.3
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def main():
    """Génère tous les sons réalistes"""
    
    print("=" * 70)
    print("  Génération de sons réalistes pour le simulateur")
    print("=" * 70)
    print()
    
    sounds_dir = create_directory()
    
    # Sons d'avions légers (moteur à pistons)
    print("Génération des sons d'avions légers...")
    generate_propeller_sound(2400, 2, f"{sounds_dir}/cessna172_idle.wav")
    generate_propeller_sound(2600, 2, f"{sounds_dir}/cessna172_cruise.wav")
    generate_propeller_sound(2700, 2, f"{sounds_dir}/cessna172_full_throttle.wav")
    
    # Sons de jets d'affaires
    print("\nGénération des sons de jets d'affaires...")
    generate_jet_sound("idle", f"{sounds_dir}/citation_idle.wav")
    generate_jet_sound("cruise", f"{sounds_dir}/citation_cruise.wav")
    generate_jet_sound("full_throttle", f"{sounds_dir}/citation_full_throttle.wav")
    
    # Sons d'avions de ligne
    print("\nGénération des sons d'avions de ligne...")
    generate_jet_sound("idle", f"{sounds_dir}/a320_idle.wav")
    generate_jet_sound("cruise", f"{sounds_dir}/a320_cruise.wav")
    generate_jet_sound("full_throttle", f"{sounds_dir}/a320_full_throttle.wav")
    
    # Sons de gros porteurs
    print("\nGénération des sons de gros porteurs...")
    generate_jet_sound("idle", f"{sounds_dir}/a380_idle.wav")
    generate_jet_sound("cruise", f"{sounds_dir}/a380_cruise.wav")
    generate_jet_sound("full_throttle", f"{sounds_dir}/a380_full_throttle.wav")
    
    # Sons d'hélicoptères
    print("\nGénération des sons d'hélicoptères...")
    generate_helicopter_rotor(350, f"{sounds_dir}/r22_rotor.wav")
    generate_helicopter_rotor(400, f"{sounds_dir}/as350_rotor.wav")
    generate_helicopter_rotor(300, f"{sounds_dir}/chinook_rotor.wav")
    
    # Sons d'environnement
    print("\nGénération des sons d'environnement...")
    generate_wind_sound("light", f"{sounds_dir}/wind_light.wav")
    generate_wind_sound("moderate", f"{sounds_dir}/wind_moderate.wav")
    generate_wind_sound("strong", f"{sounds_dir}/wind_strong.wav")
    
    # Sons d'alerte
    print("\nGénération des sons d'alerte...")
    generate_alert_sound("altitude", f"{sounds_dir}/altitude_warning.wav")
    generate_alert_sound("stall", f"{sounds_dir}/stall_warning.wav")
    generate_alert_sound("terrain", f"{sounds_dir}/terrain_warning.wav")
    generate_alert_sound("generic", f"{sounds_dir}/generic_warning.wav")
    
    # Sons radio
    print("\nGénération des sons radio...")
    generate_radio_static(f"{sounds_dir}/radio_static.wav")
    
    print()
    print("=" * 70)
    print("  ✓ Tous les sons réalistes ont été générés avec succès!")
    print(f"  Répertoire: {sounds_dir}")
    print("=" * 70)

if __name__ == "__main__":
    main()

