using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace EasySharp.ReSharperCustomSourceTemplates
{
    public static class ReSharperHelper
    {
        [SourceTemplate]
        public static void log(this object source)
        {
            //$Debug.Log($"{nameof(source)}: {source}");$END$
        }

        [SourceTemplate]
        public static void elog(this object source)
        {
            //$Debug.LogError($"{nameof(source)}: {source}");$END$
        }

        [SourceTemplate]
        public static void wlog(this object source)
        {
            //$Debug.LogWarning($"{nameof(source)}: {source}");$END$
        }
    }
}