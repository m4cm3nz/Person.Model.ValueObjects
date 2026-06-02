# Person.Model.ValueObjects

A .NET 10.0 collection of value objects for modelling Brazilian person domain properties.

```csharp
public class Company
{
    public CNPJ   CNPJ     { get; set; }
    public LandLine LandLine { get; set; }
}

var company = new Company
{
    CNPJ     = "39612247000102",
    LandLine = "5136350020"
};

Console.WriteLine(company.CNPJ);            // 39.612.247/0001-02
Console.WriteLine(company.CNPJ.Number);     // 396122470001
Console.WriteLine(company.CNPJ.CheckNumber);// 02

Console.WriteLine(company.LandLine);           // +55 (51) 3635-0020
Console.WriteLine(company.LandLine.AreaCode);  // 51
Console.WriteLine(company.LandLine.Number);    // 36350020
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

> **v2 breaking changes**
> - `IsNumeric`, `IsFourteenLength`, `IsOutOfRange` removed from the public API.
> - Static `GetNumberFrom(string)` and `GetCheckNumberFrom(string)` removed — use instance properties.
> - `ToString()` now uses substring formatting for all formats (numeric and alphanumeric).
> - Lowercase letters throw `ArgumentOutOfRangeException` instead of being silently uppercased.

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

Throws:

| Exception | Condition |
|---|---|
| `ArgumentNullException` | `null` passed to constructor |
| `InvalidOperationException` | `null` assigned via implicit operator |
| `ArgumentOutOfRangeException` | wrong length, invalid characters, or lowercase letters |
| `InvalidCastException` | check digits do not match |

### Properties

```csharp
CNPJ cnpj = "39612247000102";

Console.WriteLine((string)cnpj);    // 39612247000102  (raw, implicit)
Console.WriteLine(cnpj.Number);     // 396122470001
Console.WriteLine(cnpj.CheckNumber);// 02
Console.WriteLine(cnpj.ToString()); // 39.612.247/0001-02
```

Alphanumeric:

```csharp
CNPJ cnpj = "AB123456000110";

Console.WriteLine((string)cnpj);    // AB123456000110
Console.WriteLine(cnpj.Number);     // AB1234560001
Console.WriteLine(cnpj.CheckNumber);// 10
Console.WriteLine(cnpj.ToString()); // AB.123.456/0001-10
```

### Static helpers

```csharp
// validate without throwing
CNPJ.IsValid("39612247000102");          // true
CNPJ.IsValid("39.612.247/0001-02");      // true  (mask accepted)
CNPJ.IsValid("AB123456000110");          // true  (alphanumeric)
CNPJ.IsValid("39612237000102");          // false (wrong check digit)
CNPJ.IsValid(null);                      // false

// strip formatting characters only — does not change casing
CNPJ.StripMask("39.612.247/0001-02");    // 39612247000102
CNPJ.StripMask("AB.123.456/0001-10");    // AB123456000110
```

---

## CPF

*Cadastro de Pessoa Física* — Brazilian Social Security Number.

### Creation

```csharp
var cpf = new CPF("99194415030");
CPF cpf = "99194415030";
CPF? cpf = null;
```

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
CPF.IsNumeric("991 94 415 030");         // false
CPF.IsElevenLength("991944150302342");   // false
CPF.IsOutOfRange("99194415030");         // false
CPF.IsOutOfRange("991 944 ABC 150");     // true
CPF.IsValid("99194415030");              // true
CPF.GetNumberFrom("99194415030");        // 991944150
CPF.GetCheckNumberFrom("99194415030");   // 30
```

---

## LandLine

Brazilian landline phone number in ANATEL standard format.

Accepted patterns (all punctuation and spaces optional, no double spaces):

```
^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?([2-5]\d{3}[-| ]?\d{4})$
```

### Creation

```csharp
var landLine = new LandLine("5126352520");
LandLine landLine = "5126352520";
LandLine? landLine = null;

// multiple input formats accepted
LandLine landLine = "+55(51)2635-2520";
LandLine landLine = "+55 (51) 2635-2520";
LandLine landLine = "55 51 2635 2520";
```

### Properties

```csharp
LandLine landLine = "555126352520";

Console.WriteLine((string)landLine);       // 555126352520
Console.WriteLine(landLine.CountryCode);   // 55
Console.WriteLine(landLine.AreaCode);      // 51
Console.WriteLine(landLine.Number);        // 26352520
```

---

## Mobile

Brazilian mobile phone number in ANATEL standard format.

Accepted patterns (all punctuation and spaces optional, no double spaces):

```
^(\+?55 ?)? ?(\([1-9]{2}\)|[1-9]{2}) ?(9\d{4}[-| ]?\d{4})$
```

### Creation

```csharp
var mobile = new Mobile("51932321078");
Mobile mobile = "51932321078";
Mobile? mobile = null;

// multiple input formats accepted
Mobile mobile = "+55(51)93232-1078";
Mobile mobile = "+55 (51) 93232-1078";
Mobile mobile = "55 51 93232 1078";
```

### Properties

```csharp
Mobile mobile = "5551932321078";

Console.WriteLine((string)mobile);      // 5551932321078
Console.WriteLine(mobile.CountryCode);  // 55
Console.WriteLine(mobile.AreaCode);     // 51
Console.WriteLine(mobile.Number);       // 932321078
```