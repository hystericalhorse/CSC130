using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nim
{
	public class Board<T>
	{
		public List<Pile<T>> piles = new();
		//public List<List<T>> piles;
	}

	public class Pile<T>
	{
		public Pile() { }
		public Pile(int num, T item)
		{
			for (int i = 0; i < num; i++)
				pile.Add(item);
		}
	
		List<T> pile = new();
		public Vector2 pileCenter;
	
		public int Count() => pile.Count;
	
		public void Add(T t) => pile.Add(t);
		public void Add(T[] tarray) => pile.AddRange(tarray);
		public void Add(List<T> tlist) => pile.AddRange(tlist);
	
		public void Remove(T t) => pile.Remove(t);
		public void RemoveFirst() => pile.Remove(pile.First());
		public void RemoveLast() => pile.Remove(pile.Last());
	
		public List<T> Get() => pile;
		public T[] GetArray() => pile.ToArray();
	
		public bool IsEmpty() => pile.Count == 0;
	}
}
