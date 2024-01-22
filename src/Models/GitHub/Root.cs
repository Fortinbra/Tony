namespace Models.GitHub
{
    public class Root : Base
    {
        public string Action { get; set; }
        public WorkflowRun Workflow_run { get; set; }
        public Workflow Workflow { get; set; }
        public Repository Repository { get; set; }
        public Organization Organization { get; set; }
        public Sender Sender { get; set; }
    }


}
