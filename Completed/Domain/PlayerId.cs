using System;

namespace AkkaMjrTwo.Domain
{
    public class PlayerId : IEquatable<PlayerId>
    {
        public string Value { get; private set; }

        public PlayerId(string value)
        {
            Value = value;
        }

        public bool Equals(PlayerId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is PlayerId playerId))
                return false;

            return Equals(playerId);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
