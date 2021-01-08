# Person.Model.ValueObjects
A collection of value objects that intent to model brazilian's Person domain

### Employer Identification Number 
CNPJ - Cadastro Nacional de Pessoa Jurídica  
A string based struct that models the brazilian's Employer Identification Number

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

### Social Security Number
CPF - Cadastro de Pessoa Física
A string based struct that models the brazilian's Social Security Number  

### Creation
```c#
// with new operator
var cpf = new CPF("99194415030");

// with string implicity operator
CPF cpf = "99194415030";
```

### Dismembering
```c#
 CPF cpf = "99194415030";
 
 console.log(cpf.Raw);
 # 99194415030
 
 console.log(cpf.Number);
 # 991944150
 
 console.log(cpf.CheckNumber);
 # 30
```

### Formatting
```c#
 CPF cpf = "99194415030";

// with ToString method
console.log(cpf.ToString());
# 991.944.150-30

// with string implicity operator
console.log(cpf);
# 991.944.150-30
```

### Helper Functions
```c#
// non-numeric
CPF.IsNumeric("991 94 415 030");
# false

// not eleven size length
CPF.IsElevenLength("991944150302342");
# false

// in range
CPF.IsOutOfRange("99194415030");
# false

// out of range
// non-numeric or not eleven size length
CPF.IsOutOfRange("991 944 ABC 1503024332423");
# true
CPF.IsOutOfRange("99194415030243");
# true
CPF.IsOutOfRange("991944 ABC 150");
# true

// in range and valid cnpj
CPF.IsValid("99194415030");
# true

// get number part of cnpj in range candidate
CPF.GetNumberFrom("99194415030");
# 991944150

// get check number part of in range cnpj candidate
CPF.GetCheckNumberFrom("99194415030");
# 30
```
