using System;
using UnityEngine;

public class KeyboardAudioDetector : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private int sampleSize = 4096;
    [SerializeField] private float minimumVolume = 0.015f;

    [Header("Detection")]
    [SerializeField] private float noteTolerance = 0.05f;
    [SerializeField] private float noteStabilityTime = 0.08f;
    [SerializeField] private float releaseTime = 0.15f;

    public event Action<string> OnNotePressed;
    public event Action<string> OnNoteReleased;

    private AudioClip microphoneClip;
    private string microphoneDevice;

    private float[] samples;

    private string currentNote = "";
    private string candidateNote = "";

    private float candidateTimer;
    private float silenceTimer;

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
        if (Microphone.devices.Length == 0)
        {

            return;
        }

        foreach (string device in Microphone.devices)
        {

        }

        microphoneDevice = Microphone.devices[0];

        microphoneClip = Microphone.Start(
            microphoneDevice,
            true,
            10,
            44100
        );

        samples = new float[sampleSize];

    }

    private void Update()
    {
        if (microphoneClip == null)
            return;

        int position = Microphone.GetPosition(microphoneDevice);

        if (position < sampleSize)
            return;

        microphoneClip.GetData(
            samples,
            position - sampleSize
        );

        float volume = CalculateVolume();

        if (volume < minimumVolume)
        {
            HandleSilence();
            return;
        }

        silenceTimer = 0f;

        float frequency = DetectFrequency();

        if (frequency <= 0f)
            return;

        string detectedNote = GetClosestNote(frequency);

        if (detectedNote == "")
            return;

        HandleNote(detectedNote, frequency);
    }

    private void HandleNote(string detectedNote, float frequency)
    {
        // Si estamos tocando la misma nota, no hacemos nada.
        if (detectedNote == currentNote)
        {
            candidateNote = "";
            candidateTimer = 0f;
            return;
        }

        // Si encontramos una nueva nota
        if (detectedNote != candidateNote)
        {
            candidateNote = detectedNote;
            candidateTimer = 0f;
        }

        candidateTimer += Time.deltaTime;

        // Esperamos un pequeño tiempo para confirmar la nota
        if (candidateTimer >= noteStabilityTime)
        {
            ChangeNote(detectedNote, frequency);
        }
    }

    private void ChangeNote(string newNote, float frequency)
    {
        // Si había una nota anterior, la soltamos.
        if (!string.IsNullOrEmpty(currentNote))
        {
            OnNoteReleased?.Invoke(currentNote);


        }

        currentNote = newNote;

        OnNotePressed?.Invoke(currentNote);


        candidateNote = "";
        candidateTimer = 0f;
    }

    private void HandleSilence()
    {
        candidateNote = "";
        candidateTimer = 0f;

        if (string.IsNullOrEmpty(currentNote))
            return;

        silenceTimer += Time.deltaTime;

        if (silenceTimer >= releaseTime)
        {
            OnNoteReleased?.Invoke(currentNote);



            currentNote = "";
            silenceTimer = 0f;
        }
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

    private float DetectFrequency()
    {
        int minPeriod = Mathf.FloorToInt(
            microphoneClip.frequency / 1000f
        );

        int maxPeriod = Mathf.CeilToInt(
            microphoneClip.frequency / 150f
        );

        float bestCorrelation = 0f;
        int bestPeriod = 0;

        for (int period = minPeriod; period <= maxPeriod; period++)
        {
            float correlation = 0f;

            for (int i = 0; i < samples.Length - period; i++)
            {
                correlation += samples[i] * samples[i + period];
            }

            correlation /= samples.Length - period;

            if (correlation > bestCorrelation)
            {
                bestCorrelation = correlation;
                bestPeriod = period;
            }
        }

        if (bestPeriod == 0)
            return 0f;

        return (float)microphoneClip.frequency / bestPeriod;
    }

    private string GetClosestNote(float frequency)
    {
        float closestDifference = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < noteFrequencies.Length; i++)
        {
            float difference = Mathf.Abs(
                frequency - noteFrequencies[i]
            );

            if (difference < closestDifference)
            {
                closestDifference = difference;
                closestIndex = i;
            }
        }

        if (closestIndex == -1)
            return "";

        float tolerance =
            noteFrequencies[closestIndex] * noteTolerance;

        if (closestDifference <= tolerance)
        {
            return noteNames[closestIndex];
        }

        return "";
    }

    private void OnDestroy()
    {
        if (Microphone.IsRecording(microphoneDevice))
        {
            Microphone.End(microphoneDevice);
        }
    }
}