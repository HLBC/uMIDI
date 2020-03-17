using System;
using UnityEngine;
using Notes;

[RequireComponent(typeof(AudioSource))]
public class AudioTest : MonoBehaviour
{
    public Note[] notes = new Note[1];
    public float[] freqs = {};
    public float[] ampls = {};
    private float sampleRate;
    private float step;
    public float phase;

    public bool running = false;

    private float note_step;

    void Start()
    {
        running = false;
        sampleRate = AudioSettings.outputSampleRate;
        step = 1 / sampleRate;
        Debug.Log(sampleRate);

        note_step = Mathf.Pow(2, 1 / 12f);

        freqs = new float[notes.Length];
        for (int i = 0; i < notes.Length; i++)
            freqs[i] = NoteFreq(notes[i].KeyPos());

        ampls = new float[notes.Length];
        for (int i = 0; i < notes.Length; i++)
            ampls[i] = 1f / notes.Length;
    }

    void Update()
    {
        running = (running && Input.GetKeyDown(KeyCode.DownArrow)) 
              || (!running && Input.GetKeyDown(KeyCode.UpArrow))
               ?  !running  : running;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        int samples = data.Length / channels;

        for (int s = 0; s < samples; s++)
        {
            data[s * channels] = MultiSin(freqs, ampls, phase);
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