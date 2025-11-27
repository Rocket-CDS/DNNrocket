using System.ComponentModel.DataAnnotations;

namespace DNNrocketAPI.Components
{
    public class ExternalApiModel
    {
        public string ApiRef { get; set; }
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public ExternalApiModel()
        {
            ApiRef = string.Empty;
            ApiUrl = string.Empty;
            ApiKey = string.Empty;
        }
    }
}