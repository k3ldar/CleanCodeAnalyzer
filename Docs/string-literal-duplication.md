# String Literal Duplication : CC0010

Duplicated string literals scattered throughout code create maintenance hazards and increase the risk of inconsistency. When the same string appears in multiple locations, changing it requires finding and updating every occurrence—a process prone to human error. Extracting repeated strings to well-named constants centralizes the value, clarifies intent, and makes future refactoring safer.

## Example

```csharp
// Bad: Duplicated string literal creates maintenance burden 

public class UserService 
{ 
    public void ValidateEmail(string email) 
    { 
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email address is required");
    }

    public void SendWelcomeEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email address is required"); // Duplicate!
    
        // ... email sending logic
    }

    public void UpdateEmail(string oldEmail, string newEmail)
    {
        if (string.IsNullOrEmpty(newEmail))
            throw new ArgumentException("Email address is required"); // Duplicate!
    
        // ... update logic
    }
}

// Good: Extract to named constant 

public class UserService
{
    private const string EmailRequiredMessage = "Email address is required";

    public void ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException(EmailRequiredMessage);
    }

    public void SendWelcomeEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException(EmailRequiredMessage);
    
        // ... email sending logic
    }

    public void UpdateEmail(string oldEmail, string newEmail)
    {
        if (string.IsNullOrEmpty(newEmail))
            throw new ArgumentException(EmailRequiredMessage);
    
        // ... update logic
    }
}
```

## Rationale

- Centralized Updates: When a string value needs to change (e.g., rewording an error message, updating an API endpoint), having a single constant means one update point instead of hunting through multiple files.
- Consistency: Duplicated strings invite typos and subtle variations. A constant guarantees all usages are identical.
- Semantic Meaning: A well-named constant like `DefaultConnectionTimeout` or `ValidationFailedMessage` communicates purpose better than a raw string literal like `"30"` or `"Validation failed"`.
- Improved Searchability: Finding all usages of a named constant is straightforward with IDE navigation, whereas searching for string literals can yield false positives from comments, test data, or unrelated strings.
- Reduced Cognitive Load: Readers immediately understand the intent of `MaxRetryCount` without mentally parsing the literal value `"3"` in context.

## Detection Criteria

This rule reports string literals that:
- Appear 2 or more times in the same file
- Are at least 5 characters long (to filter trivial strings like `"OK"` or `"yes"`)
- Contain at least one letter (to skip punctuation-only strings)
- Are not already part of a `const` declaration

## Exceptions (Not Reported)

- Empty strings (`""`)
- Short strings (< 5 characters): `"OK"`, `"yes"`, `"no"`
- Single-character strings: `"x"`, `"y"`
- Strings already in const declarations: Already intentional
- Attribute arguments: Small declarative metadata
- Interpolated strings: Handled separately due to dynamic content
