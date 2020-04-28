using System;
namespace uMIDI.IO
{
    public class MidiClock
    {
        public long CurrentTick { get; private set; }

        public int TicksPerBeat { get; set; }

        public double CurrentBeat
        {
            get
            {
                return (double) CurrentTick / TicksPerBeat;
            }
        }

        public MidiClock(int ticksPerBeat)
        {
            CurrentTick = 0;
            TicksPerBeat = ticksPerBeat;
        }

        public void Advance(long amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException(
                    "Advance() must have positive argument");
            }
            CurrentTick += amount;
        }

        public long Beat2Tick(double beat)
        {
            return (long) (beat * TicksPerBeat);
        }
    }
}
