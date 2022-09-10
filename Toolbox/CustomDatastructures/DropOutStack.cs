using System.Linq;

namespace aemarcoCommons.Toolbox.CustomDatastructures
{
    public class DropOutStack<T>
    {
        private readonly T[] _items;
        private int _top;
        public DropOutStack(int capacity)
        {
            _items = new T[capacity];
        }

        public void Push(T item)
        {
            _items[_top] = item;
            _top = (_top + 1) % _items.Length;
        }
        public T Pop()
        {
            _top = (_items.Length + _top - 1) % _items.Length;
            return _items[_top];
        }

        public bool Contains(T element) => _items.Contains(element);
    }
}
