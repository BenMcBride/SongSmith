// // using System.Diagnostics;
// // using Microsoft.AspNetCore.Mvc;
// // using SongSmith.Models;
// // using System.IO;
// // using Melanchall.DryWetMidi.Core;
// // using Melanchall.DryWetMidi.Interaction;
// // using Melanchall.DryWetMidi.Common;
// // using Melanchall.DryWetMidi.MusicTheory;
// // using System;


// // namespace SongSmith.Controllers;

// // public class SongController : Controller
// // {
// //   private readonly ILogger<SongController> _logger;

// //   public SongController(ILogger<SongController> logger)
// //   {
// //     _logger = logger;
// //   }

// //   public IActionResult Dashboard()
// //   {
// //     return View();
// //   }

// //   [HttpGet("/songs/new")]
// //   public IActionResult SongSmithy()
// //   {
// //     return View("Dashboard");
// //   }

// //   private List<Melanchall.DryWetMidi.Interaction.Note> GenerateNotes(int ticks, int tempo)
// //   {
// //     var notes = new List<Melanchall.DryWetMidi.Interaction.Note>();
// //     var availablePitches = new[] { NoteName.C, NoteName.D, NoteName.E, NoteName.F, NoteName.G, NoteName.A, NoteName.B };
// //     var endPitches = new[] { NoteName.C, NoteName.E, NoteName.G };
// //     var random = new Random();
// //     int sequenceLength = 8;  // a measure contains 4 beats, so 2 measures will have 8 beats
// //     var melodySequence = new List<Melanchall.DryWetMidi.Interaction.Note>();
// //     for (int i = 0; i < sequenceLength - 1; i++)  // reserve the last note to be C, E, or G
// //     {
// //       var pitch = availablePitches[random.Next(availablePitches.Length)];
// //       var note = new Melanchall.DryWetMidi.Interaction.Note(pitch, 3, (SevenBitNumber)100);
// //       melodySequence.Add(note);
// //     }
// //     // add C, E, or G as the last note of the sequence
// //     var endPitch = endPitches[random.Next(endPitches.Length)];
// //     var endNote = new Melanchall.DryWetMidi.Interaction.Note(endPitch, 3, (SevenBitNumber)100);
// //     melodySequence.Add(endNote);
// //     long previousTime = 0;
// //     for (int i = 0; i < ticks / sequenceLength; i++)
// //     {
// //       foreach (var note in melodySequence)
// //       {
// //         long newTime = i * sequenceLength;
// //         var repeatedNote = new Melanchall.DryWetMidi.Interaction.Note(note.NoteName, 3, note.Length, newTime - previousTime);
// //         notes.Add(repeatedNote);
// //         previousTime = newTime;
// //       }
// //     }
// //     return notes;
// //   }


// //   private IEnumerable<Melanchall.DryWetMidi.Interaction.Note> GenerateDrumBeat(int ticks, SevenBitNumber drumType)
// //   {
// //     const int beatsPerMeasure = 4;
// //     var drumBeats = new List<Melanchall.DryWetMidi.Interaction.Note>();
// //     long previousTime = 0;
// //     for (int measure = 0; measure < ticks / beatsPerMeasure; measure++)
// //     {
// //       for (int beat = 0; beat < beatsPerMeasure; beat++)
// //       {
// //         long newTime = measure * beatsPerMeasure + beat;
// //         var drumBeat = new Melanchall.DryWetMidi.Interaction.Note(drumType, (SevenBitNumber)100, newTime - previousTime);
// //         drumBeats.Add(drumBeat);
// //         previousTime = newTime;
// //       }
// //     }
// //     return drumBeats;
// //   }


// //   [HttpPost("/songs/create")]
// //   public IActionResult CreateMidi(int tempo, int key, int duration)
// //   {
// //     // Generate the filename
// //     string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mid";
// //     // Calculate the duration in ticks based on the provided tempo and duration in seconds
// //     int ticksPerQuarterNote = 480; // Assuming the default 480 ticks per quarter note
// //     double microsecondsPerMinute = 60000000.0;
// //     double tempoFactor = tempo / 60.0; // Calculate the tempo factor as the ratio of the provided tempo to 60 BPM
// //     int ticks = (int)((duration * ticksPerQuarterNote * tempoFactor) / (microsecondsPerMinute / 1000000.0));
// //     // Create a list of notes based on the provided ticks and tempo
// //     var melodyNotes = GenerateNotes(ticks, tempo);

// //     var bassDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)36); // Bass Drum 1
// //     var snareDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)40); // Electric Snare
// //     var hiHatBeats = GenerateDrumBeat(ticks, (SevenBitNumber)42); // Closed Hi-Hat

// //     // Create separate track chunks for melody and drum beats
// //     var melodyTrackChunk = new TrackChunk();
// //     var bassDrumTrackChunk = new TrackChunk();
// //     var snareDrumTrackChunk = new TrackChunk();
// //     var hiHatTrackChunk = new TrackChunk();

// //     foreach (var note in melodyNotes)
// //     {
// //       var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Time, Channel = (FourBitNumber)0 };  // melody on channel 0
// //       var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)0 };  // melody on channel 0
// //       melodyTrackChunk.Events.Add(noteOnEvent);
// //       melodyTrackChunk.Events.Add(noteOffEvent);
// //     }

// //     var drumTracks = new List<(TrackChunk, IEnumerable<Melanchall.DryWetMidi.Interaction.Note>)>
// //     {
// //         (bassDrumTrackChunk, bassDrumBeats),
// //         (snareDrumTrackChunk, snareDrumBeats),
// //         (hiHatTrackChunk, hiHatBeats)
// //     };

// //     foreach (var (trackChunk, drumBeats) in drumTracks)
// //     {
// //       foreach (var note in drumBeats)
// //       {
// //         var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Time, Channel = (FourBitNumber)9 };  // drum beats on channel 9 (10 in human-readable form)
// //         var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)9 };  // drum beats on channel 9 (10 in human-readable form)
// //         trackChunk.Events.Add(noteOnEvent);
// //         trackChunk.Events.Add(noteOffEvent);
// //       }
// //     }

// //     // Create a new MIDI file with multiple tracks
// //     var midiFile = new MidiFile(melodyTrackChunk, bassDrumTrackChunk, snareDrumTrackChunk, hiHatTrackChunk);
// //     // Set tempo
// //     var tempoEvent = new SetTempoEvent((long)(60000000 / tempo)) { DeltaTime = 0 };
// //     melodyTrackChunk.Events.Insert(0, tempoEvent);

// //     // Save the MIDI file
// //     string filePath = Path.Combine("wwwroot/Songs", filename);
// //     midiFile.Write(filePath);
// //     return RedirectToAction("PlayMidi", new { filename });
// //   }

// //   [HttpGet("/Songs/PlaySong/{filename}")]
// //   public IActionResult PlayMidi(string filename)
// //   {
// //     filename = filename.Replace("\\", "/");
// //     ViewBag.FileUrl = filename;
// //     return View("MidiCreated");
// //   }

// //   [HttpGet("/Songs/{filePath}")]
// //   public IActionResult MidiFile(string filePath)
// //   {
// //     var bytes = new byte[0];
// //     using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
// //     {
// //       var br = new BinaryReader(fs);
// //       long numBytes = new FileInfo(filePath).Length;
// //       bytes = br.ReadBytes((int)numBytes);
// //     }
// //     return File(bytes, "audio/midi", filePath);
// //   }

// //   public IActionResult Privacy()
// //   {
// //     return View();
// //   }

// //   [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
// //   public IActionResult Error()
// //   {
// //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
// //   }
// // }
// //////////////////////////////////////////////////////////////////////////////////////////////////////
// //////////////////////////////////////////////////////////////////////////////////////////////////////
// //////////////////////////////////////////////////////////////////////////////////////////////////////
// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc;
// using SongSmith.Models;
// using System.IO;
// using Melanchall.DryWetMidi.Core;
// using Melanchall.DryWetMidi.Interaction;
// using Melanchall.DryWetMidi.Common;
// using Melanchall.DryWetMidi.MusicTheory;
// using System;


// namespace SongSmith.Controllers;

// public class SongController : Controller
// {
//   NoteName[] melodySequence = new NoteName[]
// {
//     NoteName.C,
//     NoteName.D,
//     NoteName.E,
//     NoteName.F,
//     NoteName.G,
//     NoteName.A,
//     NoteName.B,
//     NoteName.C
// };
//   private readonly ILogger<SongController> _logger;


//   public SongController(ILogger<SongController> logger)
//   {
//     _logger = logger;
//   }

//   public IActionResult Dashboard()
//   {
//     return View();
//   }

//   [HttpGet("/songs/new")]
//   public IActionResult SongSmithy()
//   {
//     return View("Dashboard");
//   }


//   private List<Melanchall.DryWetMidi.Interaction.Note> GenerateNotes(int ticks, int tempo)
//   {
//     int ticksPerQuarterNote = 480; // assuming tempo is defined in terms of PPQN
//     int sequenceLength = melodySequence.Length;
//     int totalQuarterNotes = ticks / ticksPerQuarterNote;
//     int noteDurationInBeats = 1; // length of each note in beats
//     int noteDurationInTicks = noteDurationInBeats * ticksPerQuarterNote;

//     var notes = new List<Melanchall.DryWetMidi.Interaction.Note>();
//     for (int i = 0; i < totalQuarterNotes; i++)
//     {
//       var note = melodySequence[i % sequenceLength]; // cycle through the melody sequence
//       long newTime = i * ticksPerQuarterNote; // time for this note
//       var repeatedNote = new Melanchall.DryWetMidi.Interaction.Note(note, 3, noteDurationInTicks, newTime);
//       notes.Add(repeatedNote);
//     }
//     return notes;
//   }

//   private IEnumerable<Melanchall.DryWetMidi.Interaction.Note> GenerateDrumBeat(int ticks, SevenBitNumber drumNumber, List<int[]> pattern, int ticksPerQuarterNote)
//   {
//     var beats = new List<Melanchall.DryWetMidi.Interaction.Note>();
//     foreach (var measure in pattern)
//     {
//       foreach (var beat in measure)
//       {
//         long newTime = beat * ticksPerQuarterNote;
//         var drumBeat = new Melanchall.DryWetMidi.Interaction.Note(drumNumber, (SevenBitNumber)100, newTime);
//         beats.Add(drumBeat);
//       }
//     }
//     return beats;
//   }


//   [HttpPost("/songs/create")]
//   public IActionResult CreateMidi(int tempo, int key, int duration)
//   {
//     int defaultTempo = 120; // Default tempo in BPM
//     int ticksPerQuarterNote = 480;
//     int bpm = tempo;

//     int microsecondsPerQuarterNote = 60000000 / bpm;
//     string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mid";

//     double microsecondsPerMinute = 60000000.0;
//     double tempoFactor = (double)tempo / defaultTempo;
//     int ticks = (int)((duration * ticksPerQuarterNote * tempoFactor) / (microsecondsPerMinute / 1000000.0));

//     var melodyNotes = GenerateNotes(ticks, ticksPerQuarterNote);

//     var hiHatMeasure = new List<int[]> { new int[] { 0, 1, 2, 3 } };
//     var bassDrumMeasure = new List<int[]> { new int[] { 0, 1 } };
//     var snareDrumMeasure = new List<int[]> { new int[] { 2 } };

//     var hiHatBeats = GenerateDrumBeat(ticks, (SevenBitNumber)42, hiHatMeasure, ticksPerQuarterNote);
//     var bassDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)36, bassDrumMeasure, ticksPerQuarterNote);
//     var snareDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)40, snareDrumMeasure, ticksPerQuarterNote);

//     var melodyTrackChunk = new TrackChunk();
//     var drumTrackChunk = new TrackChunk();

//     foreach (var note in melodyNotes)
//     {
//       var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Time, Channel = (FourBitNumber)0 };
//       melodyTrackChunk.Events.Add(noteOnEvent);
//       var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)0 };
//       melodyTrackChunk.Events.Add(noteOffEvent);
//     }

//     var drumBeats = hiHatBeats.Concat(bassDrumBeats).Concat(snareDrumBeats).OrderBy(beat => beat.Time);

//     long previousNoteTime = 0;
//     foreach (var beat in drumBeats)
//     {
//       var noteOnEvent = new NoteOnEvent((SevenBitNumber)beat.NoteNumber, (SevenBitNumber)100) { DeltaTime = beat.Time - previousNoteTime, Channel = (FourBitNumber)9 };
//       var noteOffEvent = new NoteOffEvent((SevenBitNumber)beat.NoteNumber, (SevenBitNumber)0) { DeltaTime = 0, Channel = (FourBitNumber)9 };
//       drumTrackChunk.Events.Add(noteOnEvent);
//       drumTrackChunk.Events.Add(noteOffEvent);
//       previousNoteTime = beat.Time;
//     }

//     var midiFile = new MidiFile(melodyTrackChunk, drumTrackChunk);
//     var tempoEvent = new SetTempoEvent((long)(60000000 / tempo)) { DeltaTime = 0 };
//     melodyTrackChunk.Events.Insert(0, tempoEvent);
//     drumTrackChunk.Events.Insert(0, (SetTempoEvent)tempoEvent.Clone());

//     string filePath = Path.Combine("wwwroot/Songs", filename);
//     midiFile.Write(filePath);
//     return RedirectToAction("PlayMidi", new { filename });
//   }


//   [HttpGet("/Songs/PlaySong/{filename}")]
//   public IActionResult PlayMidi(string filename)
//   {
//     filename = filename.Replace("\\", "/");
//     ViewBag.FileUrl = filename;
//     return View("MidiCreated");
//   }

//   [HttpGet("/Songs/{filePath}")]
//   public IActionResult MidiFile(string filePath)
//   {
//     var bytes = new byte[0];
//     using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
//     {
//       var br = new BinaryReader(fs);
//       long numBytes = new FileInfo(filePath).Length;
//       bytes = br.ReadBytes((int)numBytes);
//     }
//     return File(bytes, "audio/midi", filePath);
//   }

//   public IActionResult Privacy()
//   {
//     return View();
//   }

//   [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//   public IActionResult Error()
//   {
//     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//   }
// }
