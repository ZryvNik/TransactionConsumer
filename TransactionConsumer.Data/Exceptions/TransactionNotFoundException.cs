namespace TransactionConsumer.Data.Exceptions
{
    public class TransactionNotFoundException : Exception
    {
        public TransactionNotFoundException(string msg):base(msg) { }
    }
}
