using AutoMapper;
using GameStore.Application.DTOs.PublisherDtos;
using GameStore.Application.Helpers;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Newtonsoft.Json;

namespace GameStore.Application.Services.PublisherService;
public class PublisherService(
    IUnitOfWork unitOfWork,
    ISupplierRepository supplierRepository,
    IMapper mapper,
    EntityChangeLogger changeLogger) : IPublisherService
{
    public async Task<PublisherDto> AddPublisherAsync(AddPublisherDto publisherDto)
    {
        ValidateDto(publisherDto);

        var publisher = mapper.Map<Publisher>(publisherDto);
        await unitOfWork.Publishers.AddAsync(publisher);
        await unitOfWork.SaveAsync();

        changeLogger.LogEntityCreation(publisher.GetType().Name, publisher);
        return mapper.Map<PublisherDto>(publisher);
    }

    public async Task DeletePublisherAsync(string id)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            var publisher = await unitOfWork.Publishers.GetByIdAsync(guidId)
            ?? throw new EntityNotFoundException($"Publisher with Id {id} not found.");

            unitOfWork.Publishers.Delete(publisher);
            changeLogger.LogEntityDeletion(publisher.GetType().Name, publisher);
            await unitOfWork.SaveAsync();
        }
        else if (int.TryParse(id, out var intId))
        {
            var supplier = await supplierRepository.GetByIdAsync(intId) ??
                throw new EntityNotFoundException($"Category with Id {id} not found.");

            await supplierRepository.DeleteByIdAsync(intId);
            changeLogger.LogEntityDeletion(supplier.GetType().Name, supplier);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or int.", nameof(id));
        }
    }

    public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(bool includeDeleted = false)
    {
        var publishers = mapper.Map<IEnumerable<PublisherDto>>(await unitOfWork.Publishers.GetAllAsync(includeDeleted));
        var suppliers = mapper.Map<IEnumerable<PublisherDto>>(await supplierRepository.GetAllAsync(includeDeleted));

        var unitedResult = publishers.Concat(suppliers);

        return unitedResult;
    }

    public async Task<PublisherDto> GetPublisherByGameKeyAsync(string gameKey, bool includeDeleted = false)
    {
        var publisher = await unitOfWork.Publishers.GetByGameKeyAsync(gameKey, includeDeleted);

        if (publisher is null)
        {
            var supplier = await supplierRepository.GetByProductKeyAsync(gameKey, includeDeleted)
                ?? throw new EntityNotFoundException($"Publisher with game {gameKey} not found.");
            return mapper.Map<PublisherDto>(supplier);
        }
        else
        {
            return mapper.Map<PublisherDto>(publisher);
        }
    }

    public async Task<PublisherDto> GetPublisherByNameAsync(string companyName, bool includeDeleted = false)
    {
        var publisher = await unitOfWork.Publishers.GetByCompanyNameAsync(companyName, includeDeleted);

        if (publisher is null)
        {
            var supplier = await supplierRepository.GetByNameAsync(companyName, includeDeleted)
                ?? throw new EntityNotFoundException($"Publisher with name {companyName} not found.");
            return mapper.Map<PublisherDto>(supplier);
        }
        else
        {
            return mapper.Map<PublisherDto>(publisher);
        }
    }

    public async Task UpdatePublisherAsync(PublisherDto publisherDto, bool includeDeleted = false)
    {
        if (Guid.TryParse(publisherDto.Id, out var guidId))
        {
            await UpdatePublisher(publisherDto, guidId, includeDeleted);
        }
        else if (int.TryParse(publisherDto.Id, out var intId))
        {
            await UpdateSupplier(publisherDto, intId, includeDeleted);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID of genreDto is not a valid Guid or int.", nameof(publisherDto));
        }
    }

    private void ValidateDto(AddPublisherDto publisherDto)
    {
        if (unitOfWork.Publishers.NameExist(publisherDto.CompanyName))
        {
            throw new UniquePropertyException($"Publisher with company name {publisherDto.CompanyName} already exists.");
        }
    }

    private async Task UpdatePublisher(PublisherDto publisherDto, Guid guidId, bool includeDeleted = false)
    {
        var publisher = await unitOfWork.Publishers.GetByIdAsync(guidId, includeDeleted)
            ?? throw new EntityNotFoundException($"Publisher with Id {publisherDto.Id} not found.");

        if (unitOfWork.Publishers.NameExist(publisherDto.CompanyName)
            && !string.Equals(
                publisher.CompanyName,
                publisherDto.CompanyName,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UniquePropertyException($"Publisher with name {publisherDto.CompanyName} already exist");
        }

        var oldPublisher = JsonConvert.DeserializeObject<Publisher>(
            JsonConvert.SerializeObject(publisher, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

        mapper.Map(publisherDto, publisher);

        unitOfWork.Publishers.Update(publisher);
        await unitOfWork.SaveAsync();
        changeLogger.LogEntityChange(
            "UPDATE", publisher.GetType().Name, oldPublisher, publisher);
    }

    private async Task UpdateSupplier(PublisherDto publisherDto, int intId, bool includeDeleted = false)
    {
        var oldSupplier = (await supplierRepository.GetByIdAsync(intId, includeDeleted))
                ?? throw new EntityNotFoundException($"There is no supplier with Id {publisherDto.Id}");

        if (oldSupplier.CopiedToSql.HasValue && oldSupplier.CopiedToSql.Value)
        {
            var publisher = await unitOfWork.Publishers.GetBySupplierIdAsync(oldSupplier.SupplierID, includeDeleted);
            await UpdatePublisher(publisherDto, publisher.Id, includeDeleted);
        }
        else
        {
            var publisher = mapper.Map<Publisher>(oldSupplier);

            if (unitOfWork.Publishers.NameExist(publisherDto.CompanyName))
            {
                throw new UniquePropertyException($"Publisher with name {publisherDto.CompanyName} already exist");
            }

            publisher.CompanyName = publisherDto.CompanyName;
            publisher.Description = publisherDto.Description;
            publisher.HomePage = publisherDto.HomePage;

            await unitOfWork.Publishers.AddAsync(publisher);
            await unitOfWork.SaveAsync();
            await supplierRepository.MarkAsCopiedAsync(intId);
            changeLogger.LogNorthwindEntityChange(
                "UPDATE", oldSupplier.GetType().Name, oldSupplier, publisher);
        }
    }
}
