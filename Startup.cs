using LibraryAPI.Entities;
using LibraryAPI.Entities.Extensions;
using LibraryAPI.Helpers;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<LibraryContext>()
                    .AddScoped<ILibraryRepository, LibraryRepository>()
                    .AddMvc(options =>
                    {
                        options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                        options.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(options));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LibraryContext libraryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context => 
                        { context.Response.StatusCode = 500;
                            await context.Response.WriteAsync("An unexpected error occurred. Try again later.");
                        });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Author, AuthorDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
                cfg.CreateMap<AuthorForCreationDto, Author>();
                cfg.CreateMap<BookForCreationDto, Book>();
                cfg.CreateMap<BookForUpdateDto, Book>();
                cfg.CreateMap<Book, BookDto>();
            });

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
