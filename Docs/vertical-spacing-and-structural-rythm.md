# Vertical Spacing and Structural Rhythm : CC0004, CC0005, CC0008
Consistent vertical spacing creates a visual rhythm that guides the reader’s eye and reinforces logical structure.

A blank line before a control statement (e.g., if, for, while) signals a branch in logic and improves scanability.

Exactly one blank line after a completed block separates logical sections without introducing vertical noise.

No stray blank lines before closing braces keeps structures compact and prevents artificial expansion of files.

## Example
```csharp
// Bad
if (condition)
{
    DoSomething();
    DoSomethingElse();
    
}

else
{
    DoAnotherThing();
}

// Good
if (condition)
{
    DoSomething();
    DoSomethingElse();
}

else
{
    DoAnotherThing();
}
```

## Rationale
Consistent vertical rhythm allows readers to visually parse logical groupings. This predictability reduces cognitive load, speeds up code reviews, and minimizes misreads during debugging or refactoring.
pacing conventions, teams can maintain a clean and organized codebase that is easier to navigate and understand.