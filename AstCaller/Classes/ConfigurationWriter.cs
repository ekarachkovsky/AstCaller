using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AstCaller.Classes
{
    public class ConfigurationWriter
    {
        private readonly IWebHostEnvironment _environment;

        public ConfigurationWriter(IWebHostEnvironment env)
        {
            _environment = env;
        }

        public void Set(string path, string value)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fi = fileProvider.GetFileInfo("appsettings.json");

            var config = JObject.Parse(File.ReadAllText(fi.PhysicalPath));

            var pathElements = path.Split(":");

            JToken token = config;

            for (var i = 0; i < pathElements.Length - 1; i++)
            {
                var pathItem = pathElements[i];
                if (token[pathItem] == null)
                {
                    token[pathItem] = JToken.Parse("{}");
                }
                token = token[pathItem];
            }

            token[pathElements[^1]] = value;

            using StreamWriter file = File.CreateText(fi.PhysicalPath);
            using var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented };
            config.WriteTo(writer);
        }
    }
}
