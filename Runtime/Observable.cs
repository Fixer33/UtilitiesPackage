namespace Utilities
{
    // [Serializable]
    public class Observable<T> : Observable
    {
        public delegate void ObservableValueChangedDelegate(T oldValue, T newValue);

        public event ObservableValueChangedDelegate ValueChanged = delegate { };

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                    return;
                
                var oldVal = _value;
                _value = value;
                ValueChanged?.Invoke(oldVal, _value);
            }
        }

        // [SerializeField]
        private T _value;

        public Observable()
        {
            _value = default;
        }
        
        public Observable(T value)
        {
            _value = value;
        }


        public override object GetValue() => Value;
    }

    public abstract class Observable
    {
        public abstract object GetValue();
    }
}