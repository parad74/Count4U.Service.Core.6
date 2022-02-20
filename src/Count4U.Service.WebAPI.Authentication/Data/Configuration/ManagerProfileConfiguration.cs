﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Count4U.Service.Core.Server.Data
{
    public class ManagerProfileConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private const string managerProfileId = "74d87db4-22222-22222-22222-2b96e0ada52a";
       public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var count4uLocal = new ApplicationUser
            {
                Id = managerProfileId,
                UserName = "manager@test.com",
                NormalizedUserName = "MANAGER@TEST.COM",
                Email = "manager@test.com",
                NormalizedEmail = "MANAGER@TEST.COM",
                PhoneNumber = "XXXXXXXXXXXXX",
                DataServerAddress = @"http://localhost:12389",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = new Guid().ToString("D"),
                DateCreated = DateTime.Now
         };

            count4uLocal.PasswordHash = PassGenerate(count4uLocal);

            builder.HasData(count4uLocal);
        }

        public string PassGenerate(ApplicationUser user)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, "123456");
        }
    }
}
