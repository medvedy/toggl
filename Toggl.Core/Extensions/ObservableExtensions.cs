using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.Sync;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T2> ThenExecute<T1, T2>(this IObservable<T1> observable, Func<IObservable<T2>> continuation)
            => observable.SelectMany(_ => continuation());

        public static IObservable<T> Track<T>(this IObservable<T> observable, ITrackableEvent trackableEvent, IAnalyticsService service)
            => observable.Do(_ => service.Track(trackableEvent));

        public static IObservable<T> Track<T>(this IObservable<T> observable, Func<T, ITrackableEvent> eventFactory, IAnalyticsService service)
            => observable.Do(value => service.Track(eventFactory(value)));

        public static IObservable<T> Track<T>(this IObservable<T> observable, IAnalyticsEvent analyticsEvent)
            => observable.Do(analyticsEvent.Track);

        public static IObservable<T> Track<T>(this IObservable<T> observable, IAnalyticsEvent<T> analyticsEvent)
            => observable.Do(analyticsEvent.Track);

        public static IObservable<T> Track<T, T1>(this IObservable<T> observable, IAnalyticsEvent<T1> analyticsEvent, T1 parameter)
            => observable.Do(_ => analyticsEvent.Track(parameter));

        public static IObservable<T> Track<T, T1, T2>(this IObservable<T> observable, IAnalyticsEvent<T1, T2> analyticsEvent, T1 param1, T2 param2)
            => observable.Do(_ => analyticsEvent.Track(param1, param2));

        public static IObservable<T> Track<T, T1, TException>(this IObservable<T> observable, IAnalyticsEvent<T1> analyticsEvent, T1 parameter)
            where TException : Exception
            => observable.Catch<T, TException>(exception =>
            {
                analyticsEvent.Track(parameter);
                return Observable.Throw<T>(exception);
            });

        public static IObservable<ITransition> OnErrorReturnResult<T>(this IObservable<ITransition> observable, StateResult<T> errorResult)
            where T : Exception
            => observable.Catch((T exception) => Observable.Return(errorResult.Transition(exception)));

        public static IObservable<T> ReemitWhen<T>(
            this IObservable<T> observable,
            IObservable<Unit> otherObservable)
        {
            var signal = otherObservable.StartWith(Unit.Default);
            return observable.CombineLatest(signal, (t1, _) => t1);
        }

        public static IObservable<T> TrackExecution<T>(this IObservable<T> observable, ActivityObserver activityObserver)
        {
            return activityObserver.trackActivityOfObservable(observable);
        }
    }

    internal class ActivityToken<E> : IDisposable
    {
        private IObservable<E> _source;
        private IDisposable _dispose;

        internal ActivityToken(IObservable<E> source, BehaviorSubject<bool> running)
        {
            _source = source;
            _dispose = Disposable.Create(() => running.OnNext(false));
        }

        internal IObservable<E> AsObservable()
        {
            return _source;
        }

        public void Dispose()
        {
            _dispose.Dispose();
        }
    }

    public class ActivityObserver : IDisposable
    {
        public IObservable<bool> running { get; }
        private BehaviorSubject<bool> runningSubject = new BehaviorSubject<bool>(false);

        public ActivityObserver()
        {
            running = runningSubject.AsObservable();
            runningSubject.OnNext(true);
        }

        public void Dispose()
        {
            runningSubject.OnNext(false);
        }

        internal IObservable<T> trackActivityOfObservable<T>(IObservable<T> source)
        {
            return Observable.Using(
                () =>
                {
                    runningSubject.OnNext(true);
                    return new ActivityToken<T>(source, runningSubject);
                },
                t => t.AsObservable()
            );
        }
    }
}
