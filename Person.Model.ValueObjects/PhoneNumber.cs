namespace Person.Model.ValueObjects
{
    public interface IPhoneNumber
    {
        string Raw { get; }
        string CountryCode { get; }
        string AreaCode { get; }
        string Number { get; }
    }
}
