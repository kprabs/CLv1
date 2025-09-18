using AHA.IS.Common.Authorization.Domain;
using AutoMapper;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Forgerock.SuperAdmin.Models;

namespace CoreLib.Application.Common.Mapper
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<AuthService.InstanceHeaderDTO, AuthServiceNewRef.InstanceHeaderDTO>()
                .ForMember(dto => dto.ClassifiedAreaCode, opt => opt.MapFrom(src => src.ClassifiedAreaCode))
                .ForMember(dto => dto.ClassifiedSegmentCode, opt => opt.MapFrom(src => src.ClassifiedSegmentCode))
                .ForMember(dto => dto.ClassifiedAreaSegmentName, opt => opt.MapFrom(src => src.ClassifiedAreaSegmentName))
                .ForMember(dto => dto.ClassifiedAreaSegmentNkey, opt => opt.MapFrom(src => src.ClassifiedAreaSegmentNkey));

            CreateMap<AHA.IS.Common.Authorization.DTO.New.FeatureWithPermissionDTO, FeatureDTO>()
                .ForMember(dto => dto.SystemPermissionGroupSetId, opt => opt.MapFrom(src => src.SystemPermissionGroupSetId))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dto => dto.AllowsCustomAccounts, opt => opt.MapFrom(src => src.AllowsCustomAccounts))
                .ForMember(dto => dto.AllowedInstanceClassifiedSegmentCodes, opt => opt.MapFrom(src => src.AllowedInstanceClassifiedSegmentCodes))
                .ForMember(dto => dto.FeatureSelections, opt => opt.MapFrom(src => src.FeatureSelections));

            CreateMap<AHA.IS.Common.Authorization.DTO.New.FeatureSelectionWithPermissionDTO, FeatureSelectionDTO>()
                .ForMember(dto => dto.SystemPermissionGroupSetGroupingId, opt => opt.MapFrom(src => src.SystemPermissionGroupSetGroupingId))
                .ForMember(dto => dto.LabelName, opt => opt.MapFrom(src => src.LabelName))
                .ForMember(dto => dto.CustomLabelName, opt => opt.MapFrom(src => src.CustomLabelName))
                .ForMember(dto => dto.DisplayOrderNum, opt => opt.MapFrom(src => src.DisplayOrderNum))
                .ForMember(dto => dto.PermissionCode, opt => opt.MapFrom(src => src.PermissionCode));

            CreateMap<LogInSystemUser, AHA.IS.Common.Authorization.DTO.New.UserSearchResultDTO>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SourceLogInSystemUserName))
               .ReverseMap();

            CreateMap<LogInSystemUserSystemAccess, UserSystemAccessDTO>()
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffDate))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffDate))
                .ReverseMap();

            CreateMap<UserAccessDetailsDTO, UserFeatureAccessPermissionDTO>()
                .ForMember(dest => dest.OAMRoleCode, opt => opt
                    .MapFrom(src => string.Join(", ", src.SourceLogInSystemGroupNames)));

            CreateMap<AHA.IS.Common.Authorization.DTO.New.UserApplicationDetailsDTO, UserFeatureAccessPermissionDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(m => m.SystemId, d => d.MapFrom(t => t.SystemId))
                .ForMember(m => m.UserId, d => d.MapFrom(t => t.LoginSystemUserId))
                .ForMember(m => m.FirstName, d => d.MapFrom(t => t.FirstName))
                .ForMember(m => m.LastName, d => d.MapFrom(t => t.LastName))
                .ForMember(m => m.UserName, d => d.MapFrom(t => t.UserName))
                .ForMember(m => m.EmailAddress, d => d.MapFrom(t => t.EmailAddress))
                .ForMember(m => m.EffectiveDate, d => d.MapFrom(t => t.AccessEffectiveDate))
                .ForMember(m => m.TerminationDate, d => d.MapFrom(t => t.AccessTerminationDate))
                .ForMember(m => m.OAMRoleCode, d => d.MapFrom(t => t.SourceLogInSystemGroupNames.FirstOrDefault()))
                .ForMember(m => m.TenantName, d => d.MapFrom(t => t.RestrictedInstanceName))
                .ReverseMap();

            CreateMap<UserManagementEditDTO, UserFeatureAccessPermissionDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(m => m.SystemId, d => d.MapFrom(t => t.SystemId))
                .ForMember(m => m.UserId, d => d.MapFrom(t => t.UserId))
                .ForMember(m => m.FirstName, d => d.MapFrom(t => t.FirstName))
                .ForMember(m => m.LastName, d => d.MapFrom(t => t.LastName))
                .ForMember(m => m.UserName, d => d.MapFrom(t => t.UserName))
                .ForMember(m => m.EmailAddress, d => d.MapFrom(t => t.Email))
                .ForMember(m => m.EffectiveDate, d => d.MapFrom(t => t.EffectiveFrom))
                .ForMember(m => m.TerminationDate, d => d.MapFrom(t => t.EffectiveTo))
                .ForMember(m => m.OAMRoleCode, d => d.MapFrom(t => t.OAMRoleCode))
                .ForMember(m => m.TenantName, d => d.MapFrom(t => t.TenantName))
                .ReverseMap();

            CreateMap<ClassifiedSegmentInstance, ClassifiedAreaSegmentDTO>()
                .ForMember(dest => dest.ClassifiedAreaCode, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegment.ClassifiedArea.Code))
                .ForMember(dest => dest.ClassifiedAreaName, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegment.ClassifiedArea.Name))
                .ForMember(dest => dest.ClassifiedSegmentCode, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegment.ClassifiedSegment.Code))
                .ForMember(dest => dest.ClassifiedSegmentName, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegment.ClassifiedSegment.Name))
                .ForMember(dest => dest.InstanceName, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegmentName))
                .ForMember(dest => dest.InstanceNKey, opt => opt
                    .MapFrom(src => src.ClassifiedAreaSegmentNKey))
                .AfterMap((src, dest) => dest.SelectedForUser = false);

            CreateMap<UserDetails, UserDetailsDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.accountStatus.Equals("active", StringComparison.InvariantCultureIgnoreCase)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.userName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.givenName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.sn))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.mail))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.country))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.city))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.StateProvince, opt => opt.MapFrom(src => src.stateProvince))
                .ForMember(dest => dest.TelephoneNumber, opt => opt.MapFrom(src => src.telephoneNumber))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.createdBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => Utilities.DateConvertion(src.createdDate)))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => Utilities.DateConvertion(src.updatedDate)))
                .ForMember(dest => dest.LastLogInDateTime, opt => opt.MapFrom(src => Utilities.DateConvertion(src.lastLogin)))
                .ForMember(dest => dest.AgencyID, opt => opt.MapFrom(src => src.agencyId))
                .ForMember(dest => dest.UserSystemAccesses, opt => opt.MapFrom(src => src.effectiveGroups));

            CreateMap<EffectiveGroup, UserSystemAccessDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src._refResourceId));

            CreateMap<UserSearchCriteriaDTO, AHA.IS.Common.Authorization.DTO.New.SearchCriteriaDTO>()
                .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.criteria));

            CreateMap<SearchCriteria, AHA.IS.Common.Authorization.DTO.New.UserSearchResultDTO>()
                .ForMember(dest => dest.SearchField, opt => opt.MapFrom(src => src.SearchField))
                .ForMember(dest => dest.SearchValue, opt => opt.MapFrom(src => src.SearchValue));

            CreateMap<AHA.IS.Common.Authorization.DTO.New.UserApplicationDetailsDTO, UserManagementEditDTO>()
                .ForMember(m => m.SystemId, d => d.MapFrom(t => t.SystemId))
                .ForMember(m => m.UserId, d => d.MapFrom(t => t.LoginSystemUserId))
                .ForMember(m => m.FirstName, d => d.MapFrom(t => t.FirstName))
                .ForMember(m => m.LastName, d => d.MapFrom(t => t.LastName))
                .ForMember(m => m.UserName, d => d.MapFrom(t => t.UserName))
                .ForMember(m => m.Email, d => d.MapFrom(t => t.EmailAddress))
                .ForMember(m => m.EffectiveFrom, d => d.MapFrom(t => t.AccessEffectiveDate))
                .ForMember(m => m.EffectiveTo, d => d.MapFrom(t => t.AccessTerminationDate))
                .ForMember(m => m.OAMRoleCode, d => d.MapFrom(t => t.SourceLogInSystemGroupNames.FirstOrDefault()))
                .ForMember(m => m.TenantName, d => d.MapFrom(t => t.RestrictedInstanceName));

            CreateMap<UserSearchCriteriaDTO, SearchWith>()
                .ForMember(m => m.Criteria, d => d.MapFrom(t => t.criteria));

            CreateMap<SearchCriteria, Criteria>()
                .ForMember(m => m.SearchValue, d => d.MapFrom(t => t.SearchValue))
                .ForMember(m => m.SearchField, d => d.MapFrom(t => t.SearchField));

            CreateMap<Models.Groups, Forgerock.SuperAdmin.Models.Groups>();
            CreateMap<Models.MemberOfOrg, Forgerock.SuperAdmin.Models.MemberOfOrg>();

            CreateMap<CreateUserResponse, CreateUserResponseDTO>()
             .ForMember(m => m.statusCode, d => d.MapFrom(t => t.statusCode));
            CreateMap<Forgerock.SuperAdmin.Models.Message, Models.Message>();
            CreateMap<Forgerock.SuperAdmin.Models.EmptyClass, Models.EmptyClass>();
            CreateMap<Forgerock.SuperAdmin.Models.EffectiveGroups, Models.EffectiveGroups>();

            CreateMap<SystemUserLoginLog, UserNotesDTO>()
                .ForMember(m => m.UserNotes, d => d.MapFrom(t => t.RemarkDetail))
                .ForMember(m => m.UserName, d => d.MapFrom(t => t.SystemUserName))
                .ForMember(m => m.UpdatedAt, d => d.MapFrom(t => t.LastUpdateDate))
                .ForMember(m => m.UpdatedBy, d => d.MapFrom(t => t.LastUpdateUserName));

            CreateMap<UserDataResponse, UserAccessResponse>().ReverseMap();
        }
    }
}
