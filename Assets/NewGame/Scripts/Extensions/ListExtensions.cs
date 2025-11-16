// source: git-amend: https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/ListExtensions.cs

using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    static Random rng;

    /// <summary>
    /// Determines whether a collection is null or has no elements
    /// without having to enumerate the entire collection to get a count.
    ///
    /// Uses LINQ's Any() method to determine if the collection is empty,
    /// so there is some GC overhead.
    /// </summary>
    /// <param name="list">List to evaluate</param>
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || !list.Any();
    }

    /// <summary>
    /// Creates a new list that is a copy of the original list.
    /// </summary>
    /// <param name="list">The original list to be copied.</param>
    /// <returns>A new list that is a copy of the original list.</returns>
    public static List<T> Clone<T>(this IList<T> list)
    {
        List<T> newList = new List<T>();
        foreach (T item in list)
        {
            newList.Add(item);
        }

        return newList;
    }

    /// <summary>
    /// Swaps two elements in the list at the specified indices.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="indexA">The index of the first element.</param>
    /// <param name="indexB">The index of the second element.</param>
    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    /// <summary>
    /// Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm.
    /// This method modifies the input list in-place, ensuring each permutation is equally likely, and returns the list for method chaining.
    /// Reference: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    /// </summary>
    /// <param name="list">The list to be shuffled.</param>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <returns>The shuffled list.</returns>
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (rng == null) rng = new Random();
        int count = list.Count;
        while (count > 1)
        {
            --count;
            int index = rng.Next(count + 1);
            (list[index], list[count]) = (list[count], list[index]);
        }
        return list;
    }

    public static T GetRandom<T>(this IList<T> list) => list[rng.Next(list.Count)];
    public static IList<T> GetRandoms<T>(this IList<T> list, int count) => list.Take(count).ToList();
    public static T GetRandomExcept<T>(this IList<T> list, IList<T> excludeList)
    {
        List<T> filteredList = (List<T>)list.Filter(x => !excludeList.Contains(x));
        return filteredList.GetRandom();
    }

    /// <summary>
    /// Filters a collection based on a predicate and returns a new list
    /// containing the elements that match the specified condition.
    /// </summary>
    /// <param name="source">The collection to filter.</param>
    /// <param name="predicate">The condition that each element is tested against.</param>
    /// <returns>A new list containing elements that satisfy the predicate.</returns>
    public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)
    {
        List<T> list = new List<T>();
        foreach (T item in source)
        {
            if (predicate(item))
            {
                list.Add(item);
            }
        }
        return list;
    }
}
