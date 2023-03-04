namespace EndPoints.Model
{
    public interface ILink
    {
        string Href { get; }
        string Rel { get; }
        string Type { get; }
    }
}