using System;
using UnityEngine;
using Notes;

[RequireComponent(typeof(AudioSource))]
public class AudioTest : MonoBehaviour
{
    private float[] freqs = {};
    private float[] amps = {};
    private float sampleRate;
    private float step;
    private float phase;

    public bool running = false;

    private float note_step;

    void Start()
    {
        TurnOff();

        sampleRate = AudioSettings.outputSampleRate;
        step = 1 / sampleRate;

        note_step = Mathf.Pow(2, 1 / 12f);
    }

    public void TurnOn()
    {
        running = true;
    }

    public void TurnOff()
    {
        running = false;
    }

    public void SetNotes(Note[] notes)
    {
        float[] freqs = new float[notes.Length];
        for (int i = 0; i < notes.Length; i++)
            freqs[i] = NoteFreq(notes[i].KeyPos());

        float [] amps = new float[notes.Length];
        for (int i = 0; i < notes.Length; i++)
            amps[i] = 1f / notes.Length;

        SetAmpFreqPairs(freqs, amps);
    }

    public void SetAmpFreqPairs(float[] freqs, float[] amps)
    {
        this.freqs = (float[])freqs.Clone();
        this.amps = new float[freqs.Length];
        for (int i = 0; i < this.amps.Length; i++)
            this.amps[i] = (i < amps.Length ? amps[i] : 0);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        float[] curr_freqs = (float[])freqs.Clone();
        float[] curr_amps = (float[])amps.Clone();

        if (!running)
            return;

        int samples = data.Length / channels;

        for (int s = 0; s < samples; s++)
        {
            data[s * channels] = MultiSin(curr_freqs, curr_amps, phase);
            if (channels > 1)
                data[s * channels + 1] = data[s * channels];
            // Mathf.Sin(t) -> 1 cycle per 2pi seconds
            // Mathf.Sin(2pi*t) -> 1 cycle per 1 second
            phase += step;
        }
    }

    float MultiSin(float[] freq, float[] amp, float phase)
    {
        float sum = 0;
        for (int i = 0; i < freq.Length; i++)
            sum += (i < amp.Length ? amp[i] : 0) * Mathf.Sin(freq[i] * 2 * Mathf.PI * phase);
        return sum;
    }

    float NoteFreq(int n)
    {
        return 440 * Mathf.Pow(note_step, (n - 49));
    }
}