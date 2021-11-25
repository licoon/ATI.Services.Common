using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ATI.Services.Common.Behaviors;
using ATI.Services.Common.Extensions;
using ATI.Services.Common.Initializers;
using ATI.Services.Common.Tracing;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ATI.Services.Common.Metrics
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class MetricsExtensions
    {
        public static void AddMetrics(this IServiceCollection services)
        {
            services.ConfigureByName<TracingOptions>();
            services.AddSingleton<ZipkinManager>();
            services.AddTransient<MetricsInitializer>();
            MetricsConfig.Configure();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            AppHttpContext.Services = services.BuildServiceProvider(new ServiceProviderOptions().ValidateOnBuild);
            services.ConfigureByName<MetricsOptions>();
            MetricsLabels.LabelsStatic = ConfigurationManager.GetSection(nameof(MetricsOptions))?.Get<MetricsOptions>()?.LabelsAndHeaders ?? new Dictionary<string, string>();
            MetricsLabels.UserLabels = MetricsLabels.LabelsStatic?.Keys.ToArray();
            MetricsLabels.UserHeaders = MetricsLabels.LabelsStatic?.Values.ToArray();
        }

        public static void UseMetrics(this IApplicationBuilder app)
        {
            app.UseMiddleware<MetricsStatusCodeCounterMiddleware>();
        }
    }
}