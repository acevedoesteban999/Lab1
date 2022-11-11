using ClassLibrary;
using Repository;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add repository as service
            var connectionString = new ConnectionString(@"Data Source=ESTEBAN-PC\SQLEXPRESS;AttachDBFilename=C:\Users\Esteban\Desktop\Esteban\Programacion\C#\BDForm.mdf;Initial Catalog=BDForm;User ID=sa;Password=qwerty");
            builder.Services.AddSingleton(connectionString);
            builder.Services.AddScoped<IFormRepository, DBRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}