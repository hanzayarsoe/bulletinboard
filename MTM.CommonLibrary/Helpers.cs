using System.Text.RegularExpressions;

namespace MTM.CommonLibrary
{
    public static class Helpers
    {
		// Password Manager
		#region Password
		public static string HashPassword(string Password)
		{
			return BCrypt.Net.BCrypt.HashPassword(Password);
		}

		public static bool VerifyPassword(string Password, string PasswordFromDB)
		{
			return BCrypt.Net.BCrypt.Verify(Password, PasswordFromDB);
		}

		public static bool IsPasswordValid(string password)
		{
			if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
				return false;

			// Regex to enforce at least one lowercase letter, one uppercase letter, one digit, and one special character
			var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

			return passwordRegex.IsMatch(password);
		}

        #endregion

        #region GetRole
        public static int? GetRoleValue(string roleName)
        {
            switch (roleName?.ToLower())
            {
                case "admin":
                    return 1;
                case "user":
                    return 2;
                default:
                    return null;
            }
        }
        #endregion

        #region GetUniqueFileName
        public static string GetUniqueFileName(string filePath, string uploads)
        {
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string newFullPath = filePath;

            while (System.IO.File.Exists(newFullPath))
            {
                string tempFileName = $"{fileNameOnly} ({count++})";
                newFullPath = Path.Combine(uploads, tempFileName + extension);
            }

            return newFullPath;
        }
        #endregion
    }
}
