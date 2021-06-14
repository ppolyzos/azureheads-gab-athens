namespace EventManagement.Core.EF.Repositories.Sql
{
    public class QueryModule
    {
        public string AssemblyString { get; }
        public string TextToReplace { get; }

        public QueryModule(string assemblyString, string textToReplace)
        {
            AssemblyString = assemblyString;
            TextToReplace = textToReplace;
        }
    }
}