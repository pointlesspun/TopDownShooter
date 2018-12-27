/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    public class SearchParameters<T> where T : class
    {
        /// <summary>
        /// Node from which the search starts
        /// </summary>
        public T FromNode { get; set; }

        /// <summary>
        /// Node where the search should end
        /// </summary>
        public T ToNode { get; set; }

        /// <summary>
        /// Nodes making up the search result
        /// </summary>
        public T[] Nodes { get; set; }

        /// <summary>
        /// Length of the Nodes used, may be shorter than Nodes.Length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Flag indicating if this search has completed
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Unique id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Timestamp of when this search changed state, used to determine if the search
        /// parameters can be reused
        /// </summary>
        public int TimeStamp { get; set; }

        /// <summary>
        /// Checks if the from & to match the from and to nodes or vice versa
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
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
