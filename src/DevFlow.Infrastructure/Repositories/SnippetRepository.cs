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

        //Use .Include() when you KNOW you need related data
        //include is used with navigational properties. Navigation properties = C# properties that represent RELATIONSHIPS, NOT database columns
        // so when i use include it basically does a join
        public async Task<IEnumerable<CodeSnippet>> GetByUserIdAsync(int userId)
        {
            return await _context.CodeSnippets
                .Include(s => s.Owner)
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
        public async Task<IEnumerable<CodeSnippet>> SearchAsync(
            int userId,
            string? searchTerm,
            string? language,
            List<string>? tags)  // ← Add tags parameter
        {
            var query = _context.CodeSnippets
                .Include(s => s.Owner)
                .Where(s => s.OwnerId == userId);

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s =>
                    s.Title.Contains(searchTerm) ||
                    s.Code.Contains(searchTerm) ||
                    s.Tags.Contains(searchTerm));
            }

            // Filter by language
            if (!string.IsNullOrWhiteSpace(language))
            {
                query = query.Where(s => s.Language == language);
            }

            // Filter by tags (must contain ALL specified tags)
            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    query = query.Where(s => s.Tags.Contains(tag));
                }
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}