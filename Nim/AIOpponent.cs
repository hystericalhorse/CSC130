using System;

namespace Nim
{
    public class AIOpponent
    {
        public bool OnTurn<T>(ref Board<T> board)
        {
            int[] heapSizes = new int[board.piles.Count];
            for (int i = 0; i < board.piles.Count; i++)
            {
                heapSizes[i] = board.piles[i].Count();
            }

            return StratCheck(heapSizes);
        }

        private bool StratCheck(int[] heapSizes)
        {
            if (NimSumGameplay(ref heapSizes)) return true;
            if (DelusionalCheck(ref heapSizes)) return true;
            if (JoeverCheck(ref heapSizes)) return true;

            return false;
        }

        private bool NimSumGameplay(ref int[] heapSizes)
        {
            int boardNimSum = heapSizes[0];
            for (int i = 1; i < heapSizes.Length; i++)
            {
                boardNimSum ^= heapSizes[i];
            }
            int[] heapNimSums = new int[heapSizes.Length];
            for (int i = 0; i < heapSizes.Length; i++)
            {
                heapNimSums[i] = heapSizes[i] ^ boardNimSum;
            }
            for (int i = 0; i < heapSizes.Length; i++)
            {
                if (heapNimSums[i] < heapSizes[i])
                {
                    // Take cookies from index i to match the nim sum.
                    GameManager.instance.TakeFromPile(boardNimSum, i);
                    return true;
                }
            }

			RandomMove();
			return false;
		}

        private bool DelusionalCheck(ref int[] heapSizes)
        {
            int twoPlusCount = 0;
            foreach (int count in heapSizes)
            {
                if (count > 1)
                {
                    twoPlusCount++;
                }
            }
            if (twoPlusCount == 1)
            {
                for (int i = 0; i < heapSizes.Length; i++)
                {
                    if (heapSizes[i] == 1)
                    {
						GameManager.instance.TakeFromPile(heapSizes[i]-1, i);
                        return true;
						// Take all but one from pile of index i
					}
                }
            }

            return false;
        }

        private bool JoeverCheck(ref int[] heapSizes)
        {
            int singleCountHeaps = 0;
            bool onlySingles = true;
            foreach (var count in heapSizes)
            {
                if (count == 1)
                {
                    singleCountHeaps++;
                }
                if (count > 1)
                {
                    onlySingles = false;
                }
            }

            if (singleCountHeaps % 2 != 0 && onlySingles)
            {
                RandomMove();
                return true;
            }

            return false;
        }

        private bool RandomMove()
        {
            Random r = new();
			GameManager.instance.TakeFromPile(1, r.Next(0, 3));
            return true;
			// I was going to have it randomly choose a pile and amount, but it took a long time to get the rest of this code.
		}
    }
}
