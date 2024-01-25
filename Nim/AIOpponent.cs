namespace Nim
{
    internal class AIOpponent
    {
        public void OnTurn<T>(ref Board<T> board)
        {
            int[] heapSizes = new int[board.piles.Count];
            for (int i = 0; i < board.piles.Count; i++)
            {
                heapSizes[i] = board.piles[i].Count();
            }

            StratCheck(heapSizes);
        }

        private void StratCheck(int[] heapSizes)
        {
            JoeverCheck(ref heapSizes);
            DelusionalCheck(ref heapSizes);
            NimSumGameplay(ref heapSizes);
        }

        private void NimSumGameplay(ref int[] heapSizes)
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
            bool foundGoodMove = false;
            for (int i = 0; i < heapSizes.Length; i++)
            {
                if (heapNimSums[i] < heapSizes[i])
                {
                    // Take cookies from index i to match the nim sum.
                    foundGoodMove = true;
                }
            }
            if (!foundGoodMove)
            {
                RandomMove();
            }
        }

        private void DelusionalCheck(ref int[] heapSizes)
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
                        // Take all but one from pile of index i
                    }
                }
            }
        }

        private void JoeverCheck(ref int[] heapSizes)
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
            }
        }

        private void RandomMove()
        {
            // I was going to have it randomly choose a pile and amount, but it took a long time to get the rest of this code.
        }
    }
}
