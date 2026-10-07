using UnityEngine;

public class KeyboardNoteTest : MonoBehaviour
{
    [SerializeField] private KeyboardAudioDetector keyboard;

    private void OnEnable()
    {
        if (keyboard == null)
            return;

        keyboard.OnNotePressed += OnNotePressed;
        keyboard.OnNoteReleased += OnNoteReleased;
    }

    private void OnDisable()
    {
        if (keyboard == null)
            return;

        keyboard.OnNotePressed -= OnNotePressed;
        keyboard.OnNoteReleased -= OnNoteReleased;
    }

    private void OnNotePressed(string note)
    {
        Debug.Log(">>> JUGADOR TOCÓ: " + note);

        switch (note)
        {
            case "C4":
                Debug.Log("DO");
                break;

            case "D4":
                Debug.Log("RE");
                break;

            case "E4":
                Debug.Log("MI");
                break;

            case "F4":
                Debug.Log("FA");
                break;

            case "G4":
                Debug.Log("SOL");
                break;

            case "A4":
                Debug.Log("LA");
                break;

            case "B4":
                Debug.Log("SI");
                break;
        }
    }

    private void OnNoteReleased(string note)
    {
        Debug.Log(">>> JUGADOR SOLTÓ: " + note);
    }
}