namespace Person.Model.ValueObjects
{
    public interface PhoneNumber
    {
        string Raw { get; }
        string CountryCode { get; }
        string AreaCode { get; }
        string Number { get; }
    }
}
