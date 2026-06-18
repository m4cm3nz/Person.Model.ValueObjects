using System.Text.Json;
using Person.Model.ValueObjects;

namespace Person.Model.ValueObjects.Playground;

/// <summary>Result of successfully constructing a value object from user input.</summary>
public record DemoResult(
    string Formatted,
    string Raw,
    IReadOnlyList<(string Label, string Value)> Properties,
    string Json);

/// <summary>A single value object exposed in the playground.</summary>
public record DemoType(
    string Name,
    string Description,
    string Placeholder,
    Func<string, bool> IsValid,
    Func<string, DemoResult> Construct);

/// <summary>
/// Static catalogue of every value object the playground can exercise. Each entry runs the real
/// library code: validation, construction, <c>ToString()</c> masking and JSON serialization.
/// </summary>
public static class DemoCatalog
{
    private static (string, string)[] None => [];

    private static bool ConstructsOk<T>(Func<string, T> ctor, string value)
    {
        try { ctor(value); return true; }
        catch { return false; }
    }

    public static IReadOnlyList<DemoType> All { get; } =
    [
        new DemoType(
            "CPF",
            "Cadastro de Pessoa Física — 11 dígitos com 2 verificadores.",
            "381.240.360-98",
            CPF.IsValid,
            input =>
            {
                var cpf = new CPF(input);
                return new DemoResult(cpf.ToString(), (string)cpf,
                    [("Number", cpf.Number), ("CheckNumber", cpf.CheckNumber)],
                    JsonSerializer.Serialize(cpf));
            }),

        new DemoType(
            "CNPJ",
            "Cadastro Nacional de Pessoa Jurídica — 14 caracteres com 2 verificadores.",
            "11.222.333/0001-81",
            CNPJ.IsValid,
            input =>
            {
                var cnpj = new CNPJ(input);
                return new DemoResult(cnpj.ToString(), (string)cnpj,
                    [("Number", cnpj.Number), ("CheckNumber", cnpj.CheckNumber)],
                    JsonSerializer.Serialize(cnpj));
            }),

        new DemoType(
            "CEP",
            "Código de Endereçamento Postal — 8 dígitos.",
            "01310-100",
            CEP.IsValid,
            input =>
            {
                var cep = new CEP(input);
                return new DemoResult(cep.ToString(), (string)cep, None, JsonSerializer.Serialize(cep));
            }),

        new DemoType(
            "Mobile",
            "Celular brasileiro (ANATEL) — DDD + 9 dígitos.",
            "(51) 98568-0052",
            input => ConstructsOk(v => new Mobile(v), input),
            input =>
            {
                var mobile = new Mobile(input);
                return new DemoResult(mobile.ToString(), (string)mobile,
                    [("CountryCode", mobile.CountryCode), ("AreaCode", mobile.AreaCode), ("Number", mobile.Number)],
                    JsonSerializer.Serialize(mobile));
            }),

        new DemoType(
            "LandLine",
            "Telefone fixo brasileiro (ANATEL) — DDD + 8 dígitos.",
            "(51) 3635-2520",
            input => ConstructsOk(v => new LandLine(v), input),
            input =>
            {
                var landline = new LandLine(input);
                return new DemoResult(landline.ToString(), (string)landline,
                    [("CountryCode", landline.CountryCode), ("AreaCode", landline.AreaCode), ("Number", landline.Number)],
                    JsonSerializer.Serialize(landline));
            }),

        new DemoType(
            "CardNumber",
            "Número de cartão de pagamento — validação Luhn, 13–19 dígitos.",
            "4929622041254286",
            CardNumber.IsValid,
            input =>
            {
                var card = new CardNumber(input);
                return new DemoResult(card.ToString(), (string)card,
                    [("ToFormatted()", card.ToFormatted())],
                    JsonSerializer.Serialize(card));
            }),

        new DemoType(
            "PIS",
            "PIS / PASEP / NIS — 11 dígitos com 1 verificador.",
            "123.45678.91-9",
            PIS.IsValid,
            input =>
            {
                var pis = new PIS(input);
                return new DemoResult(pis.ToString(), (string)pis, None, JsonSerializer.Serialize(pis));
            }),

        new DemoType(
            "Email",
            "Endereço de e-mail — subconjunto prático de RFC 5321/5322, normalizado para minúsculas.",
            "Rafael@Example.com",
            Email.IsValid,
            input =>
            {
                var email = new Email(input);
                return new DemoResult(email.ToString(), (string)email,
                    [("Local", email.Local), ("Domain", email.Domain)],
                    JsonSerializer.Serialize(email));
            }),

        new DemoType(
            "CNH",
            "Carteira Nacional de Habilitação — 11 dígitos, algoritmo SENATRAN.",
            "84718735264",
            CNH.IsValid,
            input =>
            {
                var cnh = new CNH(input);
                return new DemoResult(cnh.ToString(), (string)cnh, None, JsonSerializer.Serialize(cnh));
            }),
    ];
}
