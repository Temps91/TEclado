using System;

[Serializable]
public class SongNote
{
    public string note;
    public float time;

    public SongNote(string note, float time)
    {
        this.note = note;
        this.time = time;
    }
}