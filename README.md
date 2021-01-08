# Person.Model.ValueObjects
Values objects that intent to model brazilian's Person domain
Implements validation, formatting and implicity operators for common types

## CNPJ - Cadastro Nacional de Pessoa Jurídica 
A string based struct that models a Employer Number Identification

### Creation
```c#
//with new operator
var cnpj = new CNPJ("39612247000102");

//with string implicity operator
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

//with ToString method
console.log(cnpj.ToString());
# 39.612.247\0001-02

//with string implicity operator
console.log(cnpj);
# 39.612.247\0001-02

```


### 
```c#
```

### 
```c#
  var isValid = CNPJ.IsValid("39612247000102");
 console.log(isValid);
 # true

 console.log(cnpj.Raw);
 # 39612247000102

```

# CPF
