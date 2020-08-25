using Journal.Domain.AggregatesModel.JobAggregate;
using System.Linq;

namespace Journal.Infrastructure.Repositories
{
    public interface IFulfillmentRepository
    {
        Fulfillment Save(Fulfillment fulfillment);
        (bool found, Fulfillment fulfillment) FindNextUnprocessedFulfillment(ProcessTypeEnum processType);
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

        public (bool found, Fulfillment fulfillment) FindNextUnprocessedFulfillment(ProcessTypeEnum processType)
        {
            var record = _context.Fulfillments
                .Where(x => x.JournalType == processType)
                .Where(x => !x.IsPending
                            && !x.IsProcessed
                            && !string.IsNullOrEmpty(x.ExtractionFulfillmentId))
                .OrderBy(x => x.Created)
                .FirstOrDefault();

            return record == null ?
                (false, null) :
                (true, record);
        }
    }
}
