using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Agriculture_Analyst.Repositories.Implementations
{
    public class PlantTaskRepository : IPlantTaskRepository
    {
        private readonly AgricultureAnalystDbContext _context;

        public PlantTaskRepository(AgricultureAnalystDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlantTask>> GetByPlantAsync(int plantId, int userId, DateTime date)
        {
            return await _context.PlantTasks
                .Where(t =>
                    t.PlantId == plantId &&
                    t.UserId == userId &&
                    !t.IsDeleted &&
                    t.NextDate.Date <= date.Date
                )
                .OrderBy(t => t.NextDate)
                .ToListAsync();
        }

        public async Task<PlantTask?> GetByIdAsync(int taskId)
        {
            return await _context.PlantTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && !t.IsDeleted);
        }

        public async Task AddAsync(PlantTask task)
        {
            _context.PlantTasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlantTask task)
        {
            _context.PlantTasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }

}
