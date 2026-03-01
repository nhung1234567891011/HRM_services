using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Extension;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRM_BE.Data.Repositories
{
    public class RevenueCommissionPolicyRepository : RepositoryBase<RevenueCommissionPolicy, int>, IRevenueCommissionPolicyRepository
    {
        private readonly IMapper _mapper;

        public RevenueCommissionPolicyRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagingResult<RevenueCommissionPolicyDto>> Paging(PagingRevenueCommissionPolicyRequest request)
        {
            var query = _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true)
                .Include(p => p.Organization)
                .Include(p => p.Tiers)
                .AsQueryable();

            if (request.OrganizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == request.OrganizationId);
            }

            if (request.TargetType.HasValue)
            {
                query = query.Where(p => p.TargetType == request.TargetType.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(p => p.Status == request.Status.Value);
            }

            query = query.ApplySorting(request.SortBy, request.OrderBy);

            var total = await query.CountAsync();
            query = query.ApplyPaging(request.PageIndex, request.PageSize);

            var data = await _mapper.ProjectTo<RevenueCommissionPolicyDto>(query).ToListAsync();
            return new PagingResult<RevenueCommissionPolicyDto>(data, request.PageIndex, request.PageSize, request.SortBy, request.OrderBy, total);
        }

        public async Task<RevenueCommissionPolicyDto> GetById(int id)
        {
            var entity = await _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true && p.Id == id)
                .Include(p => p.Organization)
                .Include(p => p.Tiers.OrderBy(t => t.SortOrder))
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new EntityNotFoundException(nameof(RevenueCommissionPolicy), $"Id = {id}");
            }

            return _mapper.Map<RevenueCommissionPolicyDto>(entity);
        }

        public async Task<RevenueCommissionPolicyDto> Create(CreateRevenueCommissionPolicyRequest request)
        {
            ValidateTiers(request.Tiers);

            var policy = new RevenueCommissionPolicy
            {
                OrganizationId = request.OrganizationId,
                TargetType = request.TargetType,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                Status = request.Status
            };

            await CreateAsync(policy);

            var tiers = request.Tiers
                .OrderBy(t => t.SortOrder)
                .Select((t, idx) => new RevenueCommissionTier
                {
                    PolicyId = policy.Id,
                    FromAmount = t.FromAmount,
                    ToAmount = t.ToAmount,
                    RatePercent = t.RatePercent,
                    SortOrder = t.SortOrder > 0 ? t.SortOrder : (idx + 1)
                })
                .ToList();

            if (tiers.Any())
            {
                await _dbContext.RevenueCommissionTiers.AddRangeAsync(tiers);
                await _dbContext.SaveChangesAsync();
            }

            return await GetById(policy.Id);
        }

        public async Task Update(int id, UpdateRevenueCommissionPolicyRequest request)
        {
            ValidateTiers(request.Tiers);

            var policy = await _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true && p.Id == id)
                .Include(p => p.Tiers)
                .FirstOrDefaultAsync();

            if (policy == null)
            {
                throw new EntityNotFoundException(nameof(RevenueCommissionPolicy), $"Id = {id}");
            }

            policy.OrganizationId = request.OrganizationId;
            policy.TargetType = request.TargetType;
            policy.EffectiveFrom = request.EffectiveFrom;
            policy.EffectiveTo = request.EffectiveTo;
            policy.Status = request.Status;

            await UpdateAsync(policy);

            // Soft delete old tiers
            foreach (var tier in policy.Tiers)
            {
                tier.IsDeleted = true;
                tier.UpdatedAt = DateTime.Now;
            }
            await _dbContext.SaveChangesAsync();

            // Add new tiers
            var newTiers = request.Tiers
                .OrderBy(t => t.SortOrder)
                .Select((t, idx) => new RevenueCommissionTier
                {
                    PolicyId = policy.Id,
                    FromAmount = t.FromAmount,
                    ToAmount = t.ToAmount,
                    RatePercent = t.RatePercent,
                    SortOrder = t.SortOrder > 0 ? t.SortOrder : (idx + 1)
                })
                .ToList();

            if (newTiers.Any())
            {
                await _dbContext.RevenueCommissionTiers.AddRangeAsync(newTiers);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateStatus(int id, Status status)
        {
            var policy = await _dbContext.RevenueCommissionPolicies
                .FirstOrDefaultAsync(p => p.IsDeleted != true && p.Id == id);

            if (policy == null)
            {
                throw new EntityNotFoundException(nameof(RevenueCommissionPolicy), $"Id = {id}");
            }

            policy.Status = status;
            await UpdateAsync(policy);
        }

        public async Task Delete(int id)
        {
            var policy = await _dbContext.RevenueCommissionPolicies
                .Where(p => p.IsDeleted != true && p.Id == id)
                .Include(p => p.Tiers)
                .FirstOrDefaultAsync();

            if (policy == null)
            {
                throw new EntityNotFoundException(nameof(RevenueCommissionPolicy), $"Id = {id}");
            }

            policy.IsDeleted = true;
            foreach (var tier in policy.Tiers)
            {
                tier.IsDeleted = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateTiers(List<RevenueCommissionTierRequest> tiers)
        {
            if (tiers == null || tiers.Count == 0)
            {
                throw new Exception("Danh sách bậc hoa hồng không được để trống.");
            }

            foreach (var t in tiers)
            {
                if (t.FromAmount < 0) throw new Exception("FromAmount không được âm.");
                if (t.ToAmount.HasValue && t.ToAmount.Value <= t.FromAmount) throw new Exception("ToAmount phải lớn hơn FromAmount.");
                if (t.RatePercent < 0) throw new Exception("RatePercent không được âm.");
            }
        }
    }
}

