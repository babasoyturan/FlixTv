using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public bool IsBanned { get; set; } = false;
        public ICollection<UserMovieCatalog>? FavoriteMovies { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ViewData>? WatchedHistory { get; set; }

        public User()
        {
            
        }

        public User(
            string name, 
            string surname, 
            string email, 
            string hashedPassword,
            bool isBanned = false)
        {
            Name = name;
            Surname = surname;
            Email = email;
            HashedPassword = hashedPassword;
            IsBanned = isBanned;
        }
    }
}
