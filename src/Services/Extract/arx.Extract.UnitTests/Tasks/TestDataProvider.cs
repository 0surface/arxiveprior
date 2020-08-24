using arx.Extract.BackgroundTasks.Core;
using arx.Extract.Data.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace arx.Extract.UnitTests.Tasks
{
    public class ArchiveQueryDatesTestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]{new DateTime(2020, 08, 08), new DateTime(2020, 08, 13), 5, new Tuple<int,DateTime,DateTime>(5, new DateTime(2020, 08, 03), new DateTime(2020, 08, 07, 23, 59, 59)) },
            new object[]{new DateTime(2020, 08, 08), new DateTime(2020, 08, 13), 1, new Tuple<int,DateTime,DateTime>(5, new DateTime(2020, 08, 03), new DateTime(2020, 08, 07, 23, 59, 59)) },
            new object[]{ DateTime.MinValue, new DateTime(2020, 08, 13), 3, new Tuple<int,DateTime,DateTime>(3, DateTime.UtcNow.Date.AddDays(-2 - 3), DateTime.UtcNow.Date.AddDays(-2).AddSeconds(-1)) },
            new object[]{ DateTime.MinValue, DateTime.MinValue, 6, new Tuple<int,DateTime,DateTime>(6, DateTime.UtcNow.Date.AddDays(-2 - 6), DateTime.UtcNow.Date.AddDays(-2).AddSeconds(-1)) },
            new object[]{new DateTime(2020, 08, 02), new DateTime(2020, 08, 07, 23, 59, 59), 5, new Tuple<int, DateTime, DateTime>(5, new DateTime(2020, 07, 28), new DateTime(2020, 08, 01, 23, 59, 59))}
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ChunkedArchiveDatesTestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]{ new FulfillmentEntity(){ QueryFromDate = new DateTime(2020, 08, 08), QueryToDate = new DateTime(2020, 08, 13) },5
                , new List<ExtractQueryDates>(){ new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 08, 03), QueryToDate = new DateTime(2020, 08, 07, 23, 59, 59) }} },

            new object[]{ new FulfillmentEntity(){ QueryFromDate = new DateTime(2020, 08, 07), QueryToDate = new DateTime(2020, 08, 13) },3
                , new List<ExtractQueryDates>(){ new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 08, 04), QueryToDate = new DateTime(2020, 08, 06, 23, 59, 59) },
                                                new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 08, 01), QueryToDate = new DateTime(2020, 08, 03, 23, 59, 59) }} },

            /* chunck 1 = 04-08-2020 --> 06-08-2020 23:59:59  has 3 days interval
             * chunck 2 = 01-08-2020 --> 03-08-2020 23:59:59  has 3 days interval
             * chunck 3 = 30-07-2020 --> 31-07-2020 23:59:59  has 2 days interval ( 8 - 2 * 3 = 2)
             */
            new object[]{ new FulfillmentEntity(){ QueryFromDate = new DateTime(2020, 08, 07), QueryToDate = new DateTime(2020, 08, 15) },3
                , new List<ExtractQueryDates>(){ new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 08, 04), QueryToDate = new DateTime(2020, 08, 06, 23, 59, 59) },
                                                new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 08, 01), QueryToDate = new DateTime(2020, 08, 03, 23, 59, 59) },
                                                new ExtractQueryDates() { QueryFromDate = new DateTime(2020, 07, 30), QueryToDate = new DateTime(2020, 07, 31, 23, 59, 59) }} }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
