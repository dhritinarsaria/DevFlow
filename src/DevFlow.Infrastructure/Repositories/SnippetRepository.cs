using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Data;

namespace DevFlow.Infrastructure.Repositories
{
    public class SnippetRepository : ISnippetRepository
    {
        private readonly DevFlowDbContext _context;

        public SnippetRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task<CodeSnippet?> GetByIdAsync(int id)
        {
            return await _context.CodeSnippets
                .Include(s => s.Owner)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<CodeSnippet>> GetByUserIdAsync(int userId)
        {
            return await _context.CodeSnippets
                .Where(s => s.OwnerId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CodeSnippet> AddAsync(CodeSnippet snippet)
        {
            snippet.CreatedAt = DateTime.UtcNow;
            await _context.CodeSnippets.AddAsync(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public async Task UpdateAsync(CodeSnippet snippet)
        {
            snippet.UpdatedAt = DateTime.UtcNow;
            _context.CodeSnippets.Update(snippet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var snippet = await _context.CodeSnippets.FindAsync(id);
            if (snippet != null)
            {
                _context.CodeSnippets.Remove(snippet);
                await _context.SaveChangesAsync();
            }
        }
    }
}