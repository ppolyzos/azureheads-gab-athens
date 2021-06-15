﻿using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventManagement.Installers.Tools.Swagger.Filters
{
    public class LowerCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths;

            // generate new keys
            var newPaths = new Dictionary<string, OpenApiPathItem>();
            var removeKeys = new List<string>();
            foreach (var path in paths)
            {
                var newKey = path.Key.ToLower();
                if (newKey == path.Key) continue;

                removeKeys.Add(path.Key);
                newPaths.Add(newKey, path.Value);
            }

            //	add the new keys
            foreach (var path in newPaths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }

            //	remove the old keys
            foreach (var key in removeKeys)
            {
                swaggerDoc.Paths.Remove(key);
            }
        }
    }
}
