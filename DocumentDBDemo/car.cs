using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDBDemo
{
    public class Car
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "manufacturer")]
        public string Manufacturer { get; set; }
        [JsonProperty(PropertyName = "manufactureyear")]
        public DateTime ManufactureYear { get; set; }
        [JsonProperty(PropertyName = "baseprice")]
        public decimal BasePrice { get; set; }

        public static implicit operator Car(ResourceResponse<Document> v)
        {
            throw new NotImplementedException();
        }
    }
}
