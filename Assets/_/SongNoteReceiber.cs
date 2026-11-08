using System.Collections.Generic;
using UnityEngine;

public class SongNoteReceiber : MonoBehaviour
{
    [SerializeField] private int selectedSong = 0;

    [Header("Velocidad de Canciones")]
    [SerializeField] private float estrellitaSpeed = 0.5f;
    [SerializeField] private float himnoSpeed = 1.0f;
    [SerializeField] private float laLaLandSpeed = 1.25f;

    [Header("Configuracion General")]
    [SerializeField] private SongNoteObject c4Prefab;
    [SerializeField] private SongNoteObject d4Prefab;
    [SerializeField] private SongNoteObject e4Prefab;
    [SerializeField] private SongNoteObject f4Prefab;
    [SerializeField] private SongNoteObject g4Prefab;
    [SerializeField] private SongNoteObject a4Prefab;
    [SerializeField] private SongNoteObject b4Prefab;

    [SerializeField] private Transform noteSpawnPoint;
    [SerializeField] private Transform noteHitPoint;

    [SerializeField] private float noteTravelTime = 2f;
    [SerializeField] private float startDelay = 2f;

    [SerializeField] private List<SongNote> songNotes = new List<SongNote>();

    private int currentNoteIndex;
    private bool songStarted;
    private double songStartDspTime;

    private void Start()
    {
        if (selectedSong == 0)
        {
            LoadEstrellita();
        }
        else if (selectedSong == 1)
        {
            LoadHimnoAlegria();
        }
        else
        {
            LoadLaLaLand();
        }

        Invoke(nameof(StartSong), 1f);
    }

    private void LoadEstrellita()
    {
        songNotes.Clear();
        float t = 0f;
        float step = estrellitaSpeed;

        string[] notes = {
            "C4", "C4", "G4", "G4", "A4", "A4", "G4",
            "F4", "F4", "E4", "E4", "D4", "D4", "C4",
            "G4", "G4", "F4", "F4", "E4", "E4", "D4",
            "G4", "G4", "F4", "F4", "E4", "E4", "D4",
            "C4", "C4", "G4", "G4", "A4", "A4", "G4",
            "F4", "F4", "E4", "E4", "D4", "D4", "C4"
        };

        for (int i = 0; i < notes.Length; i++)
        {
            songNotes.Add(new SongNote(notes[i], t));

            if (i == 6 || i == 13 || i == 20 || i == 27 || i == 34 || i == 41)
            {
                t += step * 2f;
            }
            else
            {
                t += step;
            }
        }
    }

    private void LoadHimnoAlegria()
    {
        songNotes.Clear();
        float t = 0f;
        float step = himnoSpeed;

        string[] notes = {
            "E4", "E4", "F4", "G4", "G4", "F4", "E4", "D4",
            "C4", "C4", "D4", "E4", "E4", "D4", "D4",
            "E4", "E4", "F4", "G4", "G4", "F4", "E4", "D4",
            "C4", "C4", "D4", "E4", "D4", "C4", "C4"
        };

        for (int i = 0; i < notes.Length; i++)
        {
            songNotes.Add(new SongNote(notes[i], t));

            if (i == 13 || i == 28)
            {
                t += step * 1.5f;
            }
            else if (i == 14 || i == 29)
            {
                t += step * 2f;
            }
            else
            {
                t += step;
            }
        }
    }

    private void LoadLaLaLand()
    {
        songNotes.Clear();
        float t = 0f;
        float step = laLaLandSpeed;

        string[] notes = {
            "D4", "E4", "F4", "A4", "G4",
            "F4", "E4", "D4", "E4", "C4",
            "D4", "E4", "F4", "A4", "G4",
            "F4", "E4", "D4", "E4", "C4"
        };

        for (int i = 0; i < notes.Length; i++)
        {
            songNotes.Add(new SongNote(notes[i], t));

            if (i == 4 || i == 9 || i == 14 || i == 19)
            {
                t += step * 2f;
            }
            else
            {
                t += step;
            }
        }
    }

    private void StartSong()
    {
        if (noteSpawnPoint == null || noteHitPoint == null) return;

        currentNoteIndex = 0;
        songStartDspTime = AudioSettings.dspTime + startDelay;
        songStarted = true;
    }

    private void Update()
    {
        if (!songStarted || currentNoteIndex >= songNotes.Count) return;

        double currentTime = AudioSettings.dspTime - songStartDspTime;

        while (currentNoteIndex < songNotes.Count)
        {
            SongNote nextNote = songNotes[currentNoteIndex];
            double spawnTime = nextNote.time - noteTravelTime;

            if (currentTime < spawnTime) break;

            SpawnNote(nextNote);
            currentNoteIndex++;
        }
    }

    private void SpawnNote(SongNote songNote)
    {
        SongNoteObject prefab = GetNotePrefab(songNote.note);

        if (prefab == null) return;

        SongNoteObject newNote = Instantiate(prefab, noteSpawnPoint.position, prefab.transform.rotation);
        newNote.Initialize(songNote.note, noteHitPoint.position, songNote.time, songStartDspTime, noteTravelTime);
    }

    private SongNoteObject GetNotePrefab(string note)
    {
        switch (note)
        {
            case "C4": return c4Prefab;
            case "D4": return d4Prefab;
            case "E4": return e4Prefab;
            case "F4": return f4Prefab;
            case "G4": return g4Prefab;
            case "A4": return a4Prefab;
            case "B4": return b4Prefab;
        }
        return null;
    }
}