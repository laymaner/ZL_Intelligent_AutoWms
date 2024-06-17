namespace Intelligent_AutoWms.Model.RequestDTO.Permission
{
    /// <summary>
    /// 创建角色权限
    /// </summary>
    public class CreatePermisssionDTO
    {
        public long Role_Id { get; set; }

        public List<string> Permission_Codes { get; set; } = new List<string>();
    }
}
