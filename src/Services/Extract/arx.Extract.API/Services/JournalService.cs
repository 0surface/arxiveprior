using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.API.Services
{
    public class JournalService : ExtractService.ExtractServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IPublicationRepository _publicationRepo;
        private readonly IFulfillmentRepository _fulfillmentRepo;

        public JournalService(IMapper mapper,
            IPublicationRepository publicationRepo,
            IFulfillmentRepository fulfillmentRepo)
        {
            _mapper = mapper;
            _publicationRepo = publicationRepo;
            _fulfillmentRepo = fulfillmentRepo;
        }

        public override async Task<PublicationResponse> GetExtractedPublications(PublicationRequest request, ServerCallContext context)
        {
            PublicationResponse response = new PublicationResponse() { Status = ResponseStatus.Failure };
            try
            {
                FulfillmentEntity fulfillment = await _fulfillmentRepo.GetFulfillment(request.FulfillmentId);

                if (fulfillment == null)
                {
                    string msg = $"Unable to find Archive Fulfillment record  [{request.FulfillmentId}]";
                    response.ErrorMessage = msg;
                    Log.Error(msg);
                    return await Task.FromResult(response);
                }
                else
                {
                    List<PublicationItemEntity> entities = await _publicationRepo.GetByFulfilmentId(fulfillment.FulfillmentId.ToString());                    
                    response.Publications.AddRange(_mapper.Map<List<Publication>>(entities));
                    response.Status = ResponseStatus.Success;
                    response.FulfillmentId = fulfillment.FulfillmentId.ToString();
                    response.QueryFromDate = Timestamp.FromDateTime(fulfillment.QueryFromDate);
                    response.QueryToDate = Timestamp.FromDateTime(fulfillment.QueryToDate);

                    return response;
                }
            }
            catch (Exception ex)
            {
                string msg = $"Attmepting to retrieve Archive Publications data.[{ ex.Message}]";
                Log.Fatal(ex, msg);
                response.ErrorMessage = msg;
                return response;
            }
        }
    }
}