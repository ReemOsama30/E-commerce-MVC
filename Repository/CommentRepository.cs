using E_commerce.Models;
using E_commerce.Repository;
using E_commerce_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_MVC.Repository
{
    public class CommentRepository : Repository<Comments>, ICommentRepository
    {

        public CommentRepository(Context context) : base(context)
        {

        }

        public List<Comments> GetAllIncludeUser(int id)
        {
            return context.Comments.Include(c => c.applicationUser)
                 .Where(c => c.ProductId == id && !c.IsDeleted)
                .ToList();
        }

    }
}
