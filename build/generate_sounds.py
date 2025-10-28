#!/usr/bin/env python3
"""
Générateur de sons de base pour le simulateur de vol
Génère des sons synthétiques simples en attendant des enregistrements réels
"""

import numpy as np
from scipy.io import wavfile
import os

# Fréquence d'échantillonnage
SAMPLE_RATE = 44100

def generate_sine_wave(frequency, duration, amplitude=0.3):
    """Génère une onde sinusoïdale"""
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    wave = amplitude * np.sin(2 * np.pi * frequency * t)
    return wave

def generate_noise(duration, amplitude=0.1):
    """Génère du bruit blanc"""
    samples = int(SAMPLE_RATE * duration)
    noise = amplitude * np.random.uniform(-1, 1, samples)
    return noise

def generate_engine_sound(base_freq, duration, rpm_variation=True):
    """Génère un son de moteur synthétique"""
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Fréquence fondamentale avec variation si demandé
    if rpm_variation:
        freq_mod = base_freq + 5 * np.sin(2 * np.pi * 0.5 * t)
    else:
        freq_mod = base_freq
    
    # Harmoniques pour un son plus riche
    wave = 0.4 * np.sin(2 * np.pi * freq_mod * t)
    wave += 0.2 * np.sin(2 * np.pi * 2 * freq_mod * t)
    wave += 0.1 * np.sin(2 * np.pi * 3 * freq_mod * t)
    wave += 0.05 * np.sin(2 * np.pi * 4 * freq_mod * t)
    
    # Ajouter du bruit pour le réalisme
    noise = 0.1 * np.random.uniform(-1, 1, len(t))
    wave = wave + noise
    
    # Normaliser
    wave = wave / np.max(np.abs(wave)) * 0.7
    
    return wave

def generate_jet_sound(duration):
    """Génère un son de réacteur synthétique"""
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Bruit filtré pour simuler un réacteur
    noise = np.random.uniform(-1, 1, len(t))
    
    # Filtrage passe-bas simple
    filtered = np.zeros_like(noise)
    alpha = 0.1
    for i in range(1, len(noise)):
        filtered[i] = alpha * noise[i] + (1 - alpha) * filtered[i-1]
    
    # Ajouter des harmoniques basses
    wave = 0.5 * filtered
    wave += 0.3 * np.sin(2 * np.pi * 80 * t)
    wave += 0.2 * np.sin(2 * np.pi * 160 * t)
    
    # Normaliser
    wave = wave / np.max(np.abs(wave)) * 0.7
    
    return wave

def generate_alert_sound(frequency, duration, beeps=1):
    """Génère un son d'alerte"""
    beep_duration = 0.2
    pause_duration = 0.1
    
    waves = []
    for i in range(beeps):
        # Génerer un bip
        beep = generate_sine_wave(frequency, beep_duration, amplitude=0.5)
        # Appliquer une enveloppe
        envelope = np.linspace(1, 0, len(beep))
        beep = beep * envelope
        waves.append(beep)
        
        # Ajouter une pause si ce n'est pas le dernier bip
        if i < beeps - 1:
            pause = np.zeros(int(SAMPLE_RATE * pause_duration))
            waves.append(pause)
    
    wave = np.concatenate(waves)
    
    # Compléter avec du silence si nécessaire
    if len(wave) < int(SAMPLE_RATE * duration):
        silence = np.zeros(int(SAMPLE_RATE * duration) - len(wave))
        wave = np.concatenate([wave, silence])
    
    return wave

def generate_click_sound():
    """Génère un son de clic"""
    duration = 0.05
    t = np.linspace(0, duration, int(SAMPLE_RATE * duration))
    
    # Impulsion courte
    wave = 0.8 * np.exp(-50 * t) * np.sin(2 * np.pi * 1000 * t)
    
    return wave

def save_wav(filename, data):
    """Sauvegarde un fichier WAV"""
    # Convertir en int16
    data_int16 = np.int16(data * 32767)
    wavfile.write(filename, SAMPLE_RATE, data_int16)
    print(f"Généré: {filename}")

def main():
    # Créer le dossier de sortie
    output_dir = "../assets/audio"
    os.makedirs(output_dir, exist_ok=True)
    
    print("Génération des sons pour le simulateur...")
    
    # Sons de moteurs Cessna 172
    print("\nCessna 172...")
    save_wav(f"{output_dir}/cessna172_idle.wav", generate_engine_sound(80, 5, rpm_variation=True))
    save_wav(f"{output_dir}/cessna172_cruise.wav", generate_engine_sound(120, 5, rpm_variation=True))
    save_wav(f"{output_dir}/cessna172_full_throttle.wav", generate_engine_sound(150, 5, rpm_variation=True))
    
    # Sons de réacteurs A320
    print("\nAirbus A320...")
    save_wav(f"{output_dir}/a320_idle.wav", generate_jet_sound(5))
    save_wav(f"{output_dir}/a320_cruise.wav", generate_jet_sound(5))
    save_wav(f"{output_dir}/a320_takeoff.wav", generate_jet_sound(5))
    
    # Sons de réacteurs B737
    print("\nBoeing 737...")
    save_wav(f"{output_dir}/b737_idle.wav", generate_jet_sound(5))
    save_wav(f"{output_dir}/b737_cruise.wav", generate_jet_sound(5))
    save_wav(f"{output_dir}/b737_takeoff.wav", generate_jet_sound(5))
    
    # Sons d'ambiance
    print("\nAmbiance...")
    save_wav(f"{output_dir}/cockpit_ambient.wav", generate_noise(5, amplitude=0.05))
    save_wav(f"{output_dir}/wind_light.wav", generate_noise(5, amplitude=0.1))
    save_wav(f"{output_dir}/wind_moderate.wav", generate_noise(5, amplitude=0.2))
    save_wav(f"{output_dir}/wind_strong.wav", generate_noise(5, amplitude=0.3))
    
    # Sons d'alertes
    print("\nAlertes...")
    save_wav(f"{output_dir}/stall_warning.wav", generate_alert_sound(800, 1, beeps=3))
    save_wav(f"{output_dir}/overspeed_warning.wav", generate_alert_sound(1000, 1, beeps=2))
    save_wav(f"{output_dir}/altitude_warning.wav", generate_alert_sound(600, 1, beeps=2))
    save_wav(f"{output_dir}/gear_warning.wav", generate_alert_sound(500, 1, beeps=1))
    save_wav(f"{output_dir}/fuel_low.wav", generate_alert_sound(700, 1, beeps=2))
    
    # Sons d'interface
    print("\nInterface...")
    save_wav(f"{output_dir}/menu_select.wav", generate_click_sound())
    save_wav(f"{output_dir}/menu_confirm.wav", generate_sine_wave(1000, 0.1, amplitude=0.3))
    save_wav(f"{output_dir}/menu_cancel.wav", generate_sine_wave(500, 0.1, amplitude=0.3))
    save_wav(f"{output_dir}/button_click.wav", generate_click_sound())
    
    # Sons de systèmes
    print("\nSystèmes...")
    save_wav(f"{output_dir}/gear_down.wav", generate_sine_wave(200, 2, amplitude=0.2))
    save_wav(f"{output_dir}/gear_up.wav", generate_sine_wave(250, 2, amplitude=0.2))
    save_wav(f"{output_dir}/flaps_extend.wav", generate_sine_wave(180, 1.5, amplitude=0.2))
    save_wav(f"{output_dir}/flaps_retract.wav", generate_sine_wave(220, 1.5, amplitude=0.2))
    
    print("\n✓ Génération terminée!")
    print(f"Les sons ont été sauvegardés dans: {output_dir}")
    print("\nNote: Ces sons sont synthétiques et basiques.")
    print("Pour un réalisme optimal, remplacez-les par des enregistrements réels.")

if __name__ == "__main__":
    main()

