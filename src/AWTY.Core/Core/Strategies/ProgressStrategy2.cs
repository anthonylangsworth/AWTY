using System;
using System.Reactive.Subjects;

namespace AWTY.Core.Strategies
{
    /// <summary>
    ///     The base class for progress-notification strategies.
    /// </summary>
    public abstract class ProgressStrategy2<TValue>
        : IObserver<RawProgressData<TValue>>, IObservable<ProgressData<TValue>>, IDisposable
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        /// <summary>
        ///     The subject for outgoing (processed) progress data.
        /// </summary>
        readonly Subject<ProgressData<TValue>> _progressDataSubject = new Subject<ProgressData<TValue>>();

        /// <summary>
        ///     Create a new progress strategy.
        /// </summary>
        protected ProgressStrategy2()
        {
        }

        /// <summary>
        ///     Finaliser for <see cref="ProgressStrategy2{TValue}"/>.
        /// </summary>
        ~ProgressStrategy2()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Dispose of resources being used by the progress strategy.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Dispose of resources being used by the progress strategy.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _progressDataSubject.Dispose();
        }

        /// <summary>
        ///     Report the current progress.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        public abstract void ReportProgress(TValue current, TValue total);

        /// <summary>
        ///     Called when progress has changed; raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="current">
        ///     The current progress value.
        /// </param>
        /// <param name="total">
        ///     The total value against which progress is measured.
        /// </param>
        /// <param name="percentComplete">
        ///     The percentage of completion.
        /// </param>
        protected void NotifyProgressChanged(TValue current, TValue total, int percentComplete)
        {
            _progressDataSubject.OnNext(ProgressData.Create(
                percentComplete, current, total
            ));
        }

        /// <summary>
        ///     Subscribe an observer to progress data notifications.
        /// </summary>
        /// <param name="observer">
        ///     The observer to subscribe.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the subscription.
        /// </returns>
        public IDisposable Subscribe(IObserver<ProgressData<TValue>> observer)
        {
            return _progressDataSubject.Subscribe(observer);
        }

        /// <summary>
        ///     Notify the observer of the next value in the sequence.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        void IObserver<RawProgressData<TValue>>.OnNext(RawProgressData<TValue> value)
        {
            ReportProgress(value.Current, value.Total);
        }

        /// <summary>
        ///     Notify the observer of sequence completion.
        /// </summary>
        void IObserver<RawProgressData<TValue>>.OnCompleted()
        {
            // AF: Do nothing?
        }

        /// <summary>
        ///     Notify the observer of an error in the sequence.
        /// </summary>
        /// <param name="error">
        ///     An <see cref="Exception"/> representing the error.
        /// </param>
        void IObserver<RawProgressData<TValue>>.OnError(Exception error)
        {
            _progressDataSubject.OnError(error); // AF: Is it correct to propagate here?
        }
    }
}