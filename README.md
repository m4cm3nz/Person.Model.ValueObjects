# Person.Model.ValueObjects
Values objects that intent to model brazilian's Person domain
Implements validation, formatting and implicity operators for common types

### CNPJ - Cadastro Nacional de Pessoa Jurídica 
A string based struct that models a Employer Number Identification

### Creation
```c#
// with new operator
var cnpj = new CNPJ("39612247000102");

// with string implicity operator
CNPJ cnpj = "39612247000102";
```

### Dismembering
```c#
 CNPJ cnpj = "39612247000102";
 
 console.log(cnpj.Raw);
 # 39612247000102
 
 console.log(cnpj.Number);
 # 396122470001
 
 console.log(cnpj.CheckNumber);
 # 02
```

### Formatting
```c#
 CNPJ cnpj = "39612247000102";

// with ToString method
console.log(cnpj.ToString());
# 39.612.247\0001-02

// with string implicity operator
console.log(cnpj);
# 39.612.247\0001-02
```

### Helper Functions
```c#
// non-numeric
CNPJ.IsNumeric("39 12247A00102");
# false

// not forteen size length
CNPJ.IsFourteenLength("396122470001020000");
# false

// in range
CNPJ.IsOutOfRange("39612247000102");
# false

// out of range
// non-numeric or not forteend size length
CNPJ.IsOutOfRange("396 K 224 0 SDDFG 010A0000");
# true
CNPJ.IsOutOfRange("396122470001020000");
# true
CNPJ.IsOutOfRange("39 12247A00102");
# true

// in range and valid cnpj
CNPJ.IsValid("39612247000102");
# true

// get number part of cnpj in range candidate
CNPJ.GetNumberFrom("39612247000102");
# 396122470001

// get check number part of in range cnpj candidate
CNPJ.GetCheckNumberFrom("39612247000102");
# 02
```

