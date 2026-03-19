namespace Agriculture_Analyst.Repositories.Implementations;

using System;
using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.ViewModel;
using Agriculture_Analyst.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public PlantReportViewModel GetPlantReport(int plantId)
    {
        // 1. Lấy thông tin cây
        var plant = _context.Plants.FirstOrDefault(p => p.PlantId == plantId);

        if (plant == null) return null; // Không tìm thấy

        // 2. Lấy vật tư đã xuất cho cây này
        var usedMaterials = _context.InventoryTransactions
            .Include(t => t.Item)
            .Where(t => t.PlantId == plantId && t.Type == 2) // Type 2 là Xuất
            .OrderBy(t => t.NgayGiaoDich)
            .ToList();

        // 3. Lấy nhật ký
        var diaries = _context.DiaryEntries
            .Where(d => d.PlantId == plantId)
            .OrderBy(d => d.EntryDate)
            .ToList();

        // Đóng gói trả về ViewModel
        return new PlantReportViewModel
        {
            Plant = plant,
            UsedMaterials = usedMaterials,
            TotalMaterialCost = usedMaterials.Sum(t => t.ThanhTien),
            DiaryEntries = diaries
        };
    }
}




