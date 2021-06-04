using System.Threading.Tasks;

namespace EventManagement.Core.Utilities
{
    public interface ICleanup
    {
        Task CleanupAsync();
    }
}
