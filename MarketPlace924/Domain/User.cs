using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Domain
{
    public class User
    {
        private int _userID, _role, _failedLogIns;
        private string _username, _password, _email, _phoneNumber;
        private DateTime? _bannedUntil;
        private bool _isBanned;

        public User() { }
        public User(int userID=0, string username, string email, string password, int role, int failedLogIns=0, DateTime? bannedUntil=null)
        {
            _bannedUntil = bannedUntil;
            _username = username;
            _email = email;
            _password = password;
            _role = role;
            _failedLogIns = failedLogIns;
            _username = username;
            _userID = userID;

        }

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string Email
        {
            get => _email;
            set => _email = value;
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value;
        }

        public DateTime? BannedUntil
        {
            get => _bannedUntil;
            set => _bannedUntil = value;
        }
        public bool IsBanned
        {
            get => _isBanned;
            set => _isBanned = value;

        }

        public int UserID
        {
            get => _userID;
            set => _userID = value;
        }
        public int Role
        {
            get => _role;
            set => _role = value;
        }

        public int FailedLogIns
        {
            get => _failedLogIns;
            set => _failedLogIns = value;
        }

    }
}
