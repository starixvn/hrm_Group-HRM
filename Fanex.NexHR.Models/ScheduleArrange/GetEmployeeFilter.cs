namespace Fanex.NexHR.Models.ScheduleArrange
{
    public class GetEmployeeFilter
    {
        public int ActorId { get; set; }
        public string TeamIds { get; set; }
        public string Query { get; set; }
        public bool IsIFrame { get; set; }
    }
}