using Journal.BackgroundTasks.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using System.Collections.Generic;

namespace Journal.BackgroundTasks.Services
{
    public interface ITransformService
    {
        (bool, List<Article>) MapToDomain(List<PublicationItemDto> publications, int journalProcessId);
    }
}
