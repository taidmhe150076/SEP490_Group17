using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryUser
    {
        public List<SystemUser> getAllUser();
        public List<SystemUser> getAllResearchByDepartmentId(int id);
        public SystemUser getByEmailAndPassword(string email , string password);
        public SystemUser? getUserById(int id);
        public void updateUser(SystemUser user);
        public SystemUser getUserById(int? id);
        public void addUser(SystemUser user);

        public void updatePassword (int userId, string password);

        public void  updateUserAvatar(int id , string image);
    }
}
