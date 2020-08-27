using Journal.BackgroundTasks.Types;
using System.Threading.Tasks;

namespace Journal.BackgroundTasks.Services
{
    public interface IExtractApiService
    {
        Task<PublicationResponseDto> GetExtractedPublications(string fulfillmentId);
    }
}
