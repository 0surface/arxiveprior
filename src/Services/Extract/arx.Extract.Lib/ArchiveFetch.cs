using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace arx.Extract.Lib
{
    public interface IArchiveFetch
    {
        Task<ArxivItem> GetArxivItems(string url);
    }
    public class ArchiveFetch : IArchiveFetch
    {
        public async Task<ArxivItem> GetArxivItems(string url)
        {
            string result;
            ArxivItem item = new ArxivItem();
            try
            {
                result = await HttpQuery(url);

                item = DeserializeXml(result);
                item.EntryList = item?.EntryList?.OrderBy(x => x.PublishDate)?.ToList();

                return item != null ? item : new ArxivItem();
            }
            catch (Exception ex)
            {
                item.Error = ex;
            }
            return item;
        }

        private ArxivItem DeserializeXml(string result)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result.ToString());

            XmlNodeReader xreader = new XmlNodeReader(xdoc);
            XmlSerializer deserializer = new XmlSerializer(typeof(ArxivItem));
            var item = (ArxivItem)deserializer.Deserialize(xreader);

            return item;
        }

        private async Task<string> HttpQuery(string url)
        {
            string result;
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await httpClient.SendAsync(request);
                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}
