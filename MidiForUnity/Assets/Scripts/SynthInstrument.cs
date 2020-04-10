using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMIDI;
using uMIDI.Common;

[RequireComponent(typeof(AudioSource))]
public class SynthInstrument : BasicInstrument
{
    private float sampleRate;
    private float step;

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
            data[s * channels] = MultiSin(Notes());
            if (channels > 1)
                data[s * channels + 1] = data[s * channels];
            UpdateNotes(step);
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
}
