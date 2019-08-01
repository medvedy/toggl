using System;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Shared;

namespace Toggl.Networking.ApiClients
{
    public interface IStatusApi
    {
        Task<Either<Unit, Exception>> IsAvailable();
    }
}
