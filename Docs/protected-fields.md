# Protected Fields : CC0007

Protected fields expose internal state to subclasses, leading to fragile base class problems. This makes it difficult to change or enforce invariants safely.

## Example
```csharp
// Bad
public class BaseEntity
{
    protected int id;
}

// Good
public class BaseEntity
{
    private int _id;

    public int Id
    {
        get => _id;
        protected set => _id = value;
    }
}
```

## Rationale

Encapsulation is a cornerstone of object-oriented design, allows for controlled access and internal consistency. By encouraging private fields with public or protected accessors, this rule supports safer refactoring, validation logic, and maintainable class hierarchies.
