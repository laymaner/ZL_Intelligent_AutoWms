using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.User;
using Intelligent_AutoWms.Model.RequestDTO.User;
using Intelligent_AutoWms.Model.ResponseDTO.Role;
using Intelligent_AutoWms.Model.ResponseDTO.User;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniExcelLibs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Intelligent_AutoWms.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IOptionsSnapshot<JWTOptions> _jwtOptionsSnapshot;
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<UserService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(ILogger<UserService> logger, IWebHostEnvironment webHostEnvironment, IOptionsSnapshot<JWTOptions> jwtOptionsSnapshot, Intelligent_AutoWms_DbContext db)
        {
            _logger = logger;
            _jwtOptionsSnapshot = jwtOptionsSnapshot;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <returns></returns>
        public async Task<long> CreateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO, string currentUserId)
        {
            try
            {
                WMS_Users user = new WMS_Users();
                if (!string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Code))
                {
                    if (await IsExistAsync(createOrUpdateUserDTO.Code))
                    {
                        throw new Exception($"User code {createOrUpdateUserDTO.Code} already exists, duplicate creation is not allowed");
                    }
                    user.Code = createOrUpdateUserDTO.Code;
                }
                else
                {
                    throw new Exception("User code cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Name))
                {
                    throw new Exception("User name cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Gender))
                {
                    throw new Exception("User gender cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Password))
                {
                    throw new Exception("User password cannot be empty");
                }
                user.Name = createOrUpdateUserDTO.Name;
                user.Password = MD5EncryptionUtil.Encrypt(createOrUpdateUserDTO.Password);
                user.Gender = createOrUpdateUserDTO.Gender;
                user.Status = (int)DataStatusEnum.Normal;
                user.Age = createOrUpdateUserDTO.Age;
                user.Email = createOrUpdateUserDTO.Email;
                user.Address = createOrUpdateUserDTO.Address;
                user.Birth = createOrUpdateUserDTO.Birth;
                user.Phone = createOrUpdateUserDTO.Phone;
                user.Remark = createOrUpdateUserDTO.Remark;
                user.Jwt_Version = 0;
                user.Create_Time = DateTime.Now;
                user.Creator = long.Parse(currentUserId);
                var result = await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return result.Entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除用户  软删除----将用户状态改为删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> DelUserAsync(string ids, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var result = ids.Split(',').ToList();
                    List<long> idList = result.Select(s => long.Parse(s)).ToList();
                    foreach (var id in idList)
                    {
                        if (id <= 0)
                        {
                            throw new Exception($"User ID:{id} does not exist");
                        }
                        var user = await _db.Users.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (user == null)
                        {
                            throw new Exception($"No information found for user,userId is {id}");
                        }
                        var items = await _db._User_Role_RelationShips.Where(m => m.User_Id == user.Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                        if (items != null && items.Count > 0)
                        {
                            foreach (var item in items)
                            {
                                item.Status = (int)DataStatusEnum.Delete;
                                item.Update_Time = DateTime.Now;
                                item.Updator = long.Parse(currentUserId);
                            }
                        }
                        user.Status = (int)DataStatusEnum.Delete;
                        user.Update_Time = DateTime.Now;
                        user.Updator = long.Parse(currentUserId);
                    }

                    return await _db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("The ids parameter is empty");
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 下载用户模板
        /// </summary>
        /// <returns></returns>
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            try
            {
                List<UserDownloadTemplate> list = new List<UserDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("User_Download_Template", list);             
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public async Task<FileStreamResult> ExportAsync([FromQuery] UserParamsDTO userParams)
        {
            try
            {
                var result = _db.Users.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(userParams.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(userParams.Name));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(userParams.Code));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Email))
                {
                    result = result.Where(m => m.Email.StartsWith(userParams.Email));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Address))
                {
                    result = result.Where(m => m.Address.StartsWith(userParams.Address));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Phone))
                {
                    result = result.Where(m => m.Phone.StartsWith(userParams.Phone));
                }
                var userList = result.Adapt<List<UserExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("UserInfomation", userList);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取jwt
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public async Task<TokenInfo> GetJwtDataAsync(string userCode)
        {
            try
            {
                TokenInfo tokenInfo = new TokenInfo();
                List<Claim> claims = new List<Claim>();
                var currentUser = await _db.Users.Where(m => m.Code.Equals(userCode) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (currentUser == null)
                {
                    throw new Exception($"{userCode} is not  Exist！");
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()));
                    claims.Add(new Claim("UserName", currentUser.Name));
                    claims.Add(new Claim(ClaimTypes.Name, currentUser.Code));
                    claims.Add(new Claim("Jwt_Version", currentUser.Jwt_Version.ToString()));
                    var items = await _db._User_Role_RelationShips.Where(m => m.User_Id == currentUser.Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            var role = await _db.Roles.Where(m => m.Id == item.Role_Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                            if (role != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role.Code));
                            }
                        }
                    }
                }

                string key = _jwtOptionsSnapshot.Value.SigningKey.ToString();
                DateTime expires = DateTime.Now.AddSeconds(_jwtOptionsSnapshot.Value.ExpireSeconds);
                byte[] secBytes = Encoding.UTF8.GetBytes(key);
                var secKey = new SymmetricSecurityKey(secBytes);
                var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                var tokenDescriptor = new JwtSecurityToken(claims: claims,
                    expires: expires, signingCredentials: credentials);
                string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                tokenInfo.Token = jwt;
                return tokenInfo;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<WMS_Users>> GetUserAsync([FromQuery] UserParamsDTO userParams)
        {
            try
            {
                List<UserInfo> userInfos = new List<UserInfo>();

                var result = _db.Users.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(userParams.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(userParams.Name));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(userParams.Code));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Email))
                {
                    result = result.Where(m => m.Email.StartsWith(userParams.Email));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Address))
                {
                    result = result.Where(m => m.Address.StartsWith(userParams.Address));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Phone))
                {
                    result = result.Where(m => m.Phone.StartsWith(userParams.Phone));
                }
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据用户id查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserInfo> GetUserByIdAsync(long id)
        {
            try
            {
                UserInfo userInfo = new UserInfo();
                if (id <= 0)
                {
                    throw new Exception("The user id parameter is empty");
                }
                var user = await _db.Users.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception($"No information found for User,id is {id}");
                }
                userInfo = user.Adapt<UserInfo>();
                var idList = await _db._User_Role_RelationShips.Where(m => m.User_Id == id && m.Status == (int)DataStatusEnum.Normal).Select(x => x.Id).ToListAsync();
                if (idList != null && idList.Count > 0)
                {
                    var roleList = await _db.Roles.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new RoleOptions(x.Id, x.Code, x.Name)).ToListAsync();
                    userInfo.Roles = roleList;
                }
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public async Task<BasePagination<WMS_Users>> GetUserPaginationAsync([FromQuery] UserParamsDTO userParams)
        {
            try
            {
                var result = _db.Users.Where(m => m.Status == (int)DataStatusEnum.Normal).OrderByDescending(n => n.Id).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(userParams.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(userParams.Name));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(userParams.Code));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Email))
                {
                    result = result.Where(m => m.Email.StartsWith(userParams.Email));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Address))
                {
                    result = result.Where(m => m.Address.StartsWith(userParams.Address));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Phone))
                {
                    result = result.Where(m => m.Phone.StartsWith(userParams.Phone));
                }
                return await PaginationService.PaginateAsync(result, userParams.PageIndex, userParams.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<string> ImportAsync(string path, string currentUserId)
        {
            try
            {
                var result = MiniExcelUtil.Import<UserDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断用户编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported user code");
                }
                //判断用户姓名有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported user name");
                }
                //判断用户性别有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Gender)))
                {
                    throw new Exception("There is a null value in the imported user gebder");
                }
                //判断用户密码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Password)))
                {
                    throw new Exception("There is a null value in the imported user password");
                }
                //判断用户编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("User code duplication");
                }
                var codeList = result.Select(m => m.Code).ToList();
                var items = await _db.Users.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (items != null && items.Count > 0)
                {
                    throw new Exception("User code already exists");
                }

                var data = result.Select(m => new WMS_Users
                {
                    Name = m.Name,
                    Code = m.Code,
                    Age = m.Age,
                    Gender = m.Gender,
                    Password = MD5EncryptionUtil.Encrypt(m.Password),
                    Birth = m.Birth,
                    Email = m.Email,
                    Phone = m.Phone,
                    Address = m.Address,
                    Status = (int)DataStatusEnum.Normal,
                    Creator = long.Parse(currentUserId),
                    Remark = m.Remark,
                    Create_Time = DateTime.Now,
                    Jwt_Version = 0

                });
                await _db.BulkInsertAsync(data);
                await _db.SaveChangesAsync();
                return "Import Users successful";
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据用户编码查询用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UserInfo> GetUserByCodeAsync(string userCode)
        {
            try
            {
                UserInfo userInfo = new UserInfo();
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    throw new Exception("Query parameter userCode is empty");
                }
                var user = await _db.Users.Where(m => m.Code.Equals(userCode) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception($"No information found for User,id is {userCode}");
                }
                userInfo = user.Adapt<UserInfo>();
                var idList = await _db._User_Role_RelationShips.Where(m => m.User_Id == user.Id && m.Status == (int)DataStatusEnum.Normal).Select(x => x.Id).ToListAsync();
                if (idList != null && idList.Count > 0)
                {
                    var roleList = await _db.Roles.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new RoleOptions(x.Id, x.Code, x.Name)).ToListAsync();
                    userInfo.Roles = roleList;
                }
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据用户编码判断用户是否存在
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public async Task<bool> IsExistAsync(string userCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    throw new Exception("Query parameter userCode is empty");
                }
                var user = await _db.Users.Where(m => m.Code.Equals(userCode) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public async Task<TokenInfo> LoginAsync([FromBody] UserLoginDTO userLogin)
        {
            try
            {
                TokenInfo tokenInfo = new TokenInfo();
                var currentUser = await _db.Users.Where(m => m.Code.Equals(userLogin.UserName) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (currentUser == null)
                {
                    throw new Exception($"{userLogin.UserName} is not  Exist！");
                }
                else
                {
                    if (MD5EncryptionUtil.Decrypt(currentUser.Password).Equals(userLogin.Password))
                    {
                        currentUser.Jwt_Version++;
                        currentUser.Update_Time = DateTime.Now;
                        currentUser.Updator = currentUser.Id;
                        await _db.SaveChangesAsync();

                        List<Claim> claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()));                   
                        claims.Add(new Claim(ClaimTypes.Name, currentUser.Code));
                        claims.Add(new Claim("User_Name", currentUser.Name));
                        claims.Add(new Claim("Jwt_Version", currentUser.Jwt_Version.ToString()));
                        var items = await _db._User_Role_RelationShips.Where(m => m.User_Id == currentUser.Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                        if (items != null && items.Count > 0)
                        {
                            foreach (var item in items)
                            {
                                var role = await _db.Roles.Where(m => m.Id == item.Role_Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                                if (role != null)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, role.Code));
                                }
                            }
                        }
                        string key = _jwtOptionsSnapshot.Value.SigningKey.ToString();
                        DateTime expires = DateTime.Now.AddSeconds(_jwtOptionsSnapshot.Value.ExpireSeconds);
                        byte[] secBytes = Encoding.UTF8.GetBytes(key);
                        var secKey = new SymmetricSecurityKey(secBytes);
                        var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                        var tokenDescriptor = new JwtSecurityToken(claims: claims,
                            expires: expires, signingCredentials: credentials);
                        string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                        tokenInfo.Token = jwt;
                    }
                    else
                    {
                        throw new Exception("Incorrect password");
                    }
                }
                return tokenInfo;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<string> ReSetPasswordAsync(string userCode, string password, string newPassword, string currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new Exception("Password reset failed, user code, original password, and current password cannot be empty");
                }
                if (password.Equals(newPassword))
                {
                    throw new Exception("Password reset failed, original password and current password are the same");
                }
                var user = await _db.Users.Where(m => m.Code.Equals(userCode)).SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("User does not exist");
                }
                if (user.Status != (int)DataStatusEnum.Normal)
                {
                    throw new Exception("Abnormal user status");
                }
                //判断登录密码和用户密码是否匹配
                if (!password.Equals(MD5EncryptionUtil.Decrypt(user.Password)))
                {
                    throw new Exception("Password mismatch");
                }
                user.Password = MD5EncryptionUtil.Encrypt(newPassword);
                user.Update_Time = DateTime.Now;
                user.Updator = long.Parse(currentUserId);

                await _db.SaveChangesAsync();
                return "Password reset successful";
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <returns></returns>
        public async Task<string> UpdateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO, string currentUserId)
        {
            try
            {
                if (createOrUpdateUserDTO.Id <= 0)
                {
                    throw new Exception("User ID does not exist");
                }
                var user = await _db.Users.Where(m => m.Id == createOrUpdateUserDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception($"No information found for user,userId is {createOrUpdateUserDTO.Id}");
                }
                if (!string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Code))
                {
                    if (!createOrUpdateUserDTO.Code.Equals(user.Code))
                    {
                        if (await IsExistAsync(createOrUpdateUserDTO.Code))
                        {
                            throw new Exception($"User code {createOrUpdateUserDTO.Code} already exists, duplicate creation is not allowed");
                        }
                    }
                    user.Code = createOrUpdateUserDTO.Code;
                }

                //基础信息更新
                user.Gender = string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Gender) ? user.Gender : createOrUpdateUserDTO.Gender;
                user.Age = createOrUpdateUserDTO.Age == 0 ? user.Age : createOrUpdateUserDTO.Age;
                user.Name = string.IsNullOrWhiteSpace(createOrUpdateUserDTO.Name) ? user.Name : createOrUpdateUserDTO.Name;
                user.Address = createOrUpdateUserDTO.Address;
                user.Birth = createOrUpdateUserDTO.Birth;
                user.Email = createOrUpdateUserDTO.Email;
                user.Phone = createOrUpdateUserDTO.Phone;
                user.Remark = createOrUpdateUserDTO.Remark;
                user.Updator = long.Parse(currentUserId);
                user.Update_Time = DateTime.Now;
                await _db.SaveChangesAsync();
                return "Successfully updated user information";
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 根据ids集合获取用户数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<WMS_Users>> GetUserByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Users>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Users.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes集合获取用户数据
        /// </summary>
        /// <param name="codes"></param> 
        /// <returns></returns>
        public async Task<List<WMS_Users>> GetUserByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Users>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Users.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                }
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据jwt获取用户信息
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task<JwtUserInfo> GetUserInfoFromJwtAsync(string token)
        {
            try
            {
                JwtUserInfo jwtUserInfo = new JwtUserInfo();
                string key = _jwtOptionsSnapshot.Value.SigningKey.ToString();
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // 其他可能需要的参数配置
                };
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                jwtUserInfo.Id = long.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                jwtUserInfo.Code = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                jwtUserInfo.Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "User_Name").Value;
                List<Claim> list = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                if (list != null && list.Count >= 0)
                {
                    jwtUserInfo.Roles = list.Select(c => c.Value).ToArray();
                }
                return jwtUserInfo;

            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);

            }
        }

        /// <summary>
        /// 获取用户选项集
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public async Task<List<UserOptions>> GetUserOptionsAsync([FromQuery] UserParamsDTO userParams)
        {
            try
            {
                List<UserOptions> userOptions = new List<UserOptions>();
                var result = _db.Users.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(userParams.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(userParams.Name));
                }
                if (!String.IsNullOrWhiteSpace(userParams.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(userParams.Code));
                }
                var item = await result.ToListAsync();
                userOptions = item.Adapt<List<UserOptions>>();
                return userOptions;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入 ----excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId)
        {
            try
            {
                if (!fileForm.FileName.Contains("User_Download_Template"))
                {
                    throw new Exception("Please select the correct template to import");
                }
                var stream = fileForm.OpenReadStream();
                var result = stream.Query<UserDownloadTemplate>().ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断用户编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported user code");
                }
                //判断用户姓名有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported user name");
                }
                //判断用户性别有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Gender)))
                {
                    throw new Exception("There is a null value in the imported user gebder");
                }
                //判断用户密码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Password)))
                {
                    throw new Exception("There is a null value in the imported user password");
                }
                //判断用户编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("User code duplication");
                }
                var codeList = result.Select(m => m.Code).ToList();
                var items = await _db.Users.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (items != null && items.Count > 0)
                {
                    throw new Exception("User code already exists");
                }

                var data = result.Select(m => new WMS_Users
                {
                    Name = m.Name,
                    Code = m.Code,
                    Age = m.Age,
                    Gender = m.Gender,
                    Password = MD5EncryptionUtil.Encrypt(m.Password),
                    Birth = m.Birth,
                    Email = m.Email,
                    Phone = m.Phone,
                    Address = m.Address,
                    Status = (int)DataStatusEnum.Normal,
                    Creator = currentUserId,
                    Remark = m.Remark,
                    Create_Time = DateTime.Now,
                    Jwt_Version = 0

                });
                await _db.BulkInsertAsync(data);
                await _db.SaveChangesAsync();
                return "Import Users successful";
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
