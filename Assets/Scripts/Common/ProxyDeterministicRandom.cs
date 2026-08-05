namespace Common
{
    public static class ProxyDeterministicRandom
    {
        public static int Range(int seed, int max)
        {
            unchecked
            {
                var value = (uint)seed * 2654435761u;
                value ^= value >> 15;
                value *= 2246822519u;
                value ^= value >> 13;
                return (int)(value % (uint)max);
            }
        }
    }
}
