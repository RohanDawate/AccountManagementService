using AccountManagementService.CrossCutting.Attributes;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Aspects;

namespace AccountManagementService.CrossCutting.Aspects
{
    public class GlobalLoggingFabric : TransitiveProjectFabric
    {
        public override void AmendProject(IProjectAmender amender)
        {
            amender
                .SelectTypes()
                .Where(t =>
                    !t.IsImplicitlyDeclared &&
                    t.TypeKind is TypeKind.Class || t.IsRecord &&
                    !t.Attributes.Any(typeof(NoLogAttribute)) &&
                    !t.Name.EndsWith("Middleware") &&
                    !t.Name.EndsWith("Aspect") &&
                    !t.Name.EndsWith("Fabric") &&
                    !t.Name.EndsWith("Bootstrap"))
                .SelectMany(t => t.Methods)
                .Where(m =>
                    m.Accessibility is Accessibility.Public or Accessibility.Internal &&
                    !m.IsAbstract &&
                    !m.IsImplicitlyDeclared &&
                    !m.Attributes.Any(typeof(NoLogAttribute)) &&
                    m.Name is not ("ToString" or "Equals" or "GetHashCode" or "Dispose" or "DisposeAsync"))
                .AddAspectIfEligible<LogMethodAspect>();
        }
    }
}
