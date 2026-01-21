namespace Agriculture_Analyst.Repositories.Implementations;

using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

public class PlantRepository : IPlantRepository
{
    private readonly AgricultureAnalystDbContext _context;

    public PlantRepository(AgricultureAnalystDbContext context)
    {
        _context = context;
    }

    public async Task<List<Plant>> GetByUserIdAsync(int userId)
    {
        return await _context.Plants
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<Plant?> GetByIdAsync(int id)
    {
        return await _context.Plants
            .FirstOrDefaultAsync(p => p.PlantId == id);
    }

    public async Task AddAsync(Plant plant)
    {
        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Plant plant)
    {
        _context.Plants.Update(plant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Plant plant)
    {
        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();
    }
}




