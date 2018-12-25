/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
using UnityEngine;

namespace Tds.PathFinder
{
    public class SearchParameters<T> where T : class
    {
        public T FromNode { get; set; }
        public T ToNode { get; set; }
        public T[] Nodes { get; set; }
        /// <summary>
        /// Length of the Nodes used, may be shorter than Nodes.Length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Flag indicating if this search has completed
        /// </summary>
        public bool IsComplete { get; set; }
        public int Id { get; set; }
        public int TimeStamp { get; set; }

        public bool IsMatch(T from, T to)
        {
            return (FromNode == from && ToNode == to) || (FromNode == to && ToNode == from);
        }

        /// <summary>
        /// Copies the nodes into the given store in reverse order. If the store is greater than
        /// Length, the remaining values in store will be set to null.
        /// </summary>
        /// <param name="store"></param>
        public void ReverseCopyNodes(T[] store)
        {
            var length = Mathf.Min(store.Length, Length);
            var writeIndex = 0;
            var readIndex = 0;
            
            while (writeIndex < store.Length)
            {
                if (readIndex < Length)
                {
                    var index = Length - (readIndex + 1);
                    if (Nodes[index] != null)
                    {
                        store[writeIndex] = Nodes[index];
                        writeIndex++;
                    }
                }
                else
                {
                    store[writeIndex] = null;
                    writeIndex++;
                }

                readIndex++;
            }
        }
    }
}
