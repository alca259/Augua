using System.Reflection;

namespace Home.Infrastructure;

public static class InfrastructureAssemblyReference
{
    public static Assembly GetAssembly() => typeof(InfrastructureAssemblyReference).Assembly;
}