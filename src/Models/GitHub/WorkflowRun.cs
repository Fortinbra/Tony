namespace Models.GitHub
{

    public class WorkflowRun
    {
        public long id { get; set; }
        public string name { get; set; }
        public string node_id { get; set; }
        public string head_branch { get; set; }
        public string head_sha { get; set; }
        public string path { get; set; }
        public string display_title { get; set; }
        public int run_number { get; set; }
        public string @event { get; set; }
        public string status { get; set; }
        public string conclusion { get; set; }
        public int workflow_id { get; set; }
        public long check_suite_id { get; set; }
        public string check_suite_node_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public List<string> pull_requests { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Actor actor { get; set; }
        public int run_attempt { get; set; }
        public List<ReferencedWorkflow> referenced_workflows { get; set; }
        public DateTime run_started_at { get; set; }
        public TriggeringActor triggering_actor { get; set; }
        public string jobs_url { get; set; }
        public string logs_url { get; set; }
        public string check_suite_url { get; set; }
        public string artifacts_url { get; set; }
        public string cancel_url { get; set; }
        public string rerun_url { get; set; }
        public string previous_attempt_url { get; set; }
        public string workflow_url { get; set; }
        public HeadCommit head_commit { get; set; }
        public Repository repository { get; set; }
        public HeadRepository head_repository { get; set; }
    }


}
