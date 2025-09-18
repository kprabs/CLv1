using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using System.Net.Mail;
using System.Net.Mime;

namespace CoreLib.Infrastructure.Persistence
{
    public class AttachmentDto
    {
        public string? AttachmentName { get; set; }
        public byte[]? ByteContent { get; set; }
    }

    public class EmailSenderUtil(IAppSettingsProvider appSettings) : IEmailSenderUtil
    {
        public void SendEmail(string brandName, IList<string> emailRecepients, IList<AttachmentDto>? attachmentList, string emailSubject, string emailContent, bool isHtml, string logoPath = "")
        {
            string senderEmail = string.Empty;
            if (brandName != null)
            {
                senderEmail = brandName.ToUpperInvariant() switch
                {
                    BrandNames.AH_Brand => appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeyAHEmail),
                    BrandNames.AHA_Brand => appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeyAHAEmail),
                    BrandNames.BL_Brand => appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeyBLEmail),
                    BrandNames.IA_Brand => appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeyIAEmail),
                    BrandNames.IBX_Brand or BrandNames.IBC_Brand => appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeyIBXEmail),
                    _ => throw new InvalidOperationException("Invalid brandName: " + brandName),
                };
            }
            string smtpHost = appSettings.GetAppSettingSectionKey(AppSettingsConstants.ConfigSectionEmail, AppSettingsConstants.ConfigKeySmtpHost);

            MailMessage mail = new()
            {
                From = new MailAddress(senderEmail)
            };
            mail.To.Add(string.Join(",", emailRecepients.ToArray()));
            mail.Subject = emailSubject;
            
            if (!string.IsNullOrEmpty(logoPath))
            {
                Attachment inlineLogo = new(logoPath);
                if (inlineLogo.ContentDisposition != null)
                {
                    inlineLogo.ContentDisposition.Inline = true;
                    inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                }
                inlineLogo.ContentId = "brandlogo";
                inlineLogo.ContentType.MediaType = "image/png";
                inlineLogo.ContentType.Name = Path.GetFileName(logoPath);
                mail.Attachments.Add(inlineLogo);
            }
            mail.Body = emailContent;
            mail.IsBodyHtml = isHtml;

            if (attachmentList != null)
            {
                foreach (var attachment in attachmentList)
                {
                    if (attachment.ByteContent != null)
                    {
                        MemoryStream memoryStream = new(attachment.ByteContent);
                        Attachment emailAttachment = new(memoryStream, attachment.AttachmentName, MediaTypeNames.Application.Pdf);
                        mail.Attachments.Add(emailAttachment);
                    }
                }
            }

            if (attachmentList == null || mail.Attachments.Count >= attachmentList.Count)
            {
                SmtpClient smtpClient = new(smtpHost);
                smtpClient.Send(mail);
            }
        }
    }
}
