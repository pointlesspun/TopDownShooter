/*
 * TDS (c) 2018 by AnrectB Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class CommonExtensions
    {
        public static LinkedListNode<T> FirstOrDefault<T>( this LinkedList<T> list, Func<T, bool> predicate)  {

            var current = list.First;

            while ( current  != null )
            {
                if ( predicate(current.Value))
                {
                    return current;
                }

                current = current.Next;
            }

            return default(LinkedListNode<T>);
        }

        public static void ShiftLeft<T>(this T[] array)
        {
            for ( int i = 0; i < array.Length - 1; ++i)
            {
                array[i] = array[i + 1];
            }
        }

        public static T RetrieveComponent<T>(string gameObjectTag, bool isOptional = false) where T  : Component
        {
            var obj = GameObject.FindGameObjectWithTag(gameObjectTag);

            if (!isOptional)
            {
                Contract.Requires(obj != null, "cannot retrieve component from object with tag " + gameObjectTag);
                Contract.RequiresComponent<T>(obj, "no component of type " + typeof(T) + " on object with tag " + gameObjectTag);
            }

            return obj != null ? obj.GetComponent<T>() : null;
        }
    }
}
