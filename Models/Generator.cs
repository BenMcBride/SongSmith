
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using NAudio.Midi;

// class Generator
// {
//   static void Main(string[] args)
//   {
//     if (args.Length < 3)
//     {
//       Console.WriteLine("Usage: <output-filename> <list-of-notes> <tempo>");
//       return;
//     }

//     string outputFilename = args[0];
//     int tempo = Convert.ToInt32(args[args.Length - 1]);

//     PitchParser pitchParser = new PitchParser();
//     PatchParser patchParser = new PatchParser();

//     var midiValues = pitchParser.Parse(args.Skip(1).Take(args.Length - 2));

//     if (midiValues.Any())
//     {
//       // Create a list of notes for drum beats after the fourth measure
//       List<string> drumBeats = GenerateDrumBeats(midiValues.Count(), tempo);

//       // Convert the drum beats to MIDI values
//       var drumValues = pitchParser.Parse(drumBeats);

//       // Combine the original notes and drum beats
//       var allNotes = midiValues.Concat(drumValues);

//       var exporter = new MidiExporter();
//       exporter.SaveToFile(outputFilename, allNotes, tempo);
//     }
//   }

//   private static List<string> GenerateDrumBeats(int noteCount, int tempo)
//   {
//     const int measuresBeforeDrums = 4;
//     const int beatsPerMeasure = 4;
//     const int drumNotesPerBeat = 2;

//     List<string> drumBeats = new List<string>();

//     // Determine the number of measures including drums
//     int measuresWithDrums = (noteCount / beatsPerMeasure) + measuresBeforeDrums;

//     for (int measure = 0; measure < measuresWithDrums; measure++)
//     {
//       bool isDrumMeasure = measure >= measuresBeforeDrums;

//       for (int beat = 0; beat < beatsPerMeasure; beat++)
//       {
//         bool isDrumBeat = isDrumMeasure && (beat % drumNotesPerBeat == 0);

//         if (isDrumBeat)
//         {
//           // Add drum beats
//           drumBeats.Add("Bass Drum 1");
//           drumBeats.Add("Closed Hi-Hat");
//           drumBeats.Add("Electric Snare");
//         }
//         else
//         {
//           // Add empty beats
//           drumBeats.Add(string.Empty);
//           drumBeats.Add(string.Empty);
//         }
//       }
//     }

//     return drumBeats;
//   }
// }

// public class PatchParser
// {
//   Dictionary<string, int> patchMap = new Dictionary<string, int>();

//   public PatchParser()
//   {
//     this.patchMap.Add("nylon", 25);
//     this.patchMap.Add("steel", 26);
//     this.patchMap.Add("jazz", 27);
//     this.patchMap.Add("clean", 28);
//     this.patchMap.Add("muted", 29);
//     this.patchMap.Add("distortion", 31);
//     this.patchMap.Add("bass", 33);
//     this.patchMap.Add("violin", 41);
//     this.patchMap.Add("viola", 42);
//     this.patchMap.Add("cello", 43);
//     this.patchMap.Add("sitar", 105);
//     this.patchMap.Add("banjo", 106);
//     this.patchMap.Add("fiddle", 111);
//   }

//   public int Patch(string value)
//   {
//     const int DefaultPatch = 25;

//     int patch = DefaultPatch;

//     try
//     {
//       patch = this.patchMap[value.ToLower()];
//     }
//     catch (KeyNotFoundException)
//     {
//       patch = DefaultPatch;
//     }

//     return patch;
//   }
// }

// public class PitchParser
// {
//   Dictionary<char, int> pitchOffsets;
//   Dictionary<char, int> pitchModifiers;
//   Dictionary<string, int> drumMap;

//   public PitchParser()
//   {
//     this.pitchOffsets = new Dictionary<char, int>();
//     this.pitchModifiers = new Dictionary<char, int>();
//     this.drumMap = new Dictionary<string, int>();

//     this.pitchOffsets.Add('c', 0);
//     this.pitchOffsets.Add('d', 2);
//     this.pitchOffsets.Add('e', 4);
//     this.pitchOffsets.Add('f', 5);
//     this.pitchOffsets.Add('g', 7);
//     this.pitchOffsets.Add('a', 9);
//     this.pitchOffsets.Add('b', 11);

//     this.pitchModifiers.Add('#', 1);
//     this.pitchModifiers.Add('b', -1);

//     this.drumMap.Add("closed hi-hat", 42);
//     this.drumMap.Add("electric snare", 40);
//     this.drumMap.Add("bass drum 1", 36);
//   }

//   public Pitch PitchFromString(string midiNotation)
//   {
//     const int MinimumStringLength = 2;

//     if (midiNotation.Length < MinimumStringLength)
//       throw new ArgumentException();

//     string name = FindName(midiNotation);
//     string modifier = FindModifier(midiNotation);
//     int octave = FindOctave(midiNotation);

//     int midiValue = CalculateMidiValue(name, modifier, octave);

//     return new Pitch(midiValue);
//   }

//   public IEnumerable<Pitch> Parse(IEnumerable<string> noteList)
//   {
//     var list = new List<Pitch>();

//     noteList.Where(note => !String.IsNullOrWhiteSpace(note)).ToList().ForEach(note =>
//     {
//       if (drumMap.ContainsKey(note.ToLower()))
//       {
//         list.Add(new Pitch(drumMap[note.ToLower()]));
//       }
//       else
//       {
//         list.Add(this.PitchFromString(note));
//       }
//     });

//     return list;
//   }

//   private string FindName(string midiNotation)
//   {
//     return midiNotation.Substring(0, 1).ToLower();
//   }

//   private string FindModifier(string midiNotation)
//   {
//     const int MaximumStringLength = 3;

//     if (midiNotation.Length == MaximumStringLength)
//     {
//       return midiNotation.Substring(1, 1).ToLower();
//     }

//     return string.Empty;
//   }

//   private int FindOctave(string midiNotation)
//   {
//     const int MaximumStringLength = 3;
//     int startIndex = 1;

//     if (midiNotation.Length == MaximumStringLength)
//     {
//       startIndex = 2;
//     }

//     return Int32.Parse(midiNotation.Substring(startIndex, 1));
//   }

//   private int CalculateMidiValue(string noteName, string modifier, int octave)
//   {
//     int value = 0;

//     try
//     {
//       value = this.pitchOffsets[noteName[0]];

//       if (!String.IsNullOrEmpty(modifier))
//       {
//         value += this.pitchModifiers[modifier[0]];
//       }
//     }
//     catch (KeyNotFoundException)
//     {
//       value = 0;
//     }

//     const int SemitonesInOctave = 12;

//     return value + (SemitonesInOctave * octave);
//   }
// }

// public class Pitch
// {
//   const int MinimumMidiValue = 0;
//   const int MaximumMidiValue = 255;

//   public Pitch(int value)
//   {
//     this.MidiValue = Math.Min(MaximumMidiValue, Math.Max(value, MinimumMidiValue));
//   }

//   public int MidiValue { get; private set; }
// }

// public class MidiExporter
// {
//   public void SaveToFile(string fileName, IEnumerable<Pitch> allNotes, int bpm)
//   {
//     const int MidiFileType = 0;
//     const int TicksPerQuarterNote = 120;

//     const int TrackNumber = 0;
//     const int DrumChannelNumber = 9; // Channel 10 for drums
//     const int DrumPatchNumber = 0; // Program Change 1 for Standard Drum Kit

//     long absoluteTime = 0;

//     var collection = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);

//     collection.AddEvent(new TextEvent("Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);
//     ++absoluteTime;
//     collection.AddEvent(new TempoEvent(CalculateMicrosecondsPerQuaterNote(bpm), absoluteTime), TrackNumber);

//     collection.AddEvent(new PatchChangeEvent(0, DrumChannelNumber, DrumPatchNumber), TrackNumber); // Set drum patch

//     const int NoteVelocity = 100;
//     const int NoteDuration = 3 * TicksPerQuarterNote / 4;
//     const long SpaceBetweenNotes = TicksPerQuarterNote;

//     foreach (var note in allNotes)
//     {
//       collection.AddEvent(new NoteOnEvent(absoluteTime, DrumChannelNumber, note.MidiValue, NoteVelocity, NoteDuration), TrackNumber);
//       collection.AddEvent(new NoteEvent(absoluteTime + NoteDuration, DrumChannelNumber, MidiCommandCode.NoteOff, note.MidiValue, 0), TrackNumber);

//       absoluteTime += SpaceBetweenNotes;
//     }

//     collection.PrepareForExport();
//     MidiFile.Export(fileName, collection);
//   }

//   private static int CalculateMicrosecondsPerQuaterNote(int bpm)
//   {
//     return 60 * 1000 * 1000 / bpm;
//   }
// }
