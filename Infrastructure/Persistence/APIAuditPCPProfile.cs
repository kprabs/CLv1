using AutoMapper;
using CoreLib.Application.Common.Models;
using CoreLib.Entities;

namespace CoreLib.Infrastructure.Persistence
{
    public class APIAuditPCPProfile : Profile
    {
        public APIAuditPCPProfile()
        {
            // Mapping APIAuditPCPModel to APIAuditPCP and vice versa
            CreateMap<APIAuditPCPModel, APIAuditPCP>()
                .ForMember(dest => dest.APIAuditPCPID, opt => opt.MapFrom(src => src.APIAuditPCPID))
                .ForMember(dest => dest.APIAuditID, opt => opt.MapFrom(src => src.APIAuditID))
                .ForMember(dest => dest.MemberKey, opt => opt.MapFrom(src => src.MemberKey))
                .ForMember(dest => dest.CurrentPCPKey, opt => opt.MapFrom(src => src.CurrentPCPKey))
                .ForMember(dest => dest.CurrentPCPName, opt => opt.MapFrom(src => src.CurrentPCPName))
                .ForMember(dest => dest.CurrentSupplierLocationNKey, opt => opt.MapFrom(src => src.CurrentSupplierLocationNKey))
                .ForMember(dest => dest.NewSupplierLocationNKey, opt => opt.MapFrom(src => src.NewSupplierLocationNKey))
                .ForMember(dest => dest.NewPCPKey, opt => opt.MapFrom(src => src.NewPCPKey))
                .ForMember(dest => dest.NewPCPName, opt => opt.MapFrom(src => src.NewPCPName))
                .ForMember(dest => dest.IsProcessedFlag, opt => opt.MapFrom(src => src.IsProcessedFlag))
                .ForMember(dest => dest.EffDate, opt => opt.MapFrom(src => src.EffDate))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ReverseMap(); // Enables mapping in both directions
        }


    }
}

    
    
    
    
    
    
 
