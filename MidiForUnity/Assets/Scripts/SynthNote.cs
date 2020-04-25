using System.Collections;
using System.Collections.Generic;
using uMIDI.Common;
using UnityEngine;

public class SynthNote
{
    private static readonly float FREQ_STEP = Mathf.Pow(2, 1 / 12f);
    private static readonly float MAX_AMPLITUDE = 1.0f;
    private static readonly float VELOCITY_MOD = MAX_AMPLITUDE / byte.MaxValue;
    private static readonly float DECAY_SPEED = 10; // "amps" per second

    private enum Stage
    {
        RISING,     // Starts from nothing, heads towards the note peak
        FALLING,    // Starts from peak, heads towards steady state
        STEADY,     // Starts from steady state and stays at steady state
        DECAYING,   // Starts from any state, heads toward dead
        DEAD        // Note is now completely quiet
    }

    private byte id; // Used to track equality with more rigor than float
    public float Freq { get; }
    public float Amp { get; private set; }
    public float Time { get; private set; }

    public float Velocity { get; } // TODO: Used to determine sound dynamics

    private Stage stage;
    private float peakAmp;
    private float steadyAmp;

    public SynthNote(Note note)
    {
        id = note.Pitch;
        Freq = NoteFreq(note.Pitch);
        Velocity = note.Velocity;
        Time = 0;
        Amp = 0;
        steadyAmp = Velocity * VELOCITY_MOD;
        peakAmp = 1.2f * steadyAmp;
        stage = Velocity > 0 ? Stage.RISING : Stage.DEAD;
    }

    public void AddTime(float delta)
    {
        if (!HasDecayed() && delta > 0)
        {
            Time += delta;
            UpdateAmp(delta);
        }
    }

    private void UpdateAmp(float delta)
    {
        float dest;
        Stage destStage;
        switch (stage)
        {
            case Stage.RISING:
                dest = peakAmp;
                destStage = Stage.FALLING;
                break;
            case Stage.FALLING:
                dest = steadyAmp;
                destStage = Stage.STEADY;
                break;
            case Stage.DECAYING:
                dest = 0;
                destStage = Stage.DEAD;
                break;
            default:
                return;
        }
        AmpTowardStage(dest, destStage, DECAY_SPEED * delta);
    }

    private void AmpTowardStage(float destAmp, Stage destStage, float speed)
    {
        Amp = Mathf.MoveTowards(Amp, destAmp, speed);
        if (Amp == destAmp)
            stage = destStage;
    }

    public bool HasDecayed() => stage == Stage.DEAD;

    // Note was "released" and should start to decay.
    public void Release()
    {
        stage = Stage.DECAYING;
    }

    public override bool Equals(object obj)
    {
        if ((obj as SynthNote) != null)
            return (obj as SynthNote).id == this.id;
        else if ((obj as Note) != null)
            return (obj as Note).Pitch == this.id;
        else
            return false;
    }

    public override int GetHashCode() { return id; }

    private static float NoteFreq(int n)
    {
        return 440 * Mathf.Pow(FREQ_STEP, (n - 49));
    }
}
