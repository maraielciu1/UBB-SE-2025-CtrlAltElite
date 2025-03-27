namespace MarketPlace924.Domain
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int SellerId { get; set; }
        public double Score { get; set; }

        public Review(int reviewId, int sellerId, double score)
        {
            ReviewId = reviewId;
            SellerId = sellerId;
            Score = score;
        }
    }
}