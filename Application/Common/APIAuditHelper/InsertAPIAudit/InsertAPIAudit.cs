using AutoMapper;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreLib.Application.Common.APIAuditHelper.InsertAPIAudit
{
    public class InsertApiAudit : IInsertApiAudit
    {
        private readonly ILogger _logger;
        private readonly IGroupPortalRepository _groupPortalRepository;
        private readonly IMapper _mapper; // AutoMapper instance

        public InsertApiAudit(ILogger logger, IGroupPortalRepository groupPortalRepository, IMapper mapper)
        {
            _logger = logger;
            _groupPortalRepository = groupPortalRepository;
            _mapper = mapper;
        }

        public async void InsertAuditInformation(APIAuditModel APIAuditModelRequest)
        {
            if (APIAuditModelRequest == null)
            {
                _logger.LogWarning("InsertAuditInformation called with null APIAuditModelRequest.");
                return;
            }

            _logger.LogInformation($"InsertAuditInformation started. {JsonConvert.SerializeObject(APIAuditModelRequest)}");

            try
            {
                _logger.LogInformation("Mapping APIAuditModel to APIAudit entity.");
                var apiAuditEntity = _mapper.Map<APIAudit>(APIAuditModelRequest);

                if (apiAuditEntity == null)
                {
                    _logger.LogError("Mapping APIAuditModel to APIAudit entity resulted in null.");
                    return;
                }

                _logger.LogInformation($"Mapped APIAudit entity: {JsonConvert.SerializeObject(apiAuditEntity)}");

                _logger.LogInformation("Calling SetAPIAudit on repository.");
                await _groupPortalRepository.SetAPIAudit(apiAuditEntity);
                _logger.LogInformation("SetAPIAudit completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in InsertAuditInformation.");
                throw;
            }
            finally
            {
                _logger.LogInformation("InsertAuditInformation ended.");
            }
        }
    }
}
