using System.Collections.Generic;

namespace Backend.Responses
{
    public class LessionListResponse
    {
        public List<LessionResponse> lessions { get; set; }
        public int totalPages { get; set; }
    }
}