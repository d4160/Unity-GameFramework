namespace d4160.Math
{
    public class MathUtils 
    {
        public static int Map(int value, int fromLow, int fromHigh, int toLow, int toHigh) {
			return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
		}

		public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh) {
			return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
		}
    }
}