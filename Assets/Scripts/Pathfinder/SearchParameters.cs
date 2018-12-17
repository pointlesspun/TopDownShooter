/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.PathFinder
{
    public class SearchParameters<T> where T : class
    {
        public T FromNode { get; set; }
        public T ToNode { get; set; }
        public T[] Nodes { get; set; }
        public int Length { get; set; }
        public bool IsComplete { get; set; }
        public int Id { get; set; }
        public int TimeStamp { get; set; }

        public bool IsMatch(T from, T to)
        {
            return (FromNode == from && ToNode == to) || (FromNode == to && ToNode == from);
        }
    }
}
