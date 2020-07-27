using arx.Extract.Data.Repository;
using arx.Extract.Types;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.API.Services
{
    public interface IPublicationsService
    {
        Task<List<PublicationItem>> GetBySubjectCodeByUpdatedDates(string subjectCode, DateTime updatedFromDate, DateTime updatedToDate);
        Task<List<PublicationItem>> GetByFulfilmentId(string fulfilmentId);
    }
    public class PublicationsService : IPublicationsService
    {
        private readonly IPublicationRepository _publicationRepo;
        private readonly IMapper _mapper;

        public PublicationsService(IPublicationRepository repo, IMapper mapper)
        {
            _publicationRepo = repo;
            _mapper = mapper;
        }

        public async Task<List<PublicationItem>> GetBySubjectCodeByUpdatedDates(string subjectCode, DateTime updatedFromDate, DateTime updatedToDate)
        {
            try
            {
                var result = await _publicationRepo.GetSubjectInclusiveBetweenDates(subjectCode, updatedFromDate, updatedToDate);

                if (result.Count > 0)
                {
                    return _mapper.Map<List<PublicationItem>>(result);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return await Task.FromResult(new List<PublicationItem>());
        }

        public async Task<List<PublicationItem>> GetByFulfilmentId(string fulfilmentId)
        {
            try
            {
                var result = await _publicationRepo.GetByFulfilmentId(fulfilmentId);

                if (result.Count > 0)
                {
                    return _mapper.Map<List<PublicationItem>>(result);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return await Task.FromResult(new List<PublicationItem>());
        }
    }
}
