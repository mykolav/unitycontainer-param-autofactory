using System;
using System.Collections.Generic;

namespace Unity.ParameterizedAutoFactory.Core.Caches
{
    /// <summary>
    /// Keeping static constants in generic classes is discouraged.
    /// So we have a non-generic class with the same name.
    /// </summary>
    internal static class LeastRecentlyUsedCache
    {
        public static class Defaults
        {
            public static readonly uint Capacity = 1000;
        }
    }

    /// <summary>
    /// This is a simple in-memory cache.
    /// It acts according to the Lest Recently Used policy:
    /// when the cache reaches its maximum capacity,
    /// the least recently used item is evicted.
    ///
    /// This is a slighly modified version of
    /// the original code from http://www.sinbadsoft.com/blog/a-lru-cache-implementation/
    /// </summary>
    internal class LeastRecentlyUsedCache<TKey, TValue> : ICache<TKey, TValue>
        where TValue : class
    {
        private class DoubleLinkedList
        {
            /// <summary>
            /// The head is the left-most Node.
            /// The tail is the right-most Node.
            /// </summary>
            public class Node
            {
                public Node(TKey key, TValue value)
                {
                    Key = key;
                    Value = value;
                }

                public Node Right { get; set; }
                public Node Left { get; set; }
                public TKey Key { get; }
                public TValue Value { get; set; }
            }

            /// The head is the left-most Node.
            private Node _head;

            /// The tail is the right-most Node.
            public Node Tail { get; private set; }

            public void AddOrMoveToHead(Node node)
            {
                if (node == null)
                    return;

                if (node == _head)
                    return;
 
                var right = node.Right;
                var left = node.Left;
 
                // right is null for the tail or a new node.
                if (right != null)
                {
                    // -----    -----    -----
                    // | L |    | E |    | R |
                    // -----    -----    -----
                    //   ^                 |  
                    //   |   right.Left    |
                    //   -------------------
                    right.Left = node.Left;
                }

                // left is null for the head or a new node.
                if (left != null)
                {
                    // -----    -----    -----
                    // | L |    | E |    | R |
                    // -----    -----    -----
                    //   |                 ^  
                    //   |   left.Right    |
                    //   -------------------
                    left.Right = node.Right;
                }
 
                // node is going to be the new head.
                // And the head does not have a node to the left
                // because it's the leftmost node by definition.
                node.Left = null;
                // node is going to be the new head.
                // And the current head is goin to be to the right of the new head.
                node.Right = _head;
 
                // _head is null in an empty double linked list.
                if (_head != null)
                {
                    // The new head is about to be placed to the left
                    // of the current head.
                    _head.Left = node;
                }

                // Now node is the new head.
                _head = node;
 
                // If the new head used to be the tail,
                // the node that used to be to the left of the tail
                // is the new tail.
                if (Tail == node) 
                    Tail = left;

                // Maybe we just added the first node to an empty list.
                if (Tail == null)
                {
                    // In a list with a single node,
                    // the head and the tail are the same node.
                    Tail = _head;
                }
            }

            public void RemoveTail()
            {
                // We are "forgetting" the current tail,
                // the node that used to be to the left of it is the new tail.
                Tail = Tail.Left;
                if (Tail != null)
                {
                    // By our definition, tail is the rightmost node.
                    // Hence, it cannot have a node to the right.
                    Tail.Right = null;
                }
            }
        }

        private readonly uint _capacity;
        private readonly Dictionary<TKey, DoubleLinkedList.Node> _entries;
        private readonly DoubleLinkedList _entriesSortedByUsage;
 
        public LeastRecentlyUsedCache(uint? capacity = null)
        {
            if (capacity == null)
                capacity = LeastRecentlyUsedCache.Defaults.Capacity;

            if (capacity == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    "Capacity should be greater than zero");
            }

            _capacity = capacity.Value;
            _entries = new Dictionary<TKey, DoubleLinkedList.Node>();
            _entriesSortedByUsage = new DoubleLinkedList();
        }

        public int Count => _entries.Count;
 
        public void AddOrReplace(TKey key, TValue value)
        {
            if (!_entries.TryGetValue(key, out var entry))
            {
                entry = new DoubleLinkedList.Node(key, value);

                if (_entries.Count == _capacity)
                {
                    _entries.Remove(_entriesSortedByUsage.Tail.Key);
                    _entriesSortedByUsage.RemoveTail();
                }

                _entries.Add(key, entry);
            }
 
            entry.Value = value;
            _entriesSortedByUsage.AddOrMoveToHead(entry);
        }
 
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            if (!_entries.TryGetValue(key, out var entry)) 
                return false;

            _entriesSortedByUsage.AddOrMoveToHead(entry);

            value = entry.Value;

            return true;
        }

    }
}