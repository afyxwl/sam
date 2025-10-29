namespace SAMsa.Services
{
    public sealed class StorageInfo
    {
        public bool UsingMongo { get; }
        public string Connection { get; }
        public string Database { get; }

        public StorageInfo(bool usingMongo, string connection, string database)
        {
            UsingMongo = usingMongo;
            Connection = connection;
            Database = database;
        }
    }
}
