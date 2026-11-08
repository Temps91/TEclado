using System;
using UnityEngine;

public class SongAudioAnalizer : MonoBehaviour
{
    public event Action<string, float> OnNoteDetected;

    [SerializeField] private AudioClip song;
    [SerializeField] private int sampleSize = 2048;
    [SerializeField] private float minimumVolume = 0.01f;
    [SerializeField] private float minimumConfidence = 0.35f;
    [SerializeField] private float minimumNoteDuration = 0.05f;
    [SerializeField] private float minimumNoteGap = 0.05f;

    private float[] samples;
    private string currentNote = "";
    private float currentNoteStartTime = -1f;
    private string lastDetectedNote = "";
    private float lastDetectedTime = -999f;
    private float lastAnalyzedTime = 0f;

    private readonly string[] noteNames = { "C4", "D4", "E4", "F4", "G4", "A4", "B4" };
    private readonly float[] noteFrequencies = {
        130.81f, 146.83f, 164.81f, 174.61f, 196.00f, 220.00f, 246.94f,
        261.63f, 293.66f, 329.63f, 349.23f, 392.00f, 440.00f, 493.88f,
        523.25f, 587.33f, 659.25f, 698.46f, 783.99f, 880.00f, 987.77f
    };

    private void Start()
    {
        if (song == null) return;
        samples = new float[sampleSize];
        AnalyzeSong();
    }

    private void AnalyzeSong()
    {
        int channels = song.channels;
        int sampleRate = song.frequency;
        int totalFrames = song.samples;

        float[] allData = new float[totalFrames * channels];
        song.GetData(allData, 0);

        for (int frame = 0; frame < totalFrames - sampleSize; frame += sampleSize)
        {
            for (int i = 0; i < sampleSize; i++)
            {
                float mix = 0f;
                for (int c = 0; c < channels; c++)
                {
                    mix += allData[(frame + i) * channels + c];
                }
                samples[i] = mix / channels;
            }

            float time = (float)frame / sampleRate;
            lastAnalyzedTime = time;
            float volume = CalculateVolume();

            if (volume < minimumVolume)
            {
                EndCurrentNote();
                continue;
            }

            string detectedNote = DetectNote();

            if (detectedNote == "")
            {
                EndCurrentNote();
                continue;
            }

            ProcessNote(detectedNote, time);
        }

        EndCurrentNote();
    }

    private void ProcessNote(string detectedNote, float time)
    {
        if (currentNote == "")
        {
            currentNote = detectedNote;
            currentNoteStartTime = time;
            return;
        }

        if (detectedNote == currentNote) return;

        float duration = time - currentNoteStartTime;

        if (duration >= minimumNoteDuration)
        {
            ConfirmNote(currentNote, currentNoteStartTime);
        }

        currentNote = detectedNote;
        currentNoteStartTime = time;
    }

    private void EndCurrentNote()
    {
        if (currentNote == "") return;

        float duration = lastAnalyzedTime - currentNoteStartTime;

        if (duration >= minimumNoteDuration)
        {
            ConfirmNote(currentNote, currentNoteStartTime);
        }

        currentNote = "";
        currentNoteStartTime = -1f;
    }

    private void ConfirmNote(string note, float time)
    {
        if (note == lastDetectedNote)
        {
            if (time - lastDetectedTime < minimumNoteGap) return;
        }

        lastDetectedNote = note;
        lastDetectedTime = time;
        OnNoteDetected?.Invoke(note, time);
    }

    private string DetectNote()
    {
        float[] chromaEnergies = new float[7];
        float totalEnergy = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            totalEnergy += samples[i] * samples[i];
        }

        if (totalEnergy <= 0.000001f) return "";

        float sumEnergy = 0f;

        for (int i = 0; i < noteFrequencies.Length; i++)
        {
            float energy = CalculateFrequencyEnergy(noteFrequencies[i]);
            int chromaIndex = i % 7;
            chromaEnergies[chromaIndex] += energy;
            sumEnergy += energy;
        }

        if (sumEnergy <= 0.000001f) return "";

        int strongestIndex = 0;
        for (int i = 1; i < 7; i++)
        {
            if (chromaEnergies[i] > chromaEnergies[strongestIndex])
            {
                strongestIndex = i;
            }
        }

        float confidence = chromaEnergies[strongestIndex] / sumEnergy;

        if (confidence < minimumConfidence) return "";

        return noteNames[strongestIndex];
    }

    private float CalculateFrequencyEnergy(float targetFrequency)
    {
        float sampleRate = song.frequency;
        float real = 0f;
        float imaginary = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float time = (float)i / sampleRate;
            float angle = 2f * Mathf.PI * targetFrequency * time;
            real += samples[i] * Mathf.Cos(angle);
            imaginary += samples[i] * Mathf.Sin(angle);
        }

        return real * real + imaginary * imaginary;
    }

    private float CalculateVolume()
    {
        float sum = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }

        return sum / samples.Length;
    }
}