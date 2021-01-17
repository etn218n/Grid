using System;

namespace MayBe.Unsafe
{
    public static class MaybeUnsafeExtensions
    {
        /// <summary>
        /// Converts an optional to a Nullable&lt;T&gt;.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The Nullable&lt;T&gt; instance.</returns>
        public static T? ToNullable<T>(this Maybe<T> maybe) where T : struct
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            return default(T?);
        }

        /// <summary>
        /// Returns the existing value if present, otherwise default(T).
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The existing value or a default value.</returns>
        public static T ValueOrDefault<T>(this Maybe<T> maybe)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            return default(T);
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T>(this Maybe<T> maybe)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException();
        }

        /// <summary>
        /// Converts an optional to a Nullable&lt;T&gt;.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The Nullable&lt;T&gt; instance.</returns>
        public static T? ToNullable<T, TException>(this Maybe<T, TException> maybe) where T : struct
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            return default(T?);
        }

        /// <summary>
        /// Returns the existing value if present, otherwise default(T).
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The existing value or a default value.</returns>
        public static T ValueOrDefault<T, TException>(this Maybe<T, TException> maybe)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            return default(T);
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T, TException>(this Maybe<T, TException> maybe)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException();
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <param name="errorMessage">An error message to use in case of failure.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T>(this Maybe<T> maybe, string errorMessage)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException(errorMessage);
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <param name="errorMessageFactory">A factory function generating an error message to use in case of failure.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T>(this Maybe<T> maybe, Func<string> errorMessageFactory)
        {
            if (errorMessageFactory == null) throw new ArgumentNullException(nameof(errorMessageFactory));

            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException(errorMessageFactory());
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <param name="errorMessage">An error message to use in case of failure.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T, TException>(this Maybe<T, TException> maybe, string errorMessage)
        {
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException(errorMessage);
        }

        /// <summary>
        /// Returns the existing value if present, or throws an OptionValueMissingException.
        /// </summary>
        /// <param name="maybe">The specified optional.</param>
        /// <param name="errorMessageFactory">A factory function generating an error message to use in case of failure.</param>
        /// <returns>The existing value.</returns>
        /// <exception cref="MaybeValueMissingException">Thrown when a value is not present.</exception>
        public static T ValueOrFailure<T, TException>(this Maybe<T, TException> maybe, Func<TException, string> errorMessageFactory)
        {
            if (errorMessageFactory == null) throw new ArgumentNullException(nameof(errorMessageFactory));

            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            throw new MaybeValueMissingException(errorMessageFactory(maybe.Exception));
        }
    }
}
