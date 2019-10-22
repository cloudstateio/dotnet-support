using System;
using Cloudstate;

namespace io.cloudstate.csharpsupport
{
    public interface IClientActionContext : IContext
    {
        Exception Fail(String errorMessage);

        void Forward(IServiceCall to);
    }
}