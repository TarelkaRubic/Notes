using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PS_223020_Server.BusinessLogic.Core.Interfaces;
using PS_223020_Server.BusinessLogic.Core.Models;
using PS_223020_Server.DataAccess.Core.Interfaces.DbContext;
using PS_223020_Server.DataAccess.Core.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using PS_223020_Server.Shared.Exception;
using DataAccess.Contexts;
using DataAccess.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace PS_223020_Server.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public UserService(IMapper mapper, IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Авторизует пользователя в системе
        /// </summary>
        /// <param name="numberPrefix">Код страны телефона</param>
        /// <param name="number">Номер телефона</param>
        /// <param name="password">Пароль</param>
        /// <returns>Объект UserInformationBlo</returns>
        public async Task<UserInformationBlo> Auth(int numberPrefix, int number, string password)
        {
            // Ищем в БД
            UserRto user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumberPrefix == numberPrefix && x.PhoneNumber == number && x.Password == password);

            // Что если не нашли
            if (user == null)
                throw new NotFoundException("Неверный номер телефона или пароль");

            // Что если нашли
            return await ConvertToUserInformationBlo(user);
        }

        /// <summary>
        /// Проверяет не занят ли номер телефона для регистрации
        /// </summary>
        /// <param name="numberPrefix">Код страны телефона</param>
        /// <param name="number">Номер телефона</param>
        /// <returns>true - занят, иначе нет</returns>
        public async Task<bool> DoesExist(int numberPrefix, int number)
        {
            return await _context.Users.AnyAsync(x => x.PhoneNumberPrefix == numberPrefix && x.PhoneNumber == number);
        }

        /// <summary>
        /// Возваращает информацию о пользователе
        /// </summary>
        /// <param name="userId">Иднетификатор пользователя</param>
        /// <returns>Объект UserInformationBlo</returns>
        public async Task<UserInformationBlo> Get(int userId)
        {
            // Ищем в БД
            UserRto user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            // Что если не нашли
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            // Что если нашли
            return await ConvertToUserInformationBlo(user);
        }

        /// <summary>
        /// Создает нового пользователя в системе
        /// </summary>
        /// <param name="numberPrefix">Код страны телефона</param>
        /// <param name="number">Номер телефона</param>
        /// <param name="password">Пароль</param>
        /// <returns>Объект UserInformationBlo</returns>
        public async Task<UserInformationBlo> Registration(int numberPrefix, int number, string password)
        {
            CreatePasswordHash(userIdentityBlo.Password!, out byte[] passwordHash, out byte[] passwordSalt);
            UserRto newUser = new UserRto() { PhoneNumberPrefix = numberPrefix, PhoneNumber = number, Password = password, PasswordHash = passwordHash, PasswordSalt = passwordSalt };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return await ConvertToUserInformationBlo(newUser);
        }

        /// <summary>
        /// Обновляет данные уже зарегистрированного пользователя
        /// </summary>
        /// <param name="numberPrefix">Код страны телефона</param>
        /// <param name="number">Номер телефона</param>
        /// <param name="password">Пароль</param>
        /// <param name="userUpdateBlo">Пакет новой информации, которой надо заменить старую</param>
        /// <returns>Объект UserInformationBlo</returns>
        public async Task<UserInformationBlo> Update(int numberPrefix, int number, string password, UserUpdateBlo userUpdateBlo)
        {
            // Ищем в БД
            UserRto user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumberPrefix == numberPrefix && x.PhoneNumber == number && x.Password == password);

            // Что если не нашли
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            // Если нашли, то надо обновить
            if (userUpdateBlo.IsBoy != null) user.IsBoy = userUpdateBlo.IsBoy;
            if (userUpdateBlo.Password != null) user.Password = userUpdateBlo.Password;
            if (userUpdateBlo.FirstName != null) user.FirstName = userUpdateBlo.FirstName;
            if (userUpdateBlo.LastName != null) user.LastName = userUpdateBlo.LastName;
            if (userUpdateBlo.Patronymic != null) user.Patronymic = userUpdateBlo.Patronymic;
            if (userUpdateBlo.Birthday != null) user.Birthday = userUpdateBlo.Birthday;
            if (userUpdateBlo.AvatarUrl != null) user.AvatarUrl = userUpdateBlo.AvatarUrl;

            // Сохранился
            await _context.SaveChangesAsync();

            return await ConvertToUserInformationBlo(user);
        }

        /// <summary>
        /// Конвертирует из UserRto в UserInformationBlo
        /// </summary>
        /// <param name="userRto">Объект UserRto</param>
        /// <returns>Объект UserInformationBlo</returns>
        private async Task<UserInformationBlo> ConvertToUserInformationBlo(UserRto userRto)
        {
            if (userRto == null)
                throw new ArgumentNullException(nameof(userRto));

            UserInformationBlo userInformationBlo = _mapper.Map<UserInformationBlo>(userRto);

            return userInformationBlo;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(UserRto user, byte[] securityToken)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Nickname)
            };

            var key = new SymmetricSecurityKey(securityToken);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}