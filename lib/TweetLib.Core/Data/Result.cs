using System;

namespace TweetLib.Core.Data{
    public sealed class Result<T>{
        public bool HasValue => exception == null;

        public T Value => HasValue ? value : throw new InvalidOperationException("Requested value from a failed result.");
        public Exception Exception => exception ?? throw new InvalidOperationException("Requested exception from a successful result.");

        private readonly T value;
        private readonly Exception? exception;

        public Result(T value){
            this.value = value;
            this.exception = null;
        }

        public Result(Exception exception){
            this.value = default!;
            this.exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public void Handle(Action<T> onSuccess, Action<Exception> onException){
            if (HasValue){
                onSuccess(value);
            }
            else{
                onException(exception!);
            }
        }

        public Result<R> Select<R>(Func<T, R> map){
            return HasValue ? new Result<R>(map(value)) : new Result<R>(exception!);
        }
    }
}
