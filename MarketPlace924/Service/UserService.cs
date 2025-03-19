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
        public bool validateLogin(string username, string password)

        {
            
            if(_userRepository.checkExistanceOfUsername(username))
            {
                User? user = getUserByEmail(username);
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
        public string hashPassword(string password) {
            return password;
        }
        public User? getUserByEmail(string email) {
            return _userRepository.GetUserByEmail(email);
        }
    }
}
