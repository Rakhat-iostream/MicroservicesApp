using ShoppingWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWeb.ApiCollection.Interfaces
{
    public interface IUsersApi
    {
        Task<User> GetUserById(Guid id);
        Task<bool> UpdateUser(User user);
        Task<string> RegistrationToken(User user);
        Task<string> AuthentificationToken(User user);
    }
}
