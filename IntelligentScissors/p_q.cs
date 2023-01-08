using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace IntelligentScissors
{
    class p_q<T>
    {

        class Node
        {
            public double Priority { get; set; }
            public T Object { get; set; }
        }

        //List queue = new List();

        List<Node> queue = new List<Node>();

        int heapSize = -1;
        bool _isMinPriorityQueue;
        public int Count { get { return queue.Count; } }

        public p_q(bool isMinPriorityQueue = false)
        {
            _isMinPriorityQueue = isMinPriorityQueue;
        }

        private void Swap(int i, int j)
        {
            var temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;
        } //O(1)


        private int ChildL(int i)
        {
            return i * 2 + 1;
        }//O(1)
        private int ChildR(int i)
        {
            return i * 2 + 2;
        }//O(1)



        private void MinHeapify(int i)
        {
            int left = ChildL(i);
            int right = ChildR(i);
            int lowest = i;
            if (left <= heapSize && queue[lowest].Priority > queue[left].Priority)
                lowest = left;
            if (right <= heapSize && queue[lowest].Priority > queue[right].Priority)
                lowest = right;
            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        } //O(logN)




        private void BuildHeapMin(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        } // O(logN)


        public void Enqueue(double priority, T obj)
        {
            Node node = new Node() { Priority = priority, Object = obj };
            queue.Add(node);
            heapSize++;
            //Maintaining heap
                BuildHeapMin(heapSize);
                
        } // O(logN)



        public double get_w()
        {
            return queue[0].Priority;
        }


        public T Dequeue()
        {
            if (heapSize > -1)
            {
                var returnVal = queue[0].Object;
                queue[0] = queue[heapSize];
                queue.RemoveAt(heapSize);
                heapSize--;
                   MinHeapify(0);

                return returnVal;
            }
            else
                throw new Exception("Queue is empty");
        }

        //O(logN)




















    }
}
