using Common.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using System.Collections.Generic;

namespace Journal.BackgroundTasks.Core
{
    public interface ITransformService
    {
        (bool, List<Article>) MapToDomain(List<ArxivPublication> publications, int journalProcessId);
    }
}
