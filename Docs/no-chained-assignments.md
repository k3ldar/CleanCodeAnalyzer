# No Chained Assignments : CC0002

Chained assignments (e.g., a = b = c = 0;) introduce implicit side effects and obscure the flow of data. They can lead to subtle bugs, especially when dealing with mutable types or expressions with side effects.
```csharp
## Example
// Bad
int a, b, c;
a = b = c = 0;

// Good
int a = 0;
int b = 0;
int c = 0;
```

## Rationale

Rejecting chained assignments promotes explicitness and traceability. Developers can follow each assignment individually, making the code easier to reason about and safer to refactor.
