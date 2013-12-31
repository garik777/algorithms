using System;
using System.Collections.Generic;
using System.Collections;

namespace AlgorithmsPractice
{
	interface IDoubleLinkList<T>
	{
		void Add(T item);
		void Remove(T item);
		void Find(T item); 
		void Reverse();
	}
	public class DoubleLinkList<T> : IDoubleLinkList<T>, IEnumerable<T>
	{
		private class DoubleNode<T>
		{
			public DoubleNode<T> Prev = null;
			public DoubleNode<T> Next = null;

			public T Item;

		}

		private DoubleNode<T> _head;
		private DoubleNode<T> _tail;

		public DoubleLinkList ()
		{
			_head = null;
			_tail= null;
		}

		public DoubleNode<T> Head
		{
			get { return _head;}
		}
		public DoubleNode<T> Tail 
		{
			get { return _tail; }
		}
		public void Add(T item){
		}

		public void Remove(T item){
		}

		public void Find(T item){
		
		}

		public IEnumerator<T> GetEnumerator()
		{
			var node = _head;
			while(node != null)
			{
				yield return node.Item;
				node = node.Next;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}



	}
}

