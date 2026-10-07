using System.Collections.Generic;
using UnityEngine;

public class SongNoteReceiber : MonoBehaviour
{
    [SerializeField] private SongAudioAnalizer analyzer;
    [SerializeField] private AudioSource audioSource;

    [Header("Prefabs de notas")]
    [SerializeField] private SongNoteObject c4Prefab;
    [SerializeField] private SongNoteObject d4Prefab;
    [SerializeField] private SongNoteObject e4Prefab;
    [SerializeField] private SongNoteObject f4Prefab;
    [SerializeField] private SongNoteObject g4Prefab;
    [SerializeField] private SongNoteObject a4Prefab;
    [SerializeField] private SongNoteObject b4Prefab;

    [Header("Puntos")]
    [SerializeField] private Transform noteSpawnPoint;
    [SerializeField] private Transform noteHitPoint;

    [Header("Movimiento")]
    [SerializeField] private float noteTravelTime = 2f;

    [Header("Audio")]
    [SerializeField] private float audioStartDelay = 0.1f;

    private List<SongNote> songNotes = new List<SongNote>();

    private int currentNoteIndex;

    private bool songStarted;

    private double songStartDspTime;

    private void OnEnable()
    {
        if (analyzer != null)
        {
            analyzer.OnNoteDetected += OnNoteDetected;
        }
    }

    private void OnDisable()
    {
        if (analyzer != null)
        {
            analyzer.OnNoteDetected -= OnNoteDetected;
        }
    }

    private void Start()
    {
        Invoke(
            nameof(StartSong),
            0.5f
        );
    }

    private void StartSong()
    {
        if (audioSource == null)
        {
            Debug.LogError(
                "No hay AudioSource asignado."
            );

            return;
        }

        if (analyzer == null)
        {
            Debug.LogError(
                "No hay SongAudioAnalizer asignado."
            );

            return;
        }

        if (noteSpawnPoint == null)
        {
            Debug.LogError(
                "No hay Note Spawn Point asignado."
            );

            return;
        }

        if (noteHitPoint == null)
        {
            Debug.LogError(
                "No hay Note Hit Point asignado."
            );

            return;
        }

        currentNoteIndex = 0;

        songStartDspTime =
            AudioSettings.dspTime +
            audioStartDelay;

        audioSource.PlayScheduled(
            songStartDspTime
        );

        songStarted = true;

        Debug.Log(
            "CANCION PROGRAMADA DSP: " +
            songStartDspTime.ToString("F3")
        );
    }

    private void Update()
    {
        if (!songStarted)
        {
            return;
        }

        if (currentNoteIndex >= songNotes.Count)
        {
            return;
        }

        double currentTime =
            AudioSettings.dspTime -
            songStartDspTime;

        while (
            currentNoteIndex <
            songNotes.Count
        )
        {
            SongNote nextNote =
                songNotes[currentNoteIndex];

            double spawnTime =
                nextNote.time -
                noteTravelTime;

            if (currentTime < spawnTime)
            {
                break;
            }

            SpawnNote(nextNote);

            currentNoteIndex++;
        }
    }

    private void OnNoteDetected(
        string note,
        float time
    )
    {
        SongNote songNote =
            new SongNote(
                note,
                time
            );

        songNotes.Add(songNote);

        Debug.Log(
            "NOTA RECIBIDA: " +
            note +
            " | TIEMPO: " +
            time.ToString("F2")
        );
    }

    private void SpawnNote(
        SongNote songNote
    )
    {
        SongNoteObject prefab =
            GetNotePrefab(
                songNote.note
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No hay prefab asignado para: " +
                songNote.note
            );

            return;
        }

        if (noteSpawnPoint == null)
        {
            Debug.LogError(
                "No hay Note Spawn Point asignado."
            );

            return;
        }

        if (noteHitPoint == null)
        {
            Debug.LogError(
                "No hay Note Hit Point asignado."
            );

            return;
        }

        SongNoteObject newNote =
            Instantiate(
                prefab,
                noteSpawnPoint.position,
                prefab.transform.rotation
            );

        newNote.Initialize(
            songNote.note,
            noteHitPoint.position,
            songNote.time,
            songStartDspTime
        );

        double currentTime =
            AudioSettings.dspTime -
            songStartDspTime;

        Debug.Log(
            "SPAWN: " +
            songNote.note +
            " | TIEMPO OBJETIVO: " +
            songNote.time.ToString("F2") +
            " | TIEMPO ACTUAL: " +
            currentTime.ToString("F3")
        );
    }

    private SongNoteObject GetNotePrefab(
        string note
    )
    {
        switch (note)
        {
            case "C4":
                return c4Prefab;

            case "D4":
                return d4Prefab;

            case "E4":
                return e4Prefab;

            case "F4":
                return f4Prefab;

            case "G4":
                return g4Prefab;

            case "A4":
                return a4Prefab;

            case "B4":
                return b4Prefab;
        }

        return null;
    }
}