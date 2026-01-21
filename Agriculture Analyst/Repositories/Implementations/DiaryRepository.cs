using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Agriculture_Analyst.Repositories.Implementations
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly AgricultureAnalystDbContext _context;

        public DiaryRepository(AgricultureAnalystDbContext context)
        {
            _context = context;
        }

        // (OPTIONAL) LẤY TẤT CẢ DIARY CỦA PLANT
        public async Task<List<DiaryEntry>> GetByPlantAsync(int plantId)
            => await _context.DiaryEntries
                .Where(d => d.PlantId == plantId)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();

        // ✅ METHOD BẮT BUỘC THEO INTERFACE
        public async Task<List<DiaryEntry>> GetByPlantAndDateAsync(
            int plantId,
            DateTime date)
        {
            return await _context.DiaryEntries
                .Where(d =>
                    d.PlantId == plantId &&
                    d.EntryDate.HasValue &&
                    d.EntryDate.Value.Date == date.Date)
                .OrderByDescending(d => d.EntryDate)
                .ToListAsync();
        }

        public async Task AddAsync(DiaryEntry diary)
        {
            _context.DiaryEntries.Add(diary);
            await _context.SaveChangesAsync();
        }
    }

}
