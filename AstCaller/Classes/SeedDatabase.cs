using System;
using System.Linq;
using AstCaller.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace AstCaller.Classes
{
    public class SeedDatabase
    {
        private MainContext _context;

        public SeedDatabase(MainContext context)
        {
            _context = context;
        }

        public void CreateAdminUser()
        {
            var user = new User
            {
                Login = "admin",
                Password = "12345678",
                Fullname = "admin"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void CreateCallStatuses()
        {
            _context.CallStatuses.Add(new CallStatus
            {
                Id=1,
                StatusName="В процессе",
            });

            _context.CallStatuses.Add(new CallStatus
            {
                Id = 2,
                StatusName = "Отвечен",
            });

            _context.CallStatuses.Add(new CallStatus
            {
                Id = 3,
                StatusName = "Нет ответа",
            });

            _context.CallStatuses.Add(new CallStatus
            {
                Id = 4,
                StatusName = "Просрочен",
            });

            _context.SaveChanges();
        }

        public void CreateAsteriskExtensions()
        {
            var defaultUser = _context.Users.First();
            _context.AsteriskExtensions.Add(new AsteriskExtension
            {
                Extension="play-record",
                Title="Проиграть запись",
                ModifierId= defaultUser.Id
            });

            _context.SaveChanges();
        }
    }
}
