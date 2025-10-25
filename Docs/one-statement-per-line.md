# One Statement Per Line : CC0001

Enforcing a single statement per line reduces horizontal density, which directly impacts readability. When multiple statements are packed into a single line, it becomes harder to parse intent at a glance — especially during code reviews or debugging.

This rule aligns with the principle of clarity over brevity, ensuring that each operation is visually distinct and semantically isolated.

## Example
```csharp
// Bad
int x = 10; int y = 20; int result = x + y * 2;

// Good
int x = 10;
int y = 20;
int result = x + y * 2;
```
## Rationale

One statement per line makes it easier to read diffs, track variable flow, and identify logic errors. It also improves debugging since breakpoints can be placed precisely on individual operations.
