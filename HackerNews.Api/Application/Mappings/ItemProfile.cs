using HackerNews.Api.Application.DTOs;
using AutoMapper;
using HackerNews.Api.Domain;
namespace HackerNews.Api.Application.Mappings;
public class ItemProfile : Profile
{
 public ItemProfile() => CreateMap<Item,ItemDTO>()
  .ForMember(x=>x.Uri,o=>o.MapFrom(x=>x.Url ?? ""))
  .ForMember(x=>x.PostedBy,o=>o.MapFrom(x=>x.By))
  .ForMember(x=>x.Time,o=>o.MapFrom(x=>DateTimeOffset.FromUnixTimeSeconds(x.Time).UtcDateTime))
  .ForMember(x=>x.CommentCount,o=>o.MapFrom(x=>x.Kids == null ? 0 : x.Kids.Count));
}
