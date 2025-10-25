# Line Length Constraints : CC0003

Limiting the maximum line length helps to improve diffability, reduces horizontal scrolling, and encourages modular thinking. Long lines often indicate overly complex expressions or deeply nested logic, which are harder to read, test, and maintain.

By enforcing a line length constraint, this rule promotes decomposition and better separation of concerns, making it easier to manage and extend the code over time.

## Example
```csharp
// Bad: Line is too long (exceeds the typical 100-120 character limit)
public string GenerateFormattedString(string name, string address, string phoneNumber, string email, string dateOfBirth, string membershipType) =>
    $"Name: {name}, Address: {address}, Phone: {phoneNumber}, Email: {email}, DOB: {dateOfBirth}, Membership Type: {membershipType}";

// Good: Break the logic into smaller components for clarity
public string GenerateFormattedString(string name, string address, string phoneNumber, string email, string dateOfBirth, string membershipType)
{
    var formattedAddress = $"Address: {address}";
    var formattedPhone = $"Phone: {phoneNumber}";
    var formattedEmail = $"Email: {email}";
    var formattedDob = $"DOB: {dateOfBirth}";
    var formattedMembershipType = $"Membership Type: {membershipType}";

    return $"Name: {name}, {formattedAddress}, {formattedPhone}, {formattedEmail}, {formattedDob}, {formattedMembershipType}";
}
```

## Rationale

Side-by-side diffing becomes easier when lines are shorter. Changes made to a single line (or small set of lines) are easier to identify in diffs when they don't span across long, horizontally-scrolling lines.

Readability improves because each line can be viewed in its entirety without the need for horizontal scrolling, which is particularly important for mobile devices, smaller screens, or split-screen setups during code reviews.

Maintainability is enhanced because shorter lines often indicate simpler, more focused methods or logic, improving the ease of testing and debugging.

By splitting long lines into smaller ones, you often naturally break up complex expressions or nested logic, which helps to clarify intent.

## Configuration

This rule can be configured to allow a line length base on a teams preferences and coding standards.

```
# Maximum line length for CC0003
# Default: 120 characters
# Adjust this value based on your team's preferences (common values: 80, 100, 120, 140)
dotnet_diagnostic.CC0003.max_line_length = 120
```
