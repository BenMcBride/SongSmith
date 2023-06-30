using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SongSmith.Models;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Common;
using NoteName = Melanchall.DryWetMidi.MusicTheory.NoteName;

namespace SongSmith.Controllers;
[Authorize]
public class SongController : Controller
{
  private readonly ILogger<SongController> _logger;

  public SongController(ILogger<SongController> logger)
  {
    _logger = logger;
  }

  public IActionResult Dashboard()
  {
    return View();
  }

  [HttpGet("/songs/new")]
  public IActionResult SongSmithy()
  {
    return View("Dashboard");
  }

  private List<Note> GenerateNotes(int ticks, int tempo)
  {
    var notes = new List<Note>();
    var availablePitches = new[] { NoteName.C, NoteName.D, NoteName.E, NoteName.F, NoteName.G, NoteName.A, NoteName.B };
    var endPitches = new[] { NoteName.C, NoteName.E, NoteName.G };
    var random = new Random();
    int sequenceLength = 8;
    var melodySequence = new List<Note>();
    for (int i = 0; i < sequenceLength - 1; i++)
    {
      var pitch = availablePitches[random.Next(availablePitches.Length)];
      var note = new Note(pitch, 3, (SevenBitNumber)100);
      melodySequence.Add(note);
    }
    var endPitch = endPitches[random.Next(endPitches.Length)];
    var endNote = new Note(endPitch, 3, (SevenBitNumber)100);
    melodySequence.Add(endNote);
    int totalNotes = (ticks / sequenceLength) * sequenceLength;
    long deltaTime = ticks / totalNotes; // fixed delta time for each note
    for (int i = 0; i < totalNotes; i++)
    {
      var note = melodySequence[i % sequenceLength];
      long newTime = i * deltaTime;
      var repeatedNote = new Note(note.NoteName, 3, note.Length, newTime);
      notes.Add(repeatedNote);
    }
    return notes;
  }



  private IEnumerable<Note> GenerateDrumBeat(int ticks, SevenBitNumber drumType, List<int[]> measures)
  {
    const int beatsPerMeasure = 8;
    var drumBeats = new List<Note>();
    int totalBeats = (ticks / beatsPerMeasure) * beatsPerMeasure;
    long deltaTime = ticks / totalBeats; // fixed delta time for each drum beat
    for (int i = 0; i < totalBeats; i++)
    {
      int measure = i / beatsPerMeasure;
      int beat = i % beatsPerMeasure;
      int[] currentMeasure = measures[measure % measures.Count];
      if (measure < 2 || Array.IndexOf(currentMeasure, beat) == -1)
      {
        long newTime = i * deltaTime;
        var restBeat = new Note((SevenBitNumber)2, (SevenBitNumber)100, newTime);
        drumBeats.Add(restBeat);
      }
      else
      {
        long newTime = i * deltaTime;
        var drumBeat = new Note(drumType, (SevenBitNumber)100, newTime);
        drumBeats.Add(drumBeat);
      }
    }
    return drumBeats;
  }


  private List<Note> GenerateBassLine(List<Note> melodyNotes)
  {
    var bassNotes = new List<Note>();
    for (int i = 0; i < melodyNotes.Count; i += 2)
    {
      var melodyNote = melodyNotes[i];
      var noteName = melodyNote.NoteName;
      var octave = melodyNote.Octave - 2;
      var length = melodyNote.Length * 2;
      var time = melodyNote.Time;
      if (i <= 30)
      {
        var bassNote = new Note((SevenBitNumber)2, length, time);
        bassNotes.Add(bassNote);
      }
      else
      {
        var bassNote = new Note(noteName, octave, length, time);
        bassNotes.Add(bassNote);
      }
    }
    return bassNotes;
  }

  private List<List<Note>> GenerateHarmoniousChords(List<Note> melodyNotes)
  {
    var chords = new List<List<Note>>();
    Dictionary<NoteName, NoteName[]> harmoniousChords = new Dictionary<NoteName, NoteName[]>
    {
      { NoteName.C, new NoteName[] { NoteName.C, NoteName.E, NoteName.G } },
      { NoteName.D, new NoteName[] { NoteName.D, NoteName.F, NoteName.A } },
      { NoteName.E, new NoteName[] { NoteName.E, NoteName.G, NoteName.B } },
      { NoteName.F, new NoteName[] { NoteName.F, NoteName.A, NoteName.C } },
      { NoteName.G, new NoteName[] { NoteName.G, NoteName.B, NoteName.D } },
      { NoteName.A, new NoteName[] { NoteName.A, NoteName.C, NoteName.E } },
      { NoteName.B, new NoteName[] { NoteName.B, NoteName.D, NoteName.F } },
    };

    for (int i = 0; i < melodyNotes.Count / 2 - 12; i += 2)
    {
      var chord = new List<Note>();
      var melodyNote = melodyNotes[i];
      var octave = melodyNote.Octave + 1;
      var length = melodyNote.Length * 2;
      var time = melodyNote.Time;
      if (i <= 62)
      {
        var chordNote = new Note((SevenBitNumber)2, length, time);
        chord.Add(chordNote);
      }
      else
      {
        var chordNotes = harmoniousChords[melodyNote.NoteName];
        foreach (var noteName in chordNotes)
        {
          var chordNote = new Note(noteName, octave, length, time);
          chord.Add(chordNote);
        }
      }
      chords.Add(chord);
    }
    return chords;
  }



  public IActionResult CreateMidi(int tempo, int key, int duration)
  {
    string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mid";
    int ticksPerQuarterNote = 480;
    double microsecondsPerMinute = 60000000.0;
    double tempoFactor = tempo / 60.0;
    int ticks = (int)((duration * ticksPerQuarterNote * tempoFactor) / (microsecondsPerMinute / 1000000.0));

    var melodyNotes = GenerateNotes(ticks, tempo);
    var bassLineNotes = GenerateBassLine(melodyNotes);
    var chordChords = GenerateHarmoniousChords(melodyNotes);

    var hiHatMeasure = new List<int[]> { new int[] { 0, 1, 2, 3, 4, 5, 6, 7 } };
    var bassDrumMeasure = new List<int[]> { new int[] { 0, 1, 4, 5 } };
    var snareDrumMeasure = new List<int[]> { new int[] { 2, 6 } };

    var hiHatBeats = GenerateDrumBeat(ticks, (SevenBitNumber)42, hiHatMeasure);
    var bassDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)36, bassDrumMeasure);
    var snareDrumBeats = GenerateDrumBeat(ticks, (SevenBitNumber)40, snareDrumMeasure);

    var melodyTrackChunk = new TrackChunk();
    var bassLineTrackChunk = new TrackChunk();
    var chordTrackChunk = new TrackChunk();
    var bassDrumTrackChunk = new TrackChunk();
    var snareDrumTrackChunk = new TrackChunk();
    var hiHatTrackChunk = new TrackChunk();

    foreach (var note in melodyNotes)
    {
      var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = 0, Channel = (FourBitNumber)0 };
      var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)0 };
      melodyTrackChunk.Events.Add(noteOnEvent);
      melodyTrackChunk.Events.Add(noteOffEvent);
    }
    foreach (var note in bassLineNotes)
    {
      var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = 0, Channel = (FourBitNumber)0 };
      var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)0 };
      bassLineTrackChunk.Events.Add(noteOnEvent);
      bassLineTrackChunk.Events.Add(noteOffEvent);
    }

    foreach (var chord in chordChords)
    {
      foreach (var note in chord)
      {
        note.Velocity = (SevenBitNumber)80;
        var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = 0, Channel = (FourBitNumber)0 };
        var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)0 };
        chordTrackChunk.Events.Add(noteOnEvent);
        chordTrackChunk.Events.Add(noteOffEvent);
      }
    }

    var drumTracks = new List<(TrackChunk, IEnumerable<Note>)>
    {
        (bassDrumTrackChunk, bassDrumBeats),
        (snareDrumTrackChunk, snareDrumBeats),
        (hiHatTrackChunk, hiHatBeats)
    };

    foreach (var (trackChunk, drumBeats) in drumTracks)
    {
      foreach (var note in drumBeats)
      {
        var noteOnEvent = new NoteOnEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = 0, Channel = (FourBitNumber)9 };
        var noteOffEvent = new NoteOffEvent((SevenBitNumber)note.NoteNumber, note.Velocity) { DeltaTime = note.Length, Channel = (FourBitNumber)9 };
        trackChunk.Events.Add(noteOnEvent);
        trackChunk.Events.Add(noteOffEvent);
      }
    }

    var midiFile = new MidiFile(melodyTrackChunk, bassLineTrackChunk, chordTrackChunk, bassDrumTrackChunk, snareDrumTrackChunk, hiHatTrackChunk);
    var tempoEvent = new SetTempoEvent((long)(60000000 / tempo)) { DeltaTime = 0 };
    melodyTrackChunk.Events.Insert(0, tempoEvent);

    string filePath = Path.Combine("wwwroot/Songs", filename);
    midiFile.Write(filePath);
    return RedirectToAction("PlayMidi", new { filename });
  }

  [HttpGet("/Songs/PlaySong/{filename}")]
  public IActionResult PlayMidi(string filename)
  {
    filename = filename.Replace("\\", "/");
    ViewBag.FileUrl = filename;
    return View("MidiCreated");
  }

  [HttpGet("/Songs/{filePath}")]
  public IActionResult MidiFile(string filePath)
  {
    var bytes = new byte[0];
    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
      var br = new BinaryReader(fs);
      long numBytes = new FileInfo(filePath).Length;
      bytes = br.ReadBytes((int)numBytes);
    }
    return File(bytes, "audio/midi", filePath);
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}
