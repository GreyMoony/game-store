using AutoMapper;
using GameStore.API.Models.CommentModels;
using GameStore.API.Models.GameModels;
using GameStore.API.Models.GenreModels;
using GameStore.API.Models.OrderModels;
using GameStore.API.Models.PlatformModels;
using GameStore.API.Models.PublisherModels;
using GameStore.API.Models.UserModels;
using GameStore.Application.DTOs.CommentDtos;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.DTOs.GenreDtos;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Application.DTOs.PlatformDtos;
using GameStore.Application.DTOs.PublisherDtos;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShortGameModel, ShortGameDto>();
        CreateMap<AddGameModel, AddGameDto>()
            .ForMember(dest => dest.Game, opt => opt.MapFrom(src => src.Game));
        CreateMap<GameDto, GameModel>()
            .ReverseMap();
        CreateMap<UpdateGameModel, UpdateGameDto>()
            .ForMember(dest => dest.Game, opt => opt.MapFrom(src => src.Game));

        CreateMap<AddGenreModel, AddGenreDto>();
        CreateMap<GenreModel, GenreDto>().ReverseMap();
        CreateMap<ShortGenreDto, ShortGenreModel>();

        CreateMap<AddPlatformModel, AddPlatformDto>();
        CreateMap<PlatformModel, PlatformDto>().ReverseMap();

        CreateMap<AddPublisherModel, AddPublisherDto>();
        CreateMap<PublisherModel, PublisherDto>().ReverseMap();

        CreateMap<Game, GameDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<LocalizedGame, GameDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<AddGameDto, Game>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Game.Name))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Game.Key))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Game.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))
            .ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.Game.UnitInStock))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Game.Discount))
            .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.GameGenres, opt => opt.Ignore())
            .ForMember(dest => dest.ImageName, opt => opt.Ignore())
            .ForMember(dest => dest.GamePlatforms, opt => opt.MapFrom((src, dest) =>
                src.Platforms.Select(platformId => new GamePlatform
                {
                    GameId = dest.Id,
                    PlatformId = platformId,
                }).ToList()));

        CreateMap<AddGenreDto, Genre>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ParentGenreId, opt => opt.Ignore());
        CreateMap<Genre, GenreDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<Genre, ShortGenreDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<AddPlatformDto, Platform>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        CreateMap<Platform, PlatformDto>();

        CreateMap<AddPublisherDto, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        CreateMap<Publisher, PublisherDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<PublisherDto, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));

        CreateMap<OrderGame, OrderDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString()));
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.ToString()))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("o")));
        CreateMap<OrderNorthwind, OrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderID.ToString()))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerID))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.OrderDate));
        CreateMap<OrderDto, OrderModel>();
        CreateMap<OrderDetailsDto, OrderDetailsModel>();
        CreateMap<PaymentMethodDto, PaymentMethodModel>();
        CreateMap<VisaModel, VisaDto>();
        CreateMap<PaymentRequestModel, PaymentRequestDto>();
        CreateMap<VisaDto, VisaPaymentDetails>()
            .ForMember(dest => dest.CardHolderName, opt => opt.MapFrom(src => src.Holder))
            .ForMember(dest => dest.ExpirationYear, opt => opt.MapFrom(src => src.YearExpire))
            .ForMember(dest => dest.ExpirationMonth, opt => opt.MapFrom(src => src.MonthExpire))
            .ForMember(dest => dest.CVV, opt => opt.MapFrom(src => src.CVV2))
            .ForMember(dest => dest.TransactionAmount, opt => opt.Ignore());

        CreateMap<Comment, CommentDto>();
        CreateMap<BanModel, BanDto>();
        CreateMap<AddCommentModel, AddCommentDto>()
            .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Comment.Body))
            .ForMember(dest => dest.GameKey, opt => opt.Ignore());
        CreateMap<GameQueryModel, GameQuery>();

        CreateMap<Category, ShortGenreDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryID.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName));
        CreateMap<Category, GenreDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryID.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.ParentGenreId, opt => opt.Ignore());
        CreateMap<Category, Genre>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.ParentGenreId, opt => opt.Ignore());

        CreateMap<Supplier, PublisherDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SupplierID.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Empty));
        CreateMap<Supplier, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Empty));

        CreateMap<Product, GameDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductID.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => 0));
        CreateMap<Product, Game>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<OrderNorthwindDetail, OrderDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductID.ToString()))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => (int)src.Discount));

        CreateMap<UserShortModel, UserShortDto>();
        CreateMap<CreateUserRequest, CreateUserDto>();
        CreateMap<UpdateUserRequest, UpdateUserDto>();
        CreateMap<UserModel, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<RoleShortModel, RoleShortDto>();
        CreateMap<CreateRoleRequest, CreateRoleDto>();
        CreateMap<RoleModel, RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<UpdateRoleRequest, UpdateRoleDto>();
        CreateMap<LoginModel, LoginDto>();
        CreateMap<CheckAccessRequest, CheckAccessDto>();
    }
}
