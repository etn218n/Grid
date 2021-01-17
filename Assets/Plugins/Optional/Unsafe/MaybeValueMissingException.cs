using System;

namespace MayBe.Unsafe
{
    /// <summary>
    /// Indicates a failed retrieval of a value from an empty optional.
    /// </summary>
    public class MaybeValueMissingException : Exception
    {
        internal MaybeValueMissingException()
            : base()
        {
        }

        internal MaybeValueMissingException(string message)
            : base(message)
        {
        }
    }
}
