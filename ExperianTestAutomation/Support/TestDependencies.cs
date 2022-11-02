using Autofac;
using ExperianTestAutomation.Models;
using ExperianTestAutomation.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using SpecFlow.Autofac;
using TechTalk.SpecFlow;

namespace ExperianTestAutomation.Support
{
    public class TestDependencies
    {
        [ScenarioDependencies]
        public static ContainerBuilder Container()
        {
            var builder = new ContainerBuilder();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile($"{Directory.GetCurrentDirectory()}/Configs/config.json", optional: false, reloadOnChange: true)
                .Build();

            UrlsConfigSettings urlsConfig = new UrlsConfigSettings();
            configuration.GetSection("urls").Bind(urlsConfig, bo => bo.BindNonPublicProperties = true);
            builder.Register(_ => urlsConfig).AsSelf().SingleInstance();
            builder.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))).ToArray()).SingleInstance();

            builder.RegisterType<PhotosApiClient>().As<IPhotosApiClient>();

            return builder;
        }
    }
}
