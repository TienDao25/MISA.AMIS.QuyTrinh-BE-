
using MISA.AMIS.QuyTrinh.BL.BaseBL;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using MISA.AMIS.QuyTrinh.DL;
using MISA.AMIS.QuyTrinh.BL.RoleBL;
using MISA.AMIS.QuyTrinh.DL.RoleDL;
using MISA.AMIS.QuyTrinh.BL.SubSystemBL;
using MISA.AMIS.QuyTrinh.DL.SubSystemDL;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
//
builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));
builder.Services.AddScoped(typeof(IBaseDL<>), typeof(BaseDL<>));

builder.Services.AddScoped<IRoleBL, RoleBL>();
builder.Services.AddScoped<IRoleDL, RoleDL>();

builder.Services.AddScoped<ISubSystemBL, SubSystemBL>();
builder.Services.AddScoped<ISubSystemDL, SubSystemDL>();


// Lấy dữ liệu conection string từ file appsetting
DataBaseContext.ConnectionString = builder.Configuration.GetConnectionString("MySql");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
