using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IAPIAuditService
    {
        /// <summary>
        /// Inserts a new APIAudit and its related entities.
        /// </summary>
        Task<bool> InsertAPIAudit(APIAuditModel model);
    }
}
