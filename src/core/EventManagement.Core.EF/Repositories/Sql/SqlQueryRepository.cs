using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EventManagement.Core.EF.Repositories.Sql
{
    public class SqlQueryRepository : ISqlQueryRepository
    {
        private readonly Dictionary<string, string> _queries;

        public SqlQueryRepository(ICollection<QueryModule> queryModules)
        {
            if (!queryModules.Any())
            {
                // TODO: Till you understand why queryModules are injected as empty list
                queryModules.Add(new QueryModule("BookNDrive.Api", "BookNDrive.Api.Data.Queries.Sql"));
                queryModules.Add(new QueryModule("BookNDrive.Api.Core", "BookNDrive.Api.Core.Data.Queries.Sql"));
                queryModules.Add(new QueryModule("BookNDrive.Core", "BookNDrive.Core.Data.Queries.Sql"));
                queryModules.Add(new QueryModule("BookNDrive.Core.EF", "BookNDrive.Core.EF.Data.Queries.Sql"));
            }

            _queries = new Dictionary<string, string>();

            foreach (var queryModule in queryModules)
            {
                var assembly = Assembly.Load(queryModule.AssemblyString);

                if (assembly == null) return;

                var resourceNames = assembly.GetManifestResourceNames();

                foreach (var resourceName in resourceNames)
                {
                    if (!resourceName.EndsWith("sql")) return;

                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null) continue;

                        using (var reader = new StreamReader(stream))
                        {
                            _queries.Add(resourceName.Replace($"{queryModule.TextToReplace}.", ""), reader.ReadToEnd());
                        }
                    }
                }
            }
        }

        public string GetQueryFromFile(string filepath)
        {
            var query = string.Empty;
            if (!File.Exists(filepath)) return query;

            using (var reader = new StreamReader(filepath))
            {
                query = reader.ReadToEnd();
            }

            return query;
        }


        public IEnumerable<string> GetAllCommands(string path)
        {
            if (!File.Exists(path)) yield return null;

            StringBuilder sb = null;
            foreach (var line in File.ReadLines(path))
            {
                if (string.Equals(line, "GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (sb == null || sb.Length == 0) continue;

                    var item = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                        yield return item;

                    sb = null;
                }
                else
                {
                    if (sb == null) sb = new StringBuilder();

                    sb.AppendLine(line);
                }
            }

            if (sb == null || sb.Length == 0) yield break;
            {
                var item = sb.ToString();
                if (!string.IsNullOrWhiteSpace(item))
                    yield return item;
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return _queries.Keys;
        }

        public string GetQueryFor(string key)
        {
            return !_queries.ContainsKey(key) ? string.Empty : _queries[key];
        }

        public IEnumerable<string> GetQueriesInFolder(string path)
        {
            return _queries.Keys.Where(c => c.StartsWith(path)).OrderBy(key => key);
        }
    }
}