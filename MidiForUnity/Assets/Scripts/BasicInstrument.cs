using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMIDI;
using uMIDI.Common;
using System;

public class BasicInstrument : MonoBehaviour, IMidiInstrument
{
    private static readonly float FREQ_STEP = Mathf.Pow(2, 1 / 12f);

    private List<TimedNote> notes = new List<TimedNote>();
    private Queue<IMessage> buffer = new Queue<IMessage>();
    private long ticks = 0; // TODO: Treats all times as microseconds

    public bool running = false;

    protected List<TimedNote> Notes()
    {
        return notes;
    }

    public void ProcessMidi(IMessage[] messages)
    {
        lock (buffer)
        {
            foreach (IMessage message in messages)
            {
                Debug.Log("Buffering Message: " + message);
                buffer.Enqueue(message);
            }
        }
    }

    #region IMessage Processing Functions
    private readonly Func<IMessage, BasicInstrument, bool>[] IMESSAGE_PROCESS_FUNCS =
        { OnNoteOn, OnNoteOff, OnTimeTickMessage };

    private static bool OnNoteOn(IMessage message, BasicInstrument inst)
    {
        return CheckAndApply<NoteOnMessage>(message, (msg) => inst.AddNote(msg.Note));
    }

    private static bool OnNoteOff(IMessage message, BasicInstrument inst)
    {
        return CheckAndApply<NoteOffMessage>(message, (msg) => inst.RemoveNote(msg.Note));
    }

    private static bool OnTimeTickMessage(IMessage message, BasicInstrument inst)
    {
        return CheckAndApply<TimingTickMessage>(message, (msg) => inst.ticks = msg.Time);
    }

    private static bool CheckAndApply<T>(IMessage message, Action<T> act)
    {
        if (message is T typed)
        {
            act.Invoke(typed);
            return true;
        }
        return false;
    }

    private static bool IsTimeMessage(IMessage message)
    {
        return (message as TimingTickMessage) != null;
    }
    #endregion

    private void ProcessBuffer()
    {
        // TODO: Implement priority locking so checking buffer never held up by the adding to it
        lock (buffer)
        {
            IMessage next = null;
            while (!IsTimeMessage(next) && buffer.Count > 0)
            {
                next = buffer.Dequeue();
                Debug.Log("Processing Message: " + next);
                foreach (Func<IMessage, BasicInstrument, bool> func in IMESSAGE_PROCESS_FUNCS)
                    if (func.Invoke(next, this))
                        continue;
            }
        }
    }

    // Note: delta is in seconds
    protected void UpdateNotes(float delta)
    {
        foreach (TimedNote note in notes)
            note.AddTime(delta);
        UpdateTickDelay(delta);

        if (ticks == 0)
            ProcessBuffer();
    }

    public static readonly long MICROSECONDS_PER_SECOND = 1000000;
    // Note: delta is in seconds
    private void UpdateTickDelay(float delta)
    {
        ticks -= (long)(MICROSECONDS_PER_SECOND * delta);
        if (ticks < 0)
            ticks = 0;
    }

    protected void AddNote(Note note)
    {
        notes.Add(new TimedNote(note));
    }

    protected void RemoveNote(Note note)
    {
        notes.Remove(new TimedNote(note));
    }

    #region TimedNote Class and Functions
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
