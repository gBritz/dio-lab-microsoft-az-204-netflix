namespace fnPostDatabase
{
    internal class MovieRequest
    {
        public string Id => Guid.NewGuid().ToString();
        public string Title { get; set; }
        public int Year { get; set; }
        public string Video { get; set; }
        public string Thumb { get; set; }
    }
}