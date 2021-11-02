using AutoMapper;
using OKRNotification.EF;
using OKRNotification.ViewModel.Response;

namespace OKRNotification.Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NotificationsDetails, NotificationResponse>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.NotificationsDetailsId))
                .ForMember(dest => dest.OkrId, opts => opts.MapFrom(src => src.NotificationOnTypeId == 1 ? src.NotificationOnId : 0))
                .ForMember(dest => dest.KrId, opts => opts.MapFrom(src => src.NotificationOnTypeId == 2 ? src.NotificationOnId : 0));


            CreateMap<NotificationsDetails, NotificationsDetailsResponse>();

        }

    }
}
