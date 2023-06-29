using NAudio.Midi;
namespace SongSmith.Models;
public class MidiEventCollection
{
  private List<MidiEvent> events;

  public MidiEventCollection()
  {
    events = new List<MidiEvent>();
  }

  public void AddEvent(MidiEvent midiEvent)
  {
    events.Add(midiEvent);
  }

  public void RemoveEvent(MidiEvent midiEvent)
  {
    events.Remove(midiEvent);
  }

  public IEnumerable<MidiEvent> GetEvents()
  {
    return events;
  }
}
