using Journal.Domain.AggregatesModel.JobAggregate;
using System;
using System.Linq;

namespace Journal.Infrastructure.Repositories
{
    public interface IFulfillmentRepository
    {
        Fulfillment Save(Fulfillment fulfillment);

        (bool found, DateTime fromDate, DateTime toDate) FindLatestProcessedQueryDates(ProcessTypeEnum processType);
    }

    public class FulfillmentRepository : IFulfillmentRepository
    {
        private readonly FulfillmentContext _context;

        public FulfillmentRepository(FulfillmentContext context)
        {
            _context = context;
        }

        public Fulfillment Save(Fulfillment fulfillment)
        {
            _context.Attach(fulfillment);
            _context.SaveChanges();
            return fulfillment;
        }

        public (bool found, DateTime fromDate, DateTime toDate) FindLatestProcessedQueryDates(ProcessTypeEnum processType)
        {
            var record = _context.Fulfillments
                            .Where(x => x.JournalType == processType && x.IsProcessed)
                            .OrderByDescending(x => x.JobCompletedDate)
                            .FirstOrDefault();

            return record == null ?
                    (false, DateTime.MinValue, DateTime.MinValue) :
                    (true, record.QueryFromDate, record.QueryToDate);
        }
    }
}
