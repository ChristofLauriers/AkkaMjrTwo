using System;
namespace AkkaMjrTwo.GameEngine.Domain
{
    public class Id<T>
    {
        public string Value { get; private set; }

        public Id(string value)
        {
            Value = value;
        }
    }
}
