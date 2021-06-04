namespace EventManagement.Core.Enumerations
{
    public enum TextFieldValueOperator
    {
        Equals,
        NotEquals,
        StartsWith,
        EndsWith,
        Contains,
        NotContains
    }

    public enum TextSearchMode
    {
        Simple,
        FullText
    }
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public enum QueryOrderDirection
    {
        Ascending = 1,
        Descending = 2
    }

    public enum QueryDisposition
    {
        Required = 1, // Denotes an AND operation
        Optional = 2, // Denotes an OR operation
        Forbidden = 3 // Denotes an NOT operation
    }
}
