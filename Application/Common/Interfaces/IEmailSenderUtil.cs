using CoreLib.Infrastructure.Persistence;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IEmailSenderUtil
    {
        void SendEmail(string brandName, IList<string> emailRecepients, IList<AttachmentDto> attachmentList, string emailSubject, string emailContent, bool isHtml, string logoPath = "");
    }
}
