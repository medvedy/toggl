﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Networking.ApiClients;
using Toggl.Shared;

namespace Toggl.Core.Sync
{
    public sealed class PullingApiClientAdapter<T> : IPullingApiClient<T>
    {
        private readonly IPullingSingleApiClient<T> pullingSingleApiClient;

        public PullingApiClientAdapter(IPullingSingleApiClient<T> pullingSingleApiClient)
        {
            Ensure.Argument.IsNotNull(pullingSingleApiClient, nameof(pullingSingleApiClient));

            this.pullingSingleApiClient = pullingSingleApiClient;
        }

        public IObservable<List<T>> GetAll()
            => pullingSingleApiClient.Get().Select(entity => new List<T> { entity });
    }
}
