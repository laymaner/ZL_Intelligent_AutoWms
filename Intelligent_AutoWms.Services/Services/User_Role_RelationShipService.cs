using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User_Role_RelationShip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class User_Role_RelationShipService : IUser_Role_RelationShipService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<User_Role_RelationShipService> _log;

        public User_Role_RelationShipService(Intelligent_AutoWms_DbContext dbContext, ILogger<User_Role_RelationShipService> logger)
        {
            _db = dbContext;
            _log = logger;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> CreateAsync(long userId, long roleId, long currentUserId)
        {
            try
            {
                var user = await _db.Users.Where(m => m.Id == userId && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception($"No information found for user,userId is {userId}");
                }
                var role = await _db.Roles.Where(m => m.Id == roleId && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (role == null)
                {
                    throw new Exception($"No information found for role,roleId is {roleId}");
                }
                var result = await _db._User_Role_RelationShips.Where(m => m.User_Id == userId && m.Role_Id == roleId && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (result != null && result.Count > 0)
                {
                    throw new Exception("The user role relationship already exists");
                }
                WMS_User_Role_RelationShips ship = new WMS_User_Role_RelationShips();
                ship.User_Id = userId;
                ship.Role_Id = roleId;
                ship.Status = (int)DataStatusEnum.Normal;
                ship.Create_Time = DateTime.Now;
                ship.Creator = currentUserId;
                var entity = await _db._User_Role_RelationShips.AddAsync(ship);
                await _db.SaveChangesAsync();
                return entity.Entity.Id;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> DelAsync(string ids, long currentUserId)
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
                            throw new Exception("User_Role_RelationShips ID does not exist");
                        }
                        var item = await _db._User_Role_RelationShips.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (item == null)
                        {
                            throw new Exception($"No information found for User_Role_RelationShips,Id is {id}");
                        }
                        item.Status = (int)DataStatusEnum.Delete;
                        item.Update_Time = DateTime.Now;
                        item.Updator = currentUserId;
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
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        public async Task<List<WMS_User_Role_RelationShips>> GetListAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO)
        {
            try
            {
                var result = _db._User_Role_RelationShips.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (userRoleRelationShipParamsDTO.UserId != null && userRoleRelationShipParamsDTO.UserId > 0)
                {
                    result = result.Where(m => m.User_Id == userRoleRelationShipParamsDTO.UserId);
                }
                if (userRoleRelationShipParamsDTO.RoleId != null && userRoleRelationShipParamsDTO.RoleId > 0)
                {
                    result = result.Where(m => m.Role_Id == userRoleRelationShipParamsDTO.RoleId);
                }
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        public async Task<BasePagination<WMS_User_Role_RelationShips>> GetPaginationAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO)
        {
            try
            {
                var result = _db._User_Role_RelationShips.Where(m => m.Status == (int)DataStatusEnum.Normal).OrderByDescending(n => n.Id).AsNoTracking(); ;
                if (userRoleRelationShipParamsDTO.UserId != null && userRoleRelationShipParamsDTO.UserId > 0)
                {
                    result = result.Where(m => m.User_Id == userRoleRelationShipParamsDTO.UserId);
                }
                if (userRoleRelationShipParamsDTO.RoleId != null && userRoleRelationShipParamsDTO.RoleId > 0)
                {
                    result = result.Where(m => m.Role_Id == userRoleRelationShipParamsDTO.RoleId);
                }
                return await PaginationService.PaginateAsync(result, userRoleRelationShipParamsDTO.PageIndex, userRoleRelationShipParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
