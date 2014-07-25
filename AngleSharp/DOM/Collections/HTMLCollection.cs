﻿namespace AngleSharp.DOM.Collections
{
    using AngleSharp.DOM.Html;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A specialized collection containing elements of type T.
    /// </summary>
    /// <typeparam name="T">The type of elements that can be contained.</typeparam>
    class HTMLCollection<T> : IHtmlCollection
        where T : class, IElement
    {
        #region Fields

        IEnumerable<T> _elements;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new list of HTML elements.
        /// </summary>
        /// <param name="elements">The elements to use.</param>
        internal HTMLCollection(IEnumerable<T> elements)
        {
            _elements = elements;
        }

        /// <summary>
        /// Creates a new live collection for the given parent.
        /// </summary>
        /// <param name="parent">The parent of this collection.</param>
        /// <param name="deep">[Optional] Determines if recursive search is activated.</param>
        /// <param name="predicate">[Optional] The predicate function for picking elements.</param>
        internal HTMLCollection(INode parent, Boolean deep = true, Predicate<T> predicate = null)
            : this(GetElements(parent, deep, predicate))
        {
        }

        #endregion

        #region Index

        /// <summary>
        /// Gets the specific node whose id matches the string specified by the attribute.
        /// Matching by name is only done as a last resort, only in HTML, and only if the referenced
        /// element supports the name attribute. Returns null if no node exists by the given name.
        /// </summary>
        /// <param name="index">The 0-based index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        [DomName("item")]
        public T this[Int32 index]
        {
            get { return _elements.Skip(index).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the specific node whose id matches the string specified by the attribute.
        /// Matching by name is only done as a last resort, only in HTML, and only if the referenced
        /// element supports the name attribute. Returns null if no node exists with the given name.
        /// </summary>
        /// <param name="id">The id of the element.</param>
        /// <returns>The element with the specified identifier.</returns>
        [DomName("namedItem")]
        public T this[String id]
        {
            get { return _elements.FirstOrDefault(m => m.Id == id) ?? _elements.FirstOrDefault(m => m.GetAttribute("name") == id); }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of nodes in the list.
        /// </summary>
        [DomName("length")]
        public Int32 Length
        {
            get { return _elements.Count(); }
        }

        #endregion

        #region Methods

        internal Int32 IndexOf(T item)
        {
            var index = 0;

            foreach (var element in _elements)
            {
                if (element == item)
                    return index;

                index++;
            }

            return -1;
        }

        #endregion

        #region Live

        static IEnumerable<T> GetElements(INode parent, Boolean deep, Predicate<T> predicate)
        {
            var items = deep ? GetElementsOf(parent) : GetOnlyElementsOf(parent);

            if (predicate != null)
                return items.Where(m => predicate(m));

            return items;
        }

        static IEnumerable<T> GetElementsOf(INode parent)
        {
            for (int i = 0; i < parent.ChildNodes.Length; i++)
            {
                if (parent.ChildNodes[i] is T)
                    yield return parent.ChildNodes[i] as T;

                foreach (var element in GetElementsOf(parent.ChildNodes[i]))
                    yield return element;
            }
        }

        static IEnumerable<T> GetOnlyElementsOf(INode parent)
        {
            for (int i = 0; i < parent.ChildNodes.Length; i++)
            {
                if (parent.ChildNodes[i] is T)
                    yield return parent.ChildNodes[i] as T;
            }
        }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Gets an enumerator over the contained elements.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
        {
            return _elements.OfType<Element>().GetEnumerator();
        }

        #endregion

        #region IHtmlCollection

        Element IHtmlCollection.this[int index]
        {
            get { return this[index] as Element; }
        }

        Element IHtmlCollection.this[String name]
        {
            get { return this[name] as Element; }
        }

        #endregion
    }

    /// <summary>
    /// A collection of HTML nodes.
    /// </summary>
    sealed class HTMLCollection : HTMLCollection<Element>
    {
        #region ctor

        /// <summary>
        /// Creates a new list of HTML elements.
        /// </summary>
        /// <param name="elements">The elements to use.</param>
        internal HTMLCollection(IEnumerable<Element> elements)
            : base(elements)
        {
        }

        /// <summary>
        /// Creates a new live collection for the given parent.
        /// </summary>
        /// <param name="parent">The parent of this collection.</param>
        /// <param name="deep">[Optional] Determines if recursive search is activated.</param>
        /// <param name="predicate">[Optional] The predicate function for picking elements.</param>
        internal HTMLCollection(INode parent, Boolean deep = true, Predicate<Element> predicate = null)
            : base(parent, deep, predicate)
        {
        }

        #endregion
    }

    /// <summary>
    /// A collection of HTML form controls.
    /// </summary>
    sealed class HTMLFormControlsCollection : HTMLCollection<HTMLFormControlElement>, IHtmlFormControlsCollection
    {
        #region ctor

        internal HTMLFormControlsCollection(IEnumerable<HTMLFormControlElement> elements)
            : base(elements)
        {
        }

        internal HTMLFormControlsCollection(Element parent)
            : base(parent)
        {
        }

        #endregion

        #region IHtmlFormControlsCollection

        Int32 IHtmlCollection.Length
        {
            get { return Length; }
        }

        Element IHtmlCollection.this[Int32 index]
        {
            get { return this[index]; }
        }

        Element IHtmlCollection.this[String name]
        {
            get { return this[name]; }
        }

        IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
        {
            var enumerator = base.GetEnumerator();

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
