# Contributing

Thank you for your interest in contributing to **Person.Model.ValueObjects**.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Any editor — Visual Studio, Rider, or VS Code with the C# extension

## Setting up

```bash
git clone https://github.com/m4cm3nz/Person.Model.ValueObjects.git
cd Person.Model.ValueObjects
dotnet build
dotnet test
```

## How to contribute

### Reporting a bug

Open an issue using the **Bug report** template. Include a minimal reproduction snippet.

### Proposing a new value object

Open an issue using the **Value object proposal** template **before writing any code**. A proposal needs a citable official specification (Receita Federal, ANATEL, Correios, DENATRAN, etc.) and a defined validation algorithm. Proposals without a spec will be closed.

### Submitting a pull request

1. Fork the repository and create a branch from `master`.
2. Make your changes following the conventions below.
3. Run `dotnet build` and `dotnet test` — both must pass with zero warnings.
4. Open a PR against `master` using the provided template.

PRs that add a value object without a linked approved proposal may be closed without review.

---

## Conventions

### Value object anatomy

Every value object in this library follows the same pattern:

```csharp
[JsonConverter(typeof(XxxConverter))]
public readonly struct Xxx : IEquatable<Xxx>
{
    // private backing field(s)
    // public properties (no setter)
    // constructor — validates and assigns
    // override ToString()
    // implicit operator string (cast to raw value)
    // implicit operator Xxx(string) — throws InvalidOperationException for null
    // IEquatable<Xxx>, operator ==, operator !=, GetHashCode
    // static IsValid(string?) — never throws, returns bool
    // static StripMask(string) — if the type has a display mask
}
```

And a matching converter in `Person.Model.ValueObjects.Json`:

```csharp
public class XxxConverter : JsonConverter<Xxx>
{
    public override bool HandleNull => true;

    public override Xxx Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            throw new JsonException("Xxx cannot be null. Use Xxx? for nullable.");
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected a JSON string for Xxx, got {reader.TokenType}.");
        return new(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Xxx value, JsonSerializerOptions options)
    {
        var raw = (string)value;
        if (raw is null)
            throw new JsonException("Cannot serialize a default (uninitialized) Xxx.");
        writer.WriteStringValue(raw);
    }
}
```

### Testing

- Every public behavior must have at least one test.
- Tests must cover: valid input, invalid input (wrong format, wrong check digit, null), `IsValid`, `StripMask`, `ToString`, equality, and JSON round-trip.
- Use NUnit. Follow the naming pattern in existing test files.

### Commit messages

Use the conventional commits format:

```
feat: add CEP value object
fix: correct LandLine area code extraction for inputs without DDI
docs: update README with CEP examples
chore: bump version to 10.1.0
perf: replace LINQ in CPF check-digit with explicit loop
```

### Comments

Write no comments unless the *why* is non-obvious — a hidden constraint, a regulatory edge case, or a workaround for a specific bug. Do not describe what the code does.

---

## Branch and release model

- `master` is the stable branch. All PRs target `master`.
- Versions follow [Semantic Versioning](https://semver.org). Breaking changes increment the major version.
- A GitHub Release and a NuGet package are published automatically when a `v*` tag is pushed to `master`.
