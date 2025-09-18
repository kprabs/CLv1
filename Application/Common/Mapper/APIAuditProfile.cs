using AutoMapper;
using CoreLib.Application.Common.Models;
using CoreLib.Entities;

namespace CoreLib.Application.Common.Mapper
{
    public class APIAuditProfile : Profile
    {
        public APIAuditProfile()
        {
            // Mapping APIAuditModel to APIAudit and vice versa
            CreateMap<APIAuditModel, APIAudit>()
                .ForMember(dest => dest.APIAuditID, opt => opt.MapFrom(src => src.APIAuditID))
                .ForMember(dest => dest.SessionNKey, opt => opt.MapFrom(src => src.SessionNKey))
                .ForMember(dest => dest.SystemActivityID, opt => opt.MapFrom(src => src.SystemActivityID))
                .ForMember(dest => dest.ParentSystemActivityID, opt => opt.MapFrom(src => src.ParentSystemActivityID))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CreateUserNKey, opt => opt.MapFrom(src => src.CreateUserNKey))
                .ForMember(dest => dest.CreateForUserNKey, opt => opt.MapFrom(src => src.CreateForUserNKey))
                .ForMember(dest => dest.ClientKey, opt => opt.MapFrom(src => src.ClientKey))
                .ForMember(dest => dest.AccountKey, opt => opt.MapFrom(src => src.AccountKey))
                .ForMember(dest => dest.SubAccountKey, opt => opt.MapFrom(src => src.SubAccountKey))
                .ForMember(dest => dest.MemberKey, opt => opt.MapFrom(src => src.MemberKey))
                .ForMember(dest => dest.SubscriberKey, opt => opt.MapFrom(src => src.SubscriberKey))
                .ForMember(dest => dest.MemberPlatformCode, opt => opt.MapFrom(src => src.MemberPlatformCode))
                .ForMember(dest => dest.APIAuditPayloads, opt => opt.MapFrom(src => src.APIAuditPayloads))
                .ForMember(dest => dest.APIAuditPCPs, opt => opt.MapFrom(src => src.APIAuditPCPs))
                .ReverseMap(); // Enables mapping in both directions

            // Mapping SystemActivityModel to SystemActivity and vice versa
            CreateMap<SystemActivityModel, SystemActivity>()
                .ForMember(dest => dest.SystemActivityID, opt => opt.MapFrom(src => src.SystemActivityID))
                .ForMember(dest => dest.SystemActivityName, opt => opt.MapFrom(src => src.SystemActivityName))
                .ForMember(dest => dest.SystemActivityTypeNKey, opt => opt.MapFrom(src => src.SystemActivityTypeNKey))
                .ForMember(dest => dest.IsActiveFlag, opt => opt.MapFrom(src => src.IsActiveFlag))
                .ReverseMap(); // Enables mapping in both directions

            // Mapping APIAuditPayloadModel to APIAuditPayload and vice versa
            CreateMap<APIAuditPayloadModel, APIAuditPayload>()
                .ForMember(dest => dest.APIAuditPayloadID, opt => opt.MapFrom(src => src.APIAuditPayloadID))
                .ForMember(dest => dest.APIAuditID, opt => opt.MapFrom(src => src.APIAuditID))
                .ForMember(dest => dest.APIEndPointDetail, opt => opt.MapFrom(src => src.APIEndPointDetail))
                .ForMember(dest => dest.RequestPayloadHTML, opt => opt.MapFrom(src => src.RequestPayloadHTML))
                .ForMember(dest => dest.ResponseResultHTML, opt => opt.MapFrom(src => src.ResponseResultHTML))
                .ForMember(dest => dest.ResponseCode, opt => opt.MapFrom(src => src.ResponseCode))
                .ReverseMap(); // Enables mapping

            // Mapping configuration for MemberPlanOrderIDCard
            CreateMap<MemberPlanOrderIDCardModel, MemberPlanOrderIDCard>()
                .ForMember(dest => dest.MemberPlanOrderIDCardID, opt => opt.MapFrom(src => src.MemberPlanOrderIDCardID))
                .ForMember(dest => dest.APIAuditID, opt => opt.MapFrom(src => src.APIAuditID))
                .ForMember(dest => dest.MemberKey, opt => opt.MapFrom(src => src.MemberKey))
                .ForMember(dest => dest.PlanKey, opt => opt.MapFrom(src => src.PlanKey))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.EffDate, opt => opt.MapFrom(src => src.EffDate))
                .ReverseMap(); // Enables bidirectional mapping
                               // Mapping configuration for APIAuditPCPModel
            CreateMap<APIAuditPCPModel, APIAuditPCP>()
                .ForMember(dest => dest.APIAuditPCPID, opt => opt.MapFrom(src => src.APIAuditPCPID))
                .ForMember(dest => dest.APIAuditID, opt => opt.MapFrom(src => src.APIAuditID))
                .ForMember(dest => dest.MemberKey, opt => opt.MapFrom(src => src.MemberKey))
                .ForMember(dest => dest.CurrentSupplierLocationNKey, opt => opt.MapFrom(src => src.CurrentSupplierLocationNKey))
                .ForMember(dest => dest.CurrentPCPKey, opt => opt.MapFrom(src => src.CurrentPCPKey))
                .ForMember(dest => dest.CurrentPCPName, opt => opt.MapFrom(src => src.CurrentPCPName))
                .ForMember(dest => dest.NewSupplierLocationNKey, opt => opt.MapFrom(src => src.NewSupplierLocationNKey))
                .ForMember(dest => dest.NewPCPKey, opt => opt.MapFrom(src => src.NewPCPKey))
                .ForMember(dest => dest.NewPCPName, opt => opt.MapFrom(src => src.NewPCPName))
                .ForMember(dest => dest.EffDate, opt => opt.MapFrom(src => src.EffDate))
                .ForMember(dest => dest.IsProcessedFlag, opt => opt.MapFrom(src => src.IsProcessedFlag))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ReverseMap(); // Enables bidirectional mapping
        }
    }
}

