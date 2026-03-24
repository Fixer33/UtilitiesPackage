using System;
using System.Threading;
using UnityEngine;

namespace Utilities.Extensions
{
    public static class ObservableExtensions
    {
        public static async Awaitable WaitFor(this Observable<bool> observable, bool state, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value != state)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitUntil(this Observable<bool> observable, Func<bool, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitWhile(this Observable<bool> observable, Func<bool, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        public static async Awaitable WaitUntilHigherThan(this Observable<float> observable, float value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value <= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitUntilLowerThan(this Observable<float> observable, float value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value >= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        public static async Awaitable WaitUntil(this Observable<float> observable, Func<float, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitWhile(this Observable<float> observable, Func<float, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitUntilHigherThan(this Observable<int> observable, int value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value <= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        public static async Awaitable WaitUntilLowerThan(this Observable<int> observable, int value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value >= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        public static async Awaitable WaitUntil(this Observable<int> observable, Func<int, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        public static async Awaitable WaitWhile(this Observable<int> observable, Func<int, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
    }
}