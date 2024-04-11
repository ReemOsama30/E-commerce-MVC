using E_commerce.Repository;
using E_commerce_MVC.Models;

namespace E_commerce_MVC.Repository
{
    public interface ICommentRepository : IRepository<Comments>
    {
        public List<Comments> GetAllIncludeUser(int id);
    }
}