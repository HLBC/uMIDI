using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Notes;

public class ChordSequencer : MonoBehaviour
{
    [SerializeField] private Note[] sequence = new Note[1];
    private int sequenceTick = 0;
    [SerializeField] private int[] chordIntervals = { 3, 5 };

    [SerializeField] private AudioTest source;
    [Tooltip("Tempo is quarter note beats per minute.")]
    [SerializeField] private int tempo = 90;
    private float tickTime;

    [SerializeField] private float delayTicker = 1;
    [SerializeField] private float percentNoteOn = 0.9f;

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioTest>();
        tickTime = 60f / tempo; // seconds per beat

        if (source != null)
            StartCoroutine(StartTicker());
    }

    private IEnumerator StartTicker()
    {
        yield return new WaitForSecondsRealtime(delayTicker);
        source.TurnOn();
        StartCoroutine(Ticker());
    }

    private IEnumerator Ticker()
    {
        while (true)
        {
            Note[] chord = new Note[chordIntervals.Length + 1];
            chord[0] = sequence[sequenceTick]; // base
            Debug.Log("Chord:");
            Debug.Log("Note: " + chord[0].Name + chord[0].Octave);
            for (int i = 1; i < chord.Length; i++)
            {
                //Debug.Log(chordIntervals[i - 1] + " " + HalfSteps(chordIntervals[i - 1]));
                int name = (int)chord[0].Name + HalfSteps(chordIntervals[i - 1]);
                chord[i] = new Note((NoteName)(name % Note.ALL_NOTES),
                                    chord[0].Octave + name / Note.ALL_NOTES);
                Debug.Log("Note: " + chord[i].Name + chord[i].Octave);
            }

            source.SetNotes(chord);

            yield return new WaitForSecondsRealtime(tickTime * percentNoteOn);

            source.SetNotes(new Note[0]);
            yield return new WaitForSecondsRealtime(tickTime * (1 - percentNoteOn));

            sequenceTick = (sequenceTick + 1) % sequence.Length;
        }
    }

    int HalfSteps(int intervalAboveBase)
    {
        int x = (intervalAboveBase - 1) / 7;
        return 2 * (intervalAboveBase - 1) - 2 * x + ((intervalAboveBase - 1) < 7 * x + 3 ? 0 : 1);
    }
}
