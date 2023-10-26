using Newtonsoft.Json;

namespace WebAPI.CrossCuttingConcerns.Exceptions
{
    public class Failure
    {
        public string Property { get; set; }
        public List<string> Errors { get; set; }
    }

    public class ErrorModel
    {
        public List<Failure> Failures { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
    }

    public class ErrorDynamicResponseModel
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
