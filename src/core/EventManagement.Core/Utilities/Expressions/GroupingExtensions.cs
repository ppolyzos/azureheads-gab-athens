using System;
using System.Collections.Generic;
using System.Linq;

namespace EventManagement.Core.Utilities.Expressions
{
    public static class GroupingExtensions
    {
        public static Dictionary<TKey, int> CountBy<T, TKey>(this IEnumerable<T> set, Func<T, TKey> predicate)
        {
            return set.GroupBy(predicate).ToDictionary(g => g.Key, g => g.Count());
        }


        public static Dictionary<int, int> CountConsecutiveBy<T>(this IEnumerable<T> set,
            Func<T, int> keySelector)
        {

            // TODO: MAKE LESS ITERATION / REVISIT ALGORITHM
            var consecutive = set.ChunkBy(keySelector).ToList();
            var dictionary = consecutive
                .Select(c => c.Key)
                .Distinct()
                .Select(key => consecutive.Where(c => c.Key == key).OrderByDescending(c => c.Count()).FirstOrDefault())
                .Where(r => r != null)
                .ToDictionary(r => r.Key, r => r.Count());

            return dictionary;
        }

        private static IEnumerable<IGrouping<TKey, TSource>> ChunkBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ChunkBy(keySelector, EqualityComparer<TKey>.Default);
        }

        private static IEnumerable<IGrouping<TKey, TSource>> ChunkBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            // Flag to signal end of source sequence. 
            const bool noMoreSourceElements = true;

            // Auto-generated iterator for the source array.        
            var enumerator = source.GetEnumerator();

            // Move to the first element in the source sequence. 
            if (!enumerator.MoveNext()) yield break;

            // Iterate through source sequence and create a copy of each Chunk. 
            // On each pass, the iterator advances to the first element of the next "Chunk" 
            // in the source sequence. This loop corresponds to the outer foreach loop that 
            // executes the query.
            while (true)
            {
                // Get the key for the current Chunk. The source iterator will churn through 
                // the source sequence until it finds an element with a key that doesn't match. 
                var key = keySelector(enumerator.Current);

                // Make a new Chunk (group) object that initially has one GroupItem, which is a copy of the current source element.
                var current = new Chunk<TKey, TSource>(key, enumerator, value => comparer.Equals(key, keySelector(value)));

                // Return the Chunk. A Chunk is an IGrouping<TKey,TSource>, which is the return value of the ChunkBy method. 
                // At this point the Chunk only has the first element in its source sequence. The remaining elements will be 
                // returned only when the client code foreach's over this chunk. See Chunk.GetEnumerator for more info. 
                yield return current;

                // Check to see whether (a) the chunk has made a copy of all its source elements or  
                // (b) the iterator has reached the end of the source sequence. If the caller uses an inner 
                // foreach loop to iterate the chunk items, and that loop ran to completion, 
                // then the Chunk.GetEnumerator method will already have made 
                // copies of all chunk items before we get here. If the Chunk.GetEnumerator loop did not 
                // enumerate all elements in the chunk, we need to do it here to avoid corrupting the iterator 
                // for clients that may be calling us on a separate thread. 
                if (current.CopyAllChunkElements() == noMoreSourceElements)
                {
                    yield break;
                }
            }
        }

        // A Chunk is a contiguous group of one or more source elements that have the same key. A Chunk  
        // has a key and a list of ChunkItem objects, which are copies of the elements in the source sequence. 
        class Chunk<TKey, TSource> : IGrouping<TKey, TSource>
        {
            // INVARIANT: DoneCopyingChunk == true ||  
            //   (predicate != null && predicate(enumerator.Current) && current.Value == enumerator.Current) 

            // A Chunk has a linked list of ChunkItems, which represent the elements in the current chunk. Each ChunkItem 
            // has a reference to the next ChunkItem in the list. 
            class ChunkItem
            {
                public ChunkItem(TSource value)
                {
                    Value = value;
                }
                public readonly TSource Value;
                public ChunkItem Next;
            }
            // The value that is used to determine matching elements 

            // Stores a reference to the enumerator for the source sequence 
            private IEnumerator<TSource> _enumerator;

            // A reference to the predicate that is used to compare keys. 
            private Func<TSource, bool> _predicate;

            // Stores the contents of the first source element that 
            // belongs with this chunk. 
            private readonly ChunkItem _head;

            // End of the list. It is repositioned each time a new 
            // ChunkItem is added. 
            private ChunkItem _tail;

            // Flag to indicate the source iterator has reached the end of the source sequence. 
            private bool _isLastSourceElement;

            // Private object for thread syncronization 
            private readonly object _mLock;

            // REQUIRES: enumerator != null && predicate != null 
            public Chunk(TKey key, IEnumerator<TSource> enumerator, Func<TSource, bool> predicate)
            {
                this.Key = key;
                this._enumerator = enumerator;
                this._predicate = predicate;

                // A Chunk always contains at least one element.
                _head = new ChunkItem(enumerator.Current);

                // The end and beginning are the same until the list contains > 1 elements.
                _tail = _head;

                _mLock = new object();
            }

            // Indicates that all chunk elements have been copied to the list of ChunkItems,  
            // and the source enumerator is either at the end, or else on an element with a new key. 
            // the tail of the linked list is set to null in the CopyNextChunkElement method if the 
            // key of the next element does not match the current chunk's key, or there are no more elements in the source. 
            private bool DoneCopyingChunk => _tail == null;

            // Adds one ChunkItem to the current group 
            // REQUIRES: !DoneCopyingChunk && lock(this) 
            private void CopyNextChunkElement()
            {
                // Try to advance the iterator on the source sequence. 
                // If MoveNext returns false we are at the end, and isLastSourceElement is set to true
                _isLastSourceElement = !_enumerator.MoveNext();

                // If we are (a) at the end of the source, or (b) at the end of the current chunk 
                // then null out the enumerator and predicate for reuse with the next chunk. 
                if (_isLastSourceElement || !_predicate(_enumerator.Current))
                {
                    _enumerator = null;
                    _predicate = null;
                }
                else
                {
                    _tail.Next = new ChunkItem(_enumerator.Current);
                }

                // tail will be null if we are at the end of the chunk elements 
                // This check is made in DoneCopyingChunk.
                _tail = _tail.Next;
            }

            // Called after the end of the last chunk was reached. It first checks whether 
            // there are more elements in the source sequence. If there are, it  
            // Returns true if enumerator for this chunk was exhausted. 
            internal bool CopyAllChunkElements()
            {
                while (true)
                {
                    lock (_mLock)
                    {
                        if (DoneCopyingChunk)
                        {
                            // If isLastSourceElement is false, 
                            // it signals to the outer iterator 
                            // to continue iterating. 
                            return _isLastSourceElement;
                        }
                        else
                        {
                            CopyNextChunkElement();
                        }
                    }
                }
            }

            public TKey Key { get; }

            // Invoked by the inner foreach loop. This method stays just one step ahead 
            // of the client requests. It adds the next element of the chunk only after 
            // the clients requests the last element in the list so far. 
            public IEnumerator<TSource> GetEnumerator()
            {
                //Specify the initial element to enumerate.
                ChunkItem current = _head;

                // There should always be at least one ChunkItem in a Chunk. 
                while (current != null)
                {
                    // Yield the current item in the list. 
                    yield return current.Value;

                    // Copy the next item from the source sequence,  
                    // if we are at the end of our local list. 
                    lock (_mLock)
                    {
                        if (current == _tail)
                        {
                            CopyNextChunkElement();
                        }
                    }

                    // Move to the next ChunkItem in the list.
                    current = current.Next;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}