# Avoid Magic Numbers : CC0009

Hardcoded numeric, string, or character literals (""magic numbers"") obscure intent and make future changes error-prone. Replacing them with well-named constants clarifies semantics and centralizes values.

## Example
```csharp
// Bad
if (age >= 18)
    RegisterVoter(age);

// Good
private const int AgeOfMajority = 18;

if (age >= AgeOfMajority)
    RegisterVoter(age);
```

## Rationale

Named constants make code self-documenting, provide semantic meaning, and make updates safer.
- Communicate purpose (semantic meaning over incidental value)
- Reduce duplication
- Make refactoring safer (single change point)
- Improve discoverability during code review

## Exceptions (Allowed Inline)
- 0, 1, -1 (common trivial identity/sentinel values)
- Empty string ""
- true / false
- Enum member initializers
- Loop initializers like for (int i = 0; ...)

## Guidance
- Prefer domain-oriented names (AgeOfMajority, MaxRetryCount, DefaultBufferSize).
- Scope constants at the smallest viable visibility (private unless shared).
- Group related constants logically.
- Reassess whether a configuration setting (app config / options) is more appropriate for frequently changed values.