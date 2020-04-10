using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMIDI;
using uMIDI.Common;

public class TestSequencer : MonoBehaviour
{
    public BasicInstrument instrument;

    [SerializeField] private Notes.Note[] sequence = new Notes.Note[1];
    private int sequenceTick = 0;

    // Assume the offset between beats is NOT less than half the duration
    public float beatOffset = 0.5f;
    public float beatDuration = 0.25f;

    void Start()
    {
        if (instrument == null)
            instrument = GetComponent<BasicInstrument>();

        if (instrument != null)
            StartCoroutine(StartTicker());
    }

    private IEnumerator StartTicker()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        instrument.running = true;
        StartCoroutine(Ticker());
    }

    private static TimingTickMessage TickOf(float seconds)
    {
        return new TimingTickMessage((long)(BasicInstrument.MICROSECONDS_PER_SECOND * seconds));
    }

    private IEnumerator Ticker()
    {
        Note prev = new Note(0, 0, 0, 0);
        while (true)
        {
            Note note = new Note(0, (byte)sequence[sequenceTick].KeyPos(), 1, 0);

            NoteOnMessage noteOn = new NoteOnMessage(note, 0);
            NoteOffMessage noteOff = new NoteOffMessage(prev, 0);

            if (beatOffset <= beatDuration)
            {
                // New note on messages come before previous note off messages
                // |-----|    => offset
                // |--------| => duration
                // [--prev--]
                //          |
                //       [--curr--]
                //       |        |
                //             [--next--]
                //             |
                //   On, wait, off, wait
                TimingTickMessage afterOn  = TickOf(beatDuration - beatOffset);
                TimingTickMessage afterOff = TickOf(2 * beatOffset - beatDuration);
                instrument.ProcessMidi(new IMessage[] { noteOn, afterOn, noteOff, afterOff });
            }
            else
            {
                // New note on messages come AFTER previous note off messages
                // |-----------| => offset
                // |--------|    => duration
                // [--prev--]
                //          |
                //             [--curr--]
                //             |        |
                //                         [--next--]
                //                         |
                //   Off, wait, on, wait
                TimingTickMessage afterOff = TickOf(beatOffset);
                TimingTickMessage afterOn  = TickOf(beatDuration);
                instrument.ProcessMidi(new IMessage[] { noteOff, afterOff, noteOn, afterOn });
            }

            sequenceTick = (sequenceTick + 1) % sequence.Length;
            prev = note;
            yield return new WaitForSeconds(beatOffset);
        }
    }
}
