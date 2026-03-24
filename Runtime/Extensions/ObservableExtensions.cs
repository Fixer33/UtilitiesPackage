using System;
using System.Threading;
using UnityEngine;

namespace Utilities.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="Observable{T}"/> to support asynchronous waiting operations.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Waits until the observable reaches the specified state.
        /// </summary>
        /// <param name="observable">The observable boolean to watch.</param>
        /// <param name="state">The target state to wait for.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitFor(this Observable<bool> observable, bool state, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value != state)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits until the observable value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable boolean to watch.</param>
        /// <param name="predicate">The condition to meet.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntil(this Observable<bool> observable, Func<bool, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits while the observable value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable boolean to watch.</param>
        /// <param name="predicate">The condition to maintain while waiting.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitWhile(this Observable<bool> observable, Func<bool, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Waits until the observable float value is higher than the specified value.
        /// </summary>
        /// <param name="observable">The observable float to watch.</param>
        /// <param name="value">The threshold value.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntilHigherThan(this Observable<float> observable, float value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value <= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits until the observable float value is lower than the specified value.
        /// </summary>
        /// <param name="observable">The observable float to watch.</param>
        /// <param name="value">The threshold value.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntilLowerThan(this Observable<float> observable, float value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value >= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Waits until the observable float value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable float to watch.</param>
        /// <param name="predicate">The condition to meet.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntil(this Observable<float> observable, Func<float, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits while the observable float value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable float to watch.</param>
        /// <param name="predicate">The condition to maintain while waiting.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitWhile(this Observable<float> observable, Func<float, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits until the observable integer value is higher than the specified value.
        /// </summary>
        /// <param name="observable">The observable integer to watch.</param>
        /// <param name="value">The threshold value.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntilHigherThan(this Observable<int> observable, int value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value <= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Waits until the observable integer value is lower than the specified value.
        /// </summary>
        /// <param name="observable">The observable integer to watch.</param>
        /// <param name="value">The threshold value.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntilLowerThan(this Observable<int> observable, int value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (observable.Value >= value)
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Waits until the observable integer value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable integer to watch.</param>
        /// <param name="predicate">The condition to meet.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
        public static async Awaitable WaitUntil(this Observable<int> observable, Func<int, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!predicate(observable.Value))
            {
                await Awaitable.EndOfFrameAsync(cancellationToken);
            }
        }
        
        /// <summary>
        /// Waits while the observable integer value satisfies the specified predicate.
        /// </summary>
        /// <param name="observable">The observable integer to watch.</param>
        /// <param name="predicate">The condition to maintain while waiting.</param>
        /// <param name="cancellationToken">Token to cancel the waiting operation.</param>
        /// <returns>An awaitable task.</returns>
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