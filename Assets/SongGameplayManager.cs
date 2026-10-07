using UnityEngine;

public class SongGameplayManager : MonoBehaviour
{
    [SerializeField] private KeyboardAudioDetector playerDetector;

    private string expectedNote = "";

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

    public void SetExpectedNote(string note)
    {
        expectedNote = note;

        Debug.Log(
            "NOTA ESPERADA: " +
            expectedNote
        );
    }

    private void OnPlayerNotePressed(string playerNote)
    {
        Debug.Log(
            "NOTA DEL JUGADOR: " +
            playerNote
        );

        if (playerNote == expectedNote)
        {
            Debug.Log("CORRECTO");
        }
        else
        {
            Debug.Log("INCORRECTO");
        }
    }
}