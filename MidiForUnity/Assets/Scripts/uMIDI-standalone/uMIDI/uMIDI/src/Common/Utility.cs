using System;

namespace uMIDI.Common
{
    /// <summary>
    /// Class of utility functions.
    /// </summary>
    public class MessageUtility
    {
        private MessageUtility() { }

        /// <summary>
        /// Converts a MidiMessage struct of byte data to an IMessage object.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeDelta"></param>
        /// <returns></returns>
        public static IMessage ToIMessage(MidiMessage msg, long timeDelta)
        {
            if (0x80 <= msg.Status && msg.Status < 0x90)
            {
                return new NoteOffMessage(new Note(
                    (byte)(msg.Status % 0x10),
                    msg.Data[0],
                    msg.Data[1]
                    ),
                    timeDelta);
            }
            else if (0x90 <= msg.Status && msg.Status < 0xa0)
            {
                return new NoteOnMessage(new Note(
                    (byte)(msg.Status % 0x10),
                    msg.Data[0],
                    msg.Data[1]
                    ),
                    timeDelta);
            }
            else if (msg.Status == 0xff)
            {
                throw new ArgumentException("This is a meta event. Please " +
                    "use the IMetaMessage.ToIMetaMessage() static method.");
            }
            else
            {
                throw new ArgumentException("MidiMessage not recognized");
            }
        }

        /// <summary>
        /// Converts a MetaMessage struct of byte data to an IMetaMessage
        /// object.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeDelta"></param>
        /// <returns></returns>
        public static IMetaMessage ToIMetaMessage(MetaMessage msg,
            long timeDelta)
        {
            switch (msg.MetaType)
            {
                case (byte)MetaType.TEMPO:
                    byte data1 = msg.Data[0];
                    byte data2 = msg.Data[1];
                    byte data3 = msg.Data[2];
                    int microsecondsPerBeat = data1 * 0x10000 + data2 * 0x100 +
                        data3;
                    double tempo = 60 * 1e6 / (double)microsecondsPerBeat;
                    return new TempoMetaMessage(
                        tempo,
                        timeDelta
                        );
                case (byte)MetaType.TIME_SIGNATURE:
                    return new TimeSignatureMetaMessage(
                        msg.Data[0],
                        (int)Math.Pow(2, msg.Data[1]),
                        msg.Data[2],
                        msg.Data[3],
                        timeDelta
                        );
                case (byte)MetaType.KEY_SIGNATURE:
                    return new KeySignatureMetaMessage(
                        (sbyte)msg.Data[0],
                        msg.Data[1],
                        timeDelta
                        );
                default:
                    throw new ArgumentException("MetaMessage not recognized");
            }
        }
    }
}
