using System;
using System.Collections;
using System.Collections.Generic;

// THIS IMPLEMENTATION IS NOT COMPLETE AND MAY BE SCRAPPED. DO NOT USE THIS
// Custom Linked List implementation to support constant time concatenation and
// other operations in case such performance is needed in order to play MIDI in
// real time.
namespace uMIDI.src.Common
{
    public class UMidiLinkedList<T> : IList<T>
    {
        private UMidiLinkedListNode<T> head;
        private UMidiLinkedListNode<T> tail;
        private int size;

        public UMidiLinkedList()
        {
            head = null;
            size = 0;
        }

        public T this[int index]
        {
            get
            {
                UMidiLinkedListNode<T> ptr = head;
                for (int i = 0; i < index; i++)
                    ptr = ptr.Next;
                return ptr.Value;
            }
            set
            {
                UMidiLinkedListNode<T> ptr = head;
                for (int i = 0; i < index; i++)
                    ptr = ptr.Next;
                ptr.Value = value;
            }
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Count => size;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public int Add(T value)
        {
            UMidiLinkedListNode<T> node = new UMidiLinkedListNode<T>(value);

            if (head == null)
            {
                head = node;
                tail = node;
            }
            else
            {
                tail.Next = node;
                tail = node;
            }
            size++;
            return size - 1;
        }

        public void Clear()
        {
            head = null;
            size = 0;
        }

        public bool Contains(T item)
        {
            foreach (T element in this)
            {
                if (element.Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int start = 0;
            UMidiLinkedListNode<T> ptr = head;
            while (start < arrayIndex)
                ptr = ptr.Next;

            for (int i = 0; i < array.Length; i++)
            {
                if (ptr == tail)
                {
                    this.AddLast(array[i]);
                }
                else
                {
                    ptr.Value = array[i];
                    ptr = ptr.Next;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new UMidiLinkedListEnumerator<T>(this);
        }

        // TODO: This method is required, but not sure if exactly what is desired
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new UMidiLinkedListEnumerator<T>(this);
        }

        public int IndexOf(T item)
        {
            UMidiLinkedListNode<T> node = head;
            for (int i = 0; i < Count; i++)
            {
                node = node.Next;
                if (item.Equals(node))
                    return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            int i = 0;
            UMidiLinkedListNode<T> ptr = head;
            while (i < index)
            {
                ptr = ptr.Next;
                i++;
            }

            UMidiLinkedListNode<T> node = new UMidiLinkedListNode<T>(item);
            if (ptr == tail)
            {
                ptr.Next = node;
                node.Next = null;
                tail = node;
            }
            else
            {
                ptr.Next = node;
                node.Next = ptr.Next.Next;
            }
        }

        public bool Remove(T item)
        {
            UMidiLinkedListNode<T> node = head;
            while (!node.Equals(tail))
            {
                if (node.Value.Equals(item))
                {

                }
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            int i = 0;
            UMidiLinkedListNode<T> ptr = head;
            while (i < index - 1)
            {
                ptr = ptr.Next;
                i++;
            }

            if (ptr.Next == tail)
            {
                
            }
            else
            {

            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void AddLast(T item)
        {
            UMidiLinkedListNode<T> node = new UMidiLinkedListNode<T>(item);
            tail.Next = node;
            tail = node;
            size++;
        }
    }

    public class UMidiLinkedListNode<T>
    {
        public T Value { get; set; }
        public UMidiLinkedListNode<T> Next { get; set; }

        public UMidiLinkedListNode(T value)
        {
            Value = value;
            Next = null;
        }
    }

    public class UMidiLinkedListEnumerator<T> : IEnumerator<T>
    {
        public T Current => list[index];

        object IEnumerator.Current => this.Current;

        private int index;
        private UMidiLinkedList<T> list;

        public UMidiLinkedListEnumerator(UMidiLinkedList<T> list)
        {
            index = 0;
            this.list = list;
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            index++;
            if (index >= list.Count)
                return false;
            return true;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
