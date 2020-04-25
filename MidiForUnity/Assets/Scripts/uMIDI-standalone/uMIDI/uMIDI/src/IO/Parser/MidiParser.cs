using System;
using System.Collections.Generic;
using System.Linq;
using uMIDI.Common;
using uMIDI.Utility;

namespace uMIDI.IO
{
    public class MidiParser
    {
        #region --Constants--

        private static readonly List<byte> HEADER_CHUNK = new List<byte> { 0x4d, 0x54, 0x68, 0x64 };
        private static readonly List<byte> TRACK_CHUNK  = new List<byte> { 0x4d, 0x54, 0x72, 0x6b };
        private static readonly IDictionary<int, string> FORMATS = new Dictionary<int, string> { { 0, "single track file format" }, { 1, "multiple track file format" }, { 2, "multiple song file format" } };
        private static readonly IDictionary<string, int> HEADER_PART_LOCATIONS = new Dictionary<string, int> { { "format", 0 }, { "numberOfTracks", 2 }, { "unitForDeltaTiming", 4 } };
        private static readonly IDictionary<string, int> HEADER_PART_SIZES     = new Dictionary<string, int> { { "format", 2 }, { "numberOfTracks", 2 }, { "unitForDeltaTiming", 2 } };
        private static readonly IDictionary<byte, string> metaEventCodeDictionary = new Dictionary<byte, string> { { 0x00, "sequence number" }, { 0x01, "text event" }, { 0x02, "copyright notice" }, { 0x03, "sequence or track name" }, { 0x04, "instrument name" }, { 0x05, "lyric text" }, { 0x06, "marker text" }, { 0x07, "cue point" }, { 0x20, "MIDI channel prefix assignment" }, { 0x2F, "end of track" }, { 0x51, "tempo setting" }, { 0x54, "SMPTE offset" }, { 0x58, "time signature" }, { 0x59, "key signature" }, { 0x7F, "sequencer specific event" } };
        private static readonly IDictionary<byte, string> trackEventStatusDictionary = new Dictionary<byte, string> { { 0x08, "note off" }, { 0x09, "note on" } };
        private const int chunkBodyLengthSize = 4;
        private static readonly int BUFFER_SIZE = 32;

        #endregion

        #region --Functions--

        public MidiFile ConvertMidi(List<byte> midiFile, MetaMidiStream stream)
        {
            MidiFile convertedData = new MidiFile(stream, DecodeMidi(midiFile), BUFFER_SIZE);
            

            return convertedData;
        }

        public List<IMessage> DecodeMidi(List<byte> midiFile)
        {
            List<IMessage> decodedEventList = new List<IMessage>();
            int chunkSize = 1;

            if (midiFile.Count() < 4)
            {
                return decodedEventList;         //ERROR: If the code reaches here, there were extra bytes left over that weren't covered by any type of chunk.
            }

            if (IsHeaderChunk(midiFile.GetRange(0, 4)))
            {
                int bodySize = FindChunkBodyLength(midiFile.GetRange(4, 4));
                chunkSize = bodySize + HEADER_CHUNK.Count() + chunkBodyLengthSize;
                decodedEventList.Add(ProcessHeaderChunk(midiFile.GetRange(HEADER_CHUNK.Count() + chunkBodyLengthSize, bodySize)));
            }
            else if (IsTrackChunk(midiFile.GetRange(0, 4)))
            {
                int bodySize = FindChunkBodyLength(midiFile.GetRange(4, 4));
                chunkSize = bodySize + TRACK_CHUNK.Count() + chunkBodyLengthSize;
                decodedEventList = decodedEventList.Concat(ProcessTrackChunk(midiFile.GetRange(TRACK_CHUNK.Count() + chunkBodyLengthSize, bodySize))).ToList();
            }
            
            if (midiFile.Count() > chunkSize) {
                return decodedEventList.Concat(DecodeMidi(midiFile.GetRange(chunkSize, decodedEventList.Count() - 1))).ToList();
            } 
            else
            {
                return decodedEventList;
            }
        }

        /// <summary>
        /// Finds and converts the chunk's body's length as a decimal number of bytes.
        /// </summary>
        /// <param name="chunkSize">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>Length of the chunk's body's length.</returns>
        private int FindChunkBodyLength(List<byte> bodySizeChunk)
        {
            // receive 4 bytes of hexadecimal and turn into decimal to figure out how big the header chunk is then return this
            return ConcatBytes(bodySizeChunk);
        }

        /// <summary>
        /// Receives four hexadecimal byte <see cref="List{T}"/> and checks if it is a chunk identifier for a header.
        /// </summary>
        /// <param name="chunkIdentifier">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>true if header chunk; else false.</returns>
         private bool IsHeaderChunk(List<byte> chunkIdentifier)
        {
            return chunkIdentifier.SequenceEqual(HEADER_CHUNK);
        }


        /// <summary>
        /// Converts from the hexadecimal bytes from the header chunk into a <see cref="IMessage"/>.
        /// </summary>
        /// <param name="headerChunkBody">A <see cref="List{T}"/> of hexadecimal bytes that represent the header's body.</param>
        /// <returns>A <see cref="List"/> containing a single <see cref="IMessage"/>.</returns>
        private IMessage ProcessHeaderChunk(List<byte> headerChunkBody)
        {
            /*
            return new HeaderEvent()
            {
                Format = FindFormat(HeaderPartChunk(headerChunkBody, "format")),
                NumberOfTracks = FindNumberOfTracks(HeaderPartChunk(headerChunkBody, "numberOfTracks")),
                UnitForDeltaTiming = FindUnitForDeltaTiming(HeaderPartChunk(headerChunkBody, "unitForDeltaTiming"))
            };*/
            return null;
        }

        private List<byte> HeaderPartChunk(List<byte> body, string partName)
        {
            return body.GetRange(HEADER_PART_LOCATIONS[partName], HEADER_PART_SIZES[partName]);
        }

        /// <summary>
        /// Finds the song format of the file from the header body.
        /// </summary>
        /// <param name="formatInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>A <see cref="String"/> describing the format of the file.</returns>
        private string FindFormat(List<byte> formatInfo)
        {
            int formatKey = ConcatBytes(formatInfo.GetRange(0, 2));
            string formatValue;
            
            if (FORMATS.TryGetValue(formatKey, out formatValue))
            {
                return formatValue;
            } 
            else
            {
                // TODO: Maybe throw an exception here instead?
                return string.Empty;        //ERROR: If the code reaches here, there has been an error.
            }
        }

        /// <summary>
        /// Finds the number of tracks in the file from the header body.
        /// </summary>
        /// <param name="numberOfTracksInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>The number of tracks related to this header.</returns>
        private int FindNumberOfTracks(List<byte> numberOfTracksInfo)
        {
            return ConcatBytes(numberOfTracksInfo);
        }

        /// <summary>
        /// Finds the unit for delta timing (if positive, it is ticks per beat and if negative, delta times are in SMPTE compatible units).
        /// </summary>
        /// <param name="unitForDeltaTimingInfo">A <see cref="List{T}"/> of two hexadecimal bytes.</param>
        /// <returns>The unit for delta timing</returns>
        private int FindUnitForDeltaTiming(List<byte> unitForDeltaTimingInfo)
        {
            return ConcatBytes(unitForDeltaTimingInfo);
        }

        private int ConcatBytes(List<byte> bytes)
        {
            long concat = 0;
            foreach (byte b in bytes) concat = BitByBit.Concat(concat, b);
            return (int)concat;
        }

        /// <summary>
        /// Receives four hexadecimal byte <see cref="List{T}"/> and checks if it is a chunk identifier for a track.
        /// </summary>
        /// <param name="chunkIdentifier">A <see cref="List{T}"/> of four hexadecimal bytes.</param>
        /// <returns>true if track chunk; else false.</returns>
        private bool IsTrackChunk(List<byte> chunkIdentifier)
        {
            return chunkIdentifier.SequenceEqual(TRACK_CHUNK);
        }

        /// <summary>
        /// Converts from the hexadecimal bytes from the track chunk into a <see cref="List"/> of <see cref="IMessage"/>s.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <returns>A <see cref="List"/> of <see cref="IMessage"/>s that represent the information from the track chunk.</returns>
        private List<IMessage> ProcessTrackChunk(List<byte> trackChunk)
        {
            List<IMessage> decodedEventList = new List<IMessage>();
            bool endOfTrack = false;

            while (!endOfTrack)
            {
                VariableChunkSize<int> timeChunk = FindDeltaTime(trackChunk);
                trackChunk = trackChunk.GetRange(timeChunk.chunkSize, trackChunk.Count() - timeChunk.chunkSize);

                int currentEventSize = 0;
                byte nextByte = trackChunk.First();
                if (nextByte == 0xFF)
                {
                    TrackMetaEventInfo metaEventInfo = ProcessMetaEvent(trackChunk, timeChunk.data);
                    currentEventSize = metaEventInfo.eventSize;
                    decodedEventList.Add(metaEventInfo.trackOrMetaEvent);
                    if (trackChunk[1] == 0x2F)
                    {
                        endOfTrack = true;
                    }
                }
                else
                {
                    TrackMetaEventInfo trackEventInfo = ProcessTrackEvent(trackChunk, timeChunk.data);
                    currentEventSize = trackEventInfo.eventSize;
                    decodedEventList.Add(trackEventInfo.trackOrMetaEvent);
                }

                trackChunk = trackChunk.GetRange(currentEventSize, trackChunk.Count() - currentEventSize);
            }

            return decodedEventList;
        }

        /*
         * Dynamically finds the variable-sized time chunk and aggregates their
         * values.
         * Note: the back 7-bits of each byte correspond to the value, while the
         *       first bit notifies if there is another byte to follow.
         */
        private static VariableChunkSize<int> FindDeltaTime(List<byte> trackChunk)
        {
            VariableChunkSize<int> timeChunk;
            timeChunk.chunkSize = 1;
            timeChunk.data = 0;

            while (true)
            {
                byte next = trackChunk[timeChunk.chunkSize - 1];
                timeChunk.data = (int)BitByBit.ConcatIgnoreFront(timeChunk.data, next, 1);

                if (!BitByBit.IsFrontBitsOn(next, 1))
                    break;
                timeChunk.chunkSize++;
            }

            return timeChunk;
        }

        private struct VariableChunkSize<T>
        {
            internal int chunkSize;
            internal T data;
        }

        /// <summary>
        /// Converts from the hexadecimal bytes of a meta event into a <see cref="TrackMetaEventInfo"/>.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <param name="deltaTime">The time between the previous event and the current event.</param>
        /// <returns>A <see cref="TrackMetaEventInfo"/> containing <see cref="IMessage"/> information and the number of bytes the meta event was.</returns>
        private TrackMetaEventInfo ProcessMetaEvent(List<byte> trackChunk, int deltaTime)
        {
            int eventSize = trackChunk[2];
            IMessage metaEvent = null;                                  //TODO: need to find a better way to handle this
            String eventType = metaEventCodeDictionary[trackChunk[1]];

            byte[] data = new byte[eventSize];
            for (int i = 0; i < eventSize; i++)
            {
                data[i] = trackChunk[2 + i];
            }

            MetaMessage message = new MetaMessage
            {
                MetaType = trackChunk[1],
                Data = data
            };

            IMetaMessage metaMessage = IMessageUtility.ToIMetaMessage(message, deltaTime);
            /*
            switch (eventType)
            {
                case "tempo setting":
                    metaEvent = new TempoMetaMessage()
                    {
                        Tempo = string.Join("", trackChunk.GetRange(3, eventSize)),
                        Time = deltaTime
                    };
                    break;
                // TODO: Add all the meta message cases
            }
            */

            return new TrackMetaEventInfo
            {
                eventSize = eventSize,
                trackOrMetaEvent = metaEvent
            };
        }

        /// <summary>
        /// Converts from the hexadecimal bytes of a trac event into a <see cref="TrackMetaEventInfo"/>.
        /// </summary>
        /// <param name="trackChunk">A <see cref="List"/> of hexadecimal bytes that represent the track chunk.</param>
        /// <param name="deltaTime">The time between the previous event and the current event.</param>
        /// <returns>A <see cref="TrackMetaEventInfo"/> containing <see cref="IMessage"/> information and the number of bytes the meta event was.</returns>
        private TrackMetaEventInfo ProcessTrackEvent(List<byte> trackChunk, int deltaTime)
        {
            string trackEventType = trackEventStatusDictionary[trackChunk[0]];



            /*
            if (trackEventType == "note on")
            {
                IMessage trackEvent = new NoteOnMessage()
                {
                    Note = new Note()
                    {
                        Channel = BitByBit.Nib2Hex(BitByBit.RightNib(trackChunk[0])),
                        Pitch = trackChunk[1],
                        Velocity = trackChunk[2],
                        Time = deltaTime
                    }
                };
            } else if (track)
                

            return new TrackMetaEventInfo
            {
                eventSize = 3,
                trackOrMetaEvent = trackEvent
            };*/
            return null;
        }
        #endregion
        #region --Test Hooks--
        public class TestHook
        {
            private MidiParser instance;

            public TestHook(MidiParser instance)
            {
                this.instance = instance;
            }

            /*make public methods for the private methods for unit testing*/

            //public method for Unit testing FinChunkBodyLength
            public int TestFindChunkBodyLength(List<byte> bodySizeChunk)
            {
                return instance.FindChunkBodyLength(bodySizeChunk);
            }

            //public method for Unit testing IsHeaderChunk
            public bool TestIsHeaderChunk(List<byte> chunkIdentifier)
            {

                return instance.IsHeaderChunk(chunkIdentifier);

            }

            //public method for testing ProcessHeaderChunk
            public IMessage TestProcessHeaderChunk(List<byte> headerChunkBody)
            {

                return instance.ProcessHeaderChunk(headerChunkBody);
            }

            //public method for Unit testing HeaderPartChunk
            public List<byte> TestHeaderPartChunk(List<byte> body, string partName)
            {

                return instance.HeaderPartChunk(body, partName);
            }

            //public method for Unit testing FindFormat
            public string TestFindFormat(List<byte> formatInfo)
            {

                return instance.FindFormat(formatInfo);
            }

            //public method for Unit testing
            public int TestFindNumberOfTracks(List<byte> numberOfTracksInfo)
            {

                return instance.FindNumberOfTracks(numberOfTracksInfo);
            }

            //public method for Unit testing
            public int TestFindUnitForDeltaTiming(List<byte> unitForDeltaTimingInfo)
            {
                return instance.FindUnitForDeltaTiming(unitForDeltaTimingInfo);
            }

            //public method for Unit testing
            public bool TestIsTrackChunk(List<byte> chunkIdentifier)
            {
                return instance.IsTrackChunk(chunkIdentifier);

            }

            //public method for Unit testing
            public List<IMessage> TestProcessTrackChunk(List<byte> trackChunk)
            {
                return instance.ProcessTrackChunk(trackChunk);
            }

            //public method for Unit testing
            public int TestFindDeltaTimeChunkSize(List<byte> trackChunk)
            {
                return FindDeltaTime(trackChunk).chunkSize;
            }

            //public method for Unit testing
            public int TestFindDeltaTimeData(List<byte> trackChunk)
            {
                return FindDeltaTime(trackChunk).data;
            }

            //public method for Unit testing
            public TrackMetaEventInfo TestProcessMetaEvent(List<byte> trackChunk, int deltaTime)
            {
                return instance.ProcessMetaEvent(trackChunk, deltaTime);
            }

            //public method for Unit testing
            public TrackMetaEventInfo TestProcessTrackEvent(List<byte> trackChunk, int deltaTime)
            {
                return instance.ProcessTrackEvent(trackChunk, deltaTime);
            }
        }

        #endregion
    }
}
