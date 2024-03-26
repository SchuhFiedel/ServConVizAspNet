using ServConViz;

namespace ServConVis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<HttpRequestObserverOptions>( opt => opt.ServiceName = "");

            builder.Services.AddHttpRequestObserver( setup =>
            {
                setup.ServiceName = "MyTestService";
                setup.ShowIncoming = true;
                setup.ShowOutgoing = true;
                setup.ShowInLog = true;
                setup.ServConVizServerUrl = "localhost";
                setup.ServConVizServerPort = 65535;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
            app.UseIncomingRequestTrackerMiddleWare();

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}