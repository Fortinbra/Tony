namespace Models.GitHub
{
    public class Workflow
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string badge_url { get; set; }
    }


}
