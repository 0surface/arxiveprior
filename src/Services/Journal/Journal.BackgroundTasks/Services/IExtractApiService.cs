using Journal.BackgroundTasks.Types;
using System;
using System.Threading.Tasks;

namespace Journal.BackgroundTasks.Services
{
    public interface IExtractApiService
    {
        Task<PublicationResponseDto> GetFirstArchiveExtraction();
        Task<PublicationResponseDto> GetExtractedPublications(string fulfillmentId);
        Task<PublicationResponseDto> GetArchiveExtractedPublications(DateTime fromDate, DateTime toDate);
    }
}
