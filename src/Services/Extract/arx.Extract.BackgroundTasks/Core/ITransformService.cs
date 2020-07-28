using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using arx.Extract.Types;
using System.Collections.Generic;

namespace arx.Extract.BackgroundTasks.Core
{
    public interface ITransformService
    {
        (bool, int, List<PublicationItem>) TransformArxivEntriesToPublications(List<Entry> allArxivEntries);
        (bool, List<PublicationItemEntity>) TransformPublicationItemsToEntity(string fulfillmentId, string fulfillmentItemId, List<PublicationItem> publications);
    }
}
