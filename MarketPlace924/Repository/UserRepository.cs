using MarketPlace924.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Repository
{
    class UserRepository
    {
        private DBConnection.DBConnection connection;
        public UserRepository(DBConnection.DBConnection connection)
        {
            this.connection = connection;
        }
        public void addUser(User user)
        {

        }
        public User getUser(string username) { 
            return new User();
        }
        public void updateUser(User user) { }
        public void deleteUser(string username) { }
        public User getUserByEmail(string email)
        {
            return new User();
        }
        public bool checkExistanceOfEmail(string email) {
            return true;
        }
        public bool checkExistanceOfUsername(string username) {
            return true;
        }

    }
}
