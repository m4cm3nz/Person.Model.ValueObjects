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
- **All value objects**: `\d` in regex patterns replaced with `[0-9]` — `\d` in .NET matches any Unicode decimal digit (Arabic-Indic, Devanagari, etc.), not just ASCII digits. This prevented nonsense input from being correctly rejected.
- **All value objects**: `default(T).ToString()` now returns `string.Empty` instead of throwing `NullReferenceException`. The default struct state (uninitialized) is a known C# limitation; the null guard makes it safe for use in collections and nullable contexts.
- **CardNumber.IsValid**: now returns `false` for `null` input (consistent with `CNPJ.IsValid` and `CPF.IsValid`) instead of throwing `ArgumentNullException`.
- **JSON converters (LandLine / Mobile)**: `PhoneNumberFactory.Write` previously serialized a JSON object (`{"Raw":"...","CountryCode":"...","AreaCode":"...","Number":"..."}`); deserialization read only the `"Raw"` field, making the serialization asymmetric and brittle. Both converters now serialize as a plain JSON string (the canonical `Raw` value), consistent with `CardNumberConverter`. **This is a breaking change for any existing serialized JSON payload using these converters.**

---

### Documentation

- XML docs added to `CPF`, `LandLine`, `Mobile`, and `CardNumber`; translated to English across all value objects.
- README updated: `CardNumber` and `JSON converters` sections added; corrected examples and references to removed methods; updated regex patterns.

### Performance improvements

- **All validation algorithms**: `Enumerable.Distinct().Count() == 1` replaced with a zero-allocation char loop; `int[]` heap allocations in CNPJ/CPF check-digit routines replaced with `stackalloc Span<int>` — eliminates 3–5 heap allocations per `IsValid` call.
- **CPF check-digit**: LINQ closure (`values.Sum(v => v * weight--)`) replaced with an explicit loop, eliminating delegate allocation per call.
- **StripMask (CNPJ/CPF)**: four chained `string.Replace` calls replaced with a single-pass `stackalloc` char filter — reduces peak allocations from 4 to 1 per call.
- **Phone number construction (LandLine/Mobile)**: `ExtractDigits` rewritten using `stackalloc` char filter instead of `MatchCollection` + `string.Join` — reduces allocations from ~12 to 1 per digit extraction.
- **Source-generated regexes**: all five compiled regexes migrated to `[GeneratedRegex]`, eliminating runtime IL emission (`DynamicMethod`) and making the library AOT-compatible. Phone patterns use `RegexOptions.NonBacktracking` for provably O(n) matching.

### Security improvements

- **DoS via large payloads**: constructors and `IsValid` methods now reject inputs exceeding a safe maximum length before performing any string manipulation. Prevents multi-megabyte payloads from triggering multi-allocation `StripMask` chains.
- **CardNumber.ToString() masked by default**: `ToString()` now returns a masked representation (e.g. `**** **** **** 4286`) to prevent accidental PAN exposure in logs and structured output. Use `ToFormatted()` for the full formatted number.
- **JSON converters**: `CardNumberConverter` and `MobileConverter` now handle `JsonTokenType.Null` explicitly instead of passing `null!` to constructors.
