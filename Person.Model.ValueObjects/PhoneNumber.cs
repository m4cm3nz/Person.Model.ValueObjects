namespace Person.Model.ValueObjects
{
    public interface PhoneNumber
    {
        string Raw { get; set; }
        string CountryCode { get; set; }
        string AreaCode { get; set; }
        string Number { get; set; }
    }
}
