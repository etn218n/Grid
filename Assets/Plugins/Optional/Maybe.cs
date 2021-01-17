namespace MayBe
{
    /// <summary>
    /// Provides a set of functions for creating optional values.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Wraps an existing value in an Option&lt;T&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Maybe<T> Some<T>(T value) => new Maybe<T>(value, true);

        /// <summary>
        /// Wraps an existing value in an Option&lt;T, TException&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Maybe<T, TException> Some<T, TException>(T value) =>
            new Maybe<T, TException>(value, default(TException), true);

        /// <summary>
        /// Creates an empty Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public static Maybe<T> None<T>() => new Maybe<T>(default(T), false);

        /// <summary>
        /// Creates an empty Option&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An empty optional.</returns>
        public static Maybe<T, TException> None<T, TException>(TException exception) =>
            new Maybe<T, TException>(default(T), exception, false);
    }
}
