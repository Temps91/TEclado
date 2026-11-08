using UnityEngine;

public class SongNoteObject : MonoBehaviour
{
    public string NoteName { get; private set; }

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float targetSongTime;
    private double songStartDspTime;
    private float noteTravelTime;
    private bool initialized;
    private bool isHit;
    private SongGameplayManager manager;

    public void Initialize(string noteName, Vector3 target, float targetTime, double startDspTime, float travelTime)
    {
        NoteName = noteName;
        startPosition = transform.position;
        targetPosition = target;
        targetSongTime = targetTime;
        songStartDspTime = startDspTime;
        noteTravelTime = travelTime;
        initialized = true;
        manager = FindFirstObjectByType<SongGameplayManager>();
    }

    private void Update()
    {
        if (!initialized || isHit) return;

        double currentSongTime = AudioSettings.dspTime - songStartDspTime;
        double spawnTime = targetSongTime - noteTravelTime;
        float progress = (float)((currentSongTime - spawnTime) / noteTravelTime);

        transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, progress);

        if (progress > 1.5f)
        {
            if (manager != null) manager.RegisterMiss();
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        isHit = true;
        Destroy(gameObject);
    }
}