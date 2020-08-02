using arx.Extract.Data.Repository;
using arx.Extract.Types;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.API.Services
{
    public interface IFulfillmentItemService
    {
        Task<List<FulfillmentItem>> GetItems(string fulfillmentId);
    }
    public class FulfillmentItemService : IFulfillmentItemService
    {
        private readonly IFulfillmentItemRepository _repo;
        private readonly IMapper _mapper;

        public FulfillmentItemService(IFulfillmentItemRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<FulfillmentItem>> GetItems(string fulfillmentId)
        {
            try
            {
                var result = await _repo.GetFulfillmentItems(fulfillmentId);

                return (result.Count > 0) ? _mapper.Map<List<FulfillmentItem>>(result) : new List<FulfillmentItem>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
