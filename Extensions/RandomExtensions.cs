using Random = System.Random;

namespace QuotaDependentSpeed.Extensions
{
    internal static class RandomExtensions
    {
        public static double NextDouble(this Random random, double min, double max)
        {
            return (random.NextDouble() * (max - min)) + min;
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return (float)random.NextDouble(min, max);
        }
    }
}
