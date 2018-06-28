using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models;

namespace Project_2_Common.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		public DbSet<Message> Messages { get; set; }
		public DbSet<Rating> Ratings{ get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(ms => ms.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .HasConstraintName("ForeignKey_SenderID")
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(ms => ms.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .HasConstraintName("ForeignKey_ReceiverID")
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Rating>()
                .HasOne(r => r.Reviewer)
                .WithMany(i => i.GivenRatings)
                .HasForeignKey(k => k.FromId)
                .HasConstraintName("ForeignKey_ReviewerID")
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Rating>()
                .HasOne(r => r.AwardedUser)
                .WithMany(i => i.ReceivedRatings)
                .HasForeignKey(k => k.ToId)
                .HasConstraintName("ForeignKey_AwardedID")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
