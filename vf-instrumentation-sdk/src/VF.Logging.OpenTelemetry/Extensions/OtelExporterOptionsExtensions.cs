using System;
using System.Reflection;
using System.Runtime.Serialization;
using OpenTelemetry.Exporter;
using VF.Logging.OpenTelemetry.Configuration;

namespace VF.Logging.OpenTelemetry.Extensions
{
    internal static class OtelExporterOptionsExtensions
    {
        
        internal static OtlpExporterOptions MapFrom(this OtlpExporterOptions options, OtlpExporterDto dto) =>
            MapFromUseReflection(options, dto);

        internal static T MapFromUseReflection<T, T2>(T options, T2 dto)
        {
            if (dto is null || options is null)
            {
                throw new ArgumentNullException();
            }

            var optionType = options.GetType();
            var dtoType = dto.GetType();

            foreach (var member in optionType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var name = member.Name;
                var dtoMember = dtoType.GetProperty(name);
                if (dtoMember is null)
                {
                    throw new Exception($"Property \"{name}\" was not found in {dtoType.Name}");
                }

                if (member.PropertyType.IsClass && member.PropertyType != typeof(string) &&
                    !typeof(ISerializable).IsAssignableFrom(dtoMember.PropertyType))
                {
                    var dtoValue = dtoMember.GetValue(dto);
                    if (dtoValue is not null)
                    {
                        MapFromUseReflection(member.GetValue(options), dtoMember.GetValue(dto));
                    }
                }
                else
                {
                    var dtoValue = dtoMember.GetValue(dto);

                    if (dtoValue is not null)
                    {
                        member.SetValue(options, dtoMember.GetValue(dto));
                    }
                }
            }

            return options;
        }
    }
}