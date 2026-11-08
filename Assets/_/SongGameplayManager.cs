using UnityEngine;
using UnityEngine.UI;

public class SongGameplayManager : MonoBehaviour
{
    [SerializeField] private KeyboardAudioDetector playerDetector;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float hitTolerance = 1.5f;

    [SerializeField] private AudioClip c4Sound;
    [SerializeField] private AudioClip d4Sound;
    [SerializeField] private AudioClip e4Sound;
    [SerializeField] private AudioClip f4Sound;
    [SerializeField] private AudioClip g4Sound;
    [SerializeField] private AudioClip a4Sound;
    [SerializeField] private AudioClip b4Sound;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text comboText;

    private int score = 0;
    private int combo = 0;
    private int multiplier = 1;

    private void OnEnable()
    {
        if (playerDetector != null)
        {
            playerDetector.OnNotePressed += OnPlayerNotePressed;
        }
    }

    private void OnDisable()
    {
        if (playerDetector != null)
        {
            playerDetector.OnNotePressed -= OnPlayerNotePressed;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnPlayerNotePressed(string playerNote)
    {
        PlayNoteSound(playerNote);

        SongNoteObject[] activeNotes = FindObjectsByType<SongNoteObject>(FindObjectsSortMode.None);
        SongNoteObject closestNote = null;
        float closestDistance = float.MaxValue;

        foreach (SongNoteObject noteObj in activeNotes)
        {
            if (noteObj.NoteName == playerNote)
            {
                float distance = Vector3.Distance(noteObj.transform.position, hitPoint.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNote = noteObj;
                }
            }
        }

        if (closestNote != null && closestDistance <= hitTolerance)
        {
            closestNote.Hit();
            AddScore();
        }
        else
        {
            RegisterMiss();
        }
    }

    public void RegisterMiss()
    {
        combo = 0;
        multiplier = 1;
        score -= 10;
        UpdateUI();
    }

    private void AddScore()
    {
        combo++;

        if (combo >= 2)
        {
            multiplier = combo;
        }
        else
        {
            multiplier = 1;
        }

        score += 10 * multiplier;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "PUNTOS: " + score;
        }

        if (comboText != null)
        {
            if (multiplier > 1)
            {
                comboText.text = "x" + multiplier;
            }
            else
            {
                comboText.text = "";
            }
        }
    }

    private void PlayNoteSound(string note)
    {
        if (audioSource == null) return;

        switch (note)
        {
            case "C4": if (c4Sound != null) audioSource.PlayOneShot(c4Sound); break;
            case "D4": if (d4Sound != null) audioSource.PlayOneShot(d4Sound); break;
            case "E4": if (e4Sound != null) audioSource.PlayOneShot(e4Sound); break;
            case "F4": if (f4Sound != null) audioSource.PlayOneShot(f4Sound); break;
            case "G4": if (g4Sound != null) audioSource.PlayOneShot(g4Sound); break;
            case "A4": if (a4Sound != null) audioSource.PlayOneShot(a4Sound); break;
            case "B4": if (b4Sound != null) audioSource.PlayOneShot(b4Sound); break;
        }
    }
}