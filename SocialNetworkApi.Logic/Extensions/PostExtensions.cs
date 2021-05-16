using System;
using System.Diagnostics.CodeAnalysis;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class PostExtensions
    {
        public static PostDto ToDto([NotNull] this Post post)
        {
            return new PostDto()
            {
                Id = post.Id.ToString(),
                AuthorId = post.AuthorId.ToString(),
                AttachedImageId = post.AttachedImageId.ToString(),
                ProfileId = post.ProfileId.ToString(),
                Text = post.Text,
                PublishedAt = post.TimePublished
            };
        }

        public static Post Update([NotNull] this Post post, [NotNull] PostDataModel data)
        {
            post.AttachedImageId = data.AttachedImageId == null ? post.AttachedImageId : new Guid(data.AttachedImageId);
            post.Text = data.Text ?? post.Text;
            return post;
        } 
    }
}
