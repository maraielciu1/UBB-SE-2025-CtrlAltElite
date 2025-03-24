using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    internal class UserValidator
    {
        public static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
            {
                return false;
            }
            return true;
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return false;
            }
            return true;
        }

        public static bool ValidatePhone(string phoneNo)
        {
            if (string.IsNullOrWhiteSpace(phoneNo))
            {
                return false;
            }

            string phonePatter = @"\+40\d{9}";
            if (!Regex.IsMatch(phoneNo, phonePatter))
            {
                return false;
            }
            return true;
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{8,}$";
            if (!Regex.IsMatch(password, passwordPattern))
            {
                return false;
            }
            return true;
        }
    }
}
