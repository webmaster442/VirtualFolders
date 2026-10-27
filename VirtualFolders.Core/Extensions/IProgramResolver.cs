using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualFolders.Core.Extensions;

internal interface IProgramResolver
{
    IEnumerable<ProgramInformation> Resolve(IEnumerable<string> paths);
}

internal sealed class ProgramResolvers
{
    public static IEnumerable<IProgramResolver> Builtins
    {
        get
        {
            yield return new VisualStudioResolver();
        }
    }
}
