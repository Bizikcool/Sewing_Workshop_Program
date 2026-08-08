using SewingAPI.Domain.Entities;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Exceptions;
using SewingAPI.Domain.Interfaces.Repositories;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.Validation;

namespace SewingAPI.Service.Services
{
    public class ContragentService : IContragentService
    {
        private readonly IContragentRepository _contragents;

        public ContragentService(IContragentRepository contragents)
        {
            _contragents = contragents;
        }

        public Task<(IReadOnlyList<Contragent> Items, int Total)> GetPagedAsync(int page, int pageSize, string? search, ContragentType? type)
        {
            return _contragents.GetPagedAsync(NormalizePage(page), NormalizePageSize(pageSize), search, type);
        }

        public async Task<Contragent> GetByIdAsync(int id)
        {
            return await _contragents.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(nameof(Contragent), id);
        }

        public Task<Contragent> CreateAsync(Contragent contragent)
        {
            ContragentValidator.Validate(contragent);
            contragent.CreatedAt = DateTime.UtcNow;
            return _contragents.AddAsync(contragent);
        }

        public async Task<Contragent> UpdateAsync(int id, Contragent contragent)
        {
            ContragentValidator.Validate(contragent);

            var existing = await GetByIdAsync(id);
            existing.Type = contragent.Type;
            existing.Name = contragent.Name;
            existing.Contact = contragent.Contact;
            existing.Phone = contragent.Phone;

            return await _contragents.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _contragents.DeleteAsync(id);
            if (!deleted)
            {
                throw new NotFoundException(nameof(Contragent), id);
            }

            return true;
        }

        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        private static int NormalizePageSize(int pageSize) => pageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => pageSize
        };
    }
}
