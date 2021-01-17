using System;

namespace MayBe.Linq
{
    public static class MaybeLinqExtensions
    {
        public static Maybe<TResult> Select<TSource, TResult>(this Maybe<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return source.Map(selector);
        }

        public static Maybe<TResult> SelectMany<TSource, TResult>(this Maybe<TSource> source, Func<TSource, Maybe<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return source.FlatMap(selector);
        }

        public static Maybe<TResult> SelectMany<TSource, TCollection, TResult>(
                this Maybe<TSource> source,
                Func<TSource, Maybe<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
        }

        public static Maybe<TSource> Where<TSource>(this Maybe<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return source.Filter(predicate);
        }

        public static Maybe<TResult, TException> Select<TSource, TException, TResult>(this Maybe<TSource, TException> source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return source.Map(selector);
        }

        public static Maybe<TResult, TException> SelectMany<TSource, TException, TResult>(
                this Maybe<TSource, TException> source,
                Func<TSource,
                Maybe<TResult, TException>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return source.FlatMap(selector);
        }

        public static Maybe<TResult, TException> SelectMany<TSource, TException, TCollection, TResult>(
                this Maybe<TSource, TException> source,
                Func<TSource, Maybe<TCollection, TException>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
        }
    }
}
