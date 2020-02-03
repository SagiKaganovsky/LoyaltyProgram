using SpecialOffers.Models;
using System.Collections.Generic;

namespace SpecialOffers.Stores
{
    public interface IEventStore
  {
    IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
    void Raise(string eventName, object content);
  }
}