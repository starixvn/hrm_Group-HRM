namespace Fanex.NexHR.Models
{
    public class EditShiftModel : CreateShiftModel
    {
        public int ShiftId { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsUsed { get; set; }
    }
}