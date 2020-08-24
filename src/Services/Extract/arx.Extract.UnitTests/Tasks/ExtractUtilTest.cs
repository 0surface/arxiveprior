using arx.Extract.BackgroundTasks.Core;
using arx.Extract.Data.Entities;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace arx.Extract.UnitTests.Tasks
{
    public class ExtractUtilTest
    {

        [Theory]
        [ClassData(typeof(ArchiveQueryDatesTestDataGenerator))]
        public void CalculateArchiveQueryDates_ValidParams_ReturnsCorrectResults
            (DateTime fromDate, DateTime toDate, int interval, Tuple<int, DateTime, DateTime> expected)
        {
            //Arrange
            //Act
            var sut = ExtractUtil.CalculateArchiveQueryDates(fromDate, toDate, interval);

            //Assert
            sut.Should().BeEquivalentTo(expected);
            sut.Item1.Should().Be(expected.Item1);
            sut.Item2.Should().Be(expected.Item2);
            sut.Item3.Should().Be(expected.Item3);
        }

        [Theory]
        [ClassData(typeof(ChunkedArchiveDatesTestDataGenerator))]
        public void GetRequestChunkedArchiveDates_PArams_ReturnsCorrectResults(FulfillmentEntity fulfillment, int queryInterval, List<ExtractQueryDates> expected )
        {
            //Act 
            var sut = ExtractUtil.GetRequestChunkedArchiveDates(fulfillment, queryInterval);

            //Assert
            sut.Should().BeEquivalentTo(expected);
        }
    }
}
