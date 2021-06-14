using System.Collections.Generic;

namespace EventManagement.Core.EF.Repositories.Sql
{
    public interface ISqlQueryRepository
    {
        IEnumerable<string> GetKeys();
        string GetQueryFor(string key);

        string GetQueryFromFile(string filePath);

        IEnumerable<string> GetQueriesInFolder(string path);
        IEnumerable<string> GetAllCommands(string path);
    }
}