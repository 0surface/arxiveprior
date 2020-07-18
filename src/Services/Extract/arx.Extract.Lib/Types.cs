using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace arx.Extract.Lib
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class ArxivItem
    {
        [XmlIgnore]
        public Exception Error { get; set; }

        [XmlIgnore]
        public int RequestCount { get; set; }

        [XmlIgnore]
        public string Category { get; set; }


        [XmlElement(Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public int itemsPerPage { get; set; }

        [XmlElement(Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public int startIndex { get; set; }

        [XmlElement(Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public int totalResults { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("updated")]
        public DateTime Updated { get; set; }

        [XmlElement("id")]
        public string id { get; set; }

        [XmlElement("link")]
        public FeedLink FeedLink { get; set; }

        [XmlElement("entry")]
        public List<Entry> EntryList { get; set; }
    }

    [XmlRoot("entry")]
    public class Entry
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("published")]
        public DateTime PublishDate { get; set; }

        [XmlElement("updated")]
        public DateTime UpdatedDate { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("summary")]
        public string Summary { get; set; }

        [XmlElement(Namespace = "http://arxiv.org/schemas/atom")]
        public string comment { get; set; }

        [XmlElement(Namespace = "http://arxiv.org/schemas/atom")]
        public PrimarySubject primary_category { get; set; }

        [XmlElement(Namespace = "http://arxiv.org/schemas/atom")]
        public string journal_ref { get; set; }

        [XmlElement(Namespace = "http://arxiv.org/schemas/atom")]
        public string doi { get; set; }

        [XmlElement("link")]
        public List<EntryLink> Links { get; set; }

        [XmlElement("category")]
        public List<Subjects> Subjects { get; set; }

        [XmlElement("author")]
        public List<Author> Authors { get; set; }
    }


    [XmlRoot("author")]
    public class Author
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }

    [XmlRoot("category")]
    public class PrimarySubject
    {
        [XmlAttribute("term")]
        public string Subject { get; set; }
    }

    [XmlRoot("category")]
    public class Subjects
    {
        [XmlAttribute("term")]
        public string Subject { get; set; }
    }

    [XmlRoot("link")]
    public class EntryLink
    {
        [XmlAttribute("rel")]
        public string rel { get; set; }

        [XmlAttribute("type")]
        public string type { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }
    }


    [XmlRoot("link")]
    public class FeedLink
    {
        [XmlAttribute("rel")]
        public string rel { get; set; }

        [XmlAttribute("type")]
        public string type { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }
    }
}
