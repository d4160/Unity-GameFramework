namespace d4160.Utilities
{
    using UnityEngine;
    using d4160.Core;

    public static class VectorUtilities
    {
        public static Vector3 GetDifferent(Vector3[] from, Vector3[] alreadyAdded, int upperIdx = -1)
        {
            bool exists;
            int up = upperIdx == -1 ? alreadyAdded.Length : (upperIdx + 1);
            int rd = -1;
            do
            {
                rd = from.RandomIndex();
                exists = false;

                for (int i = 0; i < up; i++)
                {
                    if (from[rd] == alreadyAdded[i])
                    {
                        exists = true;
                        break;
                    }
                }
            } while (exists);

            return from[rd];
        }

        public static Vector3Int GetDifferent(Vector3Int[] from, Vector3Int[] alreadyAdded, int upperIdx = -1)
        {
            bool exists;
            int up = upperIdx == -1 ? alreadyAdded.Length : (upperIdx + 1);
            int rd = -1;
            do
            {
                rd = from.RandomIndex();
                exists = false;

                for (int i = 0; i < up; i++)
                {
                    if (from[rd] == alreadyAdded[i])
                    {
                        exists = true;
                        break;
                    }
                }
            } while (exists);

            return from[rd];
        }
    }
}
