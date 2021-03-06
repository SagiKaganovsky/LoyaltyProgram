﻿using Nancy;
using SpecialOffers.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialOffers.EventFeed
{
    public class EventsFeedModule : NancyModule
    {
        public EventsFeedModule(IEventStore eventStore) : base("/events")
        {
            Get("/", _ =>
            {
                long firstEventSequenceNumber = 0, lastEventSequenceNumber = 10;
                //if (!long.TryParse(this.Request.Query.start.Value, out firstEventSequenceNumber))
                //{

                //    firstEventSequenceNumber = 0;
                //}
                //if (!long.TryParse(this.Request.Query.end.Value, out lastEventSequenceNumber))
                //{

                //    lastEventSequenceNumber = long.MaxValue;
                //}
                return eventStore.GetEvents(firstEventSequenceNumber, lastEventSequenceNumber);
            });
        }
    }

}