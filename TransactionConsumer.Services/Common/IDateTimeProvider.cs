namespace TransactionConsumer.Services.Common
{
    public interface IDateTimeProvider
    {
        DateTime GetNowUtc();
    }
}
