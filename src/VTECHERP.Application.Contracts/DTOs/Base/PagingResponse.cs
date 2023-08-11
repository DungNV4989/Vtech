using System.Collections.Generic;

namespace VTECHERP.DTOs.Base
{
    public class PagingResponse<T> where T : class
    {
        public int Total { get;set;}
        public IEnumerable<T> Data { get; set; }
        public PagingResponse()
        {

        }

        public PagingResponse(int total, IEnumerable<T> data)
        {
            Total = total;
            Data = data;
        }
    }
}
