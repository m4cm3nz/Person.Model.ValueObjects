# Person.Model.ValueObjects

A .NET 10.0 collection of value objects for modelling Brazilian person domain properties.

```csharp
public class Company
{
    public CNPJ    CNPJ     { get; set; }
    public LandLine LandLine { get; set; }
}

var company = new Company
{
    CNPJ     = "39612247000102",
    LandLine = "5136350020"
};

Console.WriteLine(company.CNPJ);             // 39.612.247/0001-02
Console.WriteLine(company.CNPJ.Number);      // 396122470001
Console.WriteLine(company.CNPJ.CheckNumber); // 02

Console.WriteLine(company.LandLine);          // +55 (51) 3635-0020
Console.WriteLine(company.LandLine.AreaCode); // 51
Console.WriteLine(company.LandLine.Number);   // 36350020
```

---

## CNPJ

*Cadastro Nacional de Pessoa Jurídica* — Brazilian Employer Identification Number.

A read-only struct that validates and models a CNPJ. Supports both the legacy
numeric format and the **new alphanumeric format** introduced by
[IN RFB nº 2.229/2024](https://www.gov.br/receitafederal), effective July 2026.

**Format:** `[A-Z0-9]{12}[0-9]{2}` — the last two characters (check digits) are always numeric.
Formatting characters (`.` `/` `-`) are stripped automatically on construction.
Lowercase letters are **rejected** — the caller is responsible for casing.

> **v10 breaking changes**
> - `ToString()` now uses alphanumeric mask for all formats (`AB.123.456/0001-10`).
> - Lowercase letters throw `ArgumentOutOfRangeException` instead of being silently uppercased.
> - Internal helper methods (`IsNumeric`, `IsFourteenLength`, `IsOutOfRange`, `GetNumberFrom`, `GetCheckNumberFrom`) removed from the public API.

### Creation

```csharp
// constructor — with or without mask
var cnpj = new CNPJ("39612247000102");
var cnpj = new CNPJ("39.612.247/0001-02");

// implicit operator
CNPJ cnpj = "39612247000102";

// nullable
CNPJ? cnpj = null;

// alphanumeric (effective July 2026)
var cnpj = new CNPJ("AB123456000110");
```

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` passed to constructor |
| `InvalidOperationException` | `null` assigned via implicit operator — use `CNPJ?` |
| `ArgumentOutOfRangeException` | wrong length, invalid characters, or lowercase letters |
| `InvalidCastException` | check digits do not match |

### Properties

```csharp
CNPJ cnpj = "39612247000102";

Console.WriteLine((string)cnpj);     // 39612247000102
Console.WriteLine(cnpj.Number);      // 396122470001
Console.WriteLine(cnpj.CheckNumber); // 02
Console.WriteLine(cnpj.ToString());  // 39.612.247/0001-02
```

Alphanumeric:

```csharp
CNPJ cnpj = "AB123456000110";

Console.WriteLine((string)cnpj);     // AB123456000110
Console.WriteLine(cnpj.Number);      // AB1234560001
Console.WriteLine(cnpj.CheckNumber); // 10
Console.WriteLine(cnpj.ToString());  // AB.123.456/0001-10
```

### Static helpers

```csharp
CNPJ.IsValid("39612247000102");     // true
CNPJ.IsValid("39.612.247/0001-02"); // true  (mask accepted)
CNPJ.IsValid("AB123456000110");     // true  (alphanumeric)
CNPJ.IsValid("39612237000102");     // false (wrong check digit)
CNPJ.IsValid(null);                 // false

CNPJ.StripMask("39.612.247/0001-02"); // 39612247000102
CNPJ.StripMask("AB.123.456/0001-10"); // AB123456000110
```

---

## CPF

*Cadastro de Pessoa Física* — Brazilian Social Security Number.

### Creation

```csharp
var cpf = new CPF("99194415030");
var cpf = new CPF("991.944.150-30"); // mask accepted

CPF cpf = "99194415030";  // implicit operator
CPF? cpf = null;          // nullable
```

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` passed to constructor |
| `InvalidOperationException` | `null` assigned via implicit operator — use `CPF?` |
| `ArgumentOutOfRangeException` | non-numeric or wrong length |
| `InvalidCastException` | check digits do not match |

### Properties

```csharp
CPF cpf = "99194415030";

Console.WriteLine((string)cpf);     // 99194415030
Console.WriteLine(cpf.Number);      // 991944150
Console.WriteLine(cpf.CheckNumber); // 30
Console.WriteLine(cpf.ToString());  // 991.944.150-30
```

### Static helpers

```csharp
CPF.IsValid("99194415030");       // true
CPF.IsValid("991.944.150-30");    // true  (mask accepted)
CPF.IsValid("23412412412");       // false (wrong check digit)
CPF.IsValid(null);                // false

CPF.StripMask("991.944.150-30"); // 99194415030
```

---

## LandLine

Brazilian landline phone number in ANATEL standard format.
First digit of the local number must be between 2 and 5.

Accepted patterns (DDI optional, DDD required, punctuation and spaces flexible):

```
^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5][0-9]{3}[- ]?[0-9]{4})$
```

### Creation

```csharp
var landLine = new LandLine("5136352520");

// multiple input formats accepted
LandLine landLine = "+55(51)3635-2520";
LandLine landLine = "+55 (51) 3635-2520";
LandLine landLine = "51 3635 2520";

LandLine? landLine = null; // nullable
```

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` or empty string |
| `InvalidOperationException` | `null` assigned via implicit operator — use `LandLine?` |
| `ArgumentOutOfRangeException` | format does not match ANATEL pattern |

### Properties

`Raw` always returns the canonical form `CountryCode + AreaCode + Number`, regardless of the input format.

```csharp
LandLine landLine = "5136352520";

Console.WriteLine(landLine.Raw);         // 555136352520
Console.WriteLine(landLine.CountryCode); // 55
Console.WriteLine(landLine.AreaCode);    // 51
Console.WriteLine(landLine.Number);      // 36352520
Console.WriteLine(landLine.ToString());  // +55 (51) 3635-2520
```

---

## Mobile

Brazilian mobile phone number in ANATEL standard format.
First digit of the local number must be 9 (total of 9 digits).

Accepted patterns (DDI optional, DDD required, punctuation and spaces flexible):

```
^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9[0-9]{4}[- ]?[0-9]{4})$
```

### Creation

```csharp
var mobile = new Mobile("51936351064");

// multiple input formats accepted
Mobile mobile = "+55(51)93635-1064";
Mobile mobile = "+55 (51) 93635-1064";
Mobile mobile = "51 93635 1064";

Mobile? mobile = null; // nullable
```

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` or empty string |
| `InvalidOperationException` | `null` assigned via implicit operator — use `Mobile?` |
| `ArgumentOutOfRangeException` | format does not match ANATEL pattern |

### Properties

`Raw` always returns the canonical form `CountryCode + AreaCode + Number`, regardless of the input format.

```csharp
Mobile mobile = "51936351064";

Console.WriteLine(mobile.Raw);         // 5551936351064
Console.WriteLine(mobile.CountryCode); // 55
Console.WriteLine(mobile.AreaCode);    // 51
Console.WriteLine(mobile.Number);      // 936351064
Console.WriteLine(mobile.ToString());  // +55 (51) 93635-1064
```

---

## CardNumber

Payment card number validated via the Luhn algorithm (mod 10).
Accepts cards with 13 to 19 digits.

### Creation

```csharp
var card = new CardNumber("4929622041254286");
CardNumber card = "4929622041254286"; // implicit operator
```

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` passed to constructor |
| `ArgumentException` | invalid length or Luhn check fails |

### Properties

`ToString()` returns the card number with all but the last 4 digits masked, making it
safe to emit in logs and structured output. Use `ToFormatted()` to obtain the full
formatted number when the unmasked value is required.

```csharp
CardNumber card = "4929622041254286";

Console.WriteLine((string)card);       // 4929622041254286  (raw digits)
Console.WriteLine(card.ToString());    // **** **** **** 4286  (masked — safe for logs)
Console.WriteLine(card.ToFormatted()); // 4929 6220 4125 4286
```

### Static helpers

```csharp
CardNumber.IsValid("4929622041254286"); // true
CardNumber.IsValid("49538528316877");   // false (Luhn fail)
CardNumber.IsValid("123");             // false (too short)
CardNumber.IsValid(null);              // false
```

---

## JSON converters

All five value objects carry a `[JsonConverter]` attribute on the struct itself.
`System.Text.Json` discovers the converter automatically — no property annotation
or `options.Converters.Add()` call is needed.

| Type | Converter | Wire format |
|---|---|---|
| `CPF` | `CpfConverter` | `"52998224725"` |
| `CNPJ` | `CnpjConverter` | `"11222333000181"` |
| `Mobile` | `MobileConverter` | `"5551985680052"` |
| `LandLine` | `LandLineConverter` | `"555136352520"` |
| `CardNumber` | `CardNumberConverter` | `"4929622041254286"` |

All converters serialize as a **plain JSON string** — the canonical digit form, never the
formatted mask (e.g. `123.456.789-09`).

```csharp
public class Subscriber
{
    public CPF      Cpf      { get; set; }
    public CNPJ?    Cnpj     { get; set; }
    public Mobile   Mobile   { get; set; }
    public LandLine? LandLine { get; set; }
    public CardNumber CardNumber { get; set; }
}

// just works — no [JsonConverter] annotations required
var json   = JsonSerializer.Serialize(subscriber);
var result = JsonSerializer.Deserialize<Subscriber>(json);
```

### Null handling

| Scenario | Behavior |
|---|---|
| JSON `null` for nullable property (`T?`) | returns `null` |
| JSON `null` for non-nullable property (`T`) | `JsonException` |
| Non-string token (number, boolean, object) | `JsonException` |
| Invalid value (bad format, wrong check digits) | constructor exception propagates |

```csharp
// nullable — null JSON produces null value
CNPJ?    cnpj    = JsonSerializer.Deserialize<CNPJ?>("null");    // null
LandLine? line   = JsonSerializer.Deserialize<LandLine?>("null"); // null

// non-nullable — null JSON throws
JsonSerializer.Deserialize<CPF>("null");  // JsonException
```

> **v10 breaking changes**
> - `LandLineConverter` and `MobileConverter` previously serialized a JSON object
>   (`{"Raw":"...","CountryCode":"...","AreaCode":"...","Number":"..."}`). They now
>   serialize as a plain JSON string. Existing payloads produced with the old converters
>   are **not** compatible with this version.
> - `LandLineConverter` changed from `JsonConverter<LandLine?>` to `JsonConverter<LandLine>`.
>   Property-level `[JsonConverter(typeof(LandLineConverter))]` annotations remain valid
>   but are no longer needed.
> - `CpfConverter` and `CnpjConverter` are new in v10.
