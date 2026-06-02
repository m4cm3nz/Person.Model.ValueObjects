# Changelog

## [10.0.0] — 2026-06-02

### Breaking changes

#### CNPJ
- Letras minúsculas agora lançam `ArgumentOutOfRangeException` em vez de serem normalizadas silenciosamente.
- `ToString()` usa formatação de substring para todos os formatos, incluindo alfanumérico (`AB.123.456/0001-10`).

#### CPF
- `IsNumeric(string)`, `IsElevenLength(string)` e `IsOutOfRange(string)` removidos da API pública.
- `GetNumberFrom(string)` e `GetCheckNumberFrom(string)` removidos da API pública — use as propriedades `Number` e `CheckNumber` da instância.

#### LandLine / Mobile
- `Raw` agora retorna sempre a forma canônica `CountryCode + AreaCode + Number`, independentemente do formato de entrada. Código que dependia de `Raw` retornar os dígitos exatos do input original precisa ser atualizado.

---

### New features

#### CNPJ — formato alfanumérico
Suporte ao novo formato alfanumérico da Receita Federal (IN RFB nº 2.229/2024), vigente a partir de julho de 2026.

- Formato: `[A-Z0-9]{12}[0-9]{2}` — os dois últimos caracteres (dígitos verificadores) continuam sempre numéricos.
- O algoritmo de validação usa mapeamento ASCII-48 conforme especificação oficial.
- `ToString()`, `IsValid()` e `StripMask()` funcionam para ambos os formatos.

```csharp
CNPJ cnpj = "AB123456000110";
Console.WriteLine(cnpj.ToString());  // AB.123.456/0001-10
CNPJ.IsValid("AB123456000110");      // true
```

---

### Improvements

#### Plataforma
- Migração de .NET 8 para .NET 10.

#### CNPJ / CPF
- `IEquatable<T>`, `operator ==`, `operator !=` e `GetHashCode` implementados explicitamente, eliminando o custo de reflection do `ValueType` em coleções.
- Mensagens das exceções `InvalidOperationException` (atribuição nula via implicit operator) corrigidas para orientar o uso de `CNPJ?` / `CPF?`.

#### LandLine / Mobile
- Declarados como `readonly struct`, alinhando com `CNPJ`, `CPF` e `CardNumber`.
- Igualdade corrigida: dois números logicamente equivalentes criados de inputs com formatos diferentes (ex.: `"5136352520"` e `"+55 (51) 3635-2520"`) agora são iguais.
- Regex corrigido: separador `[-| ]` substituído por `[- ]` (o pipe não é separador válido em números de telefone).

#### CardNumber
- `IsValid` agora valida comprimento (13–19 dígitos) e rejeita caracteres não numéricos retornando `false`, em vez de lançar `FormatException`.
- `ToString()` formata em grupos de 4 dígitos baseado no comprimento real do número, sem padding de zeros (comportamento anterior produzia resultados incorretos para cartões Diners/AmEx).
- Propriedade pública `Number` exposta.
- `IEquatable<CardNumber>`, `operator ==` e `operator !=` implementados.
- Parâmetro do `implicit operator string` renomeado de `goodThruDate` para `cardNumber`.

---

### Bug fixes

- **CardNumber.IsValid**: `NullReferenceException` ao receber `null` substituído por `ArgumentNullException`.
- **LandLine / Mobile**: igualdade inconsistente entre instâncias do mesmo número em formatos de entrada diferentes (ver breaking change de `Raw` acima).

---

### Documentation

- XML docs adicionados em `CPF`, `LandLine`, `Mobile` e `CardNumber`.
- README atualizado: seções de `CardNumber` e `JSON converters` adicionadas; exemplos e referências a métodos removidos corrigidos; padrões de regex atualizados.
