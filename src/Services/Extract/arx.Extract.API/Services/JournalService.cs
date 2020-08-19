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
        public override Task<PublicationResponse> GetExtractedPublications(PublicationRequest request, ServerCallContext context)
        {
            PublicationResponse response = new PublicationResponse() { Status = ResponseStatus.Failure };

            //Validate Input DateTime value
            DateTime queryFromDate = request.FromDate != null ? request.FromDate.ToDateTime() : DateTime.MinValue;
            if (queryFromDate == DateTime.MinValue)
            {
                response.Status = ResponseStatus.Invalid;
                return Task.FromResult(response);
            }

            if (request.ProcessedDataType == ProcessedDataType.Archive)
                return GetArchivePublications(response, queryFromDate);


            return Task.FromResult(response);
        }

        private async Task<PublicationResponse> GetArchivePublications(PublicationResponse response, DateTime queryFromDate)
        {
            try
            {
                var fulfillment = _fulfillmentRepo.GetLastSuccessfulArchiveFulfillment(queryFromDate).Result;

                if (fulfillment == null)
                {
                    string msg = $"Unable to find Archive Fulfillment record after queryFromDate [{queryFromDate}]";
                    response.ErrorMessage = msg;
                    Log.Fatal(msg);
                    return await Task.FromResult(response);
                }
                else
                {
                    var entities = await _publicationRepo.GetByFulfilmentId(fulfillment.FulfillmentId.ToString());

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

