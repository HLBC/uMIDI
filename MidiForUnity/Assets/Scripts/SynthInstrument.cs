using System.Collections.Generic;
using UnityEngine;
using uMIDI.Common;

[RequireComponent(typeof(AudioSource))]
public class SynthInstrument : BasicInstrument
{
    private static readonly float FREQ_STEP = Mathf.Pow(2, 1 / 12f);

    private float sampleRate;
    private float step;

    private List<TimedNote> notes = new List<TimedNote>();

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        step = 1 / sampleRate;
        Debug.Log(step);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        int samples = data.Length / channels;

        for (int s = 0; s < samples; s++)
        {
            data[s * channels] = MultiSin(notes);
            if (channels > 1)
                data[s * channels + 1] = data[s * channels];
            ProcessTimeDelta(step);
        }
    }

    float MultiSin(List<TimedNote> notes)
    {
        float sum = 0;
        foreach (TimedNote note in notes)
            sum += Mathf.Sin(note.Freq * 2 * Mathf.PI * note.Time);
                // Mathf.Sin(t) -> 1 cycle per 2pi seconds
                // Mathf.Sin(2pi*t) -> 1 cycle per 1 second
        return sum / notes.Count;
    }

    #region Note Manipulation and TimedNote Class
    protected override void UpdateNotes(float delta)
    {
        foreach (TimedNote note in notes)
            note.AddTime(delta);
    }

    protected override void StartNote(Note note)
    {
        notes.Add(new TimedNote(note));
    }

    protected override void StopNote(Note note)
    {
        notes.Remove(new TimedNote(note));
    }

    private static float NoteFreq(int n)
    {
        return 440 * Mathf.Pow(FREQ_STEP, (n - 49));
    }

    protected class TimedNote
    {
        private byte id; // Used to track equality with more rigor than float
        public float Freq { get; }
        public float Velocity { get; }
        public float Time { get; private set; }

        public TimedNote(Note note)
        {
            id = note.Pitch;
            Freq = NoteFreq(note.Pitch);
            Velocity = note.Velocity;
            Time = 0;
        }

        public void AddTime(float delta)
        {
            if (delta > 0)
                Time += delta;
        }

        public override bool Equals(object obj)
        {
            TimedNote other = obj as TimedNote;
            if (other == null)
                return false;
            return other.id == this.id;
        }

        public override int GetHashCode() { return id; }
    }
    #endregion
}
