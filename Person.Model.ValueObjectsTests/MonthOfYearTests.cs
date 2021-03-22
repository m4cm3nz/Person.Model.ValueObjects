using NUnit.Framework;
using Person.Model.ValueObjects;
using Person.Model.ValueObjects.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjectsTests
{
    public class Dummy
    {
        public MonthOfYear? MonthOfYear { get; set; }
    }

    public class DummyWithAttribute
    {
        [JsonConverter(typeof(MonthOfYearConverter))]
        public MonthOfYear? MonthOfYear { get; set; }
    }

    [TestFixture]
    public class MonthOfYearTests
    {
        [TestCase("05/2020")]
        [TestCase("05-2020")]
        [TestCase("052020")]
        public void ShouldBeAbleToCreateMonthOfYearWith(string value)
        {
            MonthOfYear monthOfYear = value;
            Assert.That(monthOfYear.ToString(), Is.EqualTo("05/2020"));
            Assert.That(monthOfYear.Month, Is.EqualTo(5));
            Assert.That(monthOfYear.Year, Is.EqualTo(2020));
            Assert.That((int)monthOfYear, Is.EqualTo(202005));
        }

        [TestCase("052022")]
        [TestCase("05/2022")]
        [TestCase("05-2022")]
        [TestCase("05x2022")]
        public void ShouldBeAbleToDeserializeValidMonthOfYearWithConverterOptions(string monthOfYear)
        {
            var dummy = new Dummy
            {
                MonthOfYear = monthOfYear
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new MonthOfYearConverter());

            var json = JsonSerializer.Serialize(dummy, options);

            var newDummy = JsonSerializer.Deserialize<Dummy>(json, options);

            Assert.That(newDummy.MonthOfYear, Is.EqualTo(dummy.MonthOfYear));
        }

        [Test]
        public void ShouldBeAbleToDeserializeValidMonthOfYearWithConverterAttribute()
        {
            var dummy = new DummyWithAttribute
            {
                MonthOfYear = "05/2022"
            };

            var json = JsonSerializer.Serialize(dummy);

            var newDummy = JsonSerializer.Deserialize<DummyWithAttribute>(json);

            Assert.That(newDummy.MonthOfYear, Is.EqualTo(dummy.MonthOfYear));
        }
    }
}
