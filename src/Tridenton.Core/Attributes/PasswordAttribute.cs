namespace Tridenton.Core.Attributes;

public class PasswordAttribute : RegularExpressionAttribute
{
    public PasswordAttribute() : base("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
    {
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one special character and one digit";
    }
}