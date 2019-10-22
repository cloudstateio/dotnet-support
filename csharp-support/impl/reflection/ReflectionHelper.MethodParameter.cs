using System;
using System.Linq;
using System.Reflection;
using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{
    internal partial class ReflectionHelper
    {
        public class MethodParameter
        {

            public MethodBase Method { get; }
            public int Param { get; }
            public Type ParameterType { get; }
            public Type? GenericParameterType { get; }
            public CloudStateAttribute[] Attributes { get; }

            public MethodParameter(MethodBase method, int param)
            {

                Param = param;
                Method = method;

                var parameter = Method.GetParameters()[Param];
                ParameterType = parameter.ParameterType;
                if (Method.IsGenericMethod)
                    GenericParameterType = ParameterType.GetGenericArguments()[Param];

                Attributes = parameter
                    .GetCustomAttributes(typeof(CloudStateAttribute), true)
                    .Cast<CloudStateAttribute>()
                    .ToArray();
            }

        }

    }

}