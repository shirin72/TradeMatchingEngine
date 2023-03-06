namespace EndPoints.Model
{
    public class ResponseWrapper<T>
    {
        public T Value { get; set; }

        public IEnumerable<LinkVM> Links { get; set; }
    }
}
