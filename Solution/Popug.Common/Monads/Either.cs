namespace Popug.Common.Monads
{
    public class Either<TResult, TError>
    {
        public TResult Result { get; }
        public TError Error { get; }
        public bool HasError { init; get; }


        public static implicit operator Either<TResult, TError>(TResult result) => new Either<TResult, TError>(result);

        public static implicit operator Either<TResult, TError>(TError error) => new Either<TResult, TError>(error);

        public static Either<TResult, TError> Success(TResult result) => new Either<TResult, TError>(result);

        public static Either<TResult, TError> Failure(TError error) => new Either<TResult, TError>(error);

        public Either(TResult result)
        {
            this.Result = result;
            this.HasError = false;
        }

        public Either(TError error)
        {
            this.Error = error;
            this.HasError = true;
        }

        public T Consume<T>(Func<TResult, T> onResult, Func<TError, T> onError)
        {
            return this.HasError ? onError(this.Error) : onResult(this.Result);
        }

        public void Apply(Action<TResult> onResult, Action<TError> onError)
        {
            if (this.HasError)
            {
                onError(this.Error);
            }
            else
            {
                onResult(this.Result);
            }
        }
    }
}
