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

    public float beatSetDelay = 4;
    public int beatsPerSet = 3;

    void Start()
    {
        if (instrument == null)
            instrument = GetComponent<BasicInstrument>();

        if (instrument != null)
        {
            instrument.running = true;
            StartCoroutine(Ticker());
        }
    }

    private static long TickOf(float seconds)
    {
        return (long)(BasicInstrument.MICROSECONDS_PER_SECOND * seconds);
    }

    private void SendNote(Note note)
    {
        note.Time = TickOf(beatDuration);
        NoteOnMessage noteOn = new NoteOnMessage(note);//, TickOf(beatOffset));
        NoteOffMessage noteOff = new NoteOffMessage(note);//, TickOf(beatDuration));

        Debug.Log("Sending message: " + noteOn);
        Debug.Log("Sending message: " + noteOff);
        instrument.ProcessMidi(new IMessage[] { noteOn, noteOff });
    }

    private IEnumerator Ticker()
    {
        while (true)
        {
            yield return new WaitForSeconds(beatSetDelay);
            for (int i = 0; i < beatsPerSet; i++)
            {
                SendNote(new Note(0, (byte)sequence[sequenceTick].KeyPos(), byte.MaxValue, 0));
                sequenceTick = (sequenceTick + 1) % sequence.Length;
            }
        }
    }
}
