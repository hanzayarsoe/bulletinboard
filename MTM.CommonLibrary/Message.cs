﻿namespace MTM.CommonLibrary
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
        public const string NOT_EXIST = "{0} was not exist or deleted.";
        public const string NOT_MATCH = "{0} does not match";
        public const string PASSWORD_FORMAT_ERROR = "Password must be at least one lowercase letter, one uppercase letter, one digit, and one special character";
        public const string INCORRECT = "Incorrect {0} OR {1}";
        public const string INCORRECT_SINGLE = "Incorrect {0}";
        public const string ACCOUNT_ERROR = "{0} was {1}";
        public const string EMAIL_FAIL = "Your update email is already used by other user";
        public const string NOT_SELECTED = "File is not selected";
        public const string INVALID_FORMAT = "Invalid file format";
        public const string NOT_FOUND = "{0} was not found";
        public const string REQUIRED_NAME = "Row {0} : First Name is required";
        public const string EMAIL_EXIST = "Row {0} : Email {1} already exists";
        public const string DATE_ERROR = "Row {0} : Invalid date of birth format. Use d/M/yyyy format";
        public const string ERROR_OCCURED = "An error occurred while processing the file. Details: {0}";
        public const string CREATE_SUCCESS = "{0} have been created successfully.";
        public const string CORRESPONSE_ERROR = "Error at row corresponding to {0}: {1}";
        #endregion
    }
}
