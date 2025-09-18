using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IInsertApiAudit
    {
        void InsertAuditInformation(APIAuditModel APIAuditModelRequest);
    }
}
