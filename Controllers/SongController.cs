using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SongSmith.Models;
using NAudio.Wave;
using NAudio.Midi;
using System.IO;

namespace SongSmith.Controllers;

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

  private List<string> GenerateNotes(int key, int duration)
  {
    List<string> notes = new List<string>();
    string[] availableNotes = { "C", "D", "E", "F", "G", "A", "B" };
    Random random = new Random();
    for (int i = 0; i < duration; i++)
    {
      int randomIndex = random.Next(availableNotes.Length);
      string note = availableNotes[randomIndex] + (key / 7 + 1);
      notes.Add(note);
    }
    return notes;
  }

  [HttpPost("/songs/create")]
  public IActionResult CreateMidi(int tempo, int key, int duration)
  {
    // Generate the filename
    string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mid";

    // Create a list of notes based on the provided key and duration
    List<string> notes = GenerateNotes(key, duration);

    // Parse the notes and convert them to MIDI values
    PitchParser pitchParser = new PitchParser();
    var midiValues = pitchParser.Parse(notes);

    if (midiValues.Any())
    {
      // Save the MIDI file using the MidiExporter
      var exporter = new MidiExporter();
      string filePath = Path.Combine("Songs", filename);
      exporter.SaveToFile(filePath, midiValues);

      return RedirectToAction("PlayMidi", new { filePath });
    }

    return View();
  }

  [HttpGet("/Songs/PlaySong/{filePath}")]
  public IActionResult PlayMidi(string filePath)
  {
    filePath = filePath.Replace("\\", "/");
    ViewBag.FileUrl = filePath;
    string midiFile = "C:/Users/themo/OneDrive/Documents/Coding Dojo Stuffs/Projects/SongSmith/Songs/20230628173741.mid";
    var sequence = new MidiFile(midiFile);
    Console.WriteLine("test");
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
