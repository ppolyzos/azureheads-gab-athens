namespace EventManagement.Web.Application.Cache.Enumerations
{
    public class CacheDuration
    {
        public const int NoCache = 0;
        public const int CacheLow = 60;
        public const int CacheMedium = 180;
        public const int CacheLong = 360;
        public const int CacheDay = 3600;
    }
}