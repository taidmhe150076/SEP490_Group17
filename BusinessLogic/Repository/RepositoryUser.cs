using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryUser : IRepositoryUser
    {
        private readonly Sep490G17DbContext _context;

        public RepositoryUser(Sep490G17DbContext context)
        {
            _context = context;
        }

        public SystemUser getUserById(int? id)
        {
            SystemUser user = new SystemUser();
            try
            {
                user = _context.SystemUsers.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }
        public void addUser(SystemUser user)
        {
            try
            {
                _context.SystemUsers.Add(user);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       public List<SystemUser> getAllUser()
        {
            List<SystemUser> users = new List<SystemUser>();
            try
            {
                users = _context.SystemUsers.Include(x => x.DepartmentldNavigation).Include(x => x.RoleldNavigation).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }

        public SystemUser getByEmailAndPassword(string email, string password)
        {
            SystemUser user = new SystemUser();
            try
            {
                user = _context.SystemUsers.FirstOrDefault(x => x.Email == email && x.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public void updateUser(SystemUser user)
        {
            try
            {
                _context.SystemUsers.Update(user);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
