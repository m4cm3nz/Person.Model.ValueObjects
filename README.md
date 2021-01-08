# Person.Model.ValueObjects
Values objects of brazilian Person domain model
Implements validation, formatting and implicity operators for common types

# CNPJ - Cadastro Nacional de Pessoa Jurídica 
Employer Number Identification

```c#

 va isValid = CNPJ.IsValid("39612247000102");
 console.log(isValid);
 # true

 var cnpj = new CNPJ("39612247000102");
 
 console.log(cnpj.Raw);
 # 39612247000102
 
 console.log(cnpj.Number);
 # 396122470001
 
 console.log(cnpj.CheckNumber);
 # 02
 
```

# CPF
