using Microsoft.EntityFrameworkCore;
using Projekt001.Data;
using Projekt001.Entities;

namespace Projekt001.Repositories
{
    public class ContactFormRepository : IContactFormRepository
    {
        private readonly AppDbContext _context;

        public ContactFormRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ContactEntity contactEntity)
        {
            await _context.ContactMessages.AddAsync(contactEntity);
        }

        public async Task<List<ContactEntity>> GetAllAsync()
        {
            return await _context.ContactMessages.OrderByDescending(c => c.SendDate).ToListAsync();
        }

        public async Task<ContactEntity?> GetByIdAsync(int id)
        {
            return await _context.ContactMessages.FirstOrDefaultAsync(x => x.ContactId == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}