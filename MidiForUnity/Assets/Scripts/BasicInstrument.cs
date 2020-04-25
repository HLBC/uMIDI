using System.Collections.Generic;
using UnityEngine;
using uMIDI;
using uMIDI.Common;
using System;

public abstract class BasicInstrument : MonoBehaviour, IMidiInstrument
{
    private Queue<IMessage> buffer = new Queue<IMessage>();
    private long ticks = 0; // TODO: Treats all times as microseconds
    public bool running = false;

    public static readonly int ALL_CHANNELS = -1;
    public int channel = ALL_CHANNELS;

    protected abstract void UpdateNotes(float timeDelta);

    protected abstract void StartNote(Note note);

    protected abstract void StopNote(Note note);

    #region Timer Checking
    public static readonly long MICROSECONDS_PER_SECOND = 1000000;

    // Note: delta is in seconds
    private void UpdateTickDelay(float delta)
    {
        ticks -= (long)(MICROSECONDS_PER_SECOND * delta);
        if (ticks < 0)
            ticks = 0;
    }

    // Note: delta is in seconds
    protected void ProcessTimeDelta(float delta)
    {
        UpdateNotes(delta);
        UpdateTickDelay(delta);

        if (ticks == 0)
            ProcessBuffer();
    }
    #endregion

    #region IMessage Processing Functions
    private readonly Func<IMessage, BasicInstrument, bool>[] IMESSAGE_PROCESS_FUNCS =
        { OnNoteOn, OnNoteOff, OnTimeTickMessage };

    private static bool OnNoteOn(IMessage message, BasicInstrument inst)
    {
        return CheckAndApply<NoteOnMessage>(message, (msg) => {
                if (inst.MatchesChannel(msg.Note))
                    inst.StartNote(msg.Note);
        });
    }

    private static bool OnNoteOff(IMessage message, BasicInstrument inst)
    {
        return CheckAndApply<NoteOffMessage>(message, (msg) => {
            if (inst.MatchesChannel(msg.Note))
                inst.StopNote(msg.Note);
        });
    }

    private static bool OnTimeTickMessage(IMessage message, BasicInstrument inst)
    {
        // TODO: Once MidiStream.MillisecondsPerTick is implemented, this needs to use that.
        return CheckAndApply<TimingTickMessage>(message, (msg) => inst.ticks = msg.TimeDelta);
    }

    private bool MatchesChannel(Note note)
    {
        return channel == ALL_CHANNELS || channel == note.Channel;
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

    #region Midi Message Buffering/Processing
    /*
     * The processor needs a way to wait on a timed message without throwing
     * away the message once it has been removed from the queue. To solve this,
     * the processor looks for explicit timing messages and pre-processes the
     * messages by inserting explicity timing messages when appropriate.
     */
    public void ProcessMidi(IMessage[] messages)
    {
        lock (buffer)
        {
            foreach (IMessage message in messages)
            {
                // If the message has a time, insert a timing message before it.
                if (message.TimeDelta > 0 && !IsTimeMessage(message))
                    buffer.Enqueue(new TimingTickMessage(message.TimeDelta));

                //Debug.Log("Buffering Message: " + message);
                buffer.Enqueue(message);
            }
        }
    }

    /*
     * Continues to iterate through the buffer until empty or a timing message
     * is reached.
     */
    private void ProcessBuffer()
    {
        // TODO: Implement priority locking so checking buffer never held up by the adding to it
        lock (buffer)
        {
            IMessage next = null;
            while (!IsTimeMessage(next) && buffer.Count > 0)
            {
                next = buffer.Dequeue();
                //Debug.Log("Processing Message: " + next);
                foreach (Func<IMessage, BasicInstrument, bool> func in IMESSAGE_PROCESS_FUNCS)
                    if (func.Invoke(next, this))
                        continue;
            }
        }
    }

    // Class used to declare explicit time separations between messages being processed.
    private class TimingTickMessage : IMessage
    {
        public long TimeDelta { get; }

        public TimingTickMessage(long time)
        {
            TimeDelta = time;
        }

        public MidiMessage Message => throw new InvalidOperationException();
    }
    #endregion
}
