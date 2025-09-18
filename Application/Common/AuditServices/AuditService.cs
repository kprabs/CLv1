using AutoMapper; // AutoMapper namespace
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Entities;
using Microsoft.Extensions.Logging; // For APIAudit and related entities

namespace CoreLib.Application.Common.AuditServices
{
    public class APIAuditService(IGroupPortalRepository groupPortalRepository, IMapper mapper, ILogger logger) : IAPIAuditService
    {     
        /// <summary>
        /// Inserts a new APIAudit and its related entities using the repository.
        /// </summary>
        public async Task<bool> InsertAPIAudit(APIAuditModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                // Map the model to the entity using AutoMapper
                var apiAuditEntity = mapper.Map<APIAudit>(model);

                // Use the repository to add the entity
                await groupPortalRepository.SetAPIAudit(apiAuditEntity);

                return true; // Return success
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting APIAudit");
                return false; // Return failure
            }
        }
    }
}