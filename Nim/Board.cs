using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nim
{
	internal class Board<T>
	{
		List<Pile<T>> piles;
	}

	internal class Pile<T>
	{
		List<T> pile = new();

		public int Count() => pile.Count;

		public void Add(T t) => pile.Add(t);
		public void Add(T[] tarray) => pile.AddRange(tarray);
		public void Add(List<T> tlist) => pile.AddRange(tlist);

		public void Remove(T t) => pile.Remove(t);
		public void RemoveFirst() => pile.Remove(pile.First());
		public void RemoveLast() => pile.Remove(pile.Last());

		public List<T> Get() => pile;
		public T[] GetArray() => pile.ToArray();
	}
}
