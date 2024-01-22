namespace Models.GitHub
{
    public class HeadCommit
    {
        public string id { get; set; }
        public string tree_id { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
        public Author author { get; set; }
        public Committer committer { get; set; }
    }

}
