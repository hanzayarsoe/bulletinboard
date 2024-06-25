namespace MTM.CommonLibrary
{
    public enum AlertType
    {
        Success,
        Error,
        Warning,
        Danger
    }

    public static class Message
    {
        public const int SUCCESS = 1;
        public const int FAILURE = 2;
        public const int EXIST = 3;

        #region Message
        public const string SAVE_SUCCESS = "{0} is successfully {1}.";
        public const string FAIL = "{0} is failed";
        #endregion

        #region Account Message
        public const string ALREADY_EXIST = "{0} is already created.";
        public const string NOT_EXIST = "{0} was Not Exist or Deleted.";
        public const string NOT_MATCH = "{0} does not match";
        public const string PASSWORD_FORMAT_ERROR = "Password must be at least one lowercase letter, one uppercase letter, one digit, and one special character";
        public const string INCORRECT = "Incorrect {0} OR {1}";
        public const string INCORRECT_SINGLE = "Incorrect {0}";
        public const string ACCOUNT_ERROR = "{0} was {1}";
        public const string EMAIL_FAIL = "Your update email is already used by other user";
        #endregion
    }
}
