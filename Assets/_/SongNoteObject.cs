using UnityEngine;

public class SongNoteObject : MonoBehaviour
{
    private string note;

    private Vector3 targetPosition;

    private float targetSongTime;

    private double songStartDspTime;

    private bool initialized;

    public void Initialize(
        string noteName,
        Vector3 target,
        float targetTime,
        double startDspTime
    )
    {
        note = noteName;

        targetPosition = target;

        targetSongTime = targetTime;

        songStartDspTime = startDspTime;

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        double currentSongTime =
            AudioSettings.dspTime -
            songStartDspTime;

        double remainingTime =
            targetSongTime -
            currentSongTime;

        if (remainingTime <= 0.0)
        {
            transform.position =
                targetPosition;

            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                targetPosition
            );

        if (distance <= 0.001f)
        {
            transform.position =
                targetPosition;

            return;
        }

        float speed =
            distance /
            (float)remainingTime;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("NoteHit"))
        {
            return;
        }

        Debug.Log(
            "NOTA LLEGÓ AL COLLIDER: " +
            note
        );

        SongGameplayManager gameplayManager =
            FindFirstObjectByType<SongGameplayManager>();

        if (gameplayManager != null)
        {
            gameplayManager.SetExpectedNote(
                note
            );
        }

        Destroy(gameObject);
    }
}