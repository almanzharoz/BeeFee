namespace BeeFee.LoginApp.Services
{
    public enum UserRegistrationResult
    {
        Ok = 0,
        EmailIsEmpty = 1,
        WrongEmail = 2,
        NameIsEmpty = 3,
        PasswordIsEmpty = 4,
        EmailAlreadyExists = 5,
        UnknownError = 6
    }
}
