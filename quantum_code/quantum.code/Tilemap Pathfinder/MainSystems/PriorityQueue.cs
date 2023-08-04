using System.Collections.Generic;
using System;

namespace Quantum.Tiles {

    struct PriorityData {
        public ushort Priority;
        public ushort Index;
    }

    public unsafe struct PriorityQueueInsertion{
        private List<PriorityData> _queue;
        public int Count => _queue.Count;

        public void Init() {
            if (_queue == null) {
                _queue = new List<PriorityData>();
            } else {
                _queue.Clear();
            }

        }
        public void Enqueue(ushort index, ushort priority) {
            if (_queue.Count == 0) {
                _queue.Add( new PriorityData() {
                    Index = index,
                    Priority = priority
                });
            } else {
                for (int i = 0; i < _queue.Count; i++) {
                    if (priority > _queue[i].Priority) {
                        _queue.Insert(i, new PriorityData() {
                            Index = index,
                            Priority = priority
                        });
                        return;
                    }
                }
                _queue.Add(new PriorityData() {
                    Index = index,
                    Priority = priority
                });
            }
            
        }

        public ushort Dequeue() {
            ushort data = _queue[_queue.Count - 1].Index;
            _queue.RemoveAt(_queue.Count - 1);
            return data;
        }
    }

    public unsafe struct PriorityQueueInsertionArray
    {
        private PriorityData[] _queue;
        private int _capacity;
        private int _count;

        public PriorityQueueInsertionArray() {
            _queue = new PriorityData[8];
            _capacity = 8;
            _count = 0;
        }

        public int Count => _count;

        public void Init() {
            _count = 0;
        }

        public void Enqueue(ushort index, ushort priority) {
            if(_count == _capacity) {
                var _queue2 = new PriorityData[_capacity*2];
                Array.Copy(_queue, _queue2, _capacity);
                _queue = _queue2;
                _capacity = _queue2.Length;
            }
            if (_count == 0) {
                _queue[_count] = new PriorityData() {
                    Index = index,
                    Priority = priority
                };
            } else {
                for (int i = 0; i < _count; i++) {
                    if (priority > _queue[i].Priority) {
                        Array.Copy(_queue, i, _queue, i + 1, _queue.Length - i -1);
                        _queue[i] =  new PriorityData() {
                            Index = index,
                            Priority = priority
                        };
                        _count++;
                        return;
                        
                    }
                }
                Array.Copy(_queue, _count - 1, _queue, _count, _queue.Length - _count);
                _queue[_count - 1] = new PriorityData() {
                    Index = index,
                    Priority = priority
                };
            }
            _count++;
        }

        public ushort Dequeue() {
            ushort data = _queue[_count - 1].Index;
            _count--;
            return data;
        }
    }


    public unsafe struct Queue
    {
        private Queue<PriorityData> _queue;
        public int Count => _queue.Count;

        public void Init() {
            if (_queue == null) {
                _queue = new Queue<PriorityData>();
            } else {
                _queue.Clear();
            }

        }
        public void Enqueue(ushort index, ushort priority) {
            _queue.Enqueue(new PriorityData() {
                Index = index,
                Priority = priority
            });

        }

        public ushort Dequeue() {
            return _queue.Dequeue().Index;
        }
    }

    public unsafe struct ArrayQueue
    {
        private PriorityData[] _queue;
        private int _capacity;
        private int _count;

        public ArrayQueue() {
            _queue = new PriorityData[8];
            _capacity = 8;
            _count = 0;
        }

        public int Count => _count;

        public void Init() {
            _count = 0;
        }

        public void Enqueue(ushort index, ushort priority) {

            if (_count == _capacity) {
                var _queue2 = new PriorityData[_capacity * 2];
                Array.Copy(_queue, _queue2, _capacity);
                _queue = _queue2;
                _capacity = _queue2.Length;
            }

            Array.Copy(_queue, 0, _queue, 1, _queue.Length - 1);
            _queue[0] = new PriorityData() {
                Index = index,
                Priority = priority
            };
            _count++;
        }

        public ushort Dequeue() {
            ushort data = _queue[_count - 1].Index;
            _count--;
            return data;
        }
    }
    public unsafe struct PriorityQueueBinaryHeap
    {
        PriorityData[] _heap;
        int _capacity;
        int _count;
        public int Count => _count;

        public PriorityQueueBinaryHeap() {
            _capacity = 8;
            _heap = new PriorityData[_capacity];
            _count = 0;
        }
        public void Init() {
            _count = 0;
        }

        public int Parent(int index) {
            return (index - 1) / 2;
        }

        public void Enqueue(ushort index, ushort priority) {
            if (_count == _capacity) {
                var _queue2 = new PriorityData[_capacity * 2];
                Array.Copy(_heap, _queue2, _capacity);
                _heap = _queue2;
                _capacity = _queue2.Length;
            }

            int i = _count;
            _heap[i].Index = index;
            _heap[i].Priority = priority;
            _count++;

            while (i != 0 && _heap[i].Priority <= _heap[Parent(i)].Priority) {
                var temp = _heap[i];
                _heap[i] = _heap[Parent(i)];
                _heap[Parent(i)] = temp;
                i = Parent(i);
            }
        }

        public ushort Dequeue() {

            if (_count == 1) {
                var index = _heap[0].Index;
                _count--;
                return index;
            }

            ushort root = _heap[0].Index;

            _heap[0] = _heap[_count - 1];
            _count--;
            SortHeap(0);

            return root;
        }

        public void SortHeap(int index) {
            int left = 2 * index + 1;
            int right = 2 * index + 2;

            int smallest = index;
            if (left < _count && _heap[left].Priority < _heap[smallest].Priority) {
                smallest = left;
            }
            if (right < _count && _heap[right].Priority < _heap[smallest].Priority) {
                smallest = right;
            }

            if (smallest != index) {
                var temp = _heap[index];
                _heap[index] = _heap[smallest];
                _heap[smallest] = temp;
                SortHeap(smallest);
            }
        }
    }
}
