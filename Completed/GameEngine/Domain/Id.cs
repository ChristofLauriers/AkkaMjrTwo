using System;
namespace AkkaMjrTwo.GameEngine.Domain
{
    public class Id<T> : IEquatable<Id<T>>
    {
        public string Value { get; private set; }

        public Id(string value)
        {
            Value = value;
        }

        public bool Equals(Id<T> other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Id<T> id))
                return false;

            return Equals(id);
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
