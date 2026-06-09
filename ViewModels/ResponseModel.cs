namespace DF_EvolutionAPI.ViewModels
{
    public class ResponseModel
    {
        public int Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Messsage { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
    }
}
