using UnityEngine;

public class KeyboardNoteTest : MonoBehaviour
{
    [SerializeField] private KeyboardAudioDetector keyboard;

    [Header("Objetos de las notas")]
    [SerializeField] private KeyboardNoteObject c4Object;
    [SerializeField] private KeyboardNoteObject d4Object;
    [SerializeField] private KeyboardNoteObject e4Object;
    [SerializeField] private KeyboardNoteObject f4Object;
    [SerializeField] private KeyboardNoteObject g4Object;
    [SerializeField] private KeyboardNoteObject a4Object;
    [SerializeField] private KeyboardNoteObject b4Object;

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
        GetNoteObject(note)?.Press();
    }

    private void OnNoteReleased(string note)
    {
        GetNoteObject(note)?.Release();
    }

    private KeyboardNoteObject GetNoteObject(string note)
    {
        switch (note)
        {
            case "C4":
                return c4Object;

            case "D4":
                return d4Object;

            case "E4":
                return e4Object;

            case "F4":
                return f4Object;

            case "G4":
                return g4Object;

            case "A4":
                return a4Object;

            case "B4":
                return b4Object;
        }

        return null;
    }
}
