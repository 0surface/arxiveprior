using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override async Task StreamExtractedPublications(PublicationRequest request,
            IServerStreamWriter<PublicationResponse> responseStream,
            ServerCallContext context)
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
                }
                else
                {
                    List<PublicationItemEntity> entities = await _publicationRepo.GetByFulfilmentId(fulfillment.FulfillmentId.ToString());
                    List<Publication> publications = _mapper.Map<List<Publication>>(entities).ToList();

                    int chunkSize = 1000;
                    int countIteration = (publications.Count == 0) ? 0 :
                                    ((publications.Count <= 1000) ? 1 :
                                            1 + (int)Math.Ceiling((decimal)(publications.Count / chunkSize)));

                    Log.Information($"Start of Stream - gRPC stream chunked into [{countIteration}] chunks of [{chunkSize}]");

                    for (int i = 0; i < countIteration; i++)
                    {
                        PublicationResponse chunkedResponse = new PublicationResponse()
                        {
                            Status = ResponseStatus.Success,
                            FulfillmentId = fulfillment.FulfillmentId.ToString(),
                            QueryFromDate = Timestamp.FromDateTime(fulfillment.QueryFromDate),
                            QueryToDate = Timestamp.FromDateTime(fulfillment.QueryToDate)
                        };

                        Log.Information($"{i} : gRPC streaming : Skip {i * chunkSize}");
                        chunkedResponse.Publications.Add(publications.Skip(i * chunkSize).Take(chunkSize));

                        await responseStream.WriteAsync(chunkedResponse);
                    }

                    Log.Information($"End of Stream - gRPC streamed in [{countIteration}] chunks");
                }
            }
            catch (Exception ex)
            {
                string msg = $"Attmepting to retrieve Archive Publications data.[{ ex.Message}]";
                Log.Fatal(ex, msg);
                response.ErrorMessage = msg;
                await responseStream.WriteAsync(response);
            }
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

                    var chunk = _mapper.Map<List<Publication>>(entities).Take(1000);
                    response.Publications.AddRange(chunk);

                    //response.Publications.AddRange(_mapper.Map<List<Publication>>(entities));
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