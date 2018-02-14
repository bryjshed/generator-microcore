using AutoMapper;

namespace <%= namespace %>
{
    /// <summary>
    /// The automapper profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// The constructor for the automapping profile.
        /// </summary>
        public MappingProfile()
        {
            <%_ if(createModel) { _%>
            CreateMap<PaginatedList<<%= modelName %>>, PaginatedList<<%= modelName %>Dto>>();
            CreateMap<<%= modelName %>, <%= modelName %>Dto>()
                .ForMember(dest => dest.CreationDate,
                    opt => opt.MapFrom(src => src.CreationDate.FormatDate(Constants.ISODateTimeFormat)))
                .ForMember(dest => dest.LastUpdatedDate,
                    opt => opt.MapFrom(src => src.LastUpdatedDate.FormatDate(Constants.ISODateTimeFormat)));
            CreateMap<Create<%= modelName %>Dto, <%= modelName %>>()
            <%_ if(database === 'dynamodb') { _%>
                .ForMember(x => x.VersionNumber, opt => opt.Ignore())
            <%_ } _%>
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedDate, opt => opt.Ignore());
            CreateMap<Update<%= modelName %>Dto, <%= modelName %>>()
            <%_ if(database === 'dynamodb') { _%>
                .ForMember(x => x.VersionNumber, opt => opt.Ignore())
            <%_ } _%>
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedDate, opt => opt.Ignore());
            <%_ } _%>
        }
    }
}