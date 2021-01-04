using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenogears.Utilities
{
    public static class CollectionExtensions
    {
        private static Random rng = new Random();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="destination">The <see cref="ICollection{T}"/> to add items to.</param>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <paramref name="destination"/>. It can contain elements that are <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> collection)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                destination.Add(item);
            }
        }

        /// <summary>
        /// Enqueues the elements of the specified collection to the <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="queue">The <see cref="Queue{T}"/> to add items to.</param>
        /// <param name="collection">
        /// The collection whose elements should be added to the <paramref name="queue"/>. It can contain elements that are <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="queue"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Pushes the elements of the specified collection to the <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stack">The <see cref="Stack{T}"/> to add items to.</param>
        /// <param name="collection">
        /// The collection whose elements should be pushed on to the <paramref name="stack"/>. It can contain elements that are <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="stack"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> collection)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                stack.Push(item);
            }
        }
    }

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds items from one dictionary to the other.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="source">The dictionary items are copied from.</param>
        /// <param name="target">The dictionary items are added to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> or <paramref name="target"/> are <see langword="null"/>.</exception>
        public static void MergeInto<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            foreach (var item in source)
                target[item.Key] = item.Value;
        }

        /// <summary>
        /// Gets the element with the specified key or a default value if it is not in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dicionary">The dictionary to get element from.</param>
        /// <param name="key">The key of the element to get.</param>
        /// <param name="defaultValue">The value to return if element with specified key does not exist in the <paramref name="dicionary"/>.</param>
        /// <returns>The element with the specified key, or the default value.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="dicionary"/> is <see langword="null"/>.</exception>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dicionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            TValue result = default(TValue);

            if (dicionary.TryGetValue(key, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
        {
            TValue val;

            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }

        /// <summary>
        /// Gets the element with the specified key in the dictionary or the new value returned from the <paramref name="getValue"/> callback.
        /// If the <paramref name="shouldAdd"/> callback returns <see langword="true"/> then the new value is added to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dicionary">The dictionary to get element from.</param>
        /// <param name="key">The key of the element to get.</param>
        /// <param name="getValue">The callback delegate to return value if element with specified key does not exist in the <paramref name="dicionary"/>.</param>
        /// <param name="shouldAdd">The callback delegate to determine if the new value should be added to the <paramref name="dicionary"/>.</param>
        /// <returns>The element with the specified key, or the new value.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="dicionary"/>, <paramref name="getValue"/> or <paramref name="shouldAdd"/> are <see langword="null"/>.</exception>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dicionary, TKey key, Func<TKey, TValue> getValue, Func<TValue, bool> shouldAdd)
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            if (getValue == null)
            {
                throw new ArgumentNullException(nameof(getValue));
            }

            if (shouldAdd == null)
            {
                throw new ArgumentNullException(nameof(shouldAdd));
            }

            TValue result = default(TValue);

            if (!dicionary.TryGetValue(key, out result))
            {
                result = getValue(key);

                if (shouldAdd(result))
                    dicionary[key] = result;
            }

            return result;
        }

        /// <summary>
        /// Increments integer value in a dictionary by 1.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <param name="dicionary">The dictionary to get element from.</param>
        /// <param name="key">The key of the element to increment and get.</param>
        /// <returns>The element incremented by 1 with the specified key.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="dicionary"/> is <see langword="null"/>.</exception>
        public static int Increment<TKey>(this IDictionary<TKey, int> dicionary, TKey key)
        {
            if (dicionary == null)
            {
                throw new ArgumentNullException(nameof(dicionary));
            }

            int result = default(int);
            dicionary[key] = dicionary.TryGetValue(key, out result) ? result += 1 : result = 1;
            return result;
        }
    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if <paramref name="enumerable"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to check.</param>
        /// <returns>Returns <see langword="true"/> if <paramref name="enumerable"/> is <see langword="null"/> or empty, otherwise <see langword="false"/>.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The sequence to concatenate to the first sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the concatenated elements of the two input sequences.</returns>
        /// <exception cref="ArgumentException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, params T[] second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            return Enumerable.Concat(first, second);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
        /// <param name="source">The sequence of elements to execute the <see cref="IEnumerable{T}"/>.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}"/>1.</param>
        /// <exception cref="ArgumentException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }
    }

    public static class ListStackExtensions
    {
        /// <summary>
        /// Gets the last item in the <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stack">The <see cref="IList{T}"/> to use as a stack.</param>
        /// <returns>The last item in the collection.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="stack"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="stack"/> is empty.</exception>
        public static T Peek<T>(this IList<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            if (stack.Count == 0)
            {
                throw new ArgumentException("The stack is empty.", nameof(stack));
            }

            return stack[stack.Count - 1];
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stack">The <see cref="IList{T}"/> to use as a stack.</param>
        /// <param name="item">The item to push on to the stack. The value can be <see langword="null"/> for reference types.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="stack"/> is <see langword="null"/>.</exception>
        public static void Push<T>(this IList<T> stack, T item)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            stack.Add(item);
        }

        /// <summary>
        /// Removes and returns the object at the end of the <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stack">The <see cref="IList{T}"/> to use as a stack.</param>
        /// <returns>The object removed from the end of the <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="stack"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="stack"/> is empty.</exception>
        public static T Pop<T>(this IList<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            if (stack.Count == 0)
            {
                throw new ArgumentException("The stack is empty.", nameof(stack));
            }

            var item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return item;
        }

        /// <summary>
        /// Removes and returns the object at the start of the <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="stack">The <see cref="IList{T}"/> to use as a stack.</param>
        /// <returns>The object removed from the start of the <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="stack"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="stack"/> is empty.</exception>
        public static T PopFront<T>(this IList<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            if (stack.Count == 0)
            {
                throw new ArgumentException("The stack is empty.", nameof(stack));
            }

            var item = stack[0];
            stack.RemoveAt(0);
            return item;
        }
    }

    public static class RandomListExtensions
    {
        /// <summary>
        /// Chooses a random item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">An instance of <see cref="Random"/>.</param>
        /// <param name="collection">Collection to choose item from.</param>
        /// <returns>A random item from collection</returns>
        /// <exception cref="ArgumentNullException">If the random argument is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        public static T Choose<T>(this Random random, IList<T> collection)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection[random.Next(collection.Count)];
        }

        /// <summary>
        /// Chooses a random item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">An instance of <see cref="Random"/>.</param>
        /// <param name="collection">Collection to choose item from.</param>
        /// <returns>A random item from collection</returns>
        /// <exception cref="ArgumentNullException">If the random  is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        public static T Choose<T>(this Random random, params T[] collection)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection[random.Next(collection.Length)];
        }

        /// <summary>
        /// Shuffles the collection in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">An instance of <see cref="Random"/>.</param>
        /// <param name="collection">Collection to shuffle.</param>
        /// <exception cref="ArgumentNullException">If the random argument is null.</exception>
        /// <exception cref="ArgumentNullException">If the random collection is null.</exception>
        public static void Shuffle<T>(this Random random, IList<T> collection)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            int n = collection.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = collection[k];
                collection[k] = collection[n];
                collection[n] = value;
            }
        }
    }
}