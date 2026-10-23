using EduStream.Data;

namespace EduStream.Repositories
{
    public abstract class RepositorioBase
    {
        protected readonly EduStreamDbContext _context;

        protected RepositorioBase(EduStreamDbContext context)
        {
            _context = context;
        }
    }
}