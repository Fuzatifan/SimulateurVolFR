#!/usr/bin/env python3
"""
Script pour générer les sons radio supplémentaires
"""

import numpy as np
from scipy.io import wavfile
from scipy import signal
import os

SAMPLE_RATE = 44100

def create_directory():
    """Crée le répertoire des sons radio"""
    sounds_dir = "../assets/sounds/radio"
    os.makedirs(sounds_dir, exist_ok=True)
    return sounds_dir

def generate_radio_beep(filename):
    """
    Génère un bip radio court
    """
    duration = 0.3
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bip à 1000 Hz
    freq = 1000
    sound = np.sin(2 * np.pi * freq * t)
    
    # Envelope pour éviter les clics
    envelope = np.ones_like(sound)
    fade_samples = int(0.01 * SAMPLE_RATE)
    envelope[:fade_samples] = np.linspace(0, 1, fade_samples)
    envelope[-fade_samples:] = np.linspace(1, 0, fade_samples)
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.7
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_radio_tune(filename):
    """
    Génère un son de syntonisation radio
    """
    duration = 0.5
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Balayage de fréquence
    freq_start = 800
    freq_end = 1200
    freq = np.linspace(freq_start, freq_end, len(t))
    
    # Générer le son
    phase = 2 * np.pi * np.cumsum(freq) / SAMPLE_RATE
    sound = np.sin(phase)
    
    # Ajouter du bruit
    noise = np.random.normal(0, 0.1, len(t))
    sound = sound * 0.7 + noise * 0.3
    
    # Envelope
    envelope = np.linspace(0.3, 1, len(t))
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.6
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_radio_click(filename):
    """
    Génère un clic de bouton radio
    """
    duration = 0.1
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Clic court
    sound = np.zeros_like(t)
    click_duration = int(0.01 * SAMPLE_RATE)
    sound[:click_duration] = np.random.normal(0, 1, click_duration)
    
    # Filtrer
    sos = signal.butter(2, [200, 2000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, sound)
    
    # Envelope
    envelope = np.exp(-t * 20)
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.5
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_radio_squelch(filename):
    """
    Génère un son de squelch radio (ouverture de fréquence)
    """
    duration = 0.2
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bruit blanc court
    noise = np.random.normal(0, 1, len(t))
    
    # Filtrer pour simuler le squelch
    sos = signal.butter(2, [300, 3000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, noise)
    
    # Envelope rapide
    envelope = np.exp(-t * 15)
    sound *= envelope * 0.3
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.4
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_atc_background(filename):
    """
    Génère un fond sonore de tour de contrôle
    """
    duration = 10
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bruit de fond très léger
    noise = np.random.normal(0, 0.05, len(t))
    
    # Filtrer pour simuler l'ambiance
    sos = signal.butter(2, [100, 1000], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, noise)
    
    # Ajouter des variations
    modulation = 1 + 0.1 * np.sin(2 * np.pi * 0.3 * t)
    sound *= modulation
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.15
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_radio_interference(filename):
    """
    Génère des interférences radio
    """
    duration = 2
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bruit avec modulation
    noise = np.random.normal(0, 1, len(t))
    
    # Modulation rapide pour simuler les interférences
    interference_freq = 50
    modulation = 0.5 + 0.5 * np.sin(2 * np.pi * interference_freq * t)
    sound = noise * modulation
    
    # Filtrer
    sos = signal.butter(2, [500, 2500], 'bandpass', fs=SAMPLE_RATE, output='sos')
    sound = signal.sosfilt(sos, sound)
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.3
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_emergency_tone(filename):
    """
    Génère un ton d'urgence
    """
    duration = 1
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Deux tons alternés (comme les sirènes)
    freq1 = 800
    freq2 = 1000
    
    # Alterner toutes les 0.25 secondes
    sound = np.zeros_like(t)
    for i in range(len(t)):
        if (i // (SAMPLE_RATE // 4)) % 2 == 0:
            sound[i] = np.sin(2 * np.pi * freq1 * t[i])
        else:
            sound[i] = np.sin(2 * np.pi * freq2 * t[i])
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.8
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_frequency_change(filename):
    """
    Génère un son de changement de fréquence
    """
    duration = 0.3
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bip montant
    freq_start = 600
    freq_end = 1200
    freq = np.linspace(freq_start, freq_end, len(t))
    
    phase = 2 * np.pi * np.cumsum(freq) / SAMPLE_RATE
    sound = np.sin(phase)
    
    # Envelope
    envelope = np.linspace(1, 0.3, len(t))
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.5
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def generate_roger_beep(filename):
    """
    Génère un bip de confirmation (Roger)
    """
    duration = 0.15
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Double bip rapide
    freq = 1200
    sound = np.sin(2 * np.pi * freq * t)
    
    # Créer deux bips
    gap_start = int(0.06 * SAMPLE_RATE)
    gap_end = int(0.09 * SAMPLE_RATE)
    sound[gap_start:gap_end] = 0
    
    # Envelope
    envelope = np.ones_like(sound)
    fade = int(0.005 * SAMPLE_RATE)
    envelope[:fade] = np.linspace(0, 1, fade)
    envelope[-fade:] = np.linspace(1, 0, fade)
    sound *= envelope
    
    # Normaliser
    sound = sound / np.max(np.abs(sound)) * 0.6
    
    wavfile.write(filename, SAMPLE_RATE, (sound * 32767).astype(np.int16))
    print(f"  ✓ {os.path.basename(filename)}")

def main():
    """Génère tous les sons radio"""
    
    print("=" * 70)
    print("  Génération des sons radio")
    print("=" * 70)
    print()
    
    sounds_dir = create_directory()
    
    print("Génération des sons d'interface radio...")
    generate_radio_beep(f"{sounds_dir}/radio_beep.wav")
    generate_radio_tune(f"{sounds_dir}/radio_tune.wav")
    generate_radio_click(f"{sounds_dir}/radio_click.wav")
    generate_radio_squelch(f"{sounds_dir}/radio_squelch.wav")
    generate_frequency_change(f"{sounds_dir}/frequency_change.wav")
    generate_roger_beep(f"{sounds_dir}/roger_beep.wav")
    
    print("\nGénération des sons d'ambiance...")
    generate_atc_background(f"{sounds_dir}/atc_background.wav")
    generate_radio_interference(f"{sounds_dir}/radio_interference.wav")
    
    print("\nGénération des sons d'urgence...")
    generate_emergency_tone(f"{sounds_dir}/emergency_tone.wav")
    
    print()
    print("=" * 70)
    print("  ✓ Tous les sons radio ont été générés avec succès!")
    print(f"  Répertoire: {sounds_dir}")
    print("=" * 70)

if __name__ == "__main__":
    main()

