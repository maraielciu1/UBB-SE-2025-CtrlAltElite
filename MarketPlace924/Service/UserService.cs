using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    class UserService
    {
        private UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void addUser(string username, string password, string email, int role)
        {

        }
        public User GetUser(string username)
        {
            return new User();
        }
        public bool validateLogin(string email, string password)

        {
            
            if(_userRepository.checkExistanceOfEmail(email))
            {
                User? user = getUserByEmail(email);
                if (user.Password == hashPassword(password))
                    return true;
                else
                    return false;
                
            }
            else
            {
                return false;
            }
        }

        public void UpdateUserFailedLogins(User user, int NewValueOfFailedLogIns)
        {
            _userRepository.updateUserFailedLogins(user, NewValueOfFailedLogIns);
        }
        public string hashPassword(string password) {
            return password;
        }
        public User? getUserByEmail(string email) {
            return _userRepository.GetUserByEmail(email);
        }

        public int getFaildLogInsOfUserByEmail(string email)
        {
            User user= this.getUserByEmail(email);
            int userId = user.UserID;
            return _userRepository.getFaildLogInsOfUserByUserID(userId);
        }

        public bool checkExistanceOfEmail(string email)
        {
            return _userRepository.checkExistanceOfEmail(email);
        }

        public bool canUserLogInNow(string email)
        {
            User user = getUserByEmail(email);

            // If `BannedUntil` is set and the ban time is in the future, user is still banned
            if (user.BannedUntil.HasValue && user.BannedUntil > DateTime.Now)
            {
                return false; // ❌ User CANNOT log in
            }

            return true; // ✅ User CAN log in
        }


        public void BanUserTemporary(string email, int seconds)
        {
            User user = getUserByEmail(email);
            user.BannedUntil = DateTime.Now.AddSeconds(seconds);
            _userRepository.updateUser(user);
        }

    }
}
