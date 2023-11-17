namespace DF_EvolutionAPI.Models.Response
{
    public class UserAssignedKRA
    {
        public int KRAId { get; set; }
        public int UserId { get; set; }
        public int QuarterId { get; set; }
        public int IsSpecial { get; set; }
        public string KRAName { get; set; }
        public string UserName {  get; set; }
    }
}
