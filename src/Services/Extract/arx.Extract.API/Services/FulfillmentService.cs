using arx.Extract.Data.Repository;
using arx.Extract.Types;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.API.Services
{
    public interface IFulfillmentService 
    {
        Task<List<Fulfillment>> GetFulfillments(string jobName);
        Task<List<Fulfillment>> GetFulfillmentsBetweenQueryDates(string jobName, DateTime queryFromDate, DateTime queryToDate);
        Task<Fulfillment> GetLastFulfillment(string jobName);             
        Task<List<Fulfillment>> GetFailedFulfillments(string jobName);
    }

    public class FulfillmentService : IFulfillmentService
    {
        private readonly IFulfillmentRepository _fulfillmentRepo;
        private readonly IMapper _mapper;

        public FulfillmentService(IFulfillmentRepository fulfillmentRepo, IMapper mapper)
        {
            _fulfillmentRepo = fulfillmentRepo;
            _mapper = mapper;
        }

        public async Task<List<Fulfillment>> GetFulfillments(string jobName)
        {
            try
            {
                var result = await _fulfillmentRepo.GetFulfillments(jobName);

                return (result.Count > 0) ? _mapper.Map<List<Fulfillment>>(result) : new List<Fulfillment>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<List<Fulfillment>> GetFulfillmentsBetweenQueryDates(string jobName, DateTime queryFromDate, DateTime queryToDate)
        {
            try
            {
                var result = await _fulfillmentRepo.GetFulfillmentsBetweenQueryDates(jobName, queryFromDate, queryToDate);

                return (result.Count > 0) ? _mapper.Map<List<Fulfillment>>(result) : new List<Fulfillment>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Fulfillment> GetLastFulfillment(string jobName)
        {
            try
            {
                var result = await _fulfillmentRepo.GetLastFulfillment(jobName);

                return result != null ? _mapper.Map<Fulfillment>(result) : new Fulfillment();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Fulfillment>> GetFailedFulfillments(string jobName)
        {
            try
            {
                var result = await _fulfillmentRepo.GetFailedFulfillments(jobName);

                return (result.Count > 0) ? _mapper.Map<List<Fulfillment>>(result) : new List<Fulfillment>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
