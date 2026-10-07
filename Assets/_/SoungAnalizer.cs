using System;
using UnityEngine;

public class SongAudioAnalizer : MonoBehaviour
{
    public event Action<string, float> OnNoteDetected;

    [SerializeField] private AudioClip song;

    [Header("Analisis")]
    [SerializeField] private int sampleSize = 4096;
    [SerializeField] private float minimumVolume = 0.01f;

    [Header("Deteccion")]
    [SerializeField] private float minimumConfidence = 0.55f;
    [SerializeField] private float minimumNoteDuration = 0.08f;
    [SerializeField] private float minimumNoteGap = 0.08f;

    private float[] samples;

    private string currentNote = "";
    private float currentNoteStartTime = -1f;

    private string lastDetectedNote = "";
    private float lastDetectedTime = -999f;
    private float lastAnalyzedTime = 0f;

    private readonly string[] noteNames =
    {
        "C4",
        "D4",
        "E4",
        "F4",
        "G4",
        "A4",
        "B4"
    };

    private readonly float[] noteFrequencies =
    {
        261.63f,
        293.66f,
        329.63f,
        349.23f,
        392.00f,
        440.00f,
        493.88f
    };

    private void Start()
    {
        if (song == null)
        {
            Debug.LogError("No hay ninguna cancion asignada.");
            return;
        }

        samples = new float[sampleSize];

        AnalyzeSong();
    }

    private void AnalyzeSong()
    {
        int totalSamples = song.samples;

        for (
            int position = 0;
            position < totalSamples - sampleSize;
            position += sampleSize
        )
        {
            song.GetData(samples, position);

            float time =
                (float)position / song.frequency;

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

            ProcessNote(
                detectedNote,
                time
            );
        }

        EndCurrentNote();
    }

    private void ProcessNote(
        string detectedNote,
        float time
    )
    {
        if (currentNote == "")
        {
            currentNote = detectedNote;
            currentNoteStartTime = time;
            return;
        }

        if (detectedNote == currentNote)
            return;

        float duration =
            time - currentNoteStartTime;

        if (duration >= minimumNoteDuration)
        {
            ConfirmNote(
                currentNote,
                currentNoteStartTime
            );
        }

        currentNote = detectedNote;
        currentNoteStartTime = time;
    }

    private void EndCurrentNote()
    {
        if (currentNote == "")
            return;

        float duration =
            lastAnalyzedTime -
            currentNoteStartTime;

        if (duration >= minimumNoteDuration)
        {
            ConfirmNote(
                currentNote,
                currentNoteStartTime
            );
        }

        currentNote = "";
        currentNoteStartTime = -1f;
    }

    private void ConfirmNote(
        string note,
        float time
    )
    {
        if (note == lastDetectedNote)
        {
            if (
                time - lastDetectedTime <
                minimumNoteGap
            )
            {
                return;
            }
        }

        lastDetectedNote = note;
        lastDetectedTime = time;

        Debug.Log(
            "NOTA CONFIRMADA: " +
            note +
            " | TIEMPO: " +
            time.ToString("F2") +
            " s"
        );

        OnNoteDetected?.Invoke(
            note,
            time
        );
    }

    private string DetectNote()
    {
        float[] energies =
            new float[noteFrequencies.Length];

        float totalEnergy = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float sample = samples[i];

            totalEnergy +=
                sample * sample;
        }

        if (totalEnergy <= 0.000001f)
            return "";

        for (
            int i = 0;
            i < noteFrequencies.Length;
            i++
        )
        {
            energies[i] =
                CalculateFrequencyEnergy(
                    noteFrequencies[i]
                );
        }

        int strongestIndex = 0;

        for (
            int i = 1;
            i < energies.Length;
            i++
        )
        {
            if (
                energies[i] >
                energies[strongestIndex]
            )
            {
                strongestIndex = i;
            }
        }

        float strongestEnergy =
            energies[strongestIndex];

        float sumEnergy = 0f;

        for (int i = 0; i < energies.Length; i++)
        {
            sumEnergy += energies[i];
        }

        if (sumEnergy <= 0.000001f)
            return "";

        float confidence =
            strongestEnergy /
            sumEnergy;

        if (
            confidence <
            minimumConfidence
        )
        {
            return "";
        }

        return noteNames[strongestIndex];
    }

    private float CalculateFrequencyEnergy(
        float targetFrequency
    )
    {
        float sampleRate =
            song.frequency;

        float real = 0f;
        float imaginary = 0f;

        for (
            int i = 0;
            i < samples.Length;
            i++
        )
        {
            float time =
                (float)i / sampleRate;

            float angle =
                2f *
                Mathf.PI *
                targetFrequency *
                time;

            real +=
                samples[i] *
                Mathf.Cos(angle);

            imaginary +=
                samples[i] *
                Mathf.Sin(angle);
        }

        return
            real * real +
            imaginary * imaginary;
    }

    private float CalculateVolume()
    {
        float sum = 0f;

        for (
            int i = 0;
            i < samples.Length;
            i++
        )
        {
            sum +=
                Mathf.Abs(samples[i]);
        }

        return sum / samples.Length;
    }
}