using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace arx.Extract.Lib
{
    public interface IArchiveFetch
    {
        Task<(HttpResponseMessage,ArxivItem)> GetArxivItems(string url);
    }
    public class ArchiveFetch : IArchiveFetch
    {
        public async Task<(HttpResponseMessage, ArxivItem)> GetArxivItems(string url)
        {
            string result;
            HttpResponseMessage response;
            ArxivItem item = new ArxivItem();
            try
            {
                (response, result) = await HttpQuery(url);

                item = DeserializeXml(result);
                item.EntryList = item?.EntryList?.OrderBy(x => x.PublishDate)?.ToList();

                item = item != null ? item : new ArxivItem();
                return (response, item);
            }
            catch (Exception ex)
            {
                item.Error = ex;                
                return (new HttpResponseMessage(HttpStatusCode.InternalServerError), item);
            }
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

        private async Task<(HttpResponseMessage, string)> HttpQuery(string url)
        {
            string result;
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                response = await httpClient.SendAsync(request);
                result = await response.Content.ReadAsStringAsync();
            }

            return (response, result);
        }
    }
}
