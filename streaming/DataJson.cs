using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
namespace streaming
{
    class DataJson
    {
    }
    class ImageObject
    {
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("contentType")]
        public String ContentType { get; set; }
        [JsonProperty("contentSize")]
        public long ContentSize { get; set; }
        [JsonProperty("uploadDate")]
        public String UploadDate { get; set; }
        [JsonProperty("dateCreated")]
        public String DateCreated { get; set; }
        [JsonProperty("dateModified")]
        public String DateModified { get; set; }
        [JsonProperty("datePublished")]
        public String DatePublished { get; set; }
        [JsonProperty("contentUrl")]
        public string ContentUrl { get; set; }
        public ImageObject()
        {
            this.Name = "from_nao.jpg";
            this.ContentType = "image/jpeg";
            this.UploadDate = DateTime.Now.Date.ToString();
            this.DateCreated = DateTime.Now.Date.ToString();
            this.DateModified = DateTime.Now.Date.ToString();
            this.DatePublished = DateTime.Now.Date.ToString();
        }
        public override string ToString()
        {
            return Name + " (" + ContentSize + ") " + ContentUrl;
        }
    }
}
