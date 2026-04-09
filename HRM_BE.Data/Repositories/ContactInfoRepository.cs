using AutoMapper;
using HRM_BE.Core.Data.ProfileEntity;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Profile.ContactInfo;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class ContactInfoRepository:RepositoryBase<ContactInfo,int>,IContactInfoRepository
    {
        private readonly IMapper _mapper;
        public ContactInfoRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ContactInfoDto> Create(CreateContactInfoRequest request)
        {
            var entity = _mapper.Map<ContactInfo>(request);
            var entityReturn = await CreateAsync(entity);

            return _mapper.Map<ContactInfoDto>(entityReturn);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ContactInfoDto> GetById(int id)
        {
           var contactInfo = await GetContactInfoAndCheckExsit(id);
            return _mapper.Map<ContactInfoDto>(contactInfo);
        }

        private async Task<ContactInfo> GetContactInfoAndCheckExsit(int id)
        {
            var entity = await _dbContext.ContactInfos.FindAsync(id);
            if (entity is null)
                throw new EntityNotFoundException(nameof(ContactInfo), $"Id = {id}");
            return entity;
        }

        public async Task Update(int id, UpdateContactInfoRequest request)
        {
            if (request is null)
                throw new BadHttpRequestException("Dữ liệu cập nhật không hợp lệ.");

            var entity = await GetContactInfoAndCheckExsit(id);

            var currentEmployeeId = entity.EmployeeId;
            _mapper.Map(request, entity);

            // Keep ownership stable to avoid accidental detach when payload omits EmployeeId.
            entity.EmployeeId = currentEmployeeId;

            await UpdateAsync(entity);
        }
    }

}
