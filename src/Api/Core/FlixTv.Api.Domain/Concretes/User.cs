using FlixTv.Api.Domain.Abstracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class User : IdentityUser<int>, IEntityBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsBanned { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<UserMovieCatalog>? FavoriteMovies { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ViewData>? WatchedHistory { get; set; }

        public User()
        {
            
        }
    }
}
