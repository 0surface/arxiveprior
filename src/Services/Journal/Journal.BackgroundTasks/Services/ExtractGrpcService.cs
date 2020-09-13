using arx.Extract.API.Services;
using AutoMapper;
using Journal.BackgroundTasks.Config;
using Journal.BackgroundTasks.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static arx.Extract.API.Services.ExtractService;

namespace Journal.BackgroundTasks.Services
{
    public class ExtractGrpcService : ExtractServiceClient
    {
        private readonly ILogger<ExtractGrpcService> _logger;
        private readonly IMapper _mapper;
        private readonly UrlsConfig _urlsConfig;
        private readonly JournalBackgroundTasksConfiguration _config;

        public ExtractGrpcService(ILogger<ExtractGrpcService> logger,
            IMapper mapper,
            IOptions<JournalBackgroundTasksConfiguration> config,
             IOptions<UrlsConfig> urlsConfig)

        {
            _logger = logger;
            _mapper = mapper;
            _urlsConfig = urlsConfig.Value;
            _config = config.Value;
        }

        public async Task<PublicationResponseDto> GetExtractedPublications(string fulfillmentId)
        {
            try
            {
                return await GrpcCallerService.CallService(_urlsConfig.GrpcExtract, async channel =>
                {
                    var client = new ExtractServiceClient(channel);

                    var request = new PublicationRequest
                    {
                        ProcessedDataType = ProcessedDataType.Archive,
                        FulfillmentId = fulfillmentId
                    };

                    _logger.LogInformation("grpc client created, request = {@request}", request);
                    var response = await client.GetExtractedPublicationsAsync(request);
                    _logger.LogInformation("grpc response {@response}", response);

                    var dto = MapToArxivPublication(response);

                    if (!dto.IsSuccess)
                    {
                        _logger.LogError("ExtractServiceClient grpc request returned a non-successful result.", request, dto.ErrorMessage);
                    }

                    return dto;
                });
            }
            catch (Exception ex)
            {
                return new PublicationResponseDto()
                {
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }

        private PublicationResponseDto MapToArxivPublication(PublicationResponse response)
        {

            if (response.Status != ResponseStatus.Success)
            {
                return new PublicationResponseDto()
                {
                    IsSuccess = false,
                    ErrorMessage = response.ErrorMessage
                };
            }
            else
            {
                PublicationResponseDto dto = new PublicationResponseDto()
                {
                    IsSuccess = true,
                    QueryFromDate = response.QueryFromDate.ToDateTime(),
                    QueryToDate = response.QueryToDate.ToDateTime(),
                    ExtractFulfillmentId = response.FulfillmentId
                };
                dto.PublicationItems.AddRange(_mapper.Map<List<PublicationItemDto>>(response.Publications));
                return dto;
            }

        }
    }
}
