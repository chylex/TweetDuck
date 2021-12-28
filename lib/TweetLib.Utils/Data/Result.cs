using System;

namespace TweetLib.Utils.Data {
	/// <summary>
	/// Represents a result which either has a value of type <typeparamref name="T"/>, or an <see cref="Exception"/>.
	/// </summary>
	public sealed class Result<T> {
		/// <summary>
		/// Determines whether the <see cref="Result{T}"/> has a value.
		/// </summary>
		public bool HasValue => exception == null;

		/// <summary>
		/// Returns the value, or throws <see cref="InvalidOperationException"/> if no value is present.
		/// </summary>
		public T Value => HasValue ? value : throw new InvalidOperationException("Requested value from a failed result.");

		/// <summary>
		/// Returns the <see cref="Exception"/>, or throws <see cref="InvalidOperationException"/> if this <see cref="Result{T}"/> has a value.
		/// </summary>
		public Exception Exception => exception ?? throw new InvalidOperationException("Requested exception from a successful result.");

		private readonly T value;
		private readonly Exception? exception;

		/// <summary>
		/// Initializes a <see cref="Result{T}"/> with a value.
		/// </summary>
		public Result(T value) {
			this.value = value;
			this.exception = null;
		}

		/// <summary>
		/// Initializes a <see cref="Result{T}"/> with an exception.
		/// </summary>
		public Result(Exception exception) {
			this.value = default!;
			this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
		}

		/// <summary>
		/// Executes one of the two provided <see cref="Action{T}"/> parameters depending on the state of the <see cref="Result{T}"/>.
		/// </summary>
		/// <param name="onSuccess">Executed if this <see cref="Result{T}"/> has a value.</param>
		/// <param name="onException">Executed if this <see cref="Result{T}"/> has an exception.</param>
		public void Handle(Action<T> onSuccess, Action<Exception> onException) {
			if (HasValue) {
				onSuccess(value);
			}
			else {
				onException(exception!);
			}
		}

		/// <summary>
		/// If this <see cref="Result{T}"/> has a value, applies the <paramref name="mapper"/> function to it and returns a new <see cref="Result{T}"/> with the resulting value.
		/// If this <see cref="Result{T}"/> has an exception, returns a <see cref="Result{T}"/> with the same exception but with type <typeparamref name="R"/>.
		/// </summary>
		public Result<R> Select<R>(Func<T, R> mapper) {
			return HasValue ? new Result<R>(mapper(value)) : new Result<R>(exception!);
		}
	}
}
