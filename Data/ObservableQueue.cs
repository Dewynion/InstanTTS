using System.Collections.ObjectModel;

namespace InstanTTS.Data
{
    public class ObservableQueue<T> : ObservableCollection<T>
    {
        public void Enqueue(T data)
        {
            Add(data);
        }

        public T Dequeue()
        {
            if (Count == 0)
                return default;
            T front = this[0];
            RemoveAt(0);
            return front;
        }

        public T Peek()
        {
            return Count == 0 ? default : this[0];
        }
    }
}
