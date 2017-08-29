using System.Collections.Generic;
using System.Linq;
using SimpleCRUD2.Data.Interfaces;
using SimpleCRUD2.Data.Models;
using SimpleCRUD2.Interfaces;
using SimpleCRUD2.Models;
using SimpleCRUD2.Models.ViewModels.HomeViewModels;

namespace SimpleCRUD2.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IContext context;

        public UserRepository(IContext context)
        {
            this.context = context;
        }

        // Users methods
        #region
        public int UsersCount
        {
            get
            {
                return this.context.Users.Count();
            }
        }

        public IEnumerable<UserModel> GetUsersList()
        {
            //сам знаешь что тут не так :)
            var users = this.context.Users.Select(_ => new UserModel()
            {
                UserId = _.UserId,
                Name = _.Name,
                Surname = _.Surname,
                Email = _.Email,
                Location = _.Location,
                Birthday = _.Birthday
            }).ToList();

            return users;
        }

        public IEnumerable<UserModel> GetStudentsUsersList()
        {
            var adminRole = this.context.Roles.Single(_ => _.Name.Equals("admin"));
            var moderRole = this.context.Roles.Single(_ => _.Name.Equals("moder"));

            var users = this.context.Users.Where(_ => _.Roles.Count.Equals(1)).Select(_ => new UserModel()
            {
                UserId = _.UserId,
                Name = _.Name,
                Surname = _.Surname,
                Email = _.Email,
                Location = _.Location,
                Birthday = _.Birthday
            }).ToList();

            return users;
        }

        public IEnumerable<UserModel> GetUsersListForPage(int pageNumber, int pageSize)
        {
            IEnumerable<UserModel> users = this.context.Users.Select(_ => new UserModel()
            {
                UserId = _.UserId,
                Name = _.Name,
                Surname = _.Surname,
                Email = _.Email,
                Location = _.Location,
                Birthday = _.Birthday
            }).ToList().Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return users;
        }

        public void EditUserInfo(EditUserViewModel editUserViewModel)
        {
            var user = this.context.Users.Where(_ => _.UserId == editUserViewModel.UserId).FirstOrDefault();

            user.Name = editUserViewModel.Name;
            user.Surname = editUserViewModel.Surname;
            user.Location = editUserViewModel.Location;
            user.Birthday = editUserViewModel.Birthday;

            this.context.SaveChanges();
        }

        public bool AddUser(UserModel userModel)
        {
            var user = this.context.Users.Where(_ => _.Email == userModel.Email).FirstOrDefault();

            if (user == null)
            {
                var newUser = new User()
                {
                    UserId = userModel.UserId,
                    Email = userModel.Email,
                    Password = "temp",
                    Name = userModel.Name,
                    Surname = userModel.Surname,
                    Location = userModel.Location,
                    Birthday = userModel.Birthday,
                };

                newUser.Roles.Add(this.context.Roles.First(_ => _.Name == "user"));

                this.context.Users.Add(newUser);
                this.context.SaveChanges();

                return true;
            }

            return false;
        }

        public UserModel GetUserByEmail(string email)
        {
            var user = this.context.Users.Where(_ => _.Email == email).FirstOrDefault();

            if (user != null)
            {
                var userModel = new UserModel(user);
                return userModel;
            }

            return null;
        }

        public UserModel GetUserById(int id)
        {
            var user = this.context.Users.Where(_ => _.UserId == id).FirstOrDefault();

            if (user != null)
            {
                var userModel = new UserModel(user);
                return userModel;
            }

            return null;
        }

        public void DeleteUserById(int id)
        {
            var user = this.context.Users.Where(_ => _.UserId == id).FirstOrDefault();

            if (user != null)
            {
                this.context.Users.Remove(user);
                this.context.SaveChanges();
            }
        }

        public bool ValidateUser(string email, string password)
        {
            var user = this.context.Users.Where(_ => _.Email == email && _.Password == password).FirstOrDefault();

            if (user != null)
            {
                return true;
            }

            return false;
        }

        public bool IsRegistred(string email)
        {
            var user = this.context.Users.Where(_ => _.Email == email).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        // Roles methods
        #region
        public ICollection<Role> GetRolesForUser(string email)
        {
            var user = this.context.Users.FirstOrDefault(_ => _.Email == email);

            if (user != null)
            {
                return user.Roles.ToList();
            }

            return new List<Role>();
        }

        public void CreateRole(string roleName)
        {
            Role newRole = new Role() { Name = roleName };

            this.context.Roles.Add(newRole);
            this.context.SaveChanges();
        }

        public bool IsUserInRole(string email, string roleName)
        {
            var user = this.context.Users.FirstOrDefault(_ => _.Email == email);
            var role = this.context.Roles.FirstOrDefault(_ => _.Name == roleName);

            if (user != null && role != null && user.Roles.Contains(role))
            {
                return true;
            }

            return false;
        }

        public void RemoveUsersFromRoles(string[] emails, string[] roleNames)
        {
            var users = new List<User>();
            var roles = new List<Role>();

            foreach (var email in emails)
            {
                var user = this.context.Users.Where(_ => _.Email == email).FirstOrDefault();

                foreach (var roleName in roleNames)
                {
                    var role = this.context.Roles.Where(_ => _.Name == roleName).FirstOrDefault();

                    if (user != null && role != null && this.IsUserInRole(email, roleName))
                    {
                        user.Roles.Remove(role);
                        this.context.SaveChanges();
                    }
                }
            }
        }

        public void AddUsersToRoles(string[] emails, string[] roleNames)
        {
            var users = new List<User>();
            var roles = new List<Role>();

            foreach (var email in emails)
            {
                var user = this.context.Users.Where(_ => _.Email == email).FirstOrDefault();

                foreach (var roleName in roleNames)
                {
                    var role = this.context.Roles.Where(_ => _.Name == roleName).FirstOrDefault();

                    if (user != null && role != null && !this.IsUserInRole(email, roleName))
                    {
                        user.Roles.Add(role);
                        this.context.SaveChanges();
                    }
                }
            }
        }

        public void RemoveUserFromAllRoles(int id)
        {
            var user = this.context.Users.FirstOrDefault(_ => _.UserId == id);

            if (user != null)
            {
                user.Roles.Clear();
                this.context.SaveChanges();
            }
        }
        #endregion
    }
}
