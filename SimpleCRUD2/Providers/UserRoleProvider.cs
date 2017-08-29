using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SimpleCRUD2.Interfaces;

namespace SimpleCRUD2.Providers
{
    [HandleError(ExceptionType = typeof(Exception), View = "Error")]
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                //лучше уж захардкодить в таких случаях что нибудь
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            repository.AddUsersToRoles(usernames, roleNames);
        }

        public override void CreateRole(string roleName)
        {
            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            repository.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string email)
        {
            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            var roles = repository.GetRolesForUser(email);

            if (roles.Count != 0)
            {
                return roles.Select(_ => _.Name).ToArray();
            }

            return new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string email, string roleName)
        {
            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            return repository.IsUserInRole(email, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var repository = DependencyResolver.Current.GetService<IUserRepository>();

            repository.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
