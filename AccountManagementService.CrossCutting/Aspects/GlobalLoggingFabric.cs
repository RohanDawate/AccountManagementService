using AccountManagementService.CrossCutting.Attributes;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;

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

                // ONLY weave Controllers, Services, and Repositories!
                (t.Name.EndsWith("Controller") ||
                 t.ContainingNamespace.FullName.Contains(".Business") ||
                 t.ContainingNamespace.FullName.Contains(".Features")))

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
