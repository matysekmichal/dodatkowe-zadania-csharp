﻿using System.Threading.Tasks;
using LegacyFighter.Dietary.DAL;
using Microsoft.EntityFrameworkCore;

namespace LegacyFighter.Dietary.Models
{
    public interface ITaxRuleRepository
    {
        Task<TaxRule> FindByIdAsync(long id);
        Task SaveAsync(TaxRule taxRule);
        Task DeleteAsync(TaxRule taxRule);
    }

    public class TaxRuleRepository : ITaxRuleRepository
    {
        private readonly DietaryDbContext _dbContext;
        private readonly DbSet<TaxRule> _taxRules;

        public TaxRuleRepository(DietaryDbContext dbContext)
        {
            _dbContext = dbContext;
            _taxRules = dbContext.TaxRules;
        }

        public Task<TaxRule> FindByIdAsync(long id)
            => _taxRules
                .Include(x => x.TaxConfig)
                .ThenInclude(x => x.TaxRules)
                .SingleOrDefaultAsync(x => x.Id == id);

        public Task SaveAsync(TaxRule taxRule) => _dbContext.UpsertAsync(taxRule);

        public Task DeleteAsync(TaxRule taxRule) => _dbContext.DeleteAsync(taxRule);
    }
}