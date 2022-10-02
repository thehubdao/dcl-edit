using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace EasySharp.ReSharperCustomSourceTemplates
{
    public static class ReSharperHelper
    {
        [SourceTemplate]
        public static void log(this object source)
        {
            //$Debug.Log(source);$END$
        }

        [SourceTemplate]
        public static void elog(this object source)
        {
            //$Debug.LogError(source);$END$
        }

        [SourceTemplate]
        public static void wlog(this object source)
        {
            //$Debug.LogWarning(source);$END$
        }
    }
}