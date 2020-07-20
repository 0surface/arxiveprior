using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using System;
using System.Collections.Generic;

namespace arx.Extract.BackgroundTasks.Core
{
    public static class TransformToPublications
    {
        public static List<PublicationItemEntity> TransformArxivEntriesToPublications(List<Entry> allArxivEntries)
        {
            List<PublicationItemEntity> results = new List<PublicationItemEntity>();
            try
            {
                foreach (var item in allArxivEntries)
                {

                }
            }
            catch (Exception)
            {
                //TODo:Error Handling
            }

            return results;
        }
    }
}
