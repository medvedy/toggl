using System.Threading.Tasks;

namespace Toggl.Shared.Extensions
{
    public static class TaskExtensions
    {
        public static Task<TBase> Upcast<TBase, TDerived>(this Task<TDerived> task)
            where TDerived : TBase
            => task.ContinueWith(t => (TBase)t.Result);
    }
}
