using System.Collections.Generic;
using UnityEngine;
using uMIDI.Common;

[RequireComponent(typeof(AudioSource))]
public class SynthInstrument : BasicInstrument
{
    private float sampleRate;
    private float step;

    private List<SynthNote> notes = new List<SynthNote>();

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

    float MultiSin(List<SynthNote> notes)
    {
        float sum = 0;
        foreach (SynthNote note in notes)
            sum += note.Amp * Mathf.Sin(note.Freq * 2 * Mathf.PI * note.Time);
                // Mathf.Sin(t) -> 1 cycle per 2pi seconds
                // Mathf.Sin(2pi*t) -> 1 cycle per 1 second
        return sum / notes.Count;
    }

    protected override void UpdateNotes(float delta)
    {
        notes.RemoveAll((SynthNote obj) => obj.HasDecayed());
        foreach (SynthNote note in notes)
            note.AddTime(delta);
    }

    protected override void StartNote(Note note)
    {
        notes.Add(new SynthNote(note));
    }

    protected override void StopNote(Note note)
    {
        notes.Find((SynthNote obj) => obj.Equals(note)).Release();
    }
}
