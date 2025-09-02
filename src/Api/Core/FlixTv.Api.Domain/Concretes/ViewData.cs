using FlixTv.Api.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    [Index(nameof(UserId), nameof(MovieId), IsUnique = true)]
    public class ViewData : EntityBase
    {
        public int UserId { get; set; }

        public int MovieId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int LastPositionSeconds { get; set; }

        public int MaxPositionSeconds { get; set; }

        public int WatchedSeconds { get; set; }

        public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;

        public Movie? Movie { get; set; }

        public User? User { get; set; }

        public ViewData()
        {
            
        }

        public ViewData(
            int userId, 
            int movieId)
        {
            UserId = userId;
            MovieId = movieId;
        }
    }
}