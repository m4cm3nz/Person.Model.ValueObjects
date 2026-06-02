# Changelog

## [10.0.0] — 2026-06-02

### Breaking changes

#### CNPJ
- Lowercase letters now throw `ArgumentOutOfRangeException` instead of being silently normalized.
- `ToString()` uses substring-based formatting for all formats, including alphanumeric (`AB.123.456/0001-10`).

#### CPF
- `IsNumeric(string)`, `IsElevenLength(string)`, and `IsOutOfRange(string)` removed from the public API.
- `GetNumberFrom(string)` and `GetCheckNumberFrom(string)` removed from the public API — use the `Number` and `CheckNumber` instance properties instead.

#### LandLine / Mobile
- `Raw` now always returns the canonical form `CountryCode + AreaCode + Number`, regardless of the input format. Code that relied on `Raw` returning the exact digits of the original input needs to be updated.

---

### New features

#### CNPJ — alphanumeric format
Support for the new alphanumeric CNPJ format from Receita Federal (IN RFB nº 2.229/2024), effective July 2026.

- Format: `[A-Z0-9]{12}[0-9]{2}` — the last two characters (check digits) remain always numeric.
- The validation algorithm uses ASCII-48 mapping as per the official specification.
- `ToString()`, `IsValid()`, and `StripMask()` work for both formats.

```csharp
CNPJ cnpj = "AB123456000110";
Console.WriteLine(cnpj.ToString());  // AB.123.456/0001-10
CNPJ.IsValid("AB123456000110");      // true
```

---

### Improvements

#### Platform
- Migrated from .NET 8 to .NET 10.

#### CNPJ / CPF
- `IEquatable<T>`, `operator ==`, `operator !=`, and `GetHashCode` implemented explicitly, eliminating the reflection cost of `ValueType` in collections.
- `InvalidOperationException` messages (null assignment via implicit operator) corrected to guide the use of `CNPJ?` / `CPF?`.

#### LandLine / Mobile
- Declared as `readonly struct`, aligned with `CNPJ`, `CPF`, and `CardNumber`.
- Equality fixed: two logically equivalent numbers created from inputs in different formats (e.g., `"5136352520"` and `"+55 (51) 3635-2520"`) are now equal.
- Regex fixed: separator `[-| ]` replaced by `[- ]` (the pipe character is not a valid separator in phone numbers).

#### CardNumber
- `IsValid` now validates length (13–19 digits) and rejects non-numeric characters by returning `false`, instead of throwing `FormatException`.
- `ToString()` formats into groups of 4 digits based on the actual number length, without zero-padding (previous behavior produced incorrect results for Diners/AmEx cards).
- Public property `Number` exposed.
- `IEquatable<CardNumber>`, `operator ==`, and `operator !=` implemented.
- `implicit operator string` parameter renamed from `goodThruDate` to `cardNumber`.

---

### Bug fixes

- **CardNumber.IsValid**: `NullReferenceException` when receiving `null` replaced by `ArgumentNullException`.
- **LandLine / Mobile**: inconsistent equality between instances of the same number in different input formats (see `Raw` breaking change above).

---

### Documentation

- XML docs added to `CPF`, `LandLine`, `Mobile`, and `CardNumber`; translated to English across all value objects.
- README updated: `CardNumber` and `JSON converters` sections added; corrected examples and references to removed methods; updated regex patterns.
