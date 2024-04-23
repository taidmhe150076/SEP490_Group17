using BusinessLogic.IRepository;
using DataAccess.Constants;
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
                users = _context.SystemUsers.Include(x => x.DepartmentldNavigation).Include(x => x.RoleldNavigation).ToList(); ;
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


        public void updatePassword(int userId, string password)
        {
            try
            {
                var user = _context.SystemUsers.Find(userId);

                if (user != null)
                {
                    user.Password = password;
                    _context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException($"User with ID '{userId}' not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating password : " + ex.Message);
            }
        }


        public void updateUserAvatar(int id, string image)
        {
            try
            {
                var user = _context.SystemUsers.Find(id);

                if (user != null)
                {
                    user.ImageUrl = image;
                    _context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException($"User with ID '{id}' not found.");
                }
            }catch(Exception ex)
            {
                throw new Exception ("Error updating avartar : " + ex.Message);
            }
        }

        public SystemUser? getUserById(int id)
        {
            try
            {
                var checkExits = _context.SystemUsers.Include(x => x.DepartmentldNavigation).Include(x => x.RoleldNavigation).FirstOrDefault(x => x.Id == id);
                if (checkExits == null)
                {
                    return null;
                }
                return checkExits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SystemUser> getAllResearchByDepartmentId(int id)
        {
            try
            {
                return _context.SystemUsers.Where(x => x.Departmentld == id && x.Roleld == COTSEConstants.RESEARCHER_ROLE).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
