using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace AspNetCore.Reporting.Common.Services
{
    public class CompileTimeType
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class CompileTimeTypeLoader
    {
        private readonly Guid _id;

        public CompileTimeTypeLoader()
        {

        }

        public CompileTimeTypeLoader(Guid id)
        {
            _id = id;
        }

        public IEnumerable<CompileTimeType> GetData()
        {
            return new[]
            {
                JsonConvert.DeserializeObject<CompileTimeType>("{ \"Id\": 30, \"Nome\": \"abc\" }"),
                JsonConvert.DeserializeObject<CompileTimeType>("{ \"Id\": 40, \"Nome\": \"def\" }"),
            };
        }
    }
}