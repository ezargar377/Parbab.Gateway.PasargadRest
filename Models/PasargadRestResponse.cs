


namespace ParbadGateway.PasargadRest.Models
{
    public class PasargadRestResponse<T>
    {
        public string resultMsg { get; set; }
        public int resultCode { get; set; }
        public T data { get; set; }
    }



}
