namespace CoinTrading
{
    public class CoinPairs
    {
        public enum AvailablePair { unknown, btcusdt }
        public AvailablePair Pair { get; set; }
        public double Value { get; set; }
    }
}
