﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using Android.Support.V4.Widget;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Giskard.Extensions.Reactive
{
    public static class SwipeRefreshLayoutExtensions
    {
        public static IObservable<Unit> Refreshed(this IReactive<SwipeRefreshLayout> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Refresh += e, e => reactive.Base.Refresh -= e)
                .SelectUnit();
    }
}
