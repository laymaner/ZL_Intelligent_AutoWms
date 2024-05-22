using Autofac;
using Autofac.Extensions.DependencyInjection;
using Intelligent_AutoWms.Extensions.Extensions;
using Intelligent_AutoWms.Extensions.Filter;
using Intelligent_AutoWms.Extensions.MiddleWares;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auto_Intelligent_Wms 接口文档",
        Version = "version 1.0",
        Description = "接口文档"
    });
    var file = Path.Combine(AppContext.BaseDirectory, "Intelligent_AutoWms.WebApi.xml");  // xml文档绝对路径
    var path = Path.Combine(AppContext.BaseDirectory, file); // xml文档绝对路径
    c.IncludeXmlComments(path, true); // true : 显示控制器层注释
    c.OrderActionsBy(o => o.RelativePath); // 对action的名称进行排序，如果有多个，就可以看见效果了。
});

builder.Services.AddMemoryCache();//内存缓存

builder.Services.AddLogging(LogBuilder =>
{
    LogBuilder.AddNLog("nlog.config");
});



builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //builder.RegisterModule<AutofacModule>();
    var assemblies = Assembly.GetExecutingAssembly();
    builder.RegisterAssemblyTypes(assemblies)//程序集内所有具象类 
    .Where(c => c.Name.EndsWith("Service"))
    .PublicOnly()//只要public访问权限的
    .Where(cc => cc.IsClass)//只要class型（主要为了排除值和interface类型） 
    .AsImplementedInterfaces();

    //将其他实现类程序集自动注册到容器
    var service = Assembly.Load("Intelligent_AutoWms.Services");

    builder.RegisterAssemblyTypes(service)//程序集内所有具象类 
   .Where(c => c.Name.EndsWith("Service"))
   .PublicOnly()//只要public访问权限的
   .Where(cc => cc.IsClass)//只要class型（主要为了排除值和interface类型） 
   .AsImplementedInterfaces();
});


builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

builder.Services.Configure<MvcOptions>(opt =>
{
    //opt.Filters.Add<NotLoginFilter>();
    opt.Filters.Add<TransationScopeFilter>();
    opt.Filters.Add<RateLimitFilter>();
    //opt.Filters.Add<RequestFilter>();
});

builder.Services.AddDbContext<Intelligent_AutoWms_DbContext>(option => {

    /*    ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.Development.json", true, true);
        IConfigurationRoot configurationRoot = configurationBuilder.Build();
        string conn = configurationRoot.GetSection("ConnectionStrings:conn").Value;*/

    string conn = builder.Configuration.GetSection("ConnectionStrings:conn").Value;

    option.UseSqlServer(conn);
    //批量更新 插入 删除数据
    option.UseBatchEF_MSSQL();

    //方法二 简单日志
    /*   option.LogTo(msg =>
       {
           Console.WriteLine(msg);
       });*/
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SigningKey);
    var secKey = new SymmetricSecurityKey(keyBytes);
    x.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secKey
    };
});

var app = builder.Build();


/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // 设置Swagger默认不展开接口定义
    app.UseSwaggerUI(options =>
    {
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}*/

app.UseSwagger();
// 设置Swagger默认不展开接口定义
app.UseSwaggerUI(options =>
{
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseHttpsRedirection();
app.UseAuthentication();//用户登录验证 必须在app.UseAuthorization()前注入
app.UseAuthorization();//用户角色权限验证

app.UserRequestMiddleWare();
/*//请求接口日志中间件 将接口中间件和捕捉异常中间件合并  使用扩展注册中间件
app.UseMiddleware<RequestMiddleWare>();

//抛出异常程序继续运行 异常日志写入log文件
//app.UseMiddleware<ExceptionHandlingMiddleWare>();
*/

app.MapControllers();

app.Run();

