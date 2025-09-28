using AIChat1.DTOs;
using AIChat1.Entity;
using AIChat1.Helpers;
using AutoMapper;
using static AIChat1.DTOs.ChatDto;

namespace AIChat1.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User → UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

            // Message → MessageDto
            CreateMap<Message, DTOs.MessageDto>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.User));

            // UserDto → User
            CreateMap<RegisterRequest, User>()
                .ForMember(u => u.HashedPassword, opt =>
                opt.MapFrom(req => CustomPasswordHasher.HashPassword(req.Password)));

            // Conversation -> ConversationDto
            CreateMap<Conversation, ConversationDto>();

            // FileAttachment -> FileDto
            CreateMap<FileAttachment, FileDto>()
                .ForMember(d => d.Filename,  o => o.MapFrom(s => s.Filename))
                .ForMember(d => d.Url, o => o.MapFrom(s => s.Url));
        }
    }
}
