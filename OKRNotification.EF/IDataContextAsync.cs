
using System.Threading;
using System.Threading.Tasks;

namespace OKRNotification.EF
{
    public interface IDataContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
