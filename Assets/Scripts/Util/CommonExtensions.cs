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
        /// <summary>
        /// Finds the first element in the linked list matching the given predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Shift all elements in the array one position backwards
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void ShiftBack<T>(this T[] array)
        {
            for ( int i = 0; i < array.Length - 1; ++i)
            {
                array[i] = array[i + 1];
            }
        }

        /// <summary>
        /// Retrieves an component from the object with the given gametag of type T.
        /// Eg RetrieveComponent < ScoreComponent >("Player")
        /// </summary>
        /// <typeparam name="T">type of the component to retrieve</typeparam>
        /// <param name="gameObjectTag">Tag of the game object</param>
        /// <param name="isOptional">If false a contract failure will be raised when the component is not found</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if the given vectors are within the given distance (non inclusive)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool IsInRange(this Vector2 v1, Vector2 v2, float distance)
        {
            return (v1 - v2).sqrMagnitude < distance * distance;
        }
    }
}
