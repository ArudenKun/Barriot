using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barriot.Data.Models
{
    public readonly struct Snowflake
    {
        private readonly long _value;

        /// <summary>
        ///     Creates a new snowflake from the current time.
        /// </summary>
        public Snowflake()
            => _value = DateTime.UtcNow.Ticks;

        private Snowflake(long value)
            => _value = value;

        /// <summary>
        ///     Mutates the current ulong id to a snowflake struct.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator Snowflake(long id)
            => new(id);

        /// <summary>
        ///     Mutates the current snowflake to a long.
        /// </summary>
        /// <param name="flake"></param>
        public static implicit operator long(Snowflake flake)
            => flake._value;

        /// <summary>
        ///     Checks the equality of the underlying value to another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Snowflake flake && flake._value == _value)
                return true;
            return false;
        }

        /// <summary>
        ///     Gets the hashcode of the underlying value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => _value.GetHashCode();

        public static bool operator ==(Snowflake left, Snowflake right)
            => left.Equals(right);

        public static bool operator !=(Snowflake left, Snowflake right)
            => !left.Equals(right);
    }
}
