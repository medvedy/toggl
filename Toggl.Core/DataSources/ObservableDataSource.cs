﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.DataSources
{
    public abstract class ObservableDataSource<TThreadsafe, TDatabase>
        : DataSource<TThreadsafe, TDatabase>, IObservableDataSource<TThreadsafe, TDatabase>
        where TDatabase : IDatabaseSyncable
        where TThreadsafe : IThreadSafeModel, IIdentifiable, TDatabase
    {
        public IObservable<TThreadsafe> Created { get; }

        public IObservable<EntityUpdate<TThreadsafe>> Updated { get; }

        public IObservable<long> Deleted { get; }

        protected Subject<long> DeletedSubject { get; } = new Subject<long>();

        protected Subject<TThreadsafe> CreatedSubject { get; } = new Subject<TThreadsafe>();

        protected Subject<EntityUpdate<TThreadsafe>> UpdatedSubject { get; } = new Subject<EntityUpdate<TThreadsafe>>();

        protected ObservableDataSource(IRepository<TDatabase> repository)
            : base(repository)
        {
            Created = CreatedSubject.AsObservable();
            Updated = UpdatedSubject.AsObservable();
            Deleted = DeletedSubject.AsObservable();
        }

        public override IObservable<TThreadsafe> Create(TThreadsafe entity)
            => base.Create(entity)
                .Do(CreatedSubject.OnNext);

        public override IObservable<TThreadsafe> Update(TThreadsafe entity)
            => base.Update(entity)
                .Do(updatedEntity => UpdatedSubject.OnNext(new EntityUpdate<TThreadsafe>(updatedEntity.Id, updatedEntity)));

        public override IObservable<TThreadsafe> ChangeId(long currentId, long newId)
            => base.ChangeId(currentId, newId)
                .Do(updatedEntity => UpdatedSubject.OnNext(new EntityUpdate<TThreadsafe>(currentId, updatedEntity)));

        public override IObservable<Unit> Delete(long id)
            => base.Delete(id)
                .Do(_ => DeletedSubject.OnNext(id));

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> OverwriteIfOriginalDidNotChange(
            TThreadsafe original, TThreadsafe entity)
            => base.OverwriteIfOriginalDidNotChange(original, entity)
                .Do(results => results.Do(HandleConflictResolutionResult));

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> BatchUpdate(IEnumerable<TThreadsafe> entities)
            => base.BatchUpdate(entities)
                .Do(updatedEntities => updatedEntities
                    .ForEach(HandleConflictResolutionResult));

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> DeleteAll(IEnumerable<TThreadsafe> entities)
            => base.DeleteAll(entities)
                .Do(updatedEntities => updatedEntities
                    .ForEach(HandleConflictResolutionResult));

        protected void HandleConflictResolutionResult(IConflictResolutionResult<TThreadsafe> result)
        {
            switch (result)
            {
                case DeleteResult<TThreadsafe> d:
                    DeletedSubject.OnNext(d.Id);
                    return;

                case CreateResult<TThreadsafe> c:
                    CreatedSubject.OnNext(c.Entity);
                    return;

                case UpdateResult<TThreadsafe> u:
                    UpdatedSubject.OnNext(new EntityUpdate<TThreadsafe>(u.OriginalId, u.Entity));
                    return;
            }
        }
    }
}
