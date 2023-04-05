using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Inventories;

namespace Api.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<InventoryValue, InventoryViewModel>()
                .ForMember(dest => dest.RtrictType, opt => opt.MapFrom(src => (int)src.RtrictType));

            CreateMap<Picking, PickingViewModel>()
                .ReverseMap();

            CreateMap<InvoiceSummaryLine, InvoiceSummaryViewModel>()
                .ReverseMap();

            CreateMap<DocumentLineSummary, DocumentLineSummaryLineViewModel>()
                 .ReverseMap();

            CreateMap<BinLocations, BinLocationsViewModel>()
                .ReverseMap();
            CreateMap<Series, SeriesViewModel>()
                .ReverseMap();
            CreateMap<PickingItem, PickingItemViewModel>()
                .ReverseMap();
            CreateMap<Label, LabelViewModel>()
                .ReverseMap();

            CreateMap<PackingList, PackingListViewModel>()
               .ReverseMap();

            CreateMap<GroupListing, CreateGroupListingViewModel>();
            CreateMap<GroupListing, GroupListingViewModel>();
            CreateMap<User, UserViewModel>()
                .ReverseMap();
        }
    }
}