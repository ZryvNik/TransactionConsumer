
namespace TransactionConsumer.Services.Common
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetNowUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
