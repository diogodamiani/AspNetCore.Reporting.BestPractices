using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace AspNetCore.Reporting.Common.Services
{
    public class ExpandoObjectLoader
    {
        private readonly Guid _id;

        public ExpandoObjectLoader()
        {
        }

        public ExpandoObjectLoader(Guid id)
        {
            _id = id;
        }

        public IEnumerable<ExpandoObject> GetData()
        {
            return new[]
            {
                JsonConvert.DeserializeObject<ExpandoObject>("{ \"Id\": 30, \"Nome\": \"abc\" }"),
                JsonConvert.DeserializeObject<ExpandoObject>("{ \"Id\": 40, \"Nome\": \"def\" }"),
            };
        }

        public ExpandoObject GetDataSchema()
        {
            return JsonConvert.DeserializeObject<ExpandoObject>("{ \"Id\": 30, \"Nome\": \"abc\" }");
        }
    }
}